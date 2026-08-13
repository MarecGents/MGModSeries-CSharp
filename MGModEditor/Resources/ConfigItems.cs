namespace MGEditor.Resources;

public class ConfigItems
{
    public List<KeyValue> AIHealth { get; } = new()
    {
        new KeyValue{ Key=1, Value="x1"},
        new KeyValue{ Key=2, Value="x2"},
        new KeyValue{ Key=3, Value="x3"},
        new KeyValue{ Key=4, Value="x4"},
        new KeyValue{ Key=5, Value="x5"},

    };
    public List<KeyValue> AISpawnNumber { get; } = new()
    {
        new KeyValue{ Key=-10, Value="-10" },
        new KeyValue{ Key=-8, Value="-8" },
        new KeyValue{ Key=-4, Value="-4" },
        new KeyValue{ Key=-2, Value="-2" },
        new KeyValue{ Key=0, Value="0" },
        new KeyValue{ Key=2, Value="2" },
        new KeyValue{ Key=4, Value="4" },
        new KeyValue{ Key=8, Value="8" },
        new KeyValue{ Key=10, Value="10" },
    };
    public List<KeyValue> USECRate { get; } = (new()
    {
        new KeyValue{ Key=-1, ValueKey=Translations.ConfigItemDefaultRatio },
        new KeyValue{ Key=0, Value="0%" },
        new KeyValue{ Key=10, Value="10%" },
        new KeyValue{ Key=20, Value="20%" },
        new KeyValue{ Key=30, Value="30%" },
        new KeyValue{ Key=40, Value="40%" },
        new KeyValue{ Key=50, Value="50%" },
        new KeyValue{ Key=60, Value="60%" },
        new KeyValue{ Key=70, Value="70%" },
        new KeyValue{ Key=80, Value="80%" },
        new KeyValue{ Key=90, Value="90%" },
        new KeyValue{ Key=100, Value="100%" },
    });

    public List<KeyValue> aiAmount { get; } = new()
    {
        new KeyValue{ Key="AsOnline", ValueKey=Translations.ConfigItemAiAmountAsOnline },
        new KeyValue{ Key="NoBots", ValueKey=Translations.ConfigItemAiAmountNoBots },
        new KeyValue{ Key="Low", ValueKey=Translations.ConfigItemAiAmountLow },
        new KeyValue{ Key="Medium", ValueKey=Translations.ConfigItemAiAmountMedium },
        new KeyValue{ Key="High", ValueKey=Translations.ConfigItemAiAmountHigh },
        new KeyValue{ Key="Horde", ValueKey=Translations.ConfigItemAiAmountHorde },
    };

    public List<KeyValue> aiDifficulty { get; } = new()
    {
        new KeyValue{ Key="AsOnline", ValueKey=Translations.ConfigItemAiDifficultyAsOnline },
        new KeyValue{ Key="Easy", ValueKey=Translations.ConfigItemAiDifficultyEasy },
        new KeyValue{ Key="Medium", ValueKey=Translations.ConfigItemAiDifficultyMedium },
        new KeyValue{ Key="Hard", ValueKey=Translations.ConfigItemAiDifficultyHard },
        new KeyValue{ Key="Impossible", ValueKey=Translations.ConfigItemAiDifficultyImpossible },
        new KeyValue{ Key="Random", ValueKey=Translations.ConfigItemAiDifficultyRandom },
    };

    public List<KeyValue> RaidTime { get; } = new()
    {
        new KeyValue{ Key=-1.0, ValueKey=Translations.ConfigItemRaidTimeDefault },
        new KeyValue{ Key=30.0, ValueKey=Translations.ConfigItemRaidTime30m },
        new KeyValue{ Key=60.0, ValueKey=Translations.ConfigItemRaidTime1h },
        new KeyValue{ Key=90.0, ValueKey=Translations.ConfigItemRaidTime1h30 },
        new KeyValue{ Key=120.0, ValueKey=Translations.ConfigItemRaidTime2h },
        new KeyValue{ Key=180.0, ValueKey=Translations.ConfigItemRaidTime3h },
        new KeyValue{ Key=240.0, ValueKey=Translations.ConfigItemRaidTime4h },
    };

    public List<KeyValue> BOSSSpwanChance { get; } = new()
    {
        new KeyValue{ Key=-1.0, ValueKey=Translations.ConfigItemBossSpawnChanceDefault },
        new KeyValue{ Key=0.0, Value="0%"},
        new KeyValue{ Key=10.0, Value="10%"},
        new KeyValue{ Key=20.0, Value="20%"},
        new KeyValue{ Key=30.0, Value="30%"},
        new KeyValue{ Key=40.0, Value="40%"},
        new KeyValue{ Key=50.0, Value="50%"},
        new KeyValue{ Key=60.0, Value="60%"},
        new KeyValue{ Key=70.0, Value="70%"},
        new KeyValue{ Key=80.0, Value="80%"},
        new KeyValue{ Key=90.0, Value="90%"},
        new KeyValue{ Key=100.0, Value="100%"},
    };
    public List<KeyValue> AirdropType { get; } = new() {
        new KeyValue{ Key="default", ValueKey=Translations.ConfigItemAirdropTypeDefault },
        new KeyValue{ Key="moreWeapon", ValueKey=Translations.ConfigItemAirdropTypeMoreWeapon },
        new KeyValue{ Key="moreBarter", ValueKey=Translations.ConfigItemAirdropTypeMoreBarter },
        new KeyValue{ Key="moreFoodMedical", ValueKey=Translations.ConfigItemAirdropTypeMoreFoodMedical },
        new KeyValue{ Key="moreMixed", ValueKey=Translations.ConfigItemAirdropTypeMoreMixed },
    };
    
    public List<KeyValue> WeatherMode { get; } = new()
    {
        new KeyValue{ Key="default", ValueKey=Translations.ConfigItemWeatherModeDefault },
        new KeyValue{ Key="mode1", ValueKey=Translations.ConfigItemWeatherModeClear },
        new KeyValue{ Key="mode2", ValueKey=Translations.ConfigItemWeatherModeRainy },
        new KeyValue{ Key="mode3", ValueKey=Translations.ConfigItemWeatherModeStorm },
        new KeyValue{ Key="mode4", ValueKey=Translations.ConfigItemWeatherModeMisty },
        new KeyValue{ Key="mode5", ValueKey=Translations.ConfigItemWeatherModeExtreme },
        new KeyValue{ Key="Custom", ValueKey=Translations.ConfigItemWeatherModeCustom },
    };

    public List<KeyValue> CloudMode { get; } = new()
    {
        new KeyValue{ Key="default", ValueKey=Translations.ConfigItemCloudModeDefault },
        new KeyValue{ Key="mode1", ValueKey=Translations.ConfigItemCloudModeCloudless },
        new KeyValue{ Key="mode2", ValueKey=Translations.ConfigItemCloudModeFew },
        new KeyValue{ Key="mode3", ValueKey=Translations.ConfigItemCloudModePartly },
        new KeyValue{ Key="mode4", ValueKey=Translations.ConfigItemCloudModeOvercast },
    };
    public Dictionary<string, List<double>> CloudModeWeight { get; } = new()
    {
        { "default", new List<double> { 20, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
        { "mode1", new List<double> { 20, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  } },
        { "mode2", new List<double> { 0, 0, 20, 10, 10, 5, 0, 0, 0, 0, 0  } },
        { "mode3", new List<double> { 0, 0, 0, 0, 0, 10, 20, 20, 10, 0, 0  } },
        { "mode4", new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 20  } },
    };

    public List<KeyValue> WindMode { get; } = new()
    {
        new KeyValue{ Key="default", ValueKey=Translations.ConfigItemWindModeDefault },
        new KeyValue{ Key="mode1", ValueKey=Translations.ConfigItemWindModeNone },
        new KeyValue{ Key="mode2", ValueKey=Translations.ConfigItemWindModeBreeze },
        new KeyValue{ Key="mode3", ValueKey=Translations.ConfigItemWindModeStrong },
        new KeyValue{ Key="mode4", ValueKey=Translations.ConfigItemWindModeGale },
    };
    public Dictionary<string, List<double>> WindModeWeight { get; } = new()
    {
        { "default", new List<double> { 6, 3, 2, 1, 1 } },
        { "mode1", new List<double> { 10, 0, 0, 0, 0 } },
        { "mode2", new List<double> { 0, 6, 0, 0, 0 } },
        { "mode3", new List<double> { 0, 0, 3, 3, 0 } },
        { "mode4", new List<double> { 0, 0, 0, 0, 10 } },
    };

    public List<KeyValue> RainMode { get; } = new()
    {
        new KeyValue{ Key="default", ValueKey=Translations.ConfigItemRainModeDefault },
        new KeyValue{ Key="mode1", ValueKey=Translations.ConfigItemRainModeNone },
        new KeyValue{ Key="mode2", ValueKey=Translations.ConfigItemRainModeDrizzle },
        new KeyValue{ Key="mode3", ValueKey=Translations.ConfigItemRainModeLight },
        new KeyValue{ Key="mode4", ValueKey=Translations.ConfigItemRainModeHeavy },
        new KeyValue{ Key="mode5", ValueKey=Translations.ConfigItemRainModeDownpour },
    };
    public Dictionary<string, List<double>> RainModeWeight { get; } = new()
    {
        { "default", new List<double> { 1, 0, 0, 0, 0 } },
        { "mode1", new List<double> { 0, 0, 0, 0, 0 } },
        { "mode2", new List<double> { 0, 5, 0, 0, 0 } },
        { "mode3", new List<double> { 0, 5, 5, 0, 0 } },
        { "mode4", new List<double> { 0, 0, 10, 10, 0 } },
        { "mode5", new List<double> { 0, 0, 0, 0, 10 } },
    };

    public List<KeyValue> FogMode { get; } = new()
    {
        new KeyValue{ Key="default", ValueKey=Translations.ConfigItemFogModeDefault },
        new KeyValue{ Key="mode1", ValueKey=Translations.ConfigItemFogModeNone },
        new KeyValue{ Key="mode2", ValueKey=Translations.ConfigItemFogModeLight },
        new KeyValue{ Key="mode3", ValueKey=Translations.ConfigItemFogModeMisty },
        new KeyValue{ Key="mode4", ValueKey=Translations.ConfigItemFogModeHaze },
    };
    public Dictionary<string, List<double>> FogModeWeight { get; } = new()
    {
        { "default", new List<double> { 30, 6, 4, 3, 1 } },
        { "mode1", new List<double> { 35, 0, 0, 0, 0 } },
        { "mode2", new List<double> { 0, 35, 0, 0, 0 } },
        { "mode3", new List<double> { 0, 0, 35, 0, 0 } },
        { "mode4", new List<double> { 0, 0, 0, 35, 35 } },
    };

    public List<KeyValue> LootMultiple { get; } = new()
    {
        new KeyValue{ Key=0, Value="x0" },
        new KeyValue{ Key=1, Value="x1" },
        new KeyValue{ Key=2, Value="x2" },
        new KeyValue{ Key=3, Value="x3" },
        new KeyValue{ Key=5, Value="x5" },
        new KeyValue{ Key=8, Value="x8" },
        new KeyValue{ Key=10, Value="x10" },
        new KeyValue{ Key=15, Value="x15" },
        new KeyValue{ Key=20, Value="x20" },
        new KeyValue{ Key=30, Value="x30" },
        new KeyValue{ Key=50, Value="x50" },
    };

    public List<KeyValue> SellNumber { get; } = new()
    {
        new KeyValue{ Key=1, Value="x1" },
        new KeyValue{ Key=2, Value="x2" },
        new KeyValue{ Key=5, Value="x5" },
        new KeyValue{ Key=10, Value="x10" },
        new KeyValue{ Key=20, Value="x20" },
    };

    public List<KeyValue> UpdateTime { get; } = new()
    {
        new KeyValue{ Key=-1, ValueKey=Translations.ConfigItemUpdateTimeDefault },
        new KeyValue{ Key=3600, ValueKey=Translations.ConfigItemUpdateTime60m },
        new KeyValue{ Key=1800, ValueKey=Translations.ConfigItemUpdateTime30m },
        new KeyValue{ Key=600, ValueKey=Translations.ConfigItemUpdateTime10m },
        new KeyValue{ Key=300, ValueKey=Translations.ConfigItemUpdateTime5m },
    };

    public List<KeyValue> InsuranceTime { get; } = new()
    {
        new KeyValue{ Key=-1, ValueKey=Translations.ConfigItemInsuranceTimeDefault },
        new KeyValue{ Key=4, ValueKey=Translations.ConfigItemInsuranceTimeFast },
        new KeyValue{ Key=2, ValueKey=Translations.ConfigItemInsuranceTimeVeryFast },
        new KeyValue{ Key=0, ValueKey=Translations.ConfigItemInsuranceTimeInstant },
    };
    public List<KeyValue> InsuranceCost { get; } = new()
    {
        new KeyValue{ Key=-1.0, ValueKey=Translations.ConfigItemInsuranceCostDefault },
        new KeyValue{ Key=0.01, ValueKey=Translations.ConfigItemInsuranceCostVeryCheap },
        new KeyValue{ Key=0.15, ValueKey=Translations.ConfigItemInsuranceCostNormal },
        new KeyValue{ Key=0.40, ValueKey=Translations.ConfigItemInsuranceCostExpensive },
        new KeyValue{ Key=0.60, ValueKey=Translations.ConfigItemInsuranceCostVeryExpensive },
    };
    public List<KeyValue> ReturnChance { get; } = new()
    {
        new KeyValue{ Key=-1, ValueKey=Translations.ConfigItemReturnChanceDefault },
        new KeyValue{ Key=0, Value="0%" },
        new KeyValue{ Key=20, Value="20%" },
        new KeyValue{ Key=40, Value="40%" },
        new KeyValue{ Key=60, Value="60%" },
        new KeyValue{ Key=80, Value="80%" },
        new KeyValue{ Key=100, Value="100%" },
    };
    public List<KeyValue> LoadSpeed { get; } = new()
    {
        new KeyValue{ Key="default", ValueKey=Translations.ConfigItemLoadSpeedDefault },
        new KeyValue{ Key="mode1", ValueKey=Translations.ConfigItemLoadSpeedFast },
        new KeyValue{ Key="mode2", ValueKey=Translations.ConfigItemLoadSpeedVeryFast },
        new KeyValue{ Key="mode3", ValueKey=Translations.ConfigItemLoadSpeedLightning },
    };
    public List<KeyValue> MagazineCapacity { get; } = new()
    {
        new KeyValue{ Key=1, Value="x1" },
        new KeyValue{ Key=2, Value="x2" },
        new KeyValue{ Key=3, Value="x3" },
        new KeyValue{ Key=4, Value="x4" },
        new KeyValue{ Key=5, Value="x5" },
    };

    public List<KeyValue> ItemsDurability { get; } = new()
    {
        new KeyValue{ Key=1, Value="x1" },
        new KeyValue{ Key=2, Value="x2" },
        new KeyValue{ Key=4, Value="x4" },
        new KeyValue{ Key=5, Value="x5" },
        new KeyValue{ Key=8, Value="x8" },
        new KeyValue{ Key=10, Value="x10" },
    };

    public List<KeyValue> AmmoStack { get; } = new()
    {
        new KeyValue{ Key=1, Value="x1" },
        new KeyValue{ Key=2, Value="x2" },
        new KeyValue{ Key=5, Value="x5" },
        new KeyValue{ Key=10, Value="x10" },
        new KeyValue{ Key=20, Value="x20" },
        new KeyValue{ Key=50, Value="x50" },
        new KeyValue{ Key=100, Value="x100" },
        new KeyValue{ Key=500, Value="x500" },
        new KeyValue{ Key=1000, Value="x1000" },
    };

    public List<KeyValue> MoneyStack { get; } = new()
    {
        new KeyValue{ Key=1, Value="x1" },
        new KeyValue{ Key=5, Value="x5" },
        new KeyValue{ Key=10, Value="x10" },
        new KeyValue{ Key=50, Value="x50" },
        new KeyValue{ Key=100, Value="x100" },
    };

    public List<KeyValue> HideoutTime { get; } = new()
    {
        new KeyValue{ Key=-1.0, ValueKey=Translations.ConfigItemHideoutTimeDefault },
        new KeyValue{ Key=30.0, ValueKey=Translations.ConfigItemHideoutTime30s },
        new KeyValue{ Key=300.00, ValueKey=Translations.ConfigItemHideoutTime5m },
        new KeyValue{ Key=1200.0, ValueKey=Translations.ConfigItemHideoutTime20m },
        new KeyValue{ Key=3600.0, ValueKey=Translations.ConfigItemHideoutTime1h },
        new KeyValue{ Key=10800.0, ValueKey=Translations.ConfigItemHideoutTime3h },
    };

    public List<KeyValue> BonusesLevel { get; } = new()
    {
        new KeyValue{ Key=1, ValueKey=Translations.ConfigItemBonusesLevelDefault },
        new KeyValue{ Key=2, ValueKey=Translations.ConfigItemBonusesLevelDouble },
        new KeyValue{ Key=3, ValueKey=Translations.ConfigItemBonusesLevelQuintuple },
        new KeyValue{ Key=4, ValueKey=Translations.ConfigItemBonusesLevelDecuple },
    };
    
    public List<KeyValue> RewardMultiple { get; } = new()
    {
        new KeyValue{ Key=1, ValueKey=Translations.ConfigItemRewardMultipleDefault },
        new KeyValue { Key = 2, Value = "x2" },
        new KeyValue { Key = 3, Value = "x3" },
        new KeyValue { Key = 4, Value = "x4" },
        new KeyValue { Key = 5, Value = "x5" },
        new KeyValue { Key = 10, Value = "x10" },
    };

    public List<KeyValue> ContainerExpandWidth { get; } = Enumerable.Range(1, 20)
        .Select(i => new KeyValue { Key = i, ValueFormatKey = Translations.ConfigItemContainerCells })
        .ToList();
    public List<KeyValue> ContainerExpandHeight { get; } = Enumerable.Range(1, 16)
        .Select(i => new KeyValue { Key = i, ValueFormatKey = Translations.ConfigItemContainerCells })
        .ToList();

    public List<KeyValue> MapBotDifficulty { get; } = new()
    {
        new KeyValue { Key = "default", ValueKey = Translations.ConfigItemMapBotDifficultyDefault },
        new KeyValue { Key = "easy", ValueKey = Translations.ConfigItemMapBotDifficultyEasy },
        new KeyValue { Key = "normal", ValueKey = Translations.ConfigItemMapBotDifficultyNormal },
        new KeyValue { Key = "hard", ValueKey = Translations.ConfigItemMapBotDifficultyHard },
        new KeyValue { Key = "impossible", ValueKey = Translations.ConfigItemMapBotDifficultyImpossible },
    };
}

public class KeyValue
{
    public object Key { get; set; }
    public string? Value { get; set; }

    /// <summary>翻译键：非空时显示经 TranslationService 解析的文本（i18n）。</summary>
    public string? ValueKey { get; set; }

    /// <summary>含 {0} 占位符的翻译键：非空时用 Key 格式化显示（如 "ConfigItem.Container.Cells"）。</summary>
    public string? ValueFormatKey { get; set; }

}
