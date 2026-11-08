using System;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using EFT.Animations;
using HarmonyLib;

namespace MGModClient.Services;

/// <summary>
/// 【探针 / 一次性验证】热成像 × 瞄具 的 Harmony 补丁集合。
///
/// 三个补丁点（逐条独立安装，单独报告成败，便于实机定位）：
///  1) ProceduralWeaponAnimation.InitTransforms —— 武器动画初始化（首次持械/换武器）；
///  2) ProceduralWeaponAnimation.FindAimTransforms —— 瞄具瞄准骨骼重建（换镜/改装后）；
///  3) ThermalVision.OnPreCull —— 仅 Global 模式：在游戏把全局置 1 之后再压回 0。
///
/// 设计约束：
///  - 每个补丁独立 try/catch，任一失败不影响其余；
///  - 不改动任何文件/资源，仅内存态材质属性；
///  - 绝不把异常抛给游戏。
/// </summary>
public static class ThermalScopePatch
{
    private static Harmony _harmony;
    private static ManualLogSource _logger;

    private static void Log(string m) => _logger?.LogInfo("[ThermalScope] " + m);
    private static void Warn(string m) => _logger?.LogWarning("[ThermalScope] " + m);

    /// <summary>安装全部补丁。逐条独立，返回成功安装的数量。</summary>
    public static int Install(ManualLogSource logger)
    {
        _logger = logger;
        if (_harmony != null)
        {
            Log("补丁已安装过，跳过重复安装");
            return 0;
        }

        _harmony = new Harmony(PluginInfo.GUID + ".ThermalScope");
        int ok = 0;

        ok += TryPatch(
            typeof(ProceduralWeaponAnimation),
            nameof(ProceduralWeaponAnimation.InitTransforms),
            nameof(OnInitTransformsPostfix),
            "P1.InitTransforms");

        ok += TryPatch(
            typeof(ProceduralWeaponAnimation),
            nameof(ProceduralWeaponAnimation.FindAimTransforms),
            nameof(OnFindAimTransformsPostfix),
            "P2.FindAimTransforms");

        ok += TryPatch(
            typeof(ThermalVision),
            nameof(ThermalVision.OnPreCull),
            nameof(OnPreCullPostfix),
            "P3.ThermalVision.OnPreCull");

        Log($"补丁安装结果：{ok}/3 成功");
        return ok;
    }

    /// <summary>解析目标方法 → 加 Postfix，逐条报告。</summary>
    private static int TryPatch(Type targetType, string targetName, string postfixName, string tag)
    {
        try
        {
            var candidates = targetType
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.Name == targetName && m.DeclaringType == targetType)
                .ToArray();

            if (candidates.Length == 0)
            {
                Warn($"{tag}: 未找到目标方法 {targetType.Name}.{targetName}（该方法可能已被移除/改名）");
                return 0;
            }
            if (candidates.Length > 1)
            {
                Warn($"{tag}: 命中 {candidates.Length} 个同名重载，取第一个（" +
                     string.Join(" | ", candidates.Select(c => c.GetParameters().Length + " 参")) + "）");
            }

            var postfix = typeof(ThermalScopePatch).GetMethod(postfixName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (postfix == null)
            {
                Warn($"{tag}: 未找到后置方法 {postfixName}");
                return 0;
            }

            _harmony.Patch(candidates[0], postfix: new HarmonyMethod(postfix));
            Log($"{tag}: 安装成功 → {targetType.Name}.{candidates[0].Name}（{candidates[0].GetParameters().Length} 参）");
            return 1;
        }
        catch (Exception e)
        {
            Warn($"{tag}: 安装失败 {e.GetType().Name}: {e.Message}");
            return 0;
        }
    }

    // ---------- Postfix 实现 ----------

    private static void OnInitTransformsPostfix(ProceduralWeaponAnimation __instance)
    {
        ThermalScopeProbeService.Apply(__instance, "InitTransforms");
    }

    private static void OnFindAimTransformsPostfix(ProceduralWeaponAnimation __instance)
    {
        ThermalScopeProbeService.Apply(__instance, "FindAimTransforms");
    }

    private static void OnPreCullPostfix(ThermalVision __instance)
    {
        ThermalScopeProbeService.OnThermalPreCull(__instance);
    }
}
