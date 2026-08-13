using System.Windows.Documents;
using MGEditor.Models;
using MGEditor.Resources;
using MGEditor.Services;

namespace MGEditor.ViewModels.Pages.Functions;

public partial class RaidSystemViewModel: ViewModel
{
    private bool _isInitialized = false;

    [ObservableProperty]
    private MGConfig _configJson;

    [ObservableProperty]
    private MGConfigService _mGConfigService1;

    [ObservableProperty]
    private ConfigItems _configItemList;

    public RaidSystemViewModel(MGConfigService mGConfigService)
    {
        MGConfigService1 = mGConfigService;
        MGConfigService1.SaveConfig();
    }

    public override void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    private void InitializeViewModel()
    {

        ConfigJson = MGConfigService1.ConfigJson;

        ConfigItemList = MGConfigService1.ConfigItemsList;

        EnableValueInit();

        WeatherSettingInit();

        MapInsuranceSettingInit();

        _isInitialized = true;
    }



    private void EnableValueInit()
    {
        UsecRateValue = ConfigJson.Config.USECRate.enable ? ConfigJson.Config.USECRate.value : -1;
        RaidTimeValue = ConfigJson.Locations.RaidTime.enable ? ConfigJson.Locations.RaidTime.value : -1.0;
        BossSpwanChanceValue = ConfigJson.Locations.BOSSSpwanChance.enable ? ConfigJson.Locations.BOSSSpwanChance.value : -1.0;
    }

    [ObservableProperty]
    private int _usecRateValue;

    partial void OnUsecRateValueChanged(int value)
    {
        if (value < 0)
        {
            ConfigJson.Config.USECRate.enable = false;
        }
        else
        {
            ConfigJson.Config.USECRate.enable = true;
            ConfigJson.Config.USECRate.value = value;
        }
    }

    [ObservableProperty]
    private double _raidTimeValue;
    partial void OnRaidTimeValueChanged(double value)
    {
        if (value < 0)
        {
            ConfigJson.Locations.RaidTime.enable = false;
        }
        else
        {
            ConfigJson.Locations.RaidTime.enable = true;
            ConfigJson.Locations.RaidTime.value = value;
        }
    }

    [ObservableProperty]
    private double _bossSpwanChanceValue;
    partial void OnBossSpwanChanceValueChanged(double value)
    {
        if (value < 0)
        {
            ConfigJson.Locations.BOSSSpwanChance.enable = false;
        }
        else
        {
            ConfigJson.Locations.BOSSSpwanChance.enable = true;
            ConfigJson.Locations.BOSSSpwanChance.value = value;
        }
    }

    [ObservableProperty]
    private bool _weatherSettingEnable;

    [ObservableProperty]
    private string _weatherMode;
    [ObservableProperty]
    private string _cloudsMode;
    [ObservableProperty]
    private string _windMode;
    [ObservableProperty]
    private string _rainMode;
    [ObservableProperty]
    private string _fogMode;

    private void WeatherSettingInit()
    {
        SetWeatherSettingEnable();
        WeatherMode = ConfigJson.Config.WeatherSettings.mode;
        CloudsMode = ConfigJson.Config.WeatherSettings.clouds.type;
        ConfigJson.Config.WeatherSettings.clouds.weights = ConfigItemList.CloudModeWeight[CloudsMode];
        WindMode = ConfigJson.Config.WeatherSettings.windSpeed.type;
        ConfigJson.Config.WeatherSettings.windSpeed.weights = ConfigItemList.WindModeWeight[WindMode];
        RainMode = ConfigJson.Config.WeatherSettings.rain.type;
        ConfigJson.Config.WeatherSettings.rain.weights = ConfigItemList.RainModeWeight[RainMode];
        SetRainIntensity(RainMode);
        FogMode = ConfigJson.Config.WeatherSettings.fog.type;
        ConfigJson.Config.WeatherSettings.fog.weights = ConfigItemList.FogModeWeight[FogMode];
    }
    private void SetWeatherSettingEnable()
    {
        WeatherSettingEnable = ConfigJson.Config.WeatherSettings.mode == "Custom";
    }

    partial void OnWeatherModeChanged(string value)
    {
        if (value == "Custom")
        {
            ConfigJson.Config.WeatherSettings.mode = value;
        }
        else
        {
            ConfigJson.Config.WeatherSettings.mode = value;
            switch (value)
            {
                case "default":
                    CloudsMode = "default";
                    WindMode = "default";
                    RainMode = "default";
                    FogMode = "default";
                    break;
                case "mode1":
                    CloudsMode = "mode1";
                    WindMode = "mode1";
                    RainMode = "mode1";
                    FogMode = "mode1";
                    break;
                case "mode2":
                    CloudsMode = "mode4";
                    WindMode = "mode2";
                    RainMode = "mode2";
                    FogMode = "mode2";
                    break;
                case "mode3":
                    CloudsMode = "mode4";
                    WindMode = "mode4";
                    RainMode = "mode5";
                    FogMode = "mode1";
                    break;
                case "mode4":
                    CloudsMode = "mode4";
                    WindMode = "mode1";
                    RainMode = "mode1";
                    FogMode = "mode3";
                    break;
                case "mode5":
                    CloudsMode = "mode4";
                    WindMode = "mode4";
                    RainMode = "mode5";
                    FogMode = "mode4";
                    break;
                default:
                    break;
            }
        }
        SetWeatherSettingEnable();
    }
    partial void OnCloudsModeChanged(string value)
    {
        ConfigJson.Config.WeatherSettings.clouds.type = value;
        ConfigJson.Config.WeatherSettings.clouds.weights = ConfigItemList.CloudModeWeight[value];
    }
    partial void OnWindModeChanged(string value)
    {
        ConfigJson.Config.WeatherSettings.windSpeed.type = value;
        ConfigJson.Config.WeatherSettings.windSpeed.weights = ConfigItemList.WindModeWeight[value];
    }
    partial void OnRainModeChanged(string value)
    {
        ConfigJson.Config.WeatherSettings.rain.type = value;
        ConfigJson.Config.WeatherSettings.rain.weights = ConfigItemList.RainModeWeight[value];
        SetRainIntensity(value);
    }
    private void SetRainIntensity(string value)
    {
        switch  (value)
        {
            case "mode1":
            {
                ConfigJson.Config.WeatherSettings.rainIntensity.Min = 0;
                ConfigJson.Config.WeatherSettings.rainIntensity.Max = 0;
                break;
            }
            case "mode2":
            {
                ConfigJson.Config.WeatherSettings.rainIntensity.Min = 0.2;
                ConfigJson.Config.WeatherSettings.rainIntensity.Max = 0.2;
                break;
            }
            case "mode3":
            {
                ConfigJson.Config.WeatherSettings.rainIntensity.Min = 0.4;
                ConfigJson.Config.WeatherSettings.rainIntensity.Max = 0.4;
                break;
            }
            case "mode4":
            {
                ConfigJson.Config.WeatherSettings.rainIntensity.Min = 0.75;
                ConfigJson.Config.WeatherSettings.rainIntensity.Max = 0.75;
                break;
            }
            case "mode5":
            {
                ConfigJson.Config.WeatherSettings.rainIntensity.Min = 1;
                ConfigJson.Config.WeatherSettings.rainIntensity.Max = 1;
                break;
            }
            case "default":
            {
                ConfigJson.Config.WeatherSettings.rainIntensity.Min = 0;
                ConfigJson.Config.WeatherSettings.rainIntensity.Max = 1;
                break;
            }
        }
    }
    partial void OnFogModeChanged(string value)
    {
        ConfigJson.Config.WeatherSettings.fog.type = value;
        ConfigJson.Config.WeatherSettings.fog.weights = ConfigItemList.FogModeWeight[value];
    }

    [ObservableProperty]
    private bool _isBigmapEnable;
    [ObservableProperty]
    private bool _isFactoryEnable;
    [ObservableProperty]
    private bool _isInterchangeEnable;
    [ObservableProperty]
    private bool _isLaboratoryEnable;
    [ObservableProperty]
    private bool _isLighthouseEnable;
    [ObservableProperty]
    private bool _isRezervBaseEnable;
    [ObservableProperty]
    private bool _isSandboxEnable;
    [ObservableProperty]
    private bool _isShorelineEnable;
    [ObservableProperty]
    private bool _isTarkovStreetsEnable;
    [ObservableProperty]
    private bool _isWoodsEnable;
    [ObservableProperty]
    private bool _isLabyrinthEnable;

    private void MapInsuranceSettingInit()
    {
        
        IsBigmapEnable = ConfigJson.Locations.MapInsurance["Bigmap"];
        IsFactoryEnable = ConfigJson.Locations.MapInsurance["Factory4Day"] & ConfigJson.Locations.MapInsurance["Factory4Night"];
        IsInterchangeEnable = ConfigJson.Locations.MapInsurance["Interchange"];
        IsLaboratoryEnable = ConfigJson.Locations.MapInsurance["Laboratory"];
        IsLighthouseEnable = ConfigJson.Locations.MapInsurance["Lighthouse"];
        IsRezervBaseEnable = ConfigJson.Locations.MapInsurance["RezervBase"];
        IsSandboxEnable = ConfigJson.Locations.MapInsurance["Sandbox"] & ConfigJson.Locations.MapInsurance["SandboxHigh"];
        IsShorelineEnable = ConfigJson.Locations.MapInsurance["Shoreline"];
        IsTarkovStreetsEnable = ConfigJson.Locations.MapInsurance["TarkovStreets"];
        IsWoodsEnable = ConfigJson.Locations.MapInsurance["Woods"];
        IsLabyrinthEnable = ConfigJson.Locations.MapInsurance["Labyrinth"];
    }

    partial void OnIsBigmapEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["Bigmap"] = value;
    }
    partial void OnIsFactoryEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["Factory4Day"] = value;
        ConfigJson.Locations.MapInsurance["Factory4Night"] = value;
    }
    partial void OnIsInterchangeEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["Interchange"] = value;
    }
    partial void OnIsLaboratoryEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["Laboratory"] = value;
    }

    partial void OnIsLighthouseEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["Lighthouse"] = value;
    }
    partial void OnIsRezervBaseEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["RezervBase"] = value;
    }
    partial void OnIsSandboxEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["Sandbox"] = value;
        ConfigJson.Locations.MapInsurance["SandboxHigh"] = value;
    }
    partial void OnIsShorelineEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["Shoreline"] = value;
    }
    partial void OnIsTarkovStreetsEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["TarkovStreets"] = value;
    }
    partial void OnIsWoodsEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["Woods"] = value;
    }
    partial void OnIsLabyrinthEnableChanged(bool value)
    {
        ConfigJson.Locations.MapInsurance["Labyrinth"] = value;
    }
    
}
