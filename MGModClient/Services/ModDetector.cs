using System;
using System.IO;

namespace MGModClient.Services;

/// <summary>
/// 服务端 mod 存在性检测 —— 独立判断 MG-Mod / MGGTMod 是否安装，无前置关系。
/// 路径基准：按候选顺序探测（C# SPT 服务器 SPT/user/mods 布局优先，标准 user/mods，仓库构建 SPT_Runtime/user/mods）；
/// </summary>
public static class ModDetector
{
    /// <summary>mods 目录候选（相对游戏目录）：SPT 4.1.x 起服务器目录为 SPT_Runtime/（主候选），其次兼容 4.0.x 的 SPT/，最后仓库构建输出布局。</summary>
    private static readonly string[] ModsDirCandidates =
    {
        "SPT_Runtime/user/mods", // SPT 4.1.x+ 服务器目录（SPT_Server.exe 在 SPT_Runtime/ 内，mod 在 SPT_Runtime/user/mods）
        "SPT/user/mods",         // SPT 4.0.x C# 服务器分布（兼容 4.0.13）
        "user/mods",             // 标准 SPT 安装布局（游戏目录 = SPT 根）
    };

    /// <summary>MGMod 目录名候选（源码名/发布名）。</summary>
    public static readonly string[] MGModDirNames = { "MGMod", "MGMod-CSharp" };

    /// <summary>MGGTMod 目录名候选。</summary>
    public static readonly string[] MGGTModDirNames = { "MGGTMod", "MGGTMod-CSharp" };

    /// <summary>可选覆盖：mods 目录绝对路径（检测自动失败时手动指定）。</summary>
    public static string ModsDirOverride;

    /// <summary>解析 mods 根目录（游戏目录为基准按候选顺序探测，可用覆盖）。返回第一个存在的候选；均不存在时返回首选候选路径。</summary>
    public static string GetModsDir()
    {
        if (!string.IsNullOrWhiteSpace(ModsDirOverride))
            return Path.GetFullPath(ModsDirOverride);
        var gameDir = Directory.GetCurrentDirectory(); // EscapeFromTarkov.exe 所在
        foreach (var candidate in ModsDirCandidates)
        {
            var dir = Path.GetFullPath(Path.Combine(gameDir, candidate));
            if (Directory.Exists(dir)) return dir;
        }
        return Path.GetFullPath(Path.Combine(gameDir, ModsDirCandidates[0]));
    }

    /// <summary>任一候选目录名存在即视为该 mod 已安装（独立判断）。</summary>
    public static bool IsModPresent(params string[] modDirNames)
    {
        var modsDir = GetModsDir();
        if (!Directory.Exists(modsDir)) return false;
        foreach (var name in modDirNames)
            if (Directory.Exists(Path.Combine(modsDir, name))) return true;
        return false;
    }

    /// <summary>MG-Mod 是否安装（任一候选名）。</summary>
    public static bool IsMGModPresent() => IsModPresent(MGModDirNames);

    /// <summary>MGGTMod 是否安装（任一候选名）。</summary>
    public static bool IsMGGTModPresent() => IsModPresent(MGGTModDirNames);

    /// <summary>解析已安装的 MGMod 目录名（用于拼 bundle 路径）。</summary>
    public static string GetInstalledMGModDir()
    {
        var modsDir = GetModsDir();
        if (!Directory.Exists(modsDir)) return null;
        foreach (var name in MGModDirNames)
            if (Directory.Exists(Path.Combine(modsDir, name))) return name;
        return null;
    }

    /// <summary>解析已安装的 MGGTMod 目录名（用于拼 bundle 路径）。</summary>
    public static string GetInstalledMGGTModDir()
    {
        var modsDir = GetModsDir();
        if (!Directory.Exists(modsDir)) return null;
        foreach (var name in MGGTModDirNames)
            if (Directory.Exists(Path.Combine(modsDir, name))) return name;
        return null;
    }

    /// <summary>获取 MGGTMod 的 bundle 完整路径（bundles/{分类子路径}/{bundle文件}）。</summary>
    public static string GetMGGTBundlePath(string bundleSubPath)
    {
        var dir = GetInstalledMGGTModDir();
        if (dir == null) return null;
        return Path.Combine(GetModsDir(), dir, "bundles", bundleSubPath);
    }

    /// <summary>
    /// 获取 MGGTMod 的 bundle 分类目录（bundles/{分类子目录}）。
    /// 返回 null 表示 MGGTMod 未安装；目录存在与否由调用方判断。
    /// </summary>
    public static string GetMGGTBundleDir(string bundleSubDir)
    {
        var dir = GetInstalledMGGTModDir();
        if (dir == null) return null;
        return Path.Combine(GetModsDir(), dir, "bundles", bundleSubDir);
    }

    /// <summary>
    /// 获取指定已安装 mod 的客户端资源目录（{modDir}/bundles/{ResourcesDirName}，如 bundles/resources）。
    /// 返回 null 表示该 mod 未安装；目录存在与否由调用方判断。
    /// </summary>
    public static string GetModClientResourcesDir(string installedModDirName)
    {
        if (string.IsNullOrWhiteSpace(installedModDirName)) return null;
        return Path.Combine(GetModsDir(), installedModDirName, "bundles", ClientResourceLoader.ResourcesDirName);
    }
}
