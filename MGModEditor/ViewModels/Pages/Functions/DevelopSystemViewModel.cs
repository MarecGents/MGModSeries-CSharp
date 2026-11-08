using MGEditor.Models;
using MGEditor.Resources;
using MGEditor.Services;

namespace MGEditor.ViewModels.Pages.Functions;

public partial class DevelopSystemViewModel : ViewModel
{
    private bool _isInitialized = false;

    [ObservableProperty] private MGConfig _configJson;

    [ObservableProperty] private MGConfigService _mGConfigService1;

    [ObservableProperty] private ConfigItems _configItemList;

    public DevelopSystemViewModel(MGConfigService mGConfigService)
    {
        MGConfigService1 = mGConfigService;
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

        _isInitialized = true;
    }

    private void EnableValueInit()
    {
        LoadSpeedValue = ConfigJson.Globals.LoadSpeed.mode;
        BuffsWeaponValue = ConfigJson.Config.Buffs.BuffsWeapon & ConfigJson.Globals.Buffs.BuffsWeapon;
        BuffsArmorValue = ConfigJson.Config.Buffs.BuffsArmor & ConfigJson.Globals.Buffs.BuffsArmor;
        BuildTimeValue = ConfigJson.Hideout.BuildTime.enable ? ConfigJson.Hideout.BuildTime.value : -1.0;
        ProductTimeValue = ConfigJson.Hideout.ProductTime.enable ? ConfigJson.Hideout.ProductTime.value : -1.0;
        ScavCaseTimeValue = ConfigJson.Hideout.ScavCaseTime.enable ? ConfigJson.Hideout.ScavCaseTime.value : -1.0;
        BonusesLevelValue = ConfigJson.Hideout.BonusesLevel.enable ? ConfigJson.Hideout.BonusesLevel.value : 1;
        RewardMultipleValue = ConfigJson.Hideout.Qte.RewardMultiple.enable
            ? ConfigJson.Hideout.Qte.RewardMultiple.value
            : 1;
        CultistCircleTimeValue = ConfigJson.Config.HideoutSystem.CultistCircleTime.enable 
            ? ConfigJson.Config.HideoutSystem.CultistCircleTime.value 
            : -1;
        CultistCultistLevelValue = ConfigJson.Config.HideoutSystem.CultistCultistLevel.enable 
            ? (int)((ConfigJson.Config.HideoutSystem.CultistCultistLevel.value.min - 0.7)/0.2 -1) 
            : -1;
        BonusTimeChanceValue = ConfigJson.Config.HideoutSystem.BonusTimeChance.enable 
            ? ConfigJson.Config.HideoutSystem.BonusTimeChance.value
            : -1.0;
        BonusTimeThresholdValue = ConfigJson.Config.HideoutSystem.BonusTimeThreshold.enable
            ? ConfigJson.Config.HideoutSystem.BonusTimeThreshold.value
            : -1;
        QuestValueValue = ConfigJson.Templates.QuestSystem.QuestValue.enable
            ? ConfigJson.Templates.QuestSystem.QuestValue.value
            : -1;
        QuestCDTimeValue = ConfigJson.Templates.QuestSystem.QuestCDTime.enable
            ? ConfigJson.Templates.QuestSystem.QuestCDTime.value
            : -1;
        
    }


    [ObservableProperty] private string _loadSpeedValue;

    partial void OnLoadSpeedValueChanged(string value)
    {
        ConfigJson.Globals.LoadSpeed.mode = value;
        if (value == "mode1")
        {
            ConfigJson.Globals.LoadSpeed.BaseLoadTime = 0.4;
            ConfigJson.Globals.LoadSpeed.BaseUnloadTime = 0.2;
        }
        else if (value == "mode2")
        {
            ConfigJson.Globals.LoadSpeed.BaseLoadTime = 0.1;
            ConfigJson.Globals.LoadSpeed.BaseUnloadTime = 0.1;
        }
        else if (value == "mode3")
        {
            ConfigJson.Globals.LoadSpeed.BaseLoadTime = 0.01;
            ConfigJson.Globals.LoadSpeed.BaseUnloadTime = 0.01;
        }
        else
        {
            ConfigJson.Globals.LoadSpeed.BaseLoadTime = 0.85;
            ConfigJson.Globals.LoadSpeed.BaseUnloadTime = 0.3;
        }
    }

    [ObservableProperty] private bool _buffsWeaponValue;

    partial void OnBuffsWeaponValueChanged(bool value)
    {
        ConfigJson.Config.Buffs.BuffsWeapon = value;
        ConfigJson.Globals.Buffs.BuffsWeapon = value;
    }

    [ObservableProperty] private bool _buffsArmorValue;

    partial void OnBuffsArmorValueChanged(bool value)
    {
        ConfigJson.Config.Buffs.BuffsArmor = value;
        ConfigJson.Globals.Buffs.BuffsArmor = value;
    }

    [ObservableProperty] private double _buildTimeValue;

    partial void OnBuildTimeValueChanged(double value)
    {
        if (value < 0)
        {
            ConfigJson.Hideout.BuildTime.enable = false;
        }
        else
        {
            ConfigJson.Hideout.BuildTime.enable = true;
            ConfigJson.Hideout.BuildTime.value = value;
        }
    }

    [ObservableProperty] private double _productTimeValue;

    partial void OnProductTimeValueChanged(double value)
    {
        if (value < 0)
        {
            ConfigJson.Hideout.ProductTime.enable = false;
        }
        else
        {
            ConfigJson.Hideout.ProductTime.enable = true;
            ConfigJson.Hideout.ProductTime.value = value;
        }
    }

    [ObservableProperty] private double _scavCaseTimeValue;

    partial void OnScavCaseTimeValueChanged(double value)
    {
        if (value < 0)
        {
            ConfigJson.Hideout.ScavCaseTime.enable = false;
        }
        else
        {
            ConfigJson.Hideout.ScavCaseTime.enable = true;
            ConfigJson.Hideout.ScavCaseTime.value = value;
        }
    }

    [ObservableProperty] private int _bonusesLevelValue;

    partial void OnBonusesLevelValueChanged(int value)
    {
        if (value == 1)
        {
            ConfigJson.Hideout.BonusesLevel.enable = false;
        }
        else
        {
            ConfigJson.Hideout.BonusesLevel.enable = true;
            ConfigJson.Hideout.BonusesLevel.value = value;
        }
    }

    [ObservableProperty] private int _rewardMultipleValue;

    partial void OnRewardMultipleValueChanged(int value)
    {
        if (value == 1)
        {
            ConfigJson.Hideout.Qte.RewardMultiple.enable = false;
        }
        else
        {
            ConfigJson.Hideout.Qte.RewardMultiple.enable = true;
            ConfigJson.Hideout.Qte.RewardMultiple.value = value;
        }
    }

    [ObservableProperty] private int _cultistCircleTimeValue;

    partial void OnCultistCircleTimeValueChanged(int value)
    {
        if (value == -1)
        {
            ConfigJson.Config.HideoutSystem.CultistCircleTime.enable = false;
        }
        else
        {
            ConfigJson.Config.HideoutSystem.CultistCircleTime.enable = true;
            ConfigJson.Config.HideoutSystem.CultistCircleTime.value = value;
        }
    }

    [ObservableProperty] private int _cultistCultistLevelValue;

    partial void OnCultistCultistLevelValueChanged(int value)
    {
        MinMax<double> defaultMinMax = new MinMax<double>()
        {
            min = 0.7,
            max = 1.4,
        };
        if (value == -1)
        {
            ConfigJson.Config.HideoutSystem.CultistCultistLevel.enable = false;
        }
        else
        {
            ConfigJson.Config.HideoutSystem.CultistCultistLevel.enable = true;
            defaultMinMax.min = defaultMinMax.min + (value + 1) * 0.2;
            defaultMinMax.max = defaultMinMax.max + (value + 1) * 0.2;
            ConfigJson.Config.HideoutSystem.CultistCultistLevel.value = defaultMinMax;
        }
    }

    [ObservableProperty] private double _bonusTimeChanceValue;

    partial void OnBonusTimeChanceValueChanged(double value)
    {
        if (value < 0)
        {
            ConfigJson.Config.HideoutSystem.BonusTimeChance.enable = false;
        }
        else
        {
            ConfigJson.Config.HideoutSystem.BonusTimeChance.enable = true;
            ConfigJson.Config.HideoutSystem.BonusTimeChance.value = value;
        }
    }

    [ObservableProperty] private int _bonusTimeThresholdValue;

    partial void OnBonusTimeThresholdValueChanged(int value)
    {
        if (value == -1)
        {
            ConfigJson.Config.HideoutSystem.BonusTimeThreshold.enable = false;
        }
        else
        {
            ConfigJson.Config.HideoutSystem.BonusTimeThreshold.enable = true;
            ConfigJson.Config.HideoutSystem.BonusTimeThreshold.value = value;
        }
    }

    [ObservableProperty] private double _questValueValue;

    partial void OnQuestValueValueChanged(double value)
    {
        if (value is -1)
        {
            ConfigJson.Templates.QuestSystem.QuestValue.enable = false;
        }
        else
        {
            ConfigJson.Templates.QuestSystem.QuestValue.enable = true;
            ConfigJson.Templates.QuestSystem.QuestValue.value = value;
        }
    }
    
    [ObservableProperty] private int _questCDTimeValue;

    partial void OnQuestCDTimeValueChanged(int value)
    {
        if (value is -1)
        {
            ConfigJson.Templates.QuestSystem.QuestCDTime.enable = false;
        }
        else
        {
            ConfigJson.Templates.QuestSystem.QuestCDTime.enable = true;
            ConfigJson.Templates.QuestSystem.QuestCDTime.value = value;
        }
    }
}