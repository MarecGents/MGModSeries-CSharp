using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using MGModClient.Framework;

namespace MGModClient.Services;

/// <summary>
/// 热成像瞄具白名单聚合器 —— 把「服务端插件产物」与「客户端手动配置」合成一份生效集合。
///
/// 数据来源：
///  ① 服务端：各 mod 的 `res/plugins/scope_allow_in_thermal.json`（由 CustomTraderServices 每次启动重写）
///     —— 这是**事实源**：物品是否允许热成像下使用由服务端配置决定，客户端只负责读取并合并。
///  ② 客户端：BepInEx 配置项 `ThermalScope AllowList`（手动追加，默认空；用于临时调试/未部署服务端时的验证）
///
/// 生效集合 = ① ∪ ②（忽略大小写去重）。
/// 热重载：按文件 LastWriteTimeUtc 缓存判断，服务端改配置后无需重启游戏，下一把生效。
/// </summary>
public static class ThermalScopeAllowListProvider
{
    private static readonly HashSet<string> Manual = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> Server = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, DateTime> FileStamps = new(StringComparer.OrdinalIgnoreCase);

    private static IReadOnlyCollection<string> _effective = Array.Empty<string>();
    private static ManualLogSource _logger;
    private static bool _useServerPlugins = true;

    /// <summary>生效集合（手动 ∪ 服务端，去重）。</summary>
    public static IReadOnlyCollection<string> Effective => _effective;

    public static int ManualCount => Manual.Count;
    public static int ServerCount => Server.Count;

    /// <summary>生效集合发生变化时触发（初始化、配置变更、文件热重载都会触发）。</summary>
    public static event Action<IReadOnlyCollection<string>> Changed;

    public static void Init(ManualLogSource logger)
    {
        _logger = logger;
        Manual.Clear();
        Server.Clear();
        FileStamps.Clear();
        _effective = Array.Empty<string>();
    }

    /// <summary>设置手动追加列表（逗号/分号/空白分隔；空 = 不追加）。</summary>
    public static void SetManual(string raw)
    {
        Manual.Clear();
        foreach (var id in ThermalScopeProbeService.ParseIdList(raw)) Manual.Add(id);
        Log($"手动追加白名单: {Manual.Count} 条");
        Recompute();
    }

    /// <summary>是否启用服务端插件列表。</summary>
    public static void SetUseServerPlugins(bool enabled)
    {
        if (_useServerPlugins == enabled) return;
        _useServerPlugins = enabled;
        Log($"服务端插件列表: {(enabled ? "启用" : "禁用")}");
        ReloadServerPlugins(force: true);
    }

    /// <summary>读取并合并所有已装 mod 的插件清单。返回 true 表示生效集合发生变化。</summary>
    public static bool ReloadServerPlugins(bool force = false)
    {
        if (!_useServerPlugins)
        {
            Server.Clear();
            FileStamps.Clear();
            return Recompute();
        }

        var mods = ModRegistry.With(ModCapabilities.ServerPlugins).ToList();
        if (mods.Count == 0)
        {
            Warn("未识别到提供服务端插件的 mod（ServerPlugins 能力），服务端白名单为空");
            Server.Clear();
            FileStamps.Clear();
            return Recompute();
        }

        // 先比对文件时间戳，避免每次进战局都重复读盘
        var current = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        foreach (var mod in mods)
        {
            var path = mod.PluginsFile(ModPluginFile.ScopeAllowInThermal);
            current[path] = SafeGetLastWriteUtc(path);
        }

        if (!force && SameStamps(current))
        {
            return false; // 无变化
        }

        Server.Clear();
        FileStamps.Clear();
        foreach (var kv in current) FileStamps[kv.Key] = kv.Value;

        var aggregated = ModPluginFile.ReadMerged(mods, ModPluginFile.ScopeAllowInThermal);
        var parts = new List<string>();
        foreach (var one in aggregated.PerMod)
        {
            switch (one.Status)
            {
                case "ok":
                    parts.Add($"{one.ModId}={one.Ids.Count}");
                    break;
                case "missing":
                    parts.Add($"{one.ModId}=文件缺失");
                    Log($"[{one.ModId}] 未找到插件文件（该 mod 未部署新版本？）: {one.FilePath}");
                    break;
                case "empty":
                    parts.Add($"{one.ModId}=空");
                    break;
                default:
                    parts.Add($"{one.ModId}=非法");
                    Warn($"[{one.ModId}] 插件文件解析失败: {one.Detail} @ {one.FilePath}");
                    break;
            }
        }

        foreach (var id in aggregated.Ids) Server.Add(id);
        Log($"服务端插件白名单: {string.Join(", ", parts)} → 合计 {Server.Count} 条");
        return Recompute();
    }

    /// <summary>文件时间戳变化才重读（供进战局前调用，开销≈几次 stat）。</summary>
    public static bool ReloadIfFilesChanged() => ReloadServerPlugins(force: false);

    // ==================== 内部实现 ====================

    private static bool Recompute()
    {
        var merged = new HashSet<string>(Manual, StringComparer.OrdinalIgnoreCase);
        merged.UnionWith(Server);

        var ordered = merged.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
        var changed = !ordered.SequenceEqual(_effective, StringComparer.OrdinalIgnoreCase);
        _effective = ordered;

        if (changed)
        {
            Log($"生效白名单更新: 手动={Manual.Count} + 服务端={Server.Count} → 合计 {ordered.Count}");
            try { Changed?.Invoke(_effective); }
            catch (Exception ex) { Warn($"生效集合回调异常: {ex.GetType().Name}: {ex.Message}"); }
        }
        return changed;
    }

    private static bool SameStamps(Dictionary<string, DateTime> current)
    {
        if (current.Count != FileStamps.Count) return false;
        foreach (var kv in current)
        {
            if (!FileStamps.TryGetValue(kv.Key, out var old) || old != kv.Value) return false;
        }
        return true;
    }

    private static DateTime SafeGetLastWriteUtc(string path)
    {
        try { return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue; }
        catch { return DateTime.MinValue; }
    }

    private static void Log(string message) => _logger?.LogInfo("[ThermalScope] " + message);

    private static void Warn(string message) => _logger?.LogWarning("[ThermalScope] " + message);
}
