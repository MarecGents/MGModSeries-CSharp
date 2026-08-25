using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Servers;

namespace _MGMod.types.services;

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class ConfigServices(
    ConfigServer  configServer
    )
{
    public AirdropConfig GetAirdropConfig()
    {
        return configServer.GetConfigByString<AirdropConfig>("spt-airdrop");
    }

    public BackupConfig GetBackupConfig()
    {
        return configServer.GetConfigByString<BackupConfig>("spt-backup");
    }

    public BotConfig GetBotConfig()
    {
        return configServer.GetConfigByString<BotConfig>("spt-bot");
    }

    public BtrDeliveryConfig GetBtrDeliveryConfig()
    {
        return configServer.GetConfigByString<BtrDeliveryConfig>("spt-btrdelivery");
    }
    
    public CoreConfig GetCoreConfig()
    {
        return configServer.GetConfigByString<CoreConfig>("spt-core");
    }

    public GiftsConfig GetGiftsConfig()
    {
        return configServer.GetConfigByString<GiftsConfig>("spt-gifts");
    }

    public HealthConfig GetHealthConfig()
    {
        return configServer.GetConfigByString<HealthConfig>("spt-health");
    }

    public HideoutConfig GetHideoutConfig()
    {
        return configServer.GetConfigByString<HideoutConfig>("spt-hideout");
    }

    public HttpConfig GetHttpConfig()
    {
        return configServer.GetConfigByString<HttpConfig>("spt-http");
    }

    public InRaidConfig GetInRaidConfig()
    {
        return configServer.GetConfigByString<InRaidConfig>("spt-inraid");
    }

    public InsuranceConfig GetInsuranceConfig()
    {
        return configServer.GetConfigByString<InsuranceConfig>("spt-insurance");
    }

    public InventoryConfig GetInventoryConfig()
    {
        return configServer.GetConfigByString<InventoryConfig>("spt-inventory");
    }

    public ItemConfig GetItemConfig()
    {
        return configServer.GetConfigByString<ItemConfig>("spt-item");
    }

    public LocaleConfig GetLocaleConfig()
    {
        return configServer.GetConfigByString<LocaleConfig>("spt-locale");
    }

    public LocationConfig GetLocationConfig()
    {
        return configServer.GetConfigByString<LocationConfig>("spt-location");
    }

    public LootConfig GetLootConfig()
    {
        return configServer.GetConfigByString<LootConfig>("spt-loot");
    }

    public LostOnDeathConfig GetLostOnDeathConfig()
    {
        return configServer.GetConfigByString<LostOnDeathConfig>("spt-lostondeath");
    }

    public MatchConfig GetMatchConfig()
    {
        return configServer.GetConfigByString<MatchConfig>("spt-match");
    }

    public PlayerScavConfig GetPlayerScavConfig()
    {
        return configServer.GetConfigByString<PlayerScavConfig>("spt-playerscav");
    }

    public PmcConfig GetPmcConfig()
    {
        return configServer.GetConfigByString<PmcConfig>("spt-pmc");
    }

    public PmcChatResponse GetPmcChatResponseConfig()
    {
        return configServer.GetConfigByString<PmcChatResponse>("spt-pmcchatresponse");
    }

    public QuestConfig GetQuestConfig()
    {
        return configServer.GetConfigByString<QuestConfig>("spt-quest");
    }

    public RagfairConfig GetRagfairConfig()
    {
        return configServer.GetConfigByString<RagfairConfig>("spt-ragfair");
    }

    public RepairConfig GetRepairConfig()
    {
        return configServer.GetConfigByString<RepairConfig>("spt-repair");
    }

    public ScavCaseConfig GetScavCaseConfig()
    {
        return configServer.GetConfigByString<ScavCaseConfig>("spt-scavcase");
    }

    public SeasonalEventConfig GetSeasonalEventConfig()
    {
        return configServer.GetConfigByString<SeasonalEventConfig>("spt-seasonalevents");
    }

    public TraderConfig GetTraderConfig()
    {
        return configServer.GetConfigByString<TraderConfig>("spt-trader");
    }

    public WeatherConfig GetWeatherConfig()
    {
        return configServer.GetConfigByString<WeatherConfig>("spt-weather");
    }
}