using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Bots;
using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.Server.Core.Models.Spt.Templates;
using SPTarkov.Server.Core.Services;
using Hideout = SPTarkov.Server.Core.Models.Spt.Hideout.Hideout;
using Locations = SPTarkov.Server.Core.Models.Spt.Server.Locations;

namespace _MGGTmod.types.services;

// SPT-4.0.X DatabaseServices：包装 SPT 的 DatabaseService（单数），方法名保持本 mod 既有复数命名
[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class DatabaseServices(
    DatabaseService databaseService
)
{
    public Bots GetBots()
    {
        return databaseService.GetBots();
    }

    public Hideout GetHideouts()
    {
        return databaseService.GetHideout();
    }

    public LocaleBase GetLocales()
    {
        return databaseService.GetLocales();
    }

    public Locations GetLocations()
    {
        return databaseService.GetLocations();
    }
    
    public Match GetMatches()
    {
        return databaseService.GetMatch();
    }

    public Templates GetTemplates()
    {
        return databaseService.GetTemplates();
    }

    public Dictionary<MongoId, Trader> GetTraders()
    {
        return databaseService.GetTraders();
    }

    public Globals GetGlobals()
    {
        return databaseService.GetGlobals();
    }

    public ServerBase GetServers()
    {
        return databaseService.GetServer();
    }

    public SettingsBase GetSettings()
    {
        return databaseService.GetSettings();
    }
}
