using System.Windows;
using MGEditor.Services;

namespace MGEditor.Helpers;

/// <summary>
/// 开发期溢出诊断：遍历语言包全部键，按目标控件宽度测量 en/ru 译文，
/// 报告需换行超过阈值或超出高度的项（借鉴 pretext 的"开发期验证标签不溢出"思路）。
/// 走无窗口测量路径（GetPixelsPerDip 无窗口返回 1.0），可在 CI/命令行运行。
/// </summary>
public static class OverflowChecker
{
    /// <summary>
    /// 遍历全部语言包，报告在指定宽度下超行/超高的文本项。
    /// </summary>
    /// <param name="translation">TranslationService（供 GetAllEntries 读取语言包）。</param>
    /// <param name="buttonWidth">目标按钮宽度（近似筛查，真实宽度随窗口/列动态）。</param>
    /// <param name="maxButtonLines">允许的最大按钮行数。</param>
    /// <param name="lineHeight">单行高度（近似，按字体/字号取）。</param>
    /// <param name="fontSize">测量字号。</param>
    public static IReadOnlyList<string> CheckAll(
        TranslationService translation,
        double buttonWidth = 180,
        int maxButtonLines = 2,
        double lineHeight = 20,
        double fontSize = 14)
    {
        var issues = new List<string>();
        var dpi = TextMeasurer.GetPixelsPerDip();

        foreach (var lang in TranslationService.AvailableLanguages)
        {
            var entries = translation.GetAllEntries(lang);
            foreach (var (key, value) in entries)
            {
                var ft = TextMeasurer.Prepare(value, fontSize, FontWeights.Normal, FontStyles.Normal, dpi);
                if (TextMeasurer.Overflows(ft, buttonWidth, maxButtonLines, lineHeight))
                {
                    issues.Add($"[{lang}] {key} = \"{value}\" 在 {buttonWidth}px 下超 {maxButtonLines} 行");
                }
            }
        }

        return issues;
    }
}
