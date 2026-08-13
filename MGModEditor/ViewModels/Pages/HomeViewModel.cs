using MGEditor.ControlsLookup;
using MGEditor.Helpers;
using MGEditor.Models;
using MGEditor.Resources;
using MGEditor.Services;
using MGEditor.Views.Pages;


namespace MGEditor.ViewModels.Pages;

public partial class HomeViewModel: ViewModel
{
    private bool _isInitialized = false;
    
    [ObservableProperty] 
    private INavigationService _navigationService;
    
    [ObservableProperty] 
    private AppSettingService  _appSettingService1;

    private readonly TranslationService _translationService;
    

    public HomeViewModel(
        INavigationService navigationService,
        AppSettingService appSettingService,
        TranslationService translationService
        )
    {
        NavigationService =  navigationService;
        AppSettingService1 = appSettingService;
        _translationService = translationService;
        RebuildNavigationCards();
        _translationService.LanguageChanged += RebuildNavigationCards;
    }

    public override void OnNavigatedTo()
    {
        if  (!_isInitialized)
            InitializeViewModel();
            
    }

    private void InitializeViewModel()
    {
        AppSettingService1.ApplySettings();
        
        _isInitialized = true;
    }

    [RelayCommand]
    private void OnCardClick(string parameter)
    {
        if (string.IsNullOrWhiteSpace(parameter))
        {
            return;
        }

        Type? pageType = NameToPageTypeConverter.Convert(parameter);

        if (pageType == null)
        {
            return;
        }

        _ = NavigationService.Navigate(pageType);
    }

    [ObservableProperty]
    private ICollection<NavigationCard> _navigationCards = new ObservableCollection<NavigationCard>();

    /// <summary>语言切换时重建首页卡片（名称与描述随当前语言刷新）。</summary>
    private void RebuildNavigationCards()
    {
        NavigationCards = new ObservableCollection<NavigationCard>(
            ControlPages
                .FromNamespace(typeof(HomePage).Namespace!)
                .Select(x => new NavigationCard()
                {
                    Name = GalleryPageName.GalleryName.TryGetValue(x.Name, out var nameKey)
                        ? _translationService[nameKey]
                        : x.Name,
                    Icon = x.Icon,
                    Description = _translationService[x.Description],
                    PageType = x.PageType,
                }));
    }
}
