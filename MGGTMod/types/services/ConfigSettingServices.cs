using _MGGTmod.types.models.Custom;
using _MGGTmod.types.models.Paths;
using _MGGTmod.types.server;
using _MGGTmod.types.utils;
using Spectre.Console;
using SPTarkov.Common.Logger;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace _MGGTmod.types.services;

[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class ConfigSettingServices
{
    private SptLogger<ConfigSettingServices> logger;
    private MGUtils mGUtils;
    private CustomTraderServices  customTraderServices;
    private TestServices testServices;
    public ConfigSettingServices(
        SptLogger<ConfigSettingServices> _logger,
        MGUtils _mGUtils,
        CustomTraderServices _customTraderServices,
        TestServices _testServices
        )
    {
        logger = _logger;
        mGUtils = _mGUtils;
        customTraderServices = _customTraderServices;
        testServices = _testServices;
    }

    public async Task ModSetting()
    {
        customTraderServices.Start();
        mGUtils.Log_GT("商人系统", "加载完毕。", Color.Green);
    }
}
