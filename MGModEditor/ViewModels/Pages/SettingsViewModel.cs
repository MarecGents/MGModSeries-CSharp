using MGEditor.Resources;
using MGEditor.Services;

namespace MGEditor.ViewModels.Pages;

public partial class SettingsViewModel : ViewModel
{
    private bool _isInitialized = false;

    [ObservableProperty]
    private AppSettingService _appSettingService1;

    private readonly TranslationService _translationService;

    public SettingsViewModel(
        AppSettingService appSettingService,
        TranslationService translationService)
    {
        AppSettingService1 = appSettingService;
        _translationService = translationService;
    }

	public override void OnNavigatedTo()
	{
        if (!_isInitialized)
            InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        SettingValueInit();

        _isInitialized = true;
    }

    private void SettingValueInit()
    {
        ThemeTypeValue = AppSettingService1.EditorSetting.Personalized.Theme;
        LanguageValue = AppSettingService1.EditorSetting.Personalized.Language ?? _translationService.CurrentLanguage;
        AppVersion = $"MGEditor - {GetAssemblyVersion()}";
    }

    private string GetAssemblyVersion()
    {
        return GalleryAssembly.Asssembly.GetName().Version?.ToString()
               ?? String.Empty;
    }
    
    [ObservableProperty]
    private string _themeTypeValue;

    partial void OnThemeTypeValueChanged(string value)
    {
        if (String.IsNullOrEmpty(value))
            return;
        if (value == "Light")
        {
            CustomThemeService.Remove();
            ApplicationThemeManager.Apply(ApplicationTheme.Light, WindowBackdropType.None, false);
        }
        else if (value == "Dark")
        {
            CustomThemeService.Remove();
            ApplicationThemeManager.Apply(ApplicationTheme.Dark, WindowBackdropType.None, false);
        }
        else if (value == "HighContrast")
        {
            CustomThemeService.Remove();
            ApplicationThemeManager.Apply(ApplicationTheme.HighContrast, WindowBackdropType.None, false);
        }
        else
        {
            CustomThemeService.Apply(value);
        }
        AppSettingService1.EditorSetting.Personalized.Theme = value;
    }

    public List<KeyValue> ThemeValueList { get; set; } = GetThemeValueList();

    private static List<KeyValue> GetThemeValueList()
    {
        List<KeyValue> themeValueList = new()
        {
            new KeyValue { Key = "Light", ValueKey = Translations.SettingsThemeLight },
            new KeyValue { Key = "Dark", ValueKey = Translations.SettingsThemeDark },
            new KeyValue { Key = "HighContrast", ValueKey = Translations.SettingsThemeHighContrast },
        };
        foreach (var themeConfig in CustomThemeRegistry.AllThemes)
        {
            themeValueList.Add(
                new KeyValue { Key = themeConfig.Key, ValueKey = themeConfig.DisplayNameKey }
                );
        }
        return themeValueList;
    }

    /// <summary>语言下拉项（显示各语言自称，不经过翻译，避免语言选择依赖翻译；走 KeyValue 直显）。</summary>
    public List<KeyValue> LanguageValueList { get; } = new()
    {
        new KeyValue { Key = "zh-CN", Value = "简体中文" },
        new KeyValue { Key = "en-US", Value = "English" },
        new KeyValue { Key = "ru-RU", Value = "Русский" },
        new KeyValue { Key = "fr-FR", Value = "Français" },
        new KeyValue { Key = "jp-JP", Value = "日本語" },
    };

    [ObservableProperty]
    private string _languageValue;

    partial void OnLanguageValueChanged(string value)
    {
        if (String.IsNullOrEmpty(value))
            return;
        // 立即热切换
        _translationService.Load(value);
        // 持久化
        AppSettingService1.EditorSetting.Personalized.Language = value;
        AppSettingService1.SaveSetting();
    }

    [ObservableProperty]
    private string _appVersion;

}
