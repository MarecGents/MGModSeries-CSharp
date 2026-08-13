using System;
using System.IO;

namespace MGModClient.Services;

/// <summary>
/// 服务端 mod 存在性检测 —— 独立判断 MG-Mod / MGGTMod 是否安装，无前置关系。
/// 路径基准：SPT_Runtime/user/mods（游戏目录下）；可用 ModsDirOverride 覆盖（部署布局异常时）。
/// </summary>
public static class ModDetector
{
    /// <summary>SPT 客户端运行时 mod 目录（服务端 mod 解压于此）。</summary>
    private const string SptRuntimeModsDir = @"SPT_Runtime/user/mods";

    /// <summary>MGMod 目录名候选（源码名/发布名）。</summary>
    public static readonly string[] MGModDirNames = { "MGMod", "MGMod-CSharp" };

    /// <summary>MGGTMod 目录名候选。</summary>
    public static readonly string[] MGGTModDirNames = { "MGGTMod", "MGGTMod-CSharp" };

    /// <summary>可选覆盖：SPT_Runtime/user/mods 绝对路径。</summary>
    public static string ModsDirOverride;

    /// <summary>解析 mods 根目录（游戏目录为基准，可用覆盖）。返回 null 表示无法定位。</summary>
    public static string GetModsDir()
    {
        if (!string.IsNullOrWhiteSpace(ModsDirOverride))
            return Path.GetFullPath(ModsDirOverride);
        var gameDir = Directory.GetCurrentDirectory(); // EscapeFromTarkov.exe 所在
        return Path.GetFullPath(Path.Combine(gameDir, SptRuntimeModsDir));
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
}
