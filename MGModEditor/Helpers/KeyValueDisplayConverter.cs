using System.Globalization;
using System.Windows.Data;
using MGEditor.Resources;
using MGEditor.Services;

namespace MGEditor.Helpers;

/// <summary>
/// KeyValue 显示文本解析：ValueKey → 翻译文本；ValueFormatKey → 翻译文本 + Key 格式化；否则直显 Value。
/// 与 TranslationService.CurrentLanguage 多重绑定，语言切换时自动重新解析。
/// </summary>
internal sealed class KeyValueDisplayConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length == 0 || values[0] is null)
        {
            return string.Empty;
        }

        if (values[0] is not KeyValue item)
        {
            // 非 KeyValue 项兜底直显（防御未来误用导致空白）
            return values[0].ToString() ?? string.Empty;
        }

        var translation = TranslationService.Instance;
        if (!string.IsNullOrEmpty(item.ValueKey))
        {
            return translation?[item.ValueKey] ?? item.Value ?? string.Empty;
        }

        if (!string.IsNullOrEmpty(item.ValueFormatKey))
        {
            return translation?.Get(item.ValueFormatKey, item.Key) ?? item.Value ?? string.Empty;
        }

        return item.Value ?? string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
