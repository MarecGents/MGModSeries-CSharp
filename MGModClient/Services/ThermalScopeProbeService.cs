using System;
using System.Collections.Generic;
using BepInEx.Logging;
using EFT.Animations;
using UnityEngine;

namespace MGModClient.Services;

/// <summary>
/// 【探针 / 一次性验证】把瞄具光学材质的 <c>_ThermalVisionOn</c> 按材质钉为 0，
/// 目的是在热成像开启时让反射式瞄具（红点/全息）与高倍镜恢复正常可见。
///
/// 背景（静态分析结论，待实机确认）：
///  - EFT v4.1.x 的 <c>ThermalVision</c> 每帧 <c>Shader.SetGlobalFloat("_ThermalVisionOn", 1f)</c>（OnPreCull）
///    并在 OnPostRender 复位 0；
///  - 读这个全局的正是四个「光学」shader：CW FX/Collimator、EFT/CollimatorGlass、
///    CW FX/OpticSight、Custom/OpticGlass —— 热成像后处理 Hidden/ThermalVision 不读它；
///  - 若该 uniform 落在材质常量缓冲（静态分析倾向如此），则按材质写 0 可覆盖全局 → 瞄具恢复。
///
/// 本类只做「诊断 + 施加」，不做任何资源/文件改动；失败绝不抛出到游戏。
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

    /// <summary>BepInEx 日志源，由插件在 Awake 时注入。</summary>
    public static ManualLogSource Logger;

    /// <summary>
    /// 工作模式：
    ///  - Material：只把光学材质上的 _ThermalVisionOn 钉为 0（推荐，影响面最小）；
    ///  - Global：每帧把全局 _ThermalVisionOn 置 0（用于判别「是 shader 分支还是合成覆盖」）；
    ///  - Off：不介入。
    /// </summary>
    public static string Mode = "Material";

    private static readonly HashSet<int> ProcessedRenderers = new HashSet<int>();
    private static readonly HashSet<string> LoggedShaders = new HashSet<string>(StringComparer.Ordinal);
    private static readonly HashSet<string> LoggedSkip = new HashSet<string>(StringComparer.Ordinal);
    private static bool _summaryLogged;

    private static void Log(string msg)
    {
        Logger?.LogInfo("[ThermalScope] " + msg);
    }

    private static void Warn(string msg)
    {
        Logger?.LogWarning("[ThermalScope] " + msg);
    }

    /// <summary>
    /// 对一把武器的瞄具子树施加处理。由 Harmony 补丁在
    /// ProceduralWeaponAnimation.InitTransforms / FindAimTransforms 之后调用。
    /// </summary>
    public static void Apply(ProceduralWeaponAnimation pwa, string caller)
    {
        if (pwa == null) return;
        if (string.Equals(Mode, "Off", StringComparison.OrdinalIgnoreCase)) return;

        try
        {
            var sights = pwa.ScopeAimTransforms;
            if (sights == null || sights.Count == 0)
            {
                LogSkipOnce($"{caller}: ScopeAimTransforms 为空/未就绪，跳过");
                return;
            }

            int hitRenderer = 0, hitMaterial = 0, skippedNoProp = 0, skippedShader = 0;

            for (int i = 0; i < sights.Count; i++)
            {
                var sight = sights[i];
                if (sight == null) continue;

                Transform bone = sight.Bone;
                if (bone == null) continue;

                Transform root = bone.parent;
                if (root == null) continue;

                // ① 自带热成像的瞄具（FLIR RS32 / Trijicon REAP-IR 等）：其光学材质同样读该全局，
                //    强行置 0 可能破坏其热成像功能 → 整棵子树跳过。
                if (HasThermalVisionComponent(root))
                {
                    LogSkipOnce($"{caller}: 跳过自带热成像的瞄具 '{root.name}'（子树内含 ThermalVision 组件）");
                    continue;
                }

                // ② 遍历子树内全部 MeshRenderer（含未激活：分化切换时 mode_00X 只有一个是启用的）
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

                        // 诊断：该 uniform 是否被 shader 暴露为「属性」。这是判定「按材质覆盖能否生效」的关键信号。
                        bool hasProp = false;
                        try { hasProp = mat.HasProperty(PropName); }
                        catch { /* 某些 shader 对未知名会抛，忽略 */ }

                        if (!hasProp)
                        {
                            skippedNoProp++;
                            if (LoggedShaders.Add(shaderName + "|noprop"))
                                Warn($"{caller}: shader '{shaderName}' 未把 {PropName} 暴露为属性（HasProperty=false）。" +
                                     "仍尝试写入，若无效则说明该 uniform 只能走全局。");
                        }

                        // 记录写前值（可能因非属性而抛错，忽略）
                        float before = float.NaN;
                        try { if (hasProp) before = mat.GetFloat(PropName); } catch { }

                        try
                        {
                            mat.SetFloat(PropName, 0f);
                            any = true;
                            hitMaterial++;
                            if (LoggedShaders.Add(shaderName + "|apply"))
                            {
                                Log($"{caller}: 已写入 {PropName}=0 | shader='{shaderName}' | 材质='{mat.name}' | " +
                                    $"HasProperty={hasProp} | 写前值={(float.IsNaN(before) ? "N/A" : before.ToString("0.###"))}");
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
                Log($"首次施加完成：渲染器 {hitRenderer} 个 / 材质 {hitMaterial} 个 / 非光学跳过 {skippedShader} 个 / 无属性 {skippedNoProp} 个");
            }
        }
        catch (Exception e)
        {
            // 探针绝不把异常抛给游戏
            Warn($"{caller}: Apply 异常已吞掉：{e.GetType().Name}: {e.Message}");
        }
    }

    /// <summary>子树内是否挂有 ThermalVision（= 该瞄具自带热成像）。</summary>
    private static bool HasThermalVisionComponent(Transform root)
    {
        try
        {
            return root.GetComponentsInChildren<ThermalVision>(true).Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private static void LogSkipOnce(string msg)
    {
        if (LoggedSkip.Add(msg)) Log(msg);
    }

    /// <summary>Global 模式：把全局开关置 0（由 ThermalScopePatch 在 ThermalVision.OnPreCull 之后调用）。</summary>
    public static void ForceGlobalOff()
    {
        if (!string.Equals(Mode, "Global", StringComparison.OrdinalIgnoreCase)) return;
        try
        {
            Shader.SetGlobalFloat(PropName, 0f);
        }
        catch (Exception e)
        {
            Warn($"ForceGlobalOff 异常：{e.GetType().Name}");
        }
    }

    /// <summary>模式切换时清空去重集合，便于同一局内反复试验。</summary>
    public static void ResetState()
    {
        ProcessedRenderers.Clear();
        LoggedShaders.Clear();
        LoggedSkip.Clear();
        _summaryLogged = false;
    }
}
