using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;

namespace _MGMod.types.services;

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.Preload + 1)]
public class ConfigServices(
    AirdropConfig airdrop,
    BackupConfig backup,
    BotConfig bot,
    BtrDeliveryConfig btrDelivery,
    CoreConfig core,
    GiftsConfig gifts,
    HealthConfig health,
    HideoutConfig hideout,
    HttpConfig hHttp,
    InRaidConfig inRaid,
    InsuranceConfig insurance,
    InventoryConfig inventory,
    ItemConfig item,
    LocaleConfig locale,
    LocationConfig location,
    LootConfig loot,
    LostOnDeathConfig lostOnDeath,
    MatchConfig match,
    PlayerScavConfig playerScav,
    PmcConfig pmc,
    PmcChatResponseConfig pmcChatResponse,
    QuestConfig quest,
    RagfairConfig ragfair,
    RepairConfig repair,
    ScavCaseConfig scavCase,
    SeasonalEventConfig seasonalEvent,
    TraderConfig trader,
    WeatherConfig weather
    )
{
    public AirdropConfig GetAirdropConfig()
    {
        return airdrop;
    }

    public BackupConfig GetBackupConfig()
    {
        return backup;
    }

    public BotConfig GetBotConfig()
    {
        return bot;
    }

    public BtrDeliveryConfig GetBtrDeliveryConfig()
    {
        return btrDelivery;
    }
    
    public CoreConfig GetCoreConfig()
    {
        return core;
    }

    public GiftsConfig GetGiftsConfig()
    {
        return gifts;
    }

    public HealthConfig GetHealthConfig()
    {
        return health;
    }

    public HideoutConfig GetHideoutConfig()
    {
        return hideout;
    }

    public HttpConfig GetHttpConfig()
    {
        return hHttp;
    }

    public InRaidConfig GetInRaidConfig()
    {
        return inRaid;
    }

    public InsuranceConfig GetInsuranceConfig()
    {
        return insurance;
    }

    public InventoryConfig GetInventoryConfig()
    {
        return inventory;
    }

    public ItemConfig GetItemConfig()
    {
        return item;
    }

    public LocaleConfig GetLocaleConfig()
    {
        return locale;
    }

    public LocationConfig GetLocationConfig()
    {
        return location;
    }

    public LootConfig GetLootConfig()
    {
        return loot;
    }

    public LostOnDeathConfig GetLostOnDeathConfig()
    {
        return lostOnDeath;
    }

    public MatchConfig GetMatchConfig()
    {
        return match;
    }

    public PlayerScavConfig GetPlayerScavConfig()
    {
        return playerScav;
    }

    public PmcConfig GetPmcConfig()
    {
        return pmc;
    }

    public PmcChatResponseConfig GetPmcChatResponseConfig()
    {
        return pmcChatResponse;
    }

    public QuestConfig GetQuestConfig()
    {
        return quest;
    }

    public RagfairConfig GetRagfairConfig()
    {
        return ragfair;
    }

    public RepairConfig GetRepairConfig()
    {
        return repair;
    }

    public ScavCaseConfig GetScavCaseConfig()
    {
        return scavCase;
    }

    public SeasonalEventConfig GetSeasonalEventConfig()
    {
        return seasonalEvent;
    }

    public TraderConfig GetTraderConfig()
    {
        return trader;
    }

    public WeatherConfig GetWeatherConfig()
    {
        return weather;
    }
}