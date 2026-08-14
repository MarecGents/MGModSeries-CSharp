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
        var configPath = Path.Combine(appPath, "./res/config/config.json");
        var configContent = fileUtils.TryReadFile(configPath);
        ConfigJson = configContent == null ? null : jsonReader.Deserialize<MGConfig>(configContent);
        if (ConfigJson == null)
        {
            // config.json 缺失或损坏（如上次保存中断导致半写文件）→ 回退 defaultConfig.json，
            // 避免静默空配置覆盖用户文件；并在界面给出提示（由调用方展示）。
            var defaultPath = Path.Combine(appPath, "./res/config/defaultConfig.json");
            ConfigJson = jsonReader.Deserialize<MGConfig>(fileUtils.TryReadFile(defaultPath)) ?? new MGConfig();
            Console.WriteLine($"[MGConfigService] 警告：config.json 缺失或损坏，已回退 defaultConfig.json。configPath={configPath}");
        }

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
