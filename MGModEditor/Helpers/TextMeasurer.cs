using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace MGEditor.Helpers;

/// <summary>
/// 文本测量服务（借鉴 @chenglou/pretext 的 prepare+layout 两级设计）。
/// 注意：不缓存 FormattedText 实例（其 MaxTextWidth 为可变状态，共享会互相污染）；
/// new FormattedText 开销为微秒级，每次测量新建最干净。
/// </summary>
public static class TextMeasurer
{
    /// <summary>获取当前窗口 DPI（无窗口/CI 时返回 1.0 = 96 DPI）。</summary>
    public static double GetPixelsPerDip()
        => Application.Current?.MainWindow is { } w
            ? VisualTreeHelper.GetDpi(w).PixelsPerDip
            : 1.0;

    /// <summary>prepare：构造 FormattedText（不设 MaxTextWidth = 宽度无关）。</summary>
    public static FormattedText Prepare(
        string text,
        double fontSize,
        FontWeight weight,
        FontStyle style,
        double pixelsPerDip,
        CultureInfo? culture = null)
    {
        return new FormattedText(
            text,
            culture ?? CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            new Typeface(new FontFamily("Segoe UI"), style, weight, FontStretches.Normal),
            fontSize,
            Brushes.Black,   // 仅测量用，颜色无关
            pixelsPerDip);   // .NET 9 必须传
    }

    /// <summary>layout 热路径：按给定宽度重排，返回换行后高度。</summary>
    public static double MeasureHeight(FormattedText prepared, double maxWidth)
    {
        prepared.MaxTextWidth = maxWidth;
        return prepared.Height;
    }

    /// <summary>返回给定宽度下的行数（按高度估算：行数 = Height / 单行高）。</summary>
    public static int MeasureLineCount(FormattedText prepared, double maxWidth, double lineHeight)
    {
        prepared.MaxTextWidth = maxWidth;
        return Math.Max(1, (int)Math.Ceiling(prepared.Height / lineHeight));
    }

    /// <summary>开发期溢出诊断：文本在指定宽度下是否会超过最大行数/高度（只改一次宽度）。</summary>
    public static bool Overflows(FormattedText prepared, double maxWidth, int maxLines, double lineHeight)
    {
        prepared.MaxTextWidth = maxWidth;
        return prepared.Height > maxLines * lineHeight;
    }
}
