using System;
using System.IO;
using System.Linq;

namespace MGModClient.Framework;

/// <summary>
/// 定位服务端 mod 的配置镜像文件 res/config/config.json。
/// 候选来源为 <see cref="ModRegistry.Installed"/>（因此天然支持框架改名 / 多 mod 并存），
/// 不再维护"某某 mod 名 × 某种 SPT 布局"的候选路径矩阵。
/// </summary>
public sealed class ServerConfigLocator
{
    /// <summary>解析到的 config.json 绝对路径（null = 未找到）。</summary>
    public string ConfigPath { get; private set; }

    /// <summary>该文件所属的逻辑 mod Id（null = 来自手动路径覆盖）。</summary>
    public string ModId { get; private set; }

    /// <summary>最近一次解析的失败原因（供日志）。</summary>
    public string LastReason { get; private set; }

    /// <summary>
    /// 解析：手动路径优先；否则按已安装 mod 顺序找第一个存在的 config.json。
    /// </summary>
    public string Resolve(string manualPath)
    {
        ConfigPath = null;
        ModId = null;
        LastReason = null;

        try
        {
            if (!string.IsNullOrWhiteSpace(manualPath))
            {
                if (File.Exists(manualPath))
                {
                    ConfigPath = Path.GetFullPath(manualPath);
                    LastReason = "manual";
                    return ConfigPath;
                }
                LastReason = $"手动路径不存在: {manualPath}";
            }

            var installed = ModRegistry.Installed ?? Array.Empty<ModInstallation>();

            // ① 优先"声称具备配置能力"的 mod（Known 清单顺序即优先级）
            foreach (var mod in installed.Where(m => (m.Capabilities & ModCapabilities.ConfigEditor) != 0))
            {
                if (File.Exists(mod.ConfigFile))
                {
                    ConfigPath = mod.ConfigFile;
                    ModId = mod.Id;
                    LastReason = "capability";
                    return ConfigPath;
                }
            }

            // ② 兜底：任何已安装 mod 里存在的 config.json（自动发现的 mod 也能被识别）
            foreach (var mod in installed)
            {
                if (File.Exists(mod.ConfigFile))
                {
                    ConfigPath = mod.ConfigFile;
                    ModId = mod.Id;
                    LastReason = "fallback";
                    return ConfigPath;
                }
            }

            LastReason = $"已安装 {installed.Count} 个 mod，均未找到 res/config/config.json";
            return null;
        }
        catch (Exception ex)
        {
            LastReason = $"{ex.GetType().Name}: {ex.Message}";
            ConfigPath = null;
            ModId = null;
            return null;
        }
    }
}
