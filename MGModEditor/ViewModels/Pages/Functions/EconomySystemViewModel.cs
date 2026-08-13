using MGEditor.Models;
using MGEditor.Resources;
using MGEditor.Services;

namespace MGEditor.ViewModels.Pages.Functions;

public partial class EconomySystemViewModel: ViewModel
{
    private bool _isInitialized = false;

    [ObservableProperty]
    private MGConfig _configJson;

    [ObservableProperty]
    private MGConfigService _mGConfigService1;

    [ObservableProperty]
    private ConfigItems _configItemList;

    public EconomySystemViewModel(MGConfigService mGConfigService)
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

        _isInitialized = true;
    }

    private void EnableValueInit()
    {
        UpdateTimeValue = ConfigJson.Config.UpdateTime.enable ? ConfigJson.Config.UpdateTime.value : -1;
        InsuranceTimeValue = ConfigJson.Traders.InsuranceTime.enable ? ConfigJson.Traders.InsuranceTime.value : -1;
        InsuranceCostValue = ConfigJson.Traders.InsuranceCost.enable ? ConfigJson.Traders.InsuranceCost.value : -1;
        ReturnChanceValue = ConfigJson.Config.ReturnChance.enable ? ConfigJson.Config.ReturnChance.value : -1;
    }

    [ObservableProperty]
    private int _updateTimeValue;

    partial void OnUpdateTimeValueChanged(int value)
    {
        if (value < 0)
        {
            ConfigJson.Config.UpdateTime.enable = false;
        }
        else
        {
            ConfigJson.Config.UpdateTime.enable = true;
            ConfigJson.Config.UpdateTime.value = value;
        }
    }

    [ObservableProperty]
    private int _insuranceTimeValue;

    partial void OnInsuranceTimeValueChanged(int value)
    {
        if (value < 0)
        {
            ConfigJson.Traders.InsuranceTime.enable = false;
        }
        else
        {
            ConfigJson.Traders.InsuranceTime.enable = true;
            ConfigJson.Traders.InsuranceTime.value = value;
        }
    }

    [ObservableProperty]
    private double _insuranceCostValue;
    partial void OnInsuranceCostValueChanged(double value)
    {
        if (value < 0)
        {
            ConfigJson.Traders.InsuranceCost.enable = false;
        }
        else
        {
            ConfigJson.Traders.InsuranceCost.enable = true;
            ConfigJson.Traders.InsuranceCost.value = value;
        }
    }

    [ObservableProperty]
    private int _returnChanceValue;
    partial void OnReturnChanceValueChanged(int value)
    {
        if(value < 0)
        {
            ConfigJson.Config.ReturnChance.enable = false;
        }
        else
        {
            ConfigJson.Config.ReturnChance.enable = true;
            ConfigJson.Config.ReturnChance.value = value;
        }
    }
}
