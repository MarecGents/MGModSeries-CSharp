using MGEditor.Helpers;
using MGEditor.Models;
using MGEditor.Resources;

namespace MGEditor.Services;

public class MGConfigService
{
    public MGConfig ConfigJson { get; private set; }
    public ConfigItems ConfigItemsList { get; private set; }

    private FileUtils fileUtils;
    private JsonReader jsonReader;
    private string appPath;

    public MGConfigService()
    {
        fileUtils = new FileUtils();
        jsonReader = new JsonReader();
        appPath = AppContext.BaseDirectory;
    }
    public void LoadConfig()
    {
        var configContent = fileUtils.ReadFile(Path.Combine(appPath, "./res/config/config.json"));
        ConfigJson = jsonReader.Deserialize<MGConfig>(configContent) ?? new MGConfig();

        ConfigItemsList = new ConfigItems();
    }
    public void SaveConfig()
    {
        string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        ConfigJson.saveTime = nowTime;
        var configContent = jsonReader.Serialize<MGConfig>(ConfigJson);
        fileUtils.WriteFile(Path.Combine(appPath, "./res/config/config.json"), configContent);
    }
}
