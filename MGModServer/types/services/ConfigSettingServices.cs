using _MGMod.types.models.Custom;
using _MGMod.types.models.Paths;
using _MGMod.types.server;
using _MGMod.types.utils;
using Spectre.Console;
using SPTarkov.Common.Logger;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace _MGMod.types.services;

[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class ConfigSettingServices
{
    private SptLogger<ConfigSettingServices> logger;
    private ConfigSettingType? configJson;
    private MGUtils mGUtils;

    private BotsServer botsServer;
    private ConfigsServer configsServer;
    private GlobalsServer globalsServer;
    private HideoutServer hideoutServer;
    private LocationsServer locationsServer;
    private TemplatesServer templatesServer;
    private TradersServer tradersServer;

    private CustomItemServices customItemServices;
    private KeyClassfyServices keyClassfyServices;
    private SyncFleaMarketServices syncFleaMarketServices;
    private CustomTraderServices  customTraderServices;
    private CustomProfileServices  customProfileServices;
    private CustomAssortServices  customAssortServices;
    private TestServices testServices;
    public ConfigSettingServices(
        SptLogger<ConfigSettingServices> _logger,
        MGUtils _mGUtils,

        BotsServer _botsServer,
        ConfigsServer _configsServer,
        GlobalsServer _globalsServer,
        HideoutServer _hideoutServer,
        LocationsServer _locationsServer,
        TemplatesServer _templatesServer,
        TradersServer _tradersServer,
        
        CustomTraderServices _customTraderServices,
        CustomItemServices _customItemServices,
        CustomProfileServices _customProfileServices,
        CustomAssortServices _customAssortServices,
        KeyClassfyServices _keyClassfyServices,
        SyncFleaMarketServices _syncFleaMarketServices,
            
        TestServices _testServices
        )
    {
        logger = _logger;
        mGUtils = _mGUtils;

        botsServer = _botsServer;
        configsServer = _configsServer;
        globalsServer = _globalsServer;
        hideoutServer = _hideoutServer;
        locationsServer = _locationsServer;
        templatesServer = _templatesServer;
        tradersServer = _tradersServer;

        customTraderServices = _customTraderServices;
        customItemServices = _customItemServices;
        customProfileServices = _customProfileServices;
        customAssortServices = _customAssortServices;
        keyClassfyServices = _keyClassfyServices;
        syncFleaMarketServices = _syncFleaMarketServices;
        
        testServices = _testServices;
        
        configJson = LoadConfig();
    }

    private ConfigSettingType? LoadConfig()
    {
        // 主配置：损坏/缺失容错（SPT JsonUtil 在 JSON 损坏时抛 JsonException）
        try
        {
            var config = mGUtils.GetJsonDataFromFile<ConfigSettingType>(Paths.ConfigJson);
            if (config != null) return config;
        }
        catch (Exception ex)
        {
            mGUtils.Log("常规设置", $"config.json 解析失败：{ex.Message}", Color.Red);
        }

        // 回退默认配置（与编辑器同源 defaultConfig.json，保证服务端可自愈启动）
        try
        {
            var defaultConfig = mGUtils.GetJsonDataFromFile<ConfigSettingType>(Paths.DefaultConfigJson);
            if (defaultConfig != null)
            {
                mGUtils.Log("常规设置", "config.json 缺失或损坏，已回退 defaultConfig.json。", Color.Red);
                return defaultConfig;
            }
        }
        catch (Exception ex)
        {
            mGUtils.Log("常规设置", $"defaultConfig.json 解析失败：{ex.Message}", Color.Red);
        }

        return null;
    }

    public async Task ModSetting()
    {
        if (configJson == null)
        {
            mGUtils.Log("常规设置", "config.json 与 defaultConfig.json 均缺失/损坏，配置未加载。", Color.Red);
            return;
        }

        var CustomSetting = GetMGCustomSetting();
        // testServices.Initialize();
        if (CustomSetting != null)
        {
            if (CustomSetting.SyncFlea) await syncFleaMarketServices.Start();
            if (CustomSetting.CustomTrader) customTraderServices.Start();
            if (CustomSetting.CustomItem) customItemServices.Start();
            if (CustomSetting.KeyClassfy) keyClassfyServices.Start();
            if (CustomSetting.CustomProfile) customProfileServices.Start();
            if (CustomSetting.CustomAssort) customAssortServices.Start();
        }
        if (configJson.Bot != null) botsServer.MGmodBots(GetBotSetting());
        if (configJson.Config != null) configsServer.MGmodConfigs(GetConfigSetting());
        if (configJson.Globals != null) globalsServer.MGmodGlobals(GetGlobalsSetting());
        if (configJson.Hideout != null) hideoutServer.MGmodHideout(GetHideoutSetting());
        if (configJson.Locations != null) locationsServer.MGmodLocations(GetLocationsSetting());
        if (configJson.Templates != null) templatesServer.MGmodTemplates(GetTemplatesSetting());
        if (configJson.Traders != null) tradersServer.MGmodTraders(GetTradersSetting());
        mGUtils.Log("常规设置", "已开启。", Color.Yellow);
    }

    private MGModConfig_Bot? GetBotSetting()
    { 
        if(configJson != null)
        {
            return configJson.Bot;
        }
        return null;
    }

    private MGModConfig_Config? GetConfigSetting()
    {
        if (configJson != null)
        {
            return configJson.Config;
        }
        return null;
    }
    private MGModConfig_Globals? GetGlobalsSetting()
    {
        if (configJson != null)
        {
            return configJson.Globals;
        }
        return null;

    }
    private MGModConfig_Hideout? GetHideoutSetting()
    {
        if (configJson != null)
        {
            return configJson.Hideout;
        }
        return null;
    }
    private MGModConfig_Locations? GetLocationsSetting()
    {
        if (configJson != null)
        {
            return configJson.Locations;
        }
        return null;
    }
    private MGModConfig_Templates? GetTemplatesSetting()
    {
        if (configJson != null)
        {
            return configJson.Templates;
        }
        return null;
    }
    private MGModConfig_Traders? GetTradersSetting()
    {
        if (configJson != null)
        {
            return configJson.Traders;
        }
        return null;
    }
    private MGModConfig_MGCustom? GetMGCustomSetting()
    {
        if (configJson != null)
        {
            return configJson.MGCustom;
        }
        return null;
    }
}
