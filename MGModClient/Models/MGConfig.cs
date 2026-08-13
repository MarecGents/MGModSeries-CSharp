using System.Collections.Generic;

namespace MGModClient.Models;

/// <summary>
/// MGMod 配置模型（8 域）——客户端版镜像。
/// 与服务端 ConfigSettingType.cs 字段对齐（以服务端为事实源，落地方案 §九-3）。
/// 当前为空骨架：字段由后续编辑器功能按服务端模型填充。
/// </summary>
public class MGConfig
{
    public BotConfig Bot { get; set; } = new();
    public ConfigDomain Config { get; set; } = new();
    public GlobalsConfig Globals { get; set; } = new();
    public HideoutConfig Hideout { get; set; } = new();
    public LocationsConfig Locations { get; set; } = new();
    public TemplatesConfig Templates { get; set; } = new();
    public TradersConfig Traders { get; set; } = new();
    public MGCustomConfig MGCustom { get; set; } = new();
    public string saveTime { get; set; } = "";

    /// <summary>从磁盘反序列化的模型复制到本实例（字段级赋值）。</summary>
    public void CopyFrom(MGConfig other)
    {
        if (other == null) return;
        Bot = other.Bot ?? new();
        Config = other.Config ?? new();
        Globals = other.Globals ?? new();
        Hideout = other.Hideout ?? new();
        Locations = other.Locations ?? new();
        Templates = other.Templates ?? new();
        Traders = other.Traders ?? new();
        MGCustom = other.MGCustom ?? new();
        saveTime = other.saveTime;
    }
}

// ---- 8 域占位（字段后续按服务端模型填充）----
public class BotConfig { }
public class ConfigDomain { }
public class GlobalsConfig { }
public class HideoutConfig { }
public class LocationsConfig { }
public class TemplatesConfig { }
public class TradersConfig { }
public class MGCustomConfig { }

/// <summary>EnableValue 双值模型（enable + value，与桌面版同构）。</summary>
public class EnableValueInt
{
    public bool enable { get; set; }
    public int value { get; set; }
}

public class EnableValueDouble
{
    public bool enable { get; set; }
    public double value { get; set; }
}

public class EnableValueBool
{
    public bool enable { get; set; }
    public bool value { get; set; }
}
