using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using MGEditor.Helpers;

namespace MGEditor.Services;

/// <summary>
/// i18n 翻译服务：JSON 语言包（EmbeddedResource 嵌入）→ 内存 ResourceDictionary 桥接。
/// XAML 静态文本经 {DynamicResource} 消费，C# 侧经 Get/索引器 消费，热切换即时刷新。
/// </summary>
public class TranslationService : INotifyPropertyChanged
{
    private const string ResourcePrefix = "MGEditor.i18n.";
    private const string DefaultLanguage = "zh-CN";

    /// <summary>当前支持的语言（与 res/i18n/*.json 一一对应）。</summary>
    public static readonly IReadOnlyList<string> AvailableLanguages = new[] { "zh-CN", "en-US", "ru-RU", "fr-FR", "jp-JP" };

    /// <summary>全局实例（由 DI 创建后赋值，供 XAML converter 等无 DI 上下文处使用）。</summary>
    public static TranslationService? Instance { get; set; }

    private readonly JsonReader jsonReader;
    private ResourceDictionary _dict = new ResourceDictionary();

    /// <summary>语言切换时触发（导航、首页卡片等需要重建的集合监听此事件）。</summary>
    public event Action? LanguageChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>当前语言，如 "zh-CN"。</summary>
    public string CurrentLanguage { get; private set; } = DefaultLanguage;

    public TranslationService()
    {
        jsonReader = new JsonReader();
    }

    /// <summary>C# 绑定用索引器：this["Nav.Home"]。</summary>
    public string this[string key] => Get(key);

    /// <summary>加载指定语言包（启动时/设置页切换时调用）；失败自动回退默认语言。</summary>
    public void Load(string lang)
    {
        var normalized = NormalizeLanguage(lang);
        if (normalized is null)
        {
            return;
        }

        try
        {
            var entries = ReadLanguageFile(normalized);
            var dict = new ResourceDictionary();
            foreach (var kv in entries)
            {
                dict[kv.Key] = kv.Value;
            }

            // 热切换：替换 MergedDictionaries 中的语言包 → 所有 {DynamicResource} 自动重新解析
            Application.Current.Resources.MergedDictionaries.Remove(_dict);
            Application.Current.Resources.MergedDictionaries.Insert(0, dict);
            _dict = dict;

            CurrentLanguage = normalized;
            LanguageChanged?.Invoke();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
        }
        catch (Exception ex)
        {
            // 语言包损坏/缺失：回退默认语言
            System.Diagnostics.Debug.WriteLine($"[i18n] 加载语言 {normalized} 失败: {ex.Message}");
            if (normalized != DefaultLanguage)
            {
                Load(DefaultLanguage);
            }
        }
    }

    /// <summary>取翻译文本；缺键返回 {Missing:Key} 占位（界面可见，便于发现遗漏）。</summary>
    public string Get(string key)
    {
        return _dict[key] as string ?? $"{{Missing:{key}}}";
    }

    /// <summary>取翻译文本并格式化占位符：Get("ConfigItem.Container.Cells", 20) → "20格"。</summary>
    public string Get(string key, params object[] args)
    {
        var template = Get(key);
        return args.Length > 0 && template.IndexOf("{0}", StringComparison.Ordinal) >= 0
            ? string.Format(CultureInfo.InvariantCulture, template, args)
            : template;
    }

    /// <summary>按「设置页覆盖 → 跟随系统 → 默认语言」解析并加载。</summary>
    public void ResolveAndLoad(string? savedLanguage)
    {
        Load(savedLanguage ?? ResolveFromCurrentUICulture() ?? DefaultLanguage);
    }

    /// <summary>键一致性校验：以默认语言为基线，返回各语言缺失(-)/多余(+)键，供启动诊断与测试。</summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> ValidateKeyConsistency()
    {
        var result = new Dictionary<string, IReadOnlyList<string>>();
        var baseline = ExtractKeys(DefaultLanguage);
        foreach (var lang in AvailableLanguages)
        {
            if (lang == DefaultLanguage)
            {
                continue;
            }

            var keys = ExtractKeys(lang);
            var missing = baseline.Except(keys).Select(k => $"-{k}").ToList();
            var extra = keys.Except(baseline).Select(k => $"+{k}").ToList();
            if (missing.Count > 0 || extra.Count > 0)
            {
                result[lang] = missing.Concat(extra).ToList();
            }
        }

        return result;
    }

    private string? NormalizeLanguage(string lang)
    {
        return AvailableLanguages.FirstOrDefault(l => l.Equals(lang, StringComparison.OrdinalIgnoreCase));
    }

    private string? ResolveFromCurrentUICulture()
    {
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant() switch
        {
            "zh" => "zh-CN",
            "en" => "en-US",
            _ => "en-US",
        };
    }

    /// <summary>读取嵌入 JSON 并拍平为 键→值（"Nav.Home" -> "主页"）。</summary>
    private Dictionary<string, string> ReadLanguageFile(string lang)
    {
        var asm = Assembly.GetExecutingAssembly();
        var resName = $"{ResourcePrefix}{lang}.json";
        using var stream = asm.GetManifestResourceStream(resName)
            ?? throw new FileNotFoundException($"未找到内嵌语言资源: {resName}");
        using var reader = new StreamReader(stream);
        var sections = jsonReader.Deserialize<Dictionary<string, Dictionary<string, string>>>(reader.ReadToEnd())
            ?? throw new InvalidDataException($"语言资源解析为空: {resName}");

        var flat = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var section in sections)
        {
            if (section.Key == "meta")
            {
                continue;
            }

            foreach (var entry in section.Value)
            {
                flat[$"{section.Key}.{entry.Key}"] = entry.Value;
            }
        }

        return flat;
    }

    /// <summary>获取指定语言的全部键→值（供 OverflowChecker 等开发期工具使用）。</summary>
    public IReadOnlyDictionary<string, string> GetAllEntries(string lang)
    {
        var normalized = NormalizeLanguage(lang);
        return normalized is null
            ? new Dictionary<string, string>()
            : ReadLanguageFile(normalized);
    }

    private static HashSet<string> ExtractKeys(string lang)
    {
        var asm = Assembly.GetExecutingAssembly();
        using var stream = asm.GetManifestResourceStream($"{ResourcePrefix}{lang}.json");
        using var reader = new StreamReader(stream!);
        var sections = System.Text.Json.JsonSerializer
            .Deserialize<Dictionary<string, Dictionary<string, string>>>(reader.ReadToEnd())!;

        var keys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var section in sections)
        {
            if (section.Key == "meta")
            {
                continue;
            }

            foreach (var entry in section.Value)
            {
                keys.Add($"{section.Key}.{entry.Key}");
            }
        }

        return keys;
    }
}
