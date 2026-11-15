using System.IO;

namespace MGModClient.Framework;

/// <summary>
/// 一个「已安装」的 mod 实例：逻辑描述 + 实际目录名 + 绝对根路径。
/// 路径拼接统一走 <see cref="Res"/>，避免各调用点各写一套斜杠/子目录常量。
/// </summary>
public sealed class ModInstallation
{
    /// <summary>固定子目录/文件名（与 MGModServer / MGGTMod 的落盘约定一致）。</summary>
    public const string ResDirName = "res";
    public const string PluginsDirName = "plugins";
    public const string ConfigDirName = "config";
    public const string ConfigFileName = "config.json";
    public const string BundlesDirName = "bundles";
    public const string ResourcesDirName = "resources";

    public ModDescriptor Descriptor { get; }
    public string DirectoryName { get; }
    public string Root { get; }

    /// <summary>true = 由 marker 自动发现（目录名不在已知清单里，多为框架分叉/改名后的部署）。</summary>
    public bool IsAutoDiscovered { get; }

    public string Id => Descriptor.Id;
    public ModCapabilities Capabilities => Descriptor.Capabilities;

    public ModInstallation(ModDescriptor descriptor, string directoryName, string root, bool isAutoDiscovered)
    {
        Descriptor = descriptor;
        DirectoryName = directoryName;
        Root = root;
        IsAutoDiscovered = isAutoDiscovered;
    }

    /// <summary>拼接 mod 内相对路径（自动兼容 / 与 \ 分隔符）。</summary>
    public string Res(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return Root;
        return Path.Combine(Root, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }

    /// <summary>服务端配置镜像文件 res/config/config.json。</summary>
    public string ConfigFile => Res($"{ResDirName}/{ConfigDirName}/{ConfigFileName}");

    /// <summary>服务端插件产物目录 res/plugins。</summary>
    public string PluginsDir => Res($"{ResDirName}/{PluginsDirName}");

    /// <summary>某个服务端插件产物文件（如 scope_allow_in_thermal.json）。</summary>
    public string PluginsFile(string fileName) => Path.Combine(PluginsDir, fileName);

    /// <summary>客户端资源目录 bundles/resources。</summary>
    public string ClientResourcesDir => Res($"{BundlesDirName}/{ResourcesDirName}");

    /// <summary>用文件系统事实二次确认"是否真的具备该能力所需目录/文件"。</summary>
    public bool ActuallyProvides(ModCapabilities capability)
    {
        switch (capability)
        {
            case ModCapabilities.ConfigEditor: return File.Exists(ConfigFile);
            case ModCapabilities.ClientResources: return Directory.Exists(ClientResourcesDir);
            case ModCapabilities.ServerPlugins: return Directory.Exists(PluginsDir);
            default: return false;
        }
    }

    public override string ToString() =>
        $"{Id}(dir={DirectoryName}{(IsAutoDiscovered ? ",discovered" : "")},caps={Capabilities})";
}
