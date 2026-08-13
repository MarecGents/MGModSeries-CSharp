using MGEditor.Models;
using MGEditor.Resources;
using MGEditor.Services;

namespace MGEditor.ViewModels.Pages.Functions;

public partial class ExclusiveFeatureViewModel : ViewModel
{
    private bool _isInitialized = false;

    [ObservableProperty]
    private MGConfig _configJson;

    [ObservableProperty]
    private MGConfigService _mGConfigService1;

    [ObservableProperty]
    private ConfigItems _configItemList;

    public ExclusiveFeatureViewModel(MGConfigService mGConfigService)
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

        _isInitialized = true;
    }
}
