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

        ConfigJson = TryDeserialize(configContent);

        if (ConfigJson == null)
        {
            // config.json 缺失或损坏（如上次保存中断导致半写文件）→ 先备份损坏文件再回退 defaultConfig.json，
            // 避免后续保存静默覆盖用户文件（醒目提示由服务端启动日志承担，编辑器保持轻量）。
            if (configContent != null && fileUtils.FileExists(configPath))
            {
                var backupPath = configPath + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                try { fileUtils.CopyFile(configPath, backupPath, overwrite: true); }
                catch { /* 备份失败不阻断回退 */ }
                Console.WriteLine($"[MGConfigService] 警告：config.json 损坏，已备份到 {backupPath} 并回退 defaultConfig.json。configPath={configPath}");
            }
            else
            {
                Console.WriteLine($"[MGConfigService] 警告：config.json 缺失，已回退 defaultConfig.json。configPath={configPath}");
            }

            var defaultPath = Path.Combine(appPath, "./res/config/defaultConfig.json");
            ConfigJson = TryDeserialize(fileUtils.TryReadFile(defaultPath)) ?? new MGConfig();
        }

        ConfigItemsList = new ConfigItems();
    }

    private MGConfig? TryDeserialize(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try { return jsonReader.Deserialize<MGConfig>(json); }
        catch { return null; }
    }

    public void SaveConfig()
    {
        string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        ConfigJson.saveTime = nowTime;
        var configPath = Path.Combine(appPath, "./res/config/config.json");
        var configContent = jsonReader.Serialize<MGConfig>(ConfigJson);
        if (configContent == null || !fileUtils.WriteFile(configPath, configContent))
        {
            Console.WriteLine($"[MGConfigService] 错误：config.json 保存失败。configPath={configPath}");
        }
    }
}
