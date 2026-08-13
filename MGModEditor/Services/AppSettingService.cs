using MGEditor.Helpers;
using MGEditor.Models;

namespace MGEditor.Services;

public class AppSettingService
{
    public AppSetting EditorSetting { get; private set; }
    
    private FileUtils fileUtils;
    private JsonReader jsonReader;
    private string appPath;
    
    public AppSettingService()
    {
        fileUtils = new FileUtils();
        jsonReader = new JsonReader();
        appPath = AppContext.BaseDirectory;
    }

    public void LoadSetting()
    {
        var settingContent = fileUtils.ReadFile(Path.Combine(appPath, "./res/config/EditorSettings.json"));
        EditorSetting = jsonReader.Deserialize<AppSetting>(settingContent) ?? new AppSetting();
    }

    public void ApplySettings()
    {
        // Personalized
        Personalized(EditorSetting.Personalized);
    }

    public void SaveSetting()
    {
        var settingContent = jsonReader.Serialize<AppSetting>(EditorSetting);
        fileUtils.WriteFile(Path.Combine(appPath, "./res/config/EditorSettings.json"), settingContent);
    }

    public void Personalized(AppPersonalized  appPersonalized)
    {
        // Theme
        if (appPersonalized.Theme == "Light")
        {
            CustomThemeService.Remove();
            ApplicationThemeManager.Apply(ApplicationTheme.Light, WindowBackdropType.None, false);
        }
        else if (appPersonalized.Theme == "Dark")
        {
            CustomThemeService.Remove();
            ApplicationThemeManager.Apply(ApplicationTheme.Dark, WindowBackdropType.None, false);
        }
        else if (appPersonalized.Theme == "HighContrast")
        {
            CustomThemeService.Remove();
            ApplicationThemeManager.Apply(ApplicationTheme.HighContrast, WindowBackdropType.None, false);
        }
        else
        {
            CustomThemeService.Apply(appPersonalized.Theme);
        }
        //
    }
}