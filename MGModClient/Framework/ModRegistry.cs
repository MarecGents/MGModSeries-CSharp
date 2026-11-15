using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MGModClient.Framework;

/// <summary>
/// 服务端 mod 识别与解析（客户端唯一的事实源）。
///
/// 识别分三层，逐层兜底：
///  ① <see cref="Known"/> —— 硬编码逻辑清单（**唯一需要改的地方**；框架分叉者只改这里）
///  ② Extra Mod Directories（BepInEx 配置项）—— 零代码接入，适合临时/第三方部署
///  ③ marker 自动发现 —— 扫描 mods 根下一层，按特征文件打分，兼容"框架被改名后重新编译"的部署
///
/// 关于路径：SPT 各版本布局差异由 <see cref="SptLayout"/> 单独维护，本类不出现任何布局路径。
/// </summary>
public static class ModRegistry
{
    // ══════════════════════════════════════════════════════════════════════
    //  ★ 唯一需要修改的地方（框架分叉 / 新增同框架 mod 时只改这个数组）★
    // ══════════════════════════════════════════════════════════════════════
    public static readonly ModDescriptor[] Known =
    {
        new(
            id: "MGMod",
            directoryAliases: new[] { "MGMod", "MGMod-CSharp" },
            capabilities: ModCapabilities.ConfigEditor
                        | ModCapabilities.ClientResources
                        | ModCapabilities.ServerPlugins),

        new(
            id: "MGGTMod",
            directoryAliases: new[] { "MGGTMod", "MGGTMod-CSharp" },
            capabilities: ModCapabilities.ClientResources
                        | ModCapabilities.ServerPlugins),
    };
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>marker 打分达到该阈值即认定为同框架 mod（误判无害：只会在该目录里找已知文件）。</summary>
    public const int MarkerScoreThreshold = 2;

    private static readonly List<ModInstallation> _installed = new();

    /// <summary>已解析的 mod 列表（初始化后只读）。</summary>
    public static IReadOnlyList<ModInstallation> Installed => _installed;

    /// <summary>游戏根目录（EscapeFromTarkov.exe 所在）。</summary>
    public static string GameRoot { get; private set; }

    /// <summary>mods 根绝对路径。</summary>
    public static string ModsRoot { get; private set; }

    /// <summary>命中/兜底的布局标识（spt4.1 / spt4.0 / spt3.x / flat / override）。</summary>
    public static string LayoutId { get; private set; }

    public static bool Resolved { get; private set; }

    private static Action<string> _log;
    private static Action<string> _warn;

    /// <summary>
    /// 解析已安装 mod（幂等：重复调用会重新解析）。
    /// </summary>
    /// <param name="modsRootOverride">配置覆盖的 mods 根（空 = 自动探测布局）</param>
    /// <param name="extraDirNames">配置追加的目录名（逗号/分号/空白分隔）</param>
    /// <param name="autoDiscover">是否启用 marker 自动发现</param>
    public static void Resolve(
        string modsRootOverride = null,
        string extraDirNames = null,
        bool autoDiscover = true,
        Action<string> log = null,
        Action<string> warn = null)
    {
        _log = log;
        _warn = warn;
        _installed.Clear();
        Resolved = false;

        GameRoot = SafeGetCurrentDirectory();
        Log($"游戏根目录: {GameRoot}");

        // ---------- ① mods 根 ----------
        if (!string.IsNullOrWhiteSpace(modsRootOverride))
        {
            ModsRoot = SafeGetFullPath(modsRootOverride);
            LayoutId = "override";
            Log($"使用配置覆盖的 mods 根: {ModsRoot}");
        }
        else
        {
            ModsRoot = SptLayout.ResolveModsRoot(GameRoot, out var layout);
            LayoutId = layout;
            Log($"布局探测: layout={LayoutId}, mods 根候选→{ModsRoot}");
        }

        var serverVersion = SptLayout.TryReadServerVersion(GameRoot);
        if (!string.IsNullOrWhiteSpace(serverVersion))
            Log($"服务端版本（日志用途）: {serverVersion}");

        if (string.IsNullOrEmpty(ModsRoot) || !Directory.Exists(ModsRoot))
        {
            Warn($"mods 根不存在，未识别任何 mod: {ModsRoot}（可用配置项 Mods Root Override 指定）");
            Resolved = true;
            return;
        }

        // ---------- ② 已知清单（按别名精确匹配） ----------
        foreach (var descriptor in Known)
        {
            var hit = descriptor.DirectoryAliases
                .Select(alias => new { alias, root = Path.Combine(ModsRoot, alias) })
                .FirstOrDefault(x => Directory.Exists(x.root));

            if (hit == null)
            {
                Log($"未安装: {descriptor.Id}（别名: {string.Join("/", descriptor.DirectoryAliases)}）");
                continue;
            }

            TryAdd(new ModInstallation(descriptor, hit.alias, hit.root, isAutoDiscovered: false));
        }

        // ---------- ③ 配置追加的目录名 ----------
        foreach (var extra in SplitList(extraDirNames))
        {
            var root = Path.Combine(ModsRoot, extra);
            if (!Directory.Exists(root))
            {
                Warn($"配置项 Extra Mod Directories 指定的目录不存在: {root}");
                continue;
            }
            TryAdd(new ModInstallation(BuildSyntheticDescriptor(extra, root), extra, root, isAutoDiscovered: false));
        }

        // ---------- ④ marker 自动发现 ----------
        if (autoDiscover)
        {
            AutoDiscover();
        }
        else
        {
            Log("marker 自动发现已关闭（Auto Discover Mods=false）");
        }

        Resolved = true;
        Log($"识别完成: {Describe()}");
    }

    /// <summary>指定逻辑 mod 是否已安装。</summary>
    public static bool IsInstalled(string modId) => Get(modId) != null;

    /// <summary>按逻辑 Id 取已安装实例；未安装返回 null。</summary>
    public static ModInstallation Get(string modId)
    {
        if (string.IsNullOrWhiteSpace(modId)) return null;
        return _installed.FirstOrDefault(m => string.Equals(m.Id, modId, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>按目录名取已安装实例；未安装返回 null。</summary>
    public static ModInstallation GetByDirectoryName(string dirName)
    {
        if (string.IsNullOrWhiteSpace(dirName)) return null;
        return _installed.FirstOrDefault(m => string.Equals(m.DirectoryName, dirName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>筛选"声称具备某能力"的 mod（是否真的具备由调用方用 <see cref="ModInstallation.ActuallyProvides"/> 确认）。</summary>
    public static IEnumerable<ModInstallation> With(ModCapabilities capability)
        => _installed.Where(m => (m.Capabilities & capability) == capability);

    /// <summary>一行摘要（供启动日志）。</summary>
    public static string Describe()
    {
        if (_installed.Count == 0) return "无可识别的同框架 mod";
        return string.Join(", ", _installed.Select(m =>
            $"{m.Id}[{m.DirectoryName}{(m.IsAutoDiscovered ? ",auto" : "")}]"));
    }

    // ==================== 内部实现 ====================

    private static void AutoDiscover()
    {
        string[] dirs;
        try
        {
            dirs = Directory.GetDirectories(ModsRoot);
        }
        catch (Exception e)
        {
            Warn($"扫描 mods 根失败: {e.GetType().Name}: {e.Message}");
            return;
        }

        int found = 0;
        foreach (var dir in dirs)
        {
            var dirName = Path.GetFileName(dir);
            if (string.IsNullOrEmpty(dirName)) continue;

            // 已被 ②③ 收录 → 跳过
            if (_installed.Any(m => string.Equals(m.DirectoryName, dirName, StringComparison.OrdinalIgnoreCase)))
                continue;

            var score = ScoreMarkers(dir, out var hits);
            if (score < MarkerScoreThreshold)
            {
                Log($"自动发现跳过: {dirName}（marker 分数 {score} < {MarkerScoreThreshold}{(hits != null ? "，命中: " + hits : "")}）");
                continue;
            }

            TryAdd(new ModInstallation(BuildSyntheticDescriptor(dirName, dir), dirName, dir, isAutoDiscovered: true));
            Log($"自动发现同框架 mod: {dirName}（marker 分数 {score}，命中: {hits}）");
            found++;
        }

        Log($"自动发现结束: 新增 {found} 个");
    }

    /// <summary>
    /// marker 打分（只看存在性，不读文件内容）：
    ///   res/plugins/scope_allow_in_thermal.json  +3（框架插件产物，最精确）
    ///   res/services/TraderDB（目录）             +2（框架服务结构）
    ///   traders/（目录）+ bundles.json（文件）     +2（框架商人布局）
    ///   根目录 *.staticwebassets.endpoints.json    +1（框架构建签名）
    /// </summary>
    private static int ScoreMarkers(string modRoot, out string hits)
    {
        var matched = new List<string>();
        int score = 0;

        if (File.Exists(Path.Combine(modRoot, "res", "plugins", "scope_allow_in_thermal.json")))
        {
            score += 3;
            matched.Add("plugins/scope_allow_in_thermal.json");
        }
        if (Directory.Exists(Path.Combine(modRoot, "res", "services", "TraderDB")))
        {
            score += 2;
            matched.Add("res/services/TraderDB");
        }
        if (Directory.Exists(Path.Combine(modRoot, "traders"))
            && File.Exists(Path.Combine(modRoot, "bundles.json")))
        {
            score += 2;
            matched.Add("traders+bundles.json");
        }
        try
        {
            if (Directory.GetFiles(modRoot, "*.staticwebassets.endpoints.json").Length > 0)
            {
                score += 1;
                matched.Add("staticwebassets");
            }
        }
        catch
        {
            // 无权访问等异常忽略该 marker
        }

        hits = matched.Count > 0 ? string.Join(", ", matched) : null;
        return score;
    }

    /// <summary>
    /// 为"非已知清单"的 mod 构造描述符：能力按**文件系统事实**推断，避免虚报能力。
    /// </summary>
    private static ModDescriptor BuildSyntheticDescriptor(string id, string root)
    {
        var caps = ModCapabilities.None;

        if (File.Exists(Path.Combine(root, "res", "config", "config.json")))
            caps |= ModCapabilities.ConfigEditor;
        if (Directory.Exists(Path.Combine(root, "bundles", "resources")))
            caps |= ModCapabilities.ClientResources;
        if (Directory.Exists(Path.Combine(root, "res", "plugins")))
            caps |= ModCapabilities.ServerPlugins;

        // marker 命中的框架 mod 至少按"资源 + 插件"对待（文件缺失时由各功能自行降级）
        if (caps == ModCapabilities.None)
            caps = ModCapabilities.ClientResources | ModCapabilities.ServerPlugins;

        return new ModDescriptor(id, new[] { id }, caps);
    }

    private static void TryAdd(ModInstallation installation)
    {
        if (installation == null || string.IsNullOrEmpty(installation.Root)) return;
        if (_installed.Any(m => string.Equals(m.Root, installation.Root, StringComparison.OrdinalIgnoreCase)))
            return;
        _installed.Add(installation);
    }

    private static IEnumerable<string> SplitList(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) yield break;
        foreach (var part in raw.Split(new[] { ',', ';', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var v = part.Trim().Trim('"', '\'');
            if (v.Length > 0) yield return v;
        }
    }

    private static string SafeGetCurrentDirectory()
    {
        try { return Directory.GetCurrentDirectory(); }
        catch { return string.Empty; }
    }

    private static string SafeGetFullPath(string path)
    {
        try { return Path.GetFullPath(path); }
        catch { return path; }
    }

    private static void Log(string message) => _log?.Invoke("[ModRegistry] " + message);

    private static void Warn(string message) => _warn?.Invoke("[ModRegistry] " + message);
}
