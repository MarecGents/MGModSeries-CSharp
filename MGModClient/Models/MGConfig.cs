using System.Collections.Generic;

namespace MGModClient.Models;

/// <summary>
/// MGMod 配置模型（8 域）——客户端版镜像。
/// 字段与服务端 ConfigSettingType.cs 完全对齐（以服务端为事实源，落地方案 §九-3）。
/// 补齐全部字段后，JsonUtils.Read/WriteAtomic 可无损往返 config.json，避免写回时清空配置。
/// 注意：以下字段名与 JSON key 一一对应（含服务端历史拼写），不得改名。
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

// ---- 8 域（与服务端 ConfigSettingType 逐字段对齐）----

public class BotConfig
{
    public int AIHealth { get; set; }
    public BotSystemConfig BotSystem { get; set; }
}

public class BotSystemConfig
{
    public BotBrainConfig BotBrain { get; set; }
    public string EquipmentQuality { get; set; }
    public bool BotNameAdd { get; set; }
}

public class BotBrainConfig
{
    public bool enable { get; set; }
    public string type { get; set; }
    public Dictionary<string, string> customBotBrain { get; set; }
}

public class ConfigDomain
{
    public string AirdropType { get; set; }
    public int AISpawnNumber { get; set; }
    public RaidDefaultConfig RaidDefault { get; set; }
    public EnableValueInt ReturnChance { get; set; }
    public bool BuyFoundInRaid { get; set; }
    public LootMultipleConfig LootMultiple { get; set; }
    public bool RandomContainer { get; set; }
    public EnableValueInt USECRate { get; set; }
    public bool Sell100 { get; set; }
    public bool SellFast { get; set; }
    public bool SellOptimize { get; set; }
    public bool SellNew { get; set; }
    public bool NoBlackList { get; set; }
    public BuffsConfig Buffs { get; set; }
    public EnableValueInt UpdateTime { get; set; }
    public WeatherSettingsConfig WeatherSettings { get; set; }
    public bool NoLostonDeath { get; set; }
    public bool ScavEquipmentOptimize { get; set; }
    public ConfigBotSystemConfig BotSystem { get; set; }
    public Dictionary<string, bool> GiftsAdd { get; set; }
}

public class RaidDefaultConfig
{
    public bool enable { get; set; }
    public string aiAmount { get; set; }
    public string aiDifficulty { get; set; }
    public bool bossEnabled { get; set; }
    public bool scavWars { get; set; }
    public bool taggedAndCursed { get; set; }
    public bool enablePve { get; set; }
    public bool randomWeather { get; set; }
    public bool randomTime { get; set; }
}

public class LootMultipleConfig
{
    public int Container { get; set; }
    public int Ground { get; set; }
}

public class BuffsConfig
{
    public bool BuffsWeapon { get; set; }
    public bool BuffsArmor { get; set; }
}

public class WeatherSettingsConfig
{
    public string mode { get; set; }
    public WeatherConfig clouds { get; set; }
    public WeatherConfig windSpeed { get; set; }
    public MinMax<double> rainIntensity { get; set; }
    public WeatherConfig rain { get; set; }
    public WeatherConfig fog { get; set; }
}

public class WeatherConfig
{
    public string type { get; set; }
    public List<double> values { get; set; }
    public List<double> weights { get; set; }
}

public class ConfigBotSystemConfig
{
    public string PmcWavesOptimize { get; set; }
}

public class GlobalsConfig
{
    public bool EscapeNoTimeLimit { get; set; }
    public bool FleaMarketOpenLevel { get; set; }
    public bool TakeLimit { get; set; }
    public bool ScavOptimize { get; set; }
    public bool LowTaxRate { get; set; }
    public int SellNumber { get; set; }
    public LoadSpeedConfig LoadSpeed { get; set; }
    public bool SuperHero { get; set; }
    public LootMultiplierConfig LootMultiplier { get; set; }
    public bool ArmorRepairPerfect { get; set; }
    public GlobalsBuffsConfig Buffs { get; set; }
    public bool ExpOptimize { get; set; }
}

public class LoadSpeedConfig
{
    public string mode { get; set; }
    public double BaseLoadTime { get; set; }
    public double BaseUnloadTime { get; set; }
}

public class LootMultiplierConfig
{
    public int Value { get; set; }
    public int Global { get; set; }
}

public class GlobalsBuffsConfig
{
    public bool BuffsWeapon { get; set; }
    public bool BuffsArmor { get; set; }
}

public class HideoutConfig
{
    public EnableValueDouble BuildTime { get; set; }
    public EnableValueDouble ProductTime { get; set; }
    public EnableValueDouble ScavCaseTime { get; set; }
    public bool UpgradeNoLimit { get; set; }
    public EnableValueInt BonusesLevel { get; set; }
    public bool NoNeedsFuel { get; set; }
    public QteConfig Qte { get; set; }
}

public class QteConfig
{
    public bool Sucess100 { get; set; }
    public bool NoPunish { get; set; }
    public EnableValueInt RewardMultiple { get; set; }
}

public class LocationsConfig
{
    public EnableValueDouble RaidTime { get; set; }
    public EnableValueDouble BOSSSpwanChance { get; set; }
    public bool Pass100 { get; set; }
    public bool Escape100 { get; set; }
    public Dictionary<string, bool> MapInsurance { get; set; }
    public LocationsBotSystemConfig BotSystem { get; set; }
}

public class LocationsBotSystemConfig
{
    public string ScavWavesOptimize { get; set; }
    public bool MapRefershConfig { get; set; }
    public bool PmcTacticalSquad { get; set; }
    public string MapBotDifficulty { get; set; }
}

public class TemplatesConfig
{
    public bool Examined { get; set; }
    public bool WeaponFilter { get; set; }
    public int AmmoStack { get; set; }
    public bool AmmoInfo { get; set; }
    public Dictionary<string, ContainerExpandsConfig> ContainerExpand { get; set; }
    public SafesConfig Safes { get; set; }
    public int MoneyStack { get; set; }
    public BackpackConfig Backpack { get; set; }
    public ArmorHelmetConfig Armor { get; set; }
    public ArmorHelmetConfig Helmet { get; set; }
    public EquipmentPlateConfig EquipmentPlate { get; set; }
    public bool KeysDurability { get; set; }
    public int MedcDurability { get; set; }
    public bool WeaponNoLost { get; set; }
    public bool WeaponRepairPerfect { get; set; }
    public int MagazineCapacity { get; set; }
    public bool T7ThermalImaging { get; set; }
    public bool ResetFree { get; set; }
    public QuestSystemConfig QuestSystem { get; set; }
    public bool PMCRoar { get; set; }
}

public class ContainerExpandsConfig
{
    public bool enable { get; set; }
    public string Name { get; set; }
    public int cellsH { get; set; }
    public int cellsV { get; set; }
    public bool Weight { get; set; }
    public bool Filter { get; set; }
}

public class SafesConfig
{
    public bool SizeExpand { get; set; }
    public bool Filter { get; set; }
    public bool NoWeight { get; set; }
}

public class BackpackConfig
{
    public bool Filter { get; set; }
    public bool SmallSize { get; set; }
    public bool NoBuff { get; set; }
    public bool NoWeight { get; set; }
}

public class ArmorHelmetConfig
{
    public bool Filter { get; set; }
    public bool NoBuff { get; set; }
    public bool NoWeight { get; set; }
}

public class EquipmentPlateConfig
{
    public int Durability { get; set; }
    public bool NoBuff { get; set; }
    public bool NoWeight { get; set; }
}

public class QuestSystemConfig
{
    public bool QuestOptimize { get; set; }
    public bool Quest3X4Marker { get; set; }
}

public class TradersConfig
{
    public EnableValueInt InsuranceTime { get; set; }
    public EnableValueDouble InsuranceCost { get; set; }
}

public class MGCustomConfig
{
    public bool CustomTrader { get; set; }
    public bool CustomItem { get; set; }
    public bool CustomAssort { get; set; }
    public bool CustomProfile { get; set; }
    public bool CustomBoss { get; set; }
    public bool KeyClassfy { get; set; }
    public bool SyncFlea { get; set; }
    public SeasonalActivityConfig SeasonalActivity { get; set; }
}

public class SeasonalActivityConfig
{
    public bool enable { get; set; }
    public Dictionary<string, bool> AcitvitiesSwitch { get; set; }
    public Dictionary<string, bool> NewActivitiesSwitch { get; set; }
}

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

/// <summary>MinMax 双值模型（与服务端/桌面版同构）。</summary>
public class MinMax<T>
{
    public T Min { get; set; }
    public T Max { get; set; }
}
