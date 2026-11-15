using System.IO;

namespace MGModClient.Framework;

/// <summary>
/// 一种 SPT 安装布局规则：用「标记目录是否存在」判定当前是哪种布局，并给出 mods 根候选路径。
///
/// 维护约定：**新增/调整 SPT 版本支持时只改本文件的 <see cref="Rules"/>**，其它代码不动。
/// 判定不解析版本号（SPT 的 core.json 里只有 EFT 版本，没有 SPT 版本），只按目录布局判定，
/// 更稳、更快、不受服务端文件结构调整影响。
/// </summary>
public sealed class SptLayoutRule
{
    /// <summary>布局标识，仅用于日志与排障（如 spt4.1）。</summary>
    public string Id { get; }

    /// <summary>判定标记（相对游戏根目录）；null = 兜底规则（无条件命中）。</summary>
    public string MarkerDir { get; }

    /// <summary>mods 根候选（相对游戏根目录，按顺序取第一个存在的）。</summary>
    public string[] ModsRootCandidates { get; }

    public SptLayoutRule(string id, string markerDir, params string[] modsRootCandidates)
    {
        Id = id;
        MarkerDir = markerDir;
        ModsRootCandidates = modsRootCandidates ?? System.Array.Empty<string>();
    }

    public override string ToString() => $"{Id}(marker={MarkerDir ?? "-"})";
}

/// <summary>
/// SPT 布局探测：游戏根目录 → 布局规则 → mods 根绝对路径。
/// </summary>
public static class SptLayout
{
    /// <summary>
    /// 布局规则表（顺序即优先级）：
    ///  - spt4.1：SPT 4.1.x+ 服务器独立为 SPT_Runtime/（SPT_Server.exe 在 SPT_Runtime/ 内）
    ///  - spt4.0：SPT 4.0.x 的 SPT/ 布局（兼容 4.0.13）
    ///  - spt3.x：SPT 3.x（Aki）布局，mods 直接在游戏根
    ///  - flat  ：兜底
    /// 末尾的 "..\user\mods" 变体用于 cwd 不在游戏根时的兼容探测。
    /// </summary>
    public static readonly SptLayoutRule[] Rules =
    {
        new("spt4.1", "SPT_Runtime",
            "SPT_Runtime/user/mods",
            "SPT_Runtime\\user\\mods",
            "../SPT_Runtime/user/mods"),

        new("spt4.0", "SPT",
            "SPT/user/mods",
            "SPT\\user\\mods",
            "../SPT/user/mods"),

        new("spt3.x", "Aki_Data",
            "user/mods",
            "user\\mods"),

        new("flat", null,
            "user/mods",
            "user\\mods",
            "../user/mods"),
    };

    /// <summary>按规则表探测布局：返回第一个"标记目录存在"的规则；都不匹配则返回兜底规则。</summary>
    public static SptLayoutRule Detect(string gameRoot)
    {
        if (!string.IsNullOrEmpty(gameRoot))
        {
            foreach (var rule in Rules)
            {
                if (rule.MarkerDir == null) continue;
                if (Directory.Exists(Combine(gameRoot, rule.MarkerDir))) return rule;
            }
        }
        return Rules[Rules.Length - 1];
    }

    /// <summary>
    /// 解析 mods 根绝对路径：优先返回该布局下**已存在**的候选；都不存在时返回首选候选（供日志展示）。
    /// </summary>
    public static string ResolveModsRoot(string gameRoot, out string layoutId)
    {
        var rule = Detect(gameRoot);
        layoutId = rule.Id;

        if (string.IsNullOrEmpty(gameRoot) || rule.ModsRootCandidates.Length == 0)
            return null;

        foreach (var candidate in rule.ModsRootCandidates)
        {
            var dir = Combine(gameRoot, candidate);
            if (dir != null && Directory.Exists(dir)) return dir;
        }

        return Combine(gameRoot, rule.ModsRootCandidates[0]);
    }

    /// <summary>读取服务端版本号（仅用于日志；读不到返回 null，不影响探测）。</summary>
    public static string TryReadServerVersion(string gameRoot)
    {
        try
        {
            if (string.IsNullOrEmpty(gameRoot)) return null;
            var rule = Detect(gameRoot);
            var markerRoot = rule.MarkerDir == null ? gameRoot : Combine(gameRoot, rule.MarkerDir);
            if (markerRoot == null || !Directory.Exists(markerRoot)) return null;

            foreach (var exeName in new[] { "SPT.Server.exe", "SPT.Server.Linux", "Aki.Server.exe", "Aki.Server" })
            {
                var exe = Path.Combine(markerRoot, exeName);
                if (!File.Exists(exe)) continue;
                var ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(exe).FileVersion;
                if (!string.IsNullOrWhiteSpace(ver)) return ver;
            }
        }
        catch
        {
            // 版本号仅用于日志，任何异常都忽略
        }
        return null;
    }

    private static string Combine(string root, string relative)
    {
        try
        {
            return Path.GetFullPath(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)));
        }
        catch
        {
            return null;
        }
    }
}
