using System.Collections.Generic;
using MGModClient;

namespace MGModClient.Resources;

/// <summary>
/// i18n 字典（Radar 模式，落地方案 §5.1）——键复用桌面版 Translations.cs 点分体系。
/// EN/ZH 起步；缺键回退 EN，再缺显示 {Missing:key}。
/// 当前为空骨架：翻译键由后续编辑器功能按桌面版 Translations.cs 填充。
/// </summary>
internal static class Locales
{
    private const string FallbackLanguage = "EN";

    private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
    {
        ["EN"] = new()
        {
            // 示例键（后续按桌面版 Translations.cs 全量搬入）
            ["MGEditor.Language"] = "Language",
            ["MGEditor.ServerConfigPath"] = "Server Config Path",
        },
        ["ZH"] = new()
        {
            ["MGEditor.Language"] = "语言",
            ["MGEditor.ServerConfigPath"] = "服务端配置路径",
        },
    };

    public static string Get(string key)
    {
        var lang = MGModClientPlugin.Language?.Value ?? FallbackLanguage;
        if (Translations.TryGetValue(lang, out var l) && l.TryGetValue(key, out var t))
            return t;
        if (Translations[FallbackLanguage].TryGetValue(key, out var fb))
            return fb;
        return $"{{Missing:{key}}}";
    }
}
