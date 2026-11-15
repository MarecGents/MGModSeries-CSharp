using System;
using System.Collections.Generic;
using BepInEx.Logging;
using EFT;
using EFT.Animations;
using EFT.CameraControl;
using UnityEngine;
using UnityEngine.Rendering;

namespace MGModClient.Services;

/// <summary>
/// 热成像 × 瞄具：让「指定瞄具」在热成像开启时仍可正常使用（准星/玻璃可见）。
///
/// 机制（已实机验证成立）：
///  - EFT 的 <c>ThermalVision</c> 在 OnPreCull 把全局 <c>_ThermalVisionOn</c> 置 1、OnPostRender 复位 0；
///  - 读该全局的是四个光学 shader：CW FX/Collimator、EFT/CollimatorGlass、
///    CW FX/OpticSight、Custom/OpticGlass（热成像后处理 Hidden/ThermalVision **不读**它）；
///  - 该 uniform <b>不是材质属性</b>（HasProperty=false），按材质写无效（已实机证伪）；
///  - 有效做法：在相机 <c>CameraEvent.BeforeForwardAlpha</c> 挂 CommandBuffer 把全局置 0
///    —— 不透明几何已按热成像渲染完，只影响前向透明阶段（瞄具玻璃/准星在此绘制），
///    因此瞄具恢复而热成像观感基本不变。
///
/// 生效范围（本版本新增的过滤）：
///  - 仅当 **当前装备的瞄具命中 TemplateId 白名单** 时启用；
///  - 白名单为空 → 功能完全关闭；
///  - 游戏原版瞄具一律不在白名单内 → 保持原版热成像行为（瞄具在热成像下仍不可见）。
///
/// 设计约束：不修改任何文件/资源；异常绝不上抛到游戏。
/// </summary>
public static class ThermalScopeProbeService
{
    /// <summary>仅对这四个光学 shader 的材质生效（其余材质原样放行，避免波及武器/环境）。</summary>
    private static readonly HashSet<string> OpticShaderNames = new HashSet<string>(StringComparer.Ordinal)
    {
        "CW FX/Collimator",
        "EFT/CollimatorGlass",
        "CW FX/OpticSight",
        "Custom/OpticGlass",
    };

    private const string PropName = "_ThermalVisionOn";

    /// <summary>白名单字符串的分隔符（逗号/分号/空白）。</summary>
    private static readonly char[] ListSeparators = { ',', ';', ' ', '\t', '\r', '\n' };

    /// <summary>BepInEx 日志源，由插件在 Awake 时注入。</summary>
    public static ManualLogSource Logger;

    /// <summary>
    /// 工作模式：
    ///  - ForwardAlpha：前向透明阶段前把全局置 0（**推荐 / 已验证有效**）；
    ///  - Global：每帧全局置 0（影响整帧，仅诊断用）；
    ///  - Material：按材质写（已验证无效，仅留作对照）；
    ///  - Off：不介入。
    /// </summary>
    public static string Mode = "ForwardAlpha";

    /// <summary>白名单（TemplateId，忽略大小写）。空集合 = 功能完全关闭。</summary>
    private static readonly HashSet<string> AllowedTemplateIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>已记录过诊断日志的瞄具 TemplateId（去重）。</summary>
    private static readonly HashSet<string> LoggedSights = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private static readonly HashSet<int> ProcessedRenderers = new HashSet<int>();
    private static readonly HashSet<string> LoggedShaders = new HashSet<string>(StringComparer.Ordinal);
    private static readonly HashSet<string> LoggedSkip = new HashSet<string>(StringComparer.Ordinal);

    private static ProceduralWeaponAnimation _lastPwa;
    private static bool _allowedState;
    private static bool _emptyAllowListWarned;
    private static bool _summaryLogged;

    private static void Log(string msg)
    {
        Logger?.LogInfo("[ThermalScope] " + msg);
    }

    private static void Warn(string msg)
    {
        Logger?.LogWarning("[ThermalScope] " + msg);
    }

    // ==================== 白名单 ====================

    /// <summary>
    /// 解析白名单字符串（逗号/分号/空白分隔，自动去掉首尾引号与空白）。
    /// 供手动配置与聚合器复用，保证两处解析规则完全一致。
    /// </summary>
    public static IEnumerable<string> ParseIdList(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) yield break;
        foreach (var part in raw.Split(ListSeparators, StringSplitOptions.RemoveEmptyEntries))
        {
            var id = part.Trim().Trim('"', '\'').Trim();
            if (id.Length > 0) yield return id;
        }
    }

    /// <summary>
    /// 设置生效白名单集合（由 <see cref="ThermalScopeAllowListProvider"/> 聚合后推入）。
    /// 空集合 = 功能完全关闭（游戏原版瞄具行为完全保留）。
    /// </summary>
    public static void SetAllowListSet(IEnumerable<string> ids)
    {
        AllowedTemplateIds.Clear();
        if (ids != null)
        {
            foreach (var id in ids)
            {
                if (string.IsNullOrWhiteSpace(id)) continue;
                AllowedTemplateIds.Add(id.Trim());
            }
        }

        _emptyAllowListWarned = false;

        if (AllowedTemplateIds.Count == 0)
        {
            Warn("白名单为空 → 功能已关闭：任何瞄具都不会生效，原版行为完全保留");
        }
        else
        {
            Log($"生效白名单 {AllowedTemplateIds.Count} 条 → {string.Join(", ", AllowedTemplateIds)}");
            Log("只有装备这些瞄具时功能才生效；其余瞄具（含游戏原版全部瞄具）保持原版热成像行为");
        }
    }

    /// <summary>设置白名单（字符串形式，内部转调 <see cref="SetAllowListSet"/>）。</summary>
    public static void SetAllowList(string raw) => SetAllowListSet(ParseIdList(raw));

    /// <summary>取瞄具物品的 TemplateId（即 items.json 的 _id）；取不到返回 null。</summary>
    private static string GetSightTemplateId(ProceduralWeaponAnimation.SightNBone sight)
    {
        try
        {
            if (sight == null) return null;
            var component = sight.Mod;          // SightComponent（由 FindAimTransforms 赋值）
            if (component == null) return null;
            var item = component.Item;          // 瞄具物品
            if (item == null) return null;
            return item.StringTemplateId;       // == Template._id
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 本地玩家的 PWA（可能为 null：尚未进入战局/藏身处）。
    /// EFT 中每个角色（含 bot / 其他玩家）都有自己的 ProceduralWeaponAnimation，
    /// 并会触发 InitTransforms / FindAimTransforms（见 NextObservedPlayer.BundleAnimationBones），
    /// 因此不能只依赖"最后一次回调的 PWA"。
    /// </summary>
    private static ProceduralWeaponAnimation GetLocalPlayerPwa()
    {
        try
        {
            var me = GamePlayerOwner.MyPlayer;
            return me != null ? me.ProceduralWeaponAnimation : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 当前是否应启用：装备中的瞄具集合里至少有一个命中白名单。
    /// 每帧调用（开销仅为几次属性读取），因此换枪/换镜后会立即反映最新状态。
    /// <paramref name="reason"/> 用于在状态翻转时给出可诊断的原因。
    /// </summary>
    private static bool IsAllowedNow(out string reason)
    {
        reason = null;

        if (AllowedTemplateIds.Count == 0)
        {
            reason = "白名单为空";
            return false;
        }

        // 优先取本地玩家的 PWA：其他角色（bot/其他玩家）的 PWA 也会触发同样的补丁回调，
        // 只用 _lastPwa 会被它们覆盖（见 Apply 注释）。用 UnityEngine.Object 的 == 判定可识别已销毁对象。
        var pwa = GetLocalPlayerPwa();
        if (pwa == null) pwa = _lastPwa;
        if (pwa == null)
        {
            reason = "尚无武器实例（补丁未触发或武器已销毁）";
            return false;
        }

        try
        {
            // 收枪时武器对象通常被停用：此时不应继续介入
            if (!pwa.gameObject.activeInHierarchy)
            {
                reason = $"武器对象未激活（{pwa.gameObject.name}）";
                return false;
            }

            var sights = pwa.ScopeAimTransforms;
            if (sights == null || sights.Count == 0)
            {
                reason = "ScopeAimTransforms 为空（未装备瞄具）";
                return false;
            }

            for (int i = 0; i < sights.Count; i++)
            {
                var id = GetSightTemplateId(sights[i]);
                if (id != null && AllowedTemplateIds.Contains(id)) return true;
            }

            reason = "当前瞄具不在白名单";
        }
        catch (Exception e)
        {
            reason = $"判定异常 {e.GetType().Name}";
            // 任何异常都按「不生效」处理，保证不会影响游戏
        }
        return false;
    }

    // ==================== 武器侧：诊断 + （旧的）材质路径 ====================

    /// <summary>
    /// 由 Harmony 补丁在 ProceduralWeaponAnimation.InitTransforms / FindAimTransforms 之后调用。
    /// 记录当前武器实例，并输出白名单命中诊断。
    /// </summary>
    public static void Apply(ProceduralWeaponAnimation pwa, string caller)
    {
        if (pwa == null) return;

        // 只跟踪本地玩家的武器：bot / 其他角色的武器同样会触发这两个补丁回调
        // （NextObservedPlayer.BundleAnimationBones.InitTransforms 等），
        // 若不加过滤，_lastPwa 会被它们覆盖 → 白名单判定失效、CommandBuffer 被摘除。
        var localPwa = GetLocalPlayerPwa();
        if (localPwa != null && !ReferenceEquals(localPwa, pwa)) return;

        _lastPwa = pwa;

        if (string.Equals(Mode, "Off", StringComparison.OrdinalIgnoreCase)) return;

        try
        {
            if (AllowedTemplateIds.Count == 0)
            {
                if (!_emptyAllowListWarned)
                {
                    _emptyAllowListWarned = true;
                    LogSkipOnce($"{caller}: 白名单为空，功能不启用");
                }
                return;
            }

            var sights = pwa.ScopeAimTransforms;
            if (sights == null || sights.Count == 0)
            {
                LogSkipOnce($"{caller}: ScopeAimTransforms 为空/未就绪，跳过");
                return;
            }

            bool anyAllowed = false;
            for (int i = 0; i < sights.Count; i++)
            {
                var sight = sights[i];
                if (sight == null) continue;

                var id = GetSightTemplateId(sight);
                if (id == null) continue;

                bool allowed = AllowedTemplateIds.Contains(id);
                if (allowed) anyAllowed = true;

                if (LoggedSights.Add(id))
                {
                    Log($"{caller}: 瞄具 TemplateId={id} → " +
                        (allowed ? "命中白名单（启用）" : "不在白名单（保持原版热成像行为）"));
                }
            }

            if (!anyAllowed)
            {
                LogSkipOnce($"{caller}: 当前武器的瞄具均不在白名单，本武器不启用");
                return;
            }

            // 旧的「按材质写」路径：已实机证伪（_ThermalVisionOn 不是材质属性），
            // 仅保留在 Material 模式下作对照，其余模式不再执行（避免无谓开销与误导日志）。
            if (string.Equals(Mode, "Material", StringComparison.OrdinalIgnoreCase))
            {
                ApplyMaterialPath(caller, sights);
            }
        }
        catch (Exception e)
        {
            Warn($"{caller}: Apply 异常已吞掉：{e.GetType().Name}: {e.Message}");
        }
    }

    /// <summary>【对照用，已证伪】按材质写入 _ThermalVisionOn=0。仅 Material 模式执行。</summary>
    private static void ApplyMaterialPath(string caller, List<ProceduralWeaponAnimation.SightNBone> sights)
    {
        int hitRenderer = 0, hitMaterial = 0, skippedNoProp = 0, skippedShader = 0;

        for (int i = 0; i < sights.Count; i++)
        {
            var sight = sights[i];
            if (sight == null) continue;

            // 只处理白名单内的瞄具
            var sightId = GetSightTemplateId(sight);
            if (sightId == null || !AllowedTemplateIds.Contains(sightId)) continue;

            Transform bone = sight.Bone;
            if (bone == null) continue;

            Transform root = bone.parent;
            if (root == null) continue;

            // 自带热成像的瞄具（理论上不会是白名单项，保险起见仍排除）
            if (HasThermalDevice(root))
            {
                LogSkipOnce($"{caller}: 跳过自带热成像的瞄具 '{root.name}'（子树内含热成像组件）");
                continue;
            }

            var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
            for (int r = 0; r < renderers.Length; r++)
            {
                var mr = renderers[r];
                if (mr == null) continue;

                int id = mr.GetInstanceID();
                if (ProcessedRenderers.Contains(id)) continue;

                Material[] mats;
                try
                {
                    mats = mr.materials; // 实例化材质：只影响本武器实例，不污染共享资源
                }
                catch (Exception e)
                {
                    Warn($"{caller}: 读取 {mr.name} 的 materials 失败：{e.GetType().Name}");
                    continue;
                }

                bool any = false;
                for (int m = 0; m < mats.Length; m++)
                {
                    var mat = mats[m];
                    if (mat == null) continue;

                    string shaderName = mat.shader != null ? mat.shader.name : "<null>";
                    if (!OpticShaderNames.Contains(shaderName))
                    {
                        skippedShader++;
                        if (LoggedSkip.Add(shaderName))
                            Log($"{caller}: 非光学 shader，未处理：'{shaderName}'（材质 {mat.name}）");
                        continue;
                    }

                    bool hasProp = false;
                    try { hasProp = mat.HasProperty(PropName); }
                    catch { }

                    if (!hasProp) skippedNoProp++;

                    try
                    {
                        mat.SetFloat(PropName, 0f);
                        any = true;
                        hitMaterial++;
                        if (LoggedShaders.Add(shaderName + "|apply"))
                        {
                            Log($"{caller}: [对照] 已写入 {PropName}=0 | shader='{shaderName}' | " +
                                $"材质='{mat.name}' | HasProperty={hasProp}");
                        }
                    }
                    catch (Exception e)
                    {
                        if (LoggedShaders.Add(shaderName + "|fail"))
                            Warn($"{caller}: 写入失败 shader='{shaderName}'：{e.GetType().Name}: {e.Message}");
                    }
                }

                if (any)
                {
                    ProcessedRenderers.Add(id);
                    hitRenderer++;
                }
            }
        }

        if (!_summaryLogged && (hitRenderer > 0 || hitMaterial > 0))
        {
            _summaryLogged = true;
            Log($"Material 对照路径完成：渲染器 {hitRenderer} / 材质 {hitMaterial} / 非光学跳过 {skippedShader} / 无属性 {skippedNoProp}");
        }
    }

    /// <summary>
    /// 子树内是否挂有「自带热成像」的组件。
    /// 热成像瞄具 prefab 挂的是 <see cref="ScopeThermalVisionData"/>（数据组件），
    /// **不是** <see cref="ThermalVision"/>；两个都查。
    /// </summary>
    private static bool HasThermalDevice(Transform root)
    {
        try
        {
            if (root.GetComponentsInChildren<ScopeThermalVisionData>(true).Length > 0) return true;
            if (root.GetComponentsInChildren<ThermalVision>(true).Length > 0) return true;
        }
        catch { }
        return false;
    }

    private static void LogSkipOnce(string msg)
    {
        if (LoggedSkip.Add(msg)) Log(msg);
    }

    // ==================== 相机侧：每帧判定 + 挂/摘 CommandBuffer ====================

    private static readonly Dictionary<int, CommandBuffer> ForwardAlphaBuffers = new Dictionary<int, CommandBuffer>();
    private static readonly Dictionary<int, Camera> ForwardAlphaCameras = new Dictionary<int, Camera>();

    /// <summary>
    /// ThermalVision.OnPreCull 之后调用。每帧判定「当前瞄具是否命中白名单」，
    /// 命中才介入；未命中则**立即摘除** CommandBuffer，回到完全原版行为。
    /// </summary>
    public static void OnThermalPreCull(ThermalVision thermalVision)
    {
        try
        {
            bool off = string.Equals(Mode, "Off", StringComparison.OrdinalIgnoreCase);
            string reason = off ? "模式为 Off" : null;
            bool allowed = !off && IsAllowedNow(out reason);

            if (allowed != _allowedState)
            {
                _allowedState = allowed;
                Log(allowed
                    ? $"白名单命中 → 启用（模式 {Mode}）：热成像下瞄具正常可见"
                    : $"未启用（{reason}）→ 保持原版热成像行为");
            }

            if (!allowed)
            {
                DetachForwardAlphaBuffer();
                return;
            }

            if (string.Equals(Mode, "Global", StringComparison.OrdinalIgnoreCase))
            {
                DetachForwardAlphaBuffer();
                Shader.SetGlobalFloat(PropName, 0f);
                return;
            }

            if (string.Equals(Mode, "ForwardAlpha", StringComparison.OrdinalIgnoreCase) && thermalVision != null)
            {
                EnsureForwardAlphaBuffer(thermalVision);
                return;
            }

            // Material 模式不依赖 CommandBuffer
            DetachForwardAlphaBuffer();
        }
        catch (Exception e)
        {
            Warn($"OnThermalPreCull 异常：{e.GetType().Name}: {e.Message}");
        }
    }

    private static void EnsureForwardAlphaBuffer(ThermalVision thermalVision)
    {
        var cam = thermalVision.GetComponent<Camera>();
        if (cam == null) return;

        int id = cam.GetInstanceID();
        if (ForwardAlphaBuffers.ContainsKey(id)) return;

        var cb = new CommandBuffer { name = "MGModClient.ThermalScope" };
        cb.SetGlobalFloat(Shader.PropertyToID(PropName), 0f);

        cam.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, cb);
        cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, cb);

        ForwardAlphaBuffers[id] = cb;
        ForwardAlphaCameras[id] = cam;
        Log($"已在相机 '{cam.name}' 的 BeforeForwardAlpha 挂载 SetGlobalFloat({PropName}=0)");
    }

    private static void DetachForwardAlphaBuffer()
    {
        if (ForwardAlphaBuffers.Count == 0) return;
        RemoveForwardAlphaBuffers();
    }

    private static void RemoveForwardAlphaBuffers()
    {
        foreach (var kv in ForwardAlphaBuffers)
        {
            if (ForwardAlphaCameras.TryGetValue(kv.Key, out var cam) && cam != null)
            {
                try { cam.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, kv.Value); } catch { }
            }
            try { kv.Value.Release(); } catch { }
        }
        ForwardAlphaBuffers.Clear();
        ForwardAlphaCameras.Clear();
    }

    /// <summary>配置切换时清理状态（去重集合 + CommandBuffer + 判定缓存）。</summary>
    public static void ResetState()
    {
        ProcessedRenderers.Clear();
        LoggedShaders.Clear();
        LoggedSkip.Clear();
        LoggedSights.Clear();
        _summaryLogged = false;
        _emptyAllowListWarned = false;
        _allowedState = false;
        RemoveForwardAlphaBuffers();
    }
}
