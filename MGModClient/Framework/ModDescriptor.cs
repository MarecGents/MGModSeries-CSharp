using System;

namespace MGModClient.Framework;

/// <summary>
/// 服务端 mod 提供的能力位掩码。客户端各功能按能力筛选可用的 mod，而不是按目录名硬判。
/// </summary>
[Flags]
public enum ModCapabilities
{
    None = 0,

    /// <summary>提供 res/config/config.json（配置镜像 / 配置编辑主链路）。</summary>
    ConfigEditor = 1 << 0,

    /// <summary>提供 bundles/resources/（客户端资源，如 rig 布局预制体）。</summary>
    ClientResources = 1 << 1,

    /// <summary>提供 res/plugins/*.json（服务端插件产物，如热成像白名单）。</summary>
    ServerPlugins = 1 << 2,
}

/// <summary>
/// 逻辑 mod 描述：一个 mod 可以有多个目录名别名（源码名 / 发布名 / 历史名）。
/// 逻辑 Id 与目录名解耦 —— 客户端所有功能只依赖逻辑 Id，不依赖具体目录名。
/// </summary>
public sealed class ModDescriptor
{
    public string Id { get; }
    public string[] DirectoryAliases { get; }
    public ModCapabilities Capabilities { get; }

    public ModDescriptor(string id, string[] directoryAliases, ModCapabilities capabilities)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        DirectoryAliases = directoryAliases ?? Array.Empty<string>();
        Capabilities = capabilities;
    }

    public override string ToString() => $"{Id}({string.Join("|", DirectoryAliases)})";
}
