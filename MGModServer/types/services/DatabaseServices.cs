using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _MGMod.types.services;

// SPT-4.1.X DatabaseServices
[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.Preload + 1)]
public class DatabaseServices(
    BotTable bots,
    HideoutTable hideouts,
    LocaleTable locales,
    LocationTable locations,
    MatchTable matches,
    TemplateTable templates,
    TradersTable traders,
    GlobalTable globals,
    ServerTable servers,
    SettingsTable settings
    )
{
    public BotTable GetBots()
    {
        return bots;
    }

    public HideoutTable GetHideouts()
    {
        return hideouts;
    }

    public LocaleTable GetLocales()
    {
        return locales;
    }

    public LocationTable GetLocations()
    {
        return locations;
    }
    
    public MatchTable GetMatches()
    {
        return matches;
    }

    public TemplateTable GetTemplates()
    {
        return templates;
    }

    public TradersTable GetTraders()
    {
        return traders;
    }

    public GlobalTable GetGlobals()
    {
        return globals;
    }

    public ServerTable GetServers()
    {
        return servers;
    }

    public SettingsTable GetSettings()
    {
        return settings;
    }
}