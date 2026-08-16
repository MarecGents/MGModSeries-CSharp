using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MGEditor.Controls;

/// <summary>
/// 圆形问号说明图标：悬停显示 ToolTip（10 秒），点击用 Popup 钉住展示描述文本。
/// Description 由外部以 {DynamicResource} 绑定，随语言切换自动刷新。
/// </summary>
public class InfoTip : Control
{
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description),
        typeof(string),
        typeof(InfoTip),
        new PropertyMetadata(null));

    public string? Description
    {
        get => (string?)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public InfoTip()
    {
        // 行被禁用时（如 ComboxFrame 的 IsEnabled 联动）仍允许查看说明
        IsEnabled = true;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Popup 无 PlacementTarget 时相对屏幕定位；此处锚定到问号图标，点击弹层紧贴图标右侧展开
        if (GetTemplateChild("PART_Icon") is UIElement icon &&
            GetTemplateChild("PART_Popup") is Popup popup)
        {
            popup.PlacementTarget = icon;
        }
    }
}
