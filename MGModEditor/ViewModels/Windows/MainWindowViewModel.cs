using MGEditor.Resources;
using MGEditor.Services;
using MGEditor.Views.Pages;
using MGEditor.Views.Pages.Functions;
namespace MGEditor.ViewModels.Windows;

public partial class MainWindowViewModel : ViewModel
{
    private readonly TranslationService _translationService;

    [ObservableProperty]
    private string _applicationTitle = "MG Editor";

    [ObservableProperty]
    private ObservableCollection<object> _menuItems = new();

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = new();

    [ObservableProperty]
    private ObservableCollection<Wpf.Ui.Controls.MenuItem> _trayMenuItems = new()
        {
            new Wpf.Ui.Controls.MenuItem { Header = "Home", Tag = "tray_home" }
        };

    public MainWindowViewModel(TranslationService translationService)
    {
        _translationService = translationService;
        RebuildMenuItems();
        _translationService.LanguageChanged += RebuildMenuItems;
    }

    /// <summary>语言切换时重建导航项（导航文本随当前语言刷新）。</summary>
    private void RebuildMenuItems()
    {
        MenuItems = new ObservableCollection<object>
        {
            new NavigationViewItem()
            {
                Content = _translationService[Translations.NavHome],
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home12 },
                TargetPageType = typeof(HomePage)
            },
            new NavigationViewItem(_translationService[Translations.NavRaid], SymbolRegular.PersonWalking16, typeof(RaidSystemPage)),
            new NavigationViewItem(_translationService[Translations.NavEconomy], SymbolRegular.CurrencyDollarEuro16, typeof(EconomySystemPage)),
            new NavigationViewItem(_translationService[Translations.NavDevelop], SymbolRegular.Guardian24, typeof(DevelopSystemPage)),
            new NavigationViewItem(_translationService[Translations.NavContainer], SymbolRegular.BoxArrowUp24, typeof(ContainerExpandPage)),
            new NavigationViewItem(_translationService[Translations.NavFeature], SymbolRegular.Premium12, typeof(ExclusiveFeaturePage)),
        };

        FooterMenuItems = new ObservableCollection<object>
        {
            new NavigationViewItem()
            {
                Content = _translationService[Translations.NavSettings],
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(SettingsPage)
            }
        };
    }
}
