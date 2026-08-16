namespace MGEditor.Resources;

/// <summary>
/// i18n 翻译键常量（与 res/i18n/*.json 的键一一对应，XAML/C# 共用，防手写字符串漂移）。
/// 新增文本时：先在语言包 JSON 添加键，再在此添加同值常量。
/// </summary>
public static class Translations
{
    // ---- Common ----
    public const string CommonYes = "Common.Yes";
    public const string CommonNo = "Common.No";
    public const string CommonSearch = "Common.Search";

    // ---- Nav ----
    public const string NavHome = "Nav.Home";
    public const string NavRaid = "Nav.Raid";
    public const string NavEconomy = "Nav.Economy";
    public const string NavDevelop = "Nav.Develop";
    public const string NavContainer = "Nav.Container";
    public const string NavFeature = "Nav.Feature";
    public const string NavSettings = "Nav.Settings";
    public const string NavTrayHome = "Nav.TrayHome";

    // ---- Settings ----
    public const string SettingsTitle = "Settings.Title";
    public const string SettingsHeaderPersonal = "Settings.HeaderPersonal";
    public const string SettingsButtonTheme = "Settings.ButtonTheme";
    public const string SettingsLanguage = "Settings.Language";
    public const string SettingsHeaderAbout = "Settings.HeaderAbout";
    public const string SettingsHeaderCredits = "Settings.HeaderCredits";
    public const string SettingsCreditsText = "Settings.CreditsText";
    public const string SettingsThemeLight = "Settings.ThemeLight";
    public const string SettingsThemeDark = "Settings.ThemeDark";
    public const string SettingsThemeHighContrast = "Settings.ThemeHighContrast";
    public const string SettingsLinkOddba = "Settings.LinkOddba";
    public const string SettingsLinkAifadian = "Settings.LinkAifadian";
    public const string SettingsLinkGithub = "Settings.LinkGithub";

    // ---- Raid ----
    public const string RaidHeaderAI = "Raid.Header.AI";
    public const string RaidHeaderDefaultOptions = "Raid.Header.DefaultOptions";
    public const string RaidHeaderRaid = "Raid.Header.Raid";
    public const string RaidHeaderWeather = "Raid.Header.Weather";
    public const string RaidHeaderResources = "Raid.Header.Resources";
    public const string RaidHeaderMapInsurance = "Raid.Header.MapInsurance";
    public const string RaidButtonAiHealth = "Raid.Button.AiHealth";
    public const string RaidButtonAiSpawnNumber = "Raid.Button.AiSpawnNumber";
    public const string RaidButtonUSECRatio = "Raid.Button.USECRatio";
    public const string RaidButtonDefaultOptions = "Raid.Button.DefaultOptions";
    public const string RaidButtonAiAmount = "Raid.Button.AiAmount";
    public const string RaidButtonAiDifficulty = "Raid.Button.AiDifficulty";
    public const string RaidButtonEnableBoss = "Raid.Button.EnableBoss";
    public const string RaidButtonScavCivilWar = "Raid.Button.ScavCivilWar";
    public const string RaidButtonMarkedCursed = "Raid.Button.MarkedCursed";
    public const string RaidButtonRaidTime = "Raid.Button.RaidTime";
    public const string RaidButtonBossSpawnRate = "Raid.Button.BossSpawnRate";
    public const string RaidButtonAirdropType = "Raid.Button.AirdropType";
    public const string RaidButtonAlwaysExtractSwitch = "Raid.Button.AlwaysExtractSwitch";
    public const string RaidButtonAlwaysExtractable = "Raid.Button.AlwaysExtractable";
    public const string RaidButtonUnlimitedExtractTime = "Raid.Button.UnlimitedExtractTime";
    public const string RaidButtonScavNoCooldown = "Raid.Button.ScavNoCooldown";
    public const string RaidButtonScavGearBoost = "Raid.Button.ScavGearBoost";
    public const string RaidButtonNoLootLoss = "Raid.Button.NoLootLoss";
    public const string RaidButtonGlobalWeather = "Raid.Button.GlobalWeather";
    public const string RaidButtonCloud = "Raid.Button.Cloud";
    public const string RaidButtonWind = "Raid.Button.Wind";
    public const string RaidButtonRain = "Raid.Button.Rain";
    public const string RaidButtonFog = "Raid.Button.Fog";
    public const string RaidButtonGlobalLoot = "Raid.Button.GlobalLoot";
    public const string RaidButtonContainerLoot = "Raid.Button.ContainerLoot";
    public const string RaidButtonGroundLoot = "Raid.Button.GroundLoot";
    public const string RaidButtonValuableLoot = "Raid.Button.ValuableLoot";
    public const string RaidButtonDisableRandomContainers = "Raid.Button.DisableRandomContainers";
    public const string RaidButtonAiNamePool = "Raid.Button.AiNamePool";
    public const string RaidButtonPmcTacticalSquad = "Raid.Button.PmcTacticalSquad";
    public const string RaidButtonPmcRoar = "Raid.Button.PmcRoar";
    public const string RaidButtonMapRefreshConfig = "Raid.Button.MapRefreshConfig";
    public const string RaidButtonMapBotDifficulty = "Raid.Button.MapBotDifficulty";
    public const string RaidMapCustoms = "Raid.Map.Customs";
    public const string RaidMapFactory = "Raid.Map.Factory";
    public const string RaidMapInterchange = "Raid.Map.Interchange";
    public const string RaidMapLabs = "Raid.Map.Labs";
    public const string RaidMapLighthouse = "Raid.Map.Lighthouse";
    public const string RaidMapReserve = "Raid.Map.Reserve";
    public const string RaidMapGroundZero = "Raid.Map.GroundZero";
    public const string RaidMapShoreline = "Raid.Map.Shoreline";
    public const string RaidMapStreets = "Raid.Map.Streets";
    public const string RaidMapWoods = "Raid.Map.Woods";
    public const string RaidMapLabyrinth = "Raid.Map.Labyrinth";

    // ---- Raid Desc ----
    public const string RaidDescAiHealth = "Raid.Desc.AiHealth";
    public const string RaidDescAiSpawnNumber = "Raid.Desc.AiSpawnNumber";
    public const string RaidDescUSECRatio = "Raid.Desc.USECRatio";
    public const string RaidDescAiNamePool = "Raid.Desc.AiNamePool";
    public const string RaidDescPmcTacticalSquad = "Raid.Desc.PmcTacticalSquad";
    public const string RaidDescPmcRoar = "Raid.Desc.PmcRoar";
    public const string RaidDescDefaultOptions = "Raid.Desc.DefaultOptions";
    public const string RaidDescAiAmount = "Raid.Desc.AiAmount";
    public const string RaidDescAiDifficulty = "Raid.Desc.AiDifficulty";
    public const string RaidDescEnableBoss = "Raid.Desc.EnableBoss";
    public const string RaidDescScavCivilWar = "Raid.Desc.ScavCivilWar";
    public const string RaidDescMarkedCursed = "Raid.Desc.MarkedCursed";
    public const string RaidDescRaidTime = "Raid.Desc.RaidTime";
    public const string RaidDescBossSpawnRate = "Raid.Desc.BossSpawnRate";
    public const string RaidDescAirdropType = "Raid.Desc.AirdropType";
    public const string RaidDescAlwaysExtractSwitch = "Raid.Desc.AlwaysExtractSwitch";
    public const string RaidDescAlwaysExtractable = "Raid.Desc.AlwaysExtractable";
    public const string RaidDescUnlimitedExtractTime = "Raid.Desc.UnlimitedExtractTime";
    public const string RaidDescScavNoCooldown = "Raid.Desc.ScavNoCooldown";
    public const string RaidDescScavGearBoost = "Raid.Desc.ScavGearBoost";
    public const string RaidDescNoLootLoss = "Raid.Desc.NoLootLoss";
    public const string RaidDescMapRefreshConfig = "Raid.Desc.MapRefreshConfig";
    public const string RaidDescMapBotDifficulty = "Raid.Desc.MapBotDifficulty";
    public const string RaidDescGlobalWeather = "Raid.Desc.GlobalWeather";
    public const string RaidDescCloud = "Raid.Desc.Cloud";
    public const string RaidDescWind = "Raid.Desc.Wind";
    public const string RaidDescRain = "Raid.Desc.Rain";
    public const string RaidDescFog = "Raid.Desc.Fog";
    public const string RaidDescGlobalLoot = "Raid.Desc.GlobalLoot";
    public const string RaidDescContainerLoot = "Raid.Desc.ContainerLoot";
    public const string RaidDescGroundLoot = "Raid.Desc.GroundLoot";
    public const string RaidDescValuableLoot = "Raid.Desc.ValuableLoot";
    public const string RaidDescDisableRandomContainers = "Raid.Desc.DisableRandomContainers";
    public const string RaidDescMapCustoms = "Raid.Desc.Map.Customs";
    public const string RaidDescMapFactory = "Raid.Desc.Map.Factory";
    public const string RaidDescMapInterchange = "Raid.Desc.Map.Interchange";
    public const string RaidDescMapLabs = "Raid.Desc.Map.Labs";
    public const string RaidDescMapLighthouse = "Raid.Desc.Map.Lighthouse";
    public const string RaidDescMapReserve = "Raid.Desc.Map.Reserve";
    public const string RaidDescMapGroundZero = "Raid.Desc.Map.GroundZero";
    public const string RaidDescMapShoreline = "Raid.Desc.Map.Shoreline";
    public const string RaidDescMapStreets = "Raid.Desc.Map.Streets";
    public const string RaidDescMapWoods = "Raid.Desc.Map.Woods";
    public const string RaidDescMapLabyrinth = "Raid.Desc.Map.Labyrinth";

    // ---- Raid HeaderDesc ----
    public const string RaidHeaderDescAI = "Raid.HeaderDesc.AI";
    public const string RaidHeaderDescDefaultOptions = "Raid.HeaderDesc.DefaultOptions";
    public const string RaidHeaderDescRaid = "Raid.HeaderDesc.Raid";
    public const string RaidHeaderDescWeather = "Raid.HeaderDesc.Weather";
    public const string RaidHeaderDescResources = "Raid.HeaderDesc.Resources";
    public const string RaidHeaderDescMapInsurance = "Raid.HeaderDesc.MapInsurance";

    // ---- Develop ----
    public const string DevelopHeaderBaseAttributes = "Develop.Header.BaseAttributes";
    public const string DevelopHeaderWeaponOpt = "Develop.Header.WeaponOpt";
    public const string DevelopHeaderHelmetOpt = "Develop.Header.HelmetOpt";
    public const string DevelopHeaderRigArmorOpt = "Develop.Header.RigArmorOpt";
    public const string DevelopHeaderPlateCarrierOpt = "Develop.Header.PlateCarrierOpt";
    public const string DevelopHeaderBackpackOpt = "Develop.Header.BackpackOpt";
    public const string DevelopHeaderEnchantOpt = "Develop.Header.EnchantOpt";
    public const string DevelopHeaderInsuranceOpt = "Develop.Header.InsuranceOpt";
    public const string DevelopHeaderItemAttrOpt = "Develop.Header.ItemAttrOpt";
    public const string DevelopHeaderWarehouseOpt = "Develop.Header.WarehouseOpt";
    public const string DevelopHeaderQuestSystem = "Develop.Header.QuestSystem";
    public const string DevelopHeaderHideout = "Develop.Header.Hideout";
    public const string DevelopButtonUnloadSpeed = "Develop.Button.UnloadSpeed";
    public const string DevelopButtonSkillExpBoost = "Develop.Button.SkillExpBoost";
    public const string DevelopButtonSupermanMode = "Develop.Button.SupermanMode";
    public const string DevelopButtonNoCarryLimit = "Develop.Button.NoCarryLimit";
    public const string DevelopButtonWeaponSlotCompat = "Develop.Button.WeaponSlotCompat";
    public const string DevelopButtonWeaponNoJam = "Develop.Button.WeaponNoJam";
    public const string DevelopButtonWeaponNoDurabilityLoss = "Develop.Button.WeaponNoDurabilityLoss";
    public const string DevelopButtonMagazineCapacity = "Develop.Button.MagazineCapacity";
    public const string DevelopButtonHelmetHeadsetCompat = "Develop.Button.HelmetHeadsetCompat";
    public const string DevelopButtonHelmetNoDebuff = "Develop.Button.HelmetNoDebuff";
    public const string DevelopButtonHelmetNoWeight = "Develop.Button.HelmetNoWeight";
    public const string DevelopButtonRigArmorCompat = "Develop.Button.RigArmorCompat";
    public const string DevelopButtonRigArmorNoDebuff = "Develop.Button.RigArmorNoDebuff";
    public const string DevelopButtonRigArmorNoWeight = "Develop.Button.RigArmorNoWeight";
    public const string DevelopButtonRigArmorNoDurabilityLoss = "Develop.Button.RigArmorNoDurabilityLoss";
    public const string DevelopButtonPlateCarrierDurability = "Develop.Button.PlateCarrierDurability";
    public const string DevelopButtonPlateCarrierNoDebuff = "Develop.Button.PlateCarrierNoDebuff";
    public const string DevelopButtonPlateCarrierNoWeight = "Develop.Button.PlateCarrierNoWeight";
    public const string DevelopButtonBackpackCompat = "Develop.Button.BackpackCompat";
    public const string DevelopButtonBackpackCompact = "Develop.Button.BackpackCompact";
    public const string DevelopButtonBackpackNoDebuff = "Develop.Button.BackpackNoDebuff";
    public const string DevelopButtonBackpackNoWeight = "Develop.Button.BackpackNoWeight";
    public const string DevelopButtonWeaponEnchant100 = "Develop.Button.WeaponEnchant100";
    public const string DevelopButtonArmorEnchant100 = "Develop.Button.ArmorEnchant100";
    public const string DevelopButtonInsuranceExpand = "Develop.Button.InsuranceExpand";
    public const string DevelopButtonInsuranceCompat = "Develop.Button.InsuranceCompat";
    public const string DevelopButtonInsuranceNoWeight = "Develop.Button.InsuranceNoWeight";
    public const string DevelopButtonT7ThermalBoost = "Develop.Button.T7ThermalBoost";
    public const string DevelopButtonKeyUnlimitedDurability = "Develop.Button.KeyUnlimitedDurability";
    public const string DevelopButtonMedicalItemDurability = "Develop.Button.MedicalItemDurability";
    public const string DevelopButtonAmmoStack = "Develop.Button.AmmoStack";
    public const string DevelopButtonMoneyStack = "Develop.Button.MoneyStack";
    public const string DevelopButtonFullInspect = "Develop.Button.FullInspect";
    public const string DevelopButtonQuestFreeReset = "Develop.Button.QuestFreeReset";
    public const string DevelopButtonQuestOptimize = "Develop.Button.QuestOptimize";
    public const string DevelopButtonQuest3X4Marker = "Develop.Button.Quest3X4Marker";
    public const string DevelopButtonHideoutUpgradeTime = "Develop.Button.HideoutUpgradeTime";
    public const string DevelopButtonHideoutProductionTime = "Develop.Button.HideoutProductionTime";
    public const string DevelopButtonScavCaseTime = "Develop.Button.ScavCaseTime";
    public const string DevelopButtonHideoutUpgradeUnlimited = "Develop.Button.HideoutUpgradeUnlimited";
    public const string DevelopButtonHideoutAreaBonusLevel = "Develop.Button.HideoutAreaBonusLevel";
    public const string DevelopButtonHideoutNoPower = "Develop.Button.HideoutNoPower";
    public const string DevelopButtonGymAlwaysSucceed = "Develop.Button.GymAlwaysSucceed";
    public const string DevelopButtonGymNoPenalty = "Develop.Button.GymNoPenalty";
    public const string DevelopButtonGymRewardMultiplier = "Develop.Button.GymRewardMultiplier";

    // ---- Develop Desc ----
    public const string DevelopDescUnloadSpeed = "Develop.Desc.UnloadSpeed";
    public const string DevelopDescSkillExpBoost = "Develop.Desc.SkillExpBoost";
    public const string DevelopDescSupermanMode = "Develop.Desc.SupermanMode";
    public const string DevelopDescNoCarryLimit = "Develop.Desc.NoCarryLimit";
    public const string DevelopDescWeaponSlotCompat = "Develop.Desc.WeaponSlotCompat";
    public const string DevelopDescWeaponNoJam = "Develop.Desc.WeaponNoJam";
    public const string DevelopDescWeaponNoDurabilityLoss = "Develop.Desc.WeaponNoDurabilityLoss";
    public const string DevelopDescMagazineCapacity = "Develop.Desc.MagazineCapacity";
    public const string DevelopDescHelmetHeadsetCompat = "Develop.Desc.HelmetHeadsetCompat";
    public const string DevelopDescHelmetNoDebuff = "Develop.Desc.HelmetNoDebuff";
    public const string DevelopDescHelmetNoWeight = "Develop.Desc.HelmetNoWeight";
    public const string DevelopDescRigArmorCompat = "Develop.Desc.RigArmorCompat";
    public const string DevelopDescRigArmorNoDebuff = "Develop.Desc.RigArmorNoDebuff";
    public const string DevelopDescRigArmorNoWeight = "Develop.Desc.RigArmorNoWeight";
    public const string DevelopDescRigArmorNoDurabilityLoss = "Develop.Desc.RigArmorNoDurabilityLoss";
    public const string DevelopDescPlateCarrierDurability = "Develop.Desc.PlateCarrierDurability";
    public const string DevelopDescPlateCarrierNoDebuff = "Develop.Desc.PlateCarrierNoDebuff";
    public const string DevelopDescPlateCarrierNoWeight = "Develop.Desc.PlateCarrierNoWeight";
    public const string DevelopDescBackpackCompat = "Develop.Desc.BackpackCompat";
    public const string DevelopDescBackpackCompact = "Develop.Desc.BackpackCompact";
    public const string DevelopDescBackpackNoDebuff = "Develop.Desc.BackpackNoDebuff";
    public const string DevelopDescBackpackNoWeight = "Develop.Desc.BackpackNoWeight";
    public const string DevelopDescWeaponEnchant100 = "Develop.Desc.WeaponEnchant100";
    public const string DevelopDescArmorEnchant100 = "Develop.Desc.ArmorEnchant100";
    public const string DevelopDescInsuranceExpand = "Develop.Desc.InsuranceExpand";
    public const string DevelopDescInsuranceCompat = "Develop.Desc.InsuranceCompat";
    public const string DevelopDescInsuranceNoWeight = "Develop.Desc.InsuranceNoWeight";
    public const string DevelopDescT7ThermalBoost = "Develop.Desc.T7ThermalBoost";
    public const string DevelopDescKeyUnlimitedDurability = "Develop.Desc.KeyUnlimitedDurability";
    public const string DevelopDescMedicalItemDurability = "Develop.Desc.MedicalItemDurability";
    public const string DevelopDescAmmoStack = "Develop.Desc.AmmoStack";
    public const string DevelopDescMoneyStack = "Develop.Desc.MoneyStack";
    public const string DevelopDescFullInspect = "Develop.Desc.FullInspect";
    public const string DevelopDescQuestFreeReset = "Develop.Desc.QuestFreeReset";
    public const string DevelopDescQuestOptimize = "Develop.Desc.QuestOptimize";
    public const string DevelopDescQuest3X4Marker = "Develop.Desc.Quest3X4Marker";
    public const string DevelopDescHideoutUpgradeTime = "Develop.Desc.HideoutUpgradeTime";
    public const string DevelopDescHideoutProductionTime = "Develop.Desc.HideoutProductionTime";
    public const string DevelopDescScavCaseTime = "Develop.Desc.ScavCaseTime";
    public const string DevelopDescHideoutUpgradeUnlimited = "Develop.Desc.HideoutUpgradeUnlimited";
    public const string DevelopDescHideoutAreaBonusLevel = "Develop.Desc.HideoutAreaBonusLevel";
    public const string DevelopDescHideoutNoPower = "Develop.Desc.HideoutNoPower";
    public const string DevelopDescGymAlwaysSucceed = "Develop.Desc.GymAlwaysSucceed";
    public const string DevelopDescGymNoPenalty = "Develop.Desc.GymNoPenalty";
    public const string DevelopDescGymRewardMultiplier = "Develop.Desc.GymRewardMultiplier";

    // ---- Develop HeaderDesc ----
    public const string DevelopHeaderDescBaseAttributes = "Develop.HeaderDesc.BaseAttributes";
    public const string DevelopHeaderDescWeaponOpt = "Develop.HeaderDesc.WeaponOpt";
    public const string DevelopHeaderDescHelmetOpt = "Develop.HeaderDesc.HelmetOpt";
    public const string DevelopHeaderDescRigArmorOpt = "Develop.HeaderDesc.RigArmorOpt";
    public const string DevelopHeaderDescPlateCarrierOpt = "Develop.HeaderDesc.PlateCarrierOpt";
    public const string DevelopHeaderDescBackpackOpt = "Develop.HeaderDesc.BackpackOpt";
    public const string DevelopHeaderDescEnchantOpt = "Develop.HeaderDesc.EnchantOpt";
    public const string DevelopHeaderDescInsuranceOpt = "Develop.HeaderDesc.InsuranceOpt";
    public const string DevelopHeaderDescItemAttrOpt = "Develop.HeaderDesc.ItemAttrOpt";
    public const string DevelopHeaderDescWarehouseOpt = "Develop.HeaderDesc.WarehouseOpt";
    public const string DevelopHeaderDescQuestSystem = "Develop.HeaderDesc.QuestSystem";
    public const string DevelopHeaderDescHideout = "Develop.HeaderDesc.Hideout";

    // ---- Economy ----
    public const string EconomyHeaderFlea = "Economy.Header.Flea";
    public const string EconomyHeaderTrader = "Economy.Header.Trader";
    public const string EconomyButtonFleaTradeMultiplier = "Economy.Button.FleaTradeMultiplier";
    public const string EconomyButtonFleaSell100 = "Economy.Button.FleaSell100";
    public const string EconomyButtonFleaInstantSell = "Economy.Button.FleaInstantSell";
    public const string EconomyButtonFleaBuyBoost = "Economy.Button.FleaBuyBoost";
    public const string EconomyButtonFleaBrandNew = "Economy.Button.FleaBrandNew";
    public const string EconomyButtonFleaDisableBlacklist = "Economy.Button.FleaDisableBlacklist";
    public const string EconomyButtonFleaOpenLevel = "Economy.Button.FleaOpenLevel";
    public const string EconomyButtonLowTax = "Economy.Button.LowTax";
    public const string EconomyButtonTraderSupplyTime = "Economy.Button.TraderSupplyTime";
    public const string EconomyButtonInsuranceReturnSpeed = "Economy.Button.InsuranceReturnSpeed";
    public const string EconomyButtonInsuranceCost = "Economy.Button.InsuranceCost";
    public const string EconomyButtonInsuranceReturnChance = "Economy.Button.InsuranceReturnChance";
    public const string EconomyButtonBuyWithHook = "Economy.Button.BuyWithHook";

    // ---- Economy Desc ----
    public const string EconomyDescFleaTradeMultiplier = "Economy.Desc.FleaTradeMultiplier";
    public const string EconomyDescFleaSell100 = "Economy.Desc.FleaSell100";
    public const string EconomyDescFleaInstantSell = "Economy.Desc.FleaInstantSell";
    public const string EconomyDescFleaBuyBoost = "Economy.Desc.FleaBuyBoost";
    public const string EconomyDescFleaBrandNew = "Economy.Desc.FleaBrandNew";
    public const string EconomyDescFleaDisableBlacklist = "Economy.Desc.FleaDisableBlacklist";
    public const string EconomyDescFleaOpenLevel = "Economy.Desc.FleaOpenLevel";
    public const string EconomyDescLowTax = "Economy.Desc.LowTax";
    public const string EconomyDescTraderSupplyTime = "Economy.Desc.TraderSupplyTime";
    public const string EconomyDescInsuranceReturnSpeed = "Economy.Desc.InsuranceReturnSpeed";
    public const string EconomyDescInsuranceCost = "Economy.Desc.InsuranceCost";
    public const string EconomyDescInsuranceReturnChance = "Economy.Desc.InsuranceReturnChance";
    public const string EconomyDescBuyWithHook = "Economy.Desc.BuyWithHook";

    // ---- Economy HeaderDesc ----
    public const string EconomyHeaderDescFlea = "Economy.HeaderDesc.Flea";
    public const string EconomyHeaderDescTrader = "Economy.HeaderDesc.Trader";

    // ---- Feature ----
    public const string FeatureHeaderIndependent = "Feature.Header.Independent";
    public const string FeatureHeaderOther = "Feature.Header.Other";
    public const string FeatureButtonIndependentTrader = "Feature.Button.IndependentTrader";
    public const string FeatureButtonIndependentItem = "Feature.Button.IndependentItem";
    public const string FeatureButtonIndependentPreset = "Feature.Button.IndependentPreset";
    public const string FeatureButtonIndependentSave = "Feature.Button.IndependentSave";
    public const string FeatureButtonBulletData = "Feature.Button.BulletData";
    public const string FeatureButtonKeyCategory = "Feature.Button.KeyCategory";
    public const string FeatureButtonRealTimeFlea = "Feature.Button.RealTimeFlea";

    // ---- Feature Desc ----
    public const string FeatureDescIndependentTrader = "Feature.Desc.IndependentTrader";
    public const string FeatureDescIndependentItem = "Feature.Desc.IndependentItem";
    public const string FeatureDescIndependentPreset = "Feature.Desc.IndependentPreset";
    public const string FeatureDescIndependentSave = "Feature.Desc.IndependentSave";
    public const string FeatureDescBulletData = "Feature.Desc.BulletData";
    public const string FeatureDescKeyCategory = "Feature.Desc.KeyCategory";
    public const string FeatureDescRealTimeFlea = "Feature.Desc.RealTimeFlea";

    // ---- Feature HeaderDesc ----
    public const string FeatureHeaderDescIndependent = "Feature.HeaderDesc.Independent";
    public const string FeatureHeaderDescOther = "Feature.HeaderDesc.Other";

    // ---- Container ----
    public const string ContainerButtonEnable = "Container.Button.Enable";
    public const string ContainerButtonWidth = "Container.Button.Width";
    public const string ContainerButtonHeight = "Container.Button.Height";
    public const string ContainerButtonCompatible = "Container.Button.Compatible";
    public const string ContainerButtonNoWeight = "Container.Button.NoWeight";

    // ---- Container Desc ----
    public const string ContainerDescEnable = "Container.Desc.Enable";
    public const string ContainerDescWidth = "Container.Desc.Width";
    public const string ContainerDescHeight = "Container.Desc.Height";
    public const string ContainerDescCompatible = "Container.Desc.Compatible";
    public const string ContainerDescNoWeight = "Container.Desc.NoWeight";

    // ---- ConfigItem ----
    public const string ConfigItemDefaultRatio = "ConfigItem.DefaultRatio";
    public const string ConfigItemAiAmountAsOnline = "ConfigItem.AiAmount.AsOnline";
    public const string ConfigItemAiAmountNoBots = "ConfigItem.AiAmount.NoBots";
    public const string ConfigItemAiAmountLow = "ConfigItem.AiAmount.Low";
    public const string ConfigItemAiAmountMedium = "ConfigItem.AiAmount.Medium";
    public const string ConfigItemAiAmountHigh = "ConfigItem.AiAmount.High";
    public const string ConfigItemAiAmountHorde = "ConfigItem.AiAmount.Horde";
    public const string ConfigItemAiDifficultyAsOnline = "ConfigItem.AiDifficulty.AsOnline";
    public const string ConfigItemAiDifficultyEasy = "ConfigItem.AiDifficulty.Easy";
    public const string ConfigItemAiDifficultyMedium = "ConfigItem.AiDifficulty.Medium";
    public const string ConfigItemAiDifficultyHard = "ConfigItem.AiDifficulty.Hard";
    public const string ConfigItemAiDifficultyImpossible = "ConfigItem.AiDifficulty.Impossible";
    public const string ConfigItemAiDifficultyRandom = "ConfigItem.AiDifficulty.Random";
    public const string ConfigItemRaidTimeDefault = "ConfigItem.RaidTime.Default";
    public const string ConfigItemRaidTime30m = "ConfigItem.RaidTime.30m";
    public const string ConfigItemRaidTime1h = "ConfigItem.RaidTime.1h";
    public const string ConfigItemRaidTime1h30 = "ConfigItem.RaidTime.1h30";
    public const string ConfigItemRaidTime2h = "ConfigItem.RaidTime.2h";
    public const string ConfigItemRaidTime3h = "ConfigItem.RaidTime.3h";
    public const string ConfigItemRaidTime4h = "ConfigItem.RaidTime.4h";
    public const string ConfigItemBossSpawnChanceDefault = "ConfigItem.BossSpawnChance.Default";
    public const string ConfigItemAirdropTypeDefault = "ConfigItem.AirdropType.Default";
    public const string ConfigItemAirdropTypeMoreWeapon = "ConfigItem.AirdropType.MoreWeapon";
    public const string ConfigItemAirdropTypeMoreBarter = "ConfigItem.AirdropType.MoreBarter";
    public const string ConfigItemAirdropTypeMoreFoodMedical = "ConfigItem.AirdropType.MoreFoodMedical";
    public const string ConfigItemAirdropTypeMoreMixed = "ConfigItem.AirdropType.MoreMixed";
    public const string ConfigItemWeatherModeDefault = "ConfigItem.WeatherMode.Default";
    public const string ConfigItemWeatherModeClear = "ConfigItem.WeatherMode.Clear";
    public const string ConfigItemWeatherModeRainy = "ConfigItem.WeatherMode.Rainy";
    public const string ConfigItemWeatherModeStorm = "ConfigItem.WeatherMode.Storm";
    public const string ConfigItemWeatherModeMisty = "ConfigItem.WeatherMode.Misty";
    public const string ConfigItemWeatherModeExtreme = "ConfigItem.WeatherMode.Extreme";
    public const string ConfigItemWeatherModeCustom = "ConfigItem.WeatherMode.Custom";
    public const string ConfigItemCloudModeDefault = "ConfigItem.CloudMode.Default";
    public const string ConfigItemCloudModeCloudless = "ConfigItem.CloudMode.Cloudless";
    public const string ConfigItemCloudModeFew = "ConfigItem.CloudMode.Few";
    public const string ConfigItemCloudModePartly = "ConfigItem.CloudMode.Partly";
    public const string ConfigItemCloudModeOvercast = "ConfigItem.CloudMode.Overcast";
    public const string ConfigItemWindModeDefault = "ConfigItem.WindMode.Default";
    public const string ConfigItemWindModeNone = "ConfigItem.WindMode.None";
    public const string ConfigItemWindModeBreeze = "ConfigItem.WindMode.Breeze";
    public const string ConfigItemWindModeStrong = "ConfigItem.WindMode.Strong";
    public const string ConfigItemWindModeGale = "ConfigItem.WindMode.Gale";
    public const string ConfigItemRainModeDefault = "ConfigItem.RainMode.Default";
    public const string ConfigItemRainModeNone = "ConfigItem.RainMode.None";
    public const string ConfigItemRainModeDrizzle = "ConfigItem.RainMode.Drizzle";
    public const string ConfigItemRainModeLight = "ConfigItem.RainMode.Light";
    public const string ConfigItemRainModeHeavy = "ConfigItem.RainMode.Heavy";
    public const string ConfigItemRainModeDownpour = "ConfigItem.RainMode.Downpour";
    public const string ConfigItemFogModeDefault = "ConfigItem.FogMode.Default";
    public const string ConfigItemFogModeNone = "ConfigItem.FogMode.None";
    public const string ConfigItemFogModeLight = "ConfigItem.FogMode.Light";
    public const string ConfigItemFogModeMisty = "ConfigItem.FogMode.Misty";
    public const string ConfigItemFogModeHaze = "ConfigItem.FogMode.Haze";
    public const string ConfigItemUpdateTimeDefault = "ConfigItem.UpdateTime.Default";
    public const string ConfigItemUpdateTime60m = "ConfigItem.UpdateTime.60m";
    public const string ConfigItemUpdateTime30m = "ConfigItem.UpdateTime.30m";
    public const string ConfigItemUpdateTime10m = "ConfigItem.UpdateTime.10m";
    public const string ConfigItemUpdateTime5m = "ConfigItem.UpdateTime.5m";
    public const string ConfigItemInsuranceTimeDefault = "ConfigItem.InsuranceTime.Default";
    public const string ConfigItemInsuranceTimeFast = "ConfigItem.InsuranceTime.Fast";
    public const string ConfigItemInsuranceTimeVeryFast = "ConfigItem.InsuranceTime.VeryFast";
    public const string ConfigItemInsuranceTimeInstant = "ConfigItem.InsuranceTime.Instant";
    public const string ConfigItemInsuranceCostDefault = "ConfigItem.InsuranceCost.Default";
    public const string ConfigItemInsuranceCostVeryCheap = "ConfigItem.InsuranceCost.VeryCheap";
    public const string ConfigItemInsuranceCostNormal = "ConfigItem.InsuranceCost.Normal";
    public const string ConfigItemInsuranceCostExpensive = "ConfigItem.InsuranceCost.Expensive";
    public const string ConfigItemInsuranceCostVeryExpensive = "ConfigItem.InsuranceCost.VeryExpensive";
    public const string ConfigItemReturnChanceDefault = "ConfigItem.ReturnChance.Default";
    public const string ConfigItemLoadSpeedDefault = "ConfigItem.LoadSpeed.Default";
    public const string ConfigItemLoadSpeedFast = "ConfigItem.LoadSpeed.Fast";
    public const string ConfigItemLoadSpeedVeryFast = "ConfigItem.LoadSpeed.VeryFast";
    public const string ConfigItemLoadSpeedLightning = "ConfigItem.LoadSpeed.Lightning";
    public const string ConfigItemHideoutTimeDefault = "ConfigItem.HideoutTime.Default";
    public const string ConfigItemHideoutTime30s = "ConfigItem.HideoutTime.30s";
    public const string ConfigItemHideoutTime5m = "ConfigItem.HideoutTime.5m";
    public const string ConfigItemHideoutTime20m = "ConfigItem.HideoutTime.20m";
    public const string ConfigItemHideoutTime1h = "ConfigItem.HideoutTime.1h";
    public const string ConfigItemHideoutTime3h = "ConfigItem.HideoutTime.3h";
    public const string ConfigItemBonusesLevelDefault = "ConfigItem.BonusesLevel.Default";
    public const string ConfigItemBonusesLevelDouble = "ConfigItem.BonusesLevel.Double";
    public const string ConfigItemBonusesLevelQuintuple = "ConfigItem.BonusesLevel.Quintuple";
    public const string ConfigItemBonusesLevelDecuple = "ConfigItem.BonusesLevel.Decuple";
    public const string ConfigItemRewardMultipleDefault = "ConfigItem.RewardMultiple.Default";
    public const string ConfigItemContainerCells = "ConfigItem.Container.Cells";
    public const string ConfigItemMapBotDifficultyDefault = "ConfigItem.MapBotDifficulty.Default";
    public const string ConfigItemMapBotDifficultyEasy = "ConfigItem.MapBotDifficulty.Easy";
    public const string ConfigItemMapBotDifficultyNormal = "ConfigItem.MapBotDifficulty.Normal";
    public const string ConfigItemMapBotDifficultyHard = "ConfigItem.MapBotDifficulty.Hard";
    public const string ConfigItemMapBotDifficultyImpossible = "ConfigItem.MapBotDifficulty.Impossible";

    // ---- Theme ----
    public const string ThemePurpleLight = "Theme.PurpleLight";
    public const string ThemePurpleDark = "Theme.PurpleDark";
    public const string ThemeOceanLight = "Theme.OceanLight";
    public const string ThemeOceanDark = "Theme.OceanDark";
    public const string ThemeRedLight = "Theme.RedLight";
    public const string ThemeRedDark = "Theme.RedDark";
    public const string ThemeOrangeLight = "Theme.OrangeLight";
    public const string ThemeOrangeDark = "Theme.OrangeDark";
    public const string ThemeYellowLight = "Theme.YellowLight";
    public const string ThemeYellowDark = "Theme.YellowDark";
    public const string ThemeGreenLight = "Theme.GreenLight";
    public const string ThemeGreenDark = "Theme.GreenDark";
    public const string ThemeCyanLight = "Theme.CyanLight";
    public const string ThemeCyanDark = "Theme.CyanDark";
    public const string ThemeSapphireLight = "Theme.SapphireLight";
    public const string ThemeSapphireDark = "Theme.SapphireDark";
    public const string ThemeVioletLight = "Theme.VioletLight";
    public const string ThemeVioletDark = "Theme.VioletDark";
    public const string ThemePinkLight = "Theme.PinkLight";
    public const string ThemePinkDark = "Theme.PinkDark";
    public const string ThemeBrownLight = "Theme.BrownLight";
    public const string ThemeBrownDark = "Theme.BrownDark";
    public const string ThemeGrayLight = "Theme.GrayLight";
    public const string ThemeGrayDark = "Theme.GrayDark";
    public const string ThemeBlackLight = "Theme.BlackLight";
    public const string ThemeBlackDark = "Theme.BlackDark";
    public const string ThemeWhiteLight = "Theme.WhiteLight";
    public const string ThemeWhiteDark = "Theme.WhiteDark";
    public const string ThemeMagentaLight = "Theme.MagentaLight";
    public const string ThemeMagentaDark = "Theme.MagentaDark";
    public const string ThemeLemonLight = "Theme.LemonLight";
    public const string ThemeLemonDark = "Theme.LemonDark";
    public const string ThemeIndigoLight = "Theme.IndigoLight";
    public const string ThemeIndigoDark = "Theme.IndigoDark";
    public const string ThemeTeaLight = "Theme.TeaLight";
    public const string ThemeTeaDark = "Theme.TeaDark";

    // ---- ContainerItem ----
    public const string ContainerItemBallisticPlateCase = "ContainerItem.BallisticPlateCase";
    public const string ContainerItemGingyKeychain = "ContainerItem.GingyKeychain";
    public const string ContainerItemHolodilnickThermalBag = "ContainerItem.HolodilnickThermalBag";
    public const string ContainerItemTHICCWeaponCase = "ContainerItem.THICCWeaponCase";
    public const string ContainerItemTHICCItemCase = "ContainerItem.THICCItemCase";
    public const string ContainerItemWZWallet = "ContainerItem.WZWallet";
    public const string ContainerItemStreamerItemCase = "ContainerItem.StreamerItemCase";
    public const string ContainerItemMedicineCase = "ContainerItem.MedicineCase";
    public const string ContainerItemSICCPouch = "ContainerItem.SICCPouch";
    public const string ContainerItemLuckyScavJunkBox = "ContainerItem.LuckyScavJunkBox";
    public const string ContainerItemMagazineCase = "ContainerItem.MagazineCase";
    public const string ContainerItemAmmunitionCase = "ContainerItem.AmmunitionCase";
    public const string ContainerItemPistolCase = "ContainerItem.PistolCase";
    public const string ContainerItemGrenadeCase = "ContainerItem.GrenadeCase";
    public const string ContainerItemDocumentsCase = "ContainerItem.DocumentsCase";
    public const string ContainerItemWeaponCase = "ContainerItem.WeaponCase";
    public const string ContainerItemInjectorCase = "ContainerItem.InjectorCase";
    public const string ContainerItemItemCase = "ContainerItem.ItemCase";
    public const string ContainerItemDogtagCase = "ContainerItem.DogtagCase";
    public const string ContainerItemSimpleWallet = "ContainerItem.SimpleWallet";
    public const string ContainerItemKeycardHolderCase = "ContainerItem.KeycardHolderCase";
    public const string ContainerItemKeyTool = "ContainerItem.KeyTool";
    public const string ContainerItemMoneyCase = "ContainerItem.MoneyCase";

    // ---- ContainerItem Desc ----
    public const string ContainerItemDescBallisticPlateCase = "ContainerItem.Desc.BallisticPlateCase";
    public const string ContainerItemDescGingyKeychain = "ContainerItem.Desc.GingyKeychain";
    public const string ContainerItemDescHolodilnickThermalBag = "ContainerItem.Desc.HolodilnickThermalBag";
    public const string ContainerItemDescTHICCWeaponCase = "ContainerItem.Desc.THICCWeaponCase";
    public const string ContainerItemDescTHICCItemCase = "ContainerItem.Desc.THICCItemCase";
    public const string ContainerItemDescWZWallet = "ContainerItem.Desc.WZWallet";
    public const string ContainerItemDescStreamerItemCase = "ContainerItem.Desc.StreamerItemCase";
    public const string ContainerItemDescMedicineCase = "ContainerItem.Desc.MedicineCase";
    public const string ContainerItemDescSICCPouch = "ContainerItem.Desc.SICCPouch";
    public const string ContainerItemDescLuckyScavJunkBox = "ContainerItem.Desc.LuckyScavJunkBox";
    public const string ContainerItemDescMagazineCase = "ContainerItem.Desc.MagazineCase";
    public const string ContainerItemDescAmmunitionCase = "ContainerItem.Desc.AmmunitionCase";
    public const string ContainerItemDescPistolCase = "ContainerItem.Desc.PistolCase";
    public const string ContainerItemDescGrenadeCase = "ContainerItem.Desc.GrenadeCase";
    public const string ContainerItemDescDocumentsCase = "ContainerItem.Desc.DocumentsCase";
    public const string ContainerItemDescWeaponCase = "ContainerItem.Desc.WeaponCase";
    public const string ContainerItemDescInjectorCase = "ContainerItem.Desc.InjectorCase";
    public const string ContainerItemDescItemCase = "ContainerItem.Desc.ItemCase";
    public const string ContainerItemDescDogtagCase = "ContainerItem.Desc.DogtagCase";
    public const string ContainerItemDescSimpleWallet = "ContainerItem.Desc.SimpleWallet";
    public const string ContainerItemDescKeycardHolderCase = "ContainerItem.Desc.KeycardHolderCase";
    public const string ContainerItemDescKeyTool = "ContainerItem.Desc.KeyTool";
    public const string ContainerItemDescMoneyCase = "ContainerItem.Desc.MoneyCase";

    // ---- Home ----
    public const string HomeHeroTitle = "Home.Hero.Title";
    public const string HomeHeroSubtitle = "Home.Hero.Subtitle";
    public const string HomeHeroDescription = "Home.Hero.Description";
    public const string HomeHeroCredit = "Home.Hero.Credit";

    // ---- Gallery ----
    public const string GalleryNameRaid = "Gallery.Name.Raid";
    public const string GalleryNameFeature = "Gallery.Name.Feature";
    public const string GalleryNameEconomy = "Gallery.Name.Economy";
    public const string GalleryNameDevelop = "Gallery.Name.Develop";
    public const string GalleryNameContainer = "Gallery.Name.Container";
    public const string GalleryDescRaid = "Gallery.Desc.Raid";
    public const string GalleryDescDevelop = "Gallery.Desc.Develop";
    public const string GalleryDescEconomy = "Gallery.Desc.Economy";
    public const string GalleryDescContainer = "Gallery.Desc.Container";
    public const string GalleryDescFeature = "Gallery.Desc.Feature";
}
