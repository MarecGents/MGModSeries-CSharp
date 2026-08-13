using System.IO;

namespace MGModClient.Services;

/// <summary>
/// 定位 server 侧 res/config/config.json（落地方案 §3.1）。
/// 候选路径按 SPT 4.x 典型布局枚举（server 与游戏同级 / Aki_Data / SPT_Data）。
/// </summary>
public class PathLocator
{
    private const string Rel = @"res\config\config.json";

    /// <summary>候选：游戏目录（cwd）为基准的相对路径。</summary>
    private static readonly string[] Candidates =
    {
        @"SPT_Runtime\user\mods\MGMod\" + Rel,
        @"SPT_Runtime\user\mods\MGMod-CSharp\" + Rel,
        @"SPT_Runtime\user\mods\MGGTMod\" + Rel,
        @"SPT_Runtime\user\mods\MGGTMod-CSharp\" + Rel,
        @"..\user\mods\MGMod\" + Rel,
        @"..\user\mods\MGMod-CSharp\" + Rel,
        @"..\user\mods\MGGTMod\" + Rel,
        @"..\user\mods\MGGTMod-CSharp\" + Rel,
    };

    public string ConfigPath { get; private set; }

    /// <summary>解析配置路径：手动路径优先，否则探测候选；失败返回 null（不崩溃）。</summary>
    public string Resolve(string manualPath)
    {
        if (!string.IsNullOrWhiteSpace(manualPath) && File.Exists(manualPath))
            return ConfigPath = Path.GetFullPath(manualPath);

        var cwd = Directory.GetCurrentDirectory(); // 游戏根目录（EscapeFromTarkov.exe 所在）
        foreach (var cand in Candidates)
        {
            var p = Path.GetFullPath(Path.Combine(cwd, cand));
            if (File.Exists(p)) return ConfigPath = p;
        }
        return ConfigPath = null;
    }
}
