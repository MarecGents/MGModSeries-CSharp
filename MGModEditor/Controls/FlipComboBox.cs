using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MGEditor.Controls;

/// <summary>
/// 下拉弹层位置修复用 ComboBox（替换原生 ComboBox 使用）。
///
/// 背景：Wpf.Ui 4.3.0 的 ComboBox 模板中，弹层内容包了一层带阴影占位边距的
/// EffectThicknessDecorator（Thickness=30,0,30,30）。当控件下方空间不足时，
/// WPF 默认的 Bottom 放置算法不会翻转到上方，而是将弹层"上推"——表现为可见下拉
/// 边框与控件重叠、底部越出窗口/屏幕，即页面底部 ComboxFrame 的下拉错位问题。
///
/// 修复：改用 CustomPopupPlacementCallback 提供两个候选位置（优先下方、放不下则上方），
/// 由 WPF 按"完整可见优先"自动选择；并用 -Margin 偏移补偿阴影占位，保证可见边框与控件对齐。
/// 下方空间充足时行为与默认一致（贴底、水平对齐）；空间不足时翻转到上方或贴近屏幕边缘。
///
/// 注意：WPF 的隐式样式不会自动应用到派生类型（{x:Type ComboBox} 只匹配 ComboBox 本体），
/// 因此构造函数中显式引用基类隐式样式，确保继承 Wpf.Ui 的 ComboBox 模板（否则会退回系统默认模板）。
/// </summary>
public class FlipComboBox : ComboBox
{
    private const string PopupTemplatePartName = "Popup";

    public FlipComboBox()
    {
        // 继承 Wpf.Ui 的隐式 ComboBox 样式（资源未就绪时静默跳过，模板应用时样式已可达）
        SetResourceReference(StyleProperty, typeof(ComboBox));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild(PopupTemplatePartName) is not Popup popup)
        {
            return;
        }

        popup.Placement = PlacementMode.Custom;
        popup.CustomPopupPlacementCallback = (popupSize, targetSize, _) =>
        {
            var decorator = popup.Child as FrameworkElement;
            var margin = decorator?.Margin ?? default;
            var popupHeight = decorator is { ActualHeight: > 0 } ? decorator.ActualHeight : popupSize.Height;

            return new[]
            {
                // 候选1（优先）：下方——可见边框顶边对齐控件底边（-Margin 补偿阴影占位）
                new CustomPopupPlacement(new Point(-margin.Left, targetSize.Height - margin.Top), PopupPrimaryAxis.Horizontal),
                // 候选2（兜底）：上方——可见边框底边对齐控件顶边
                new CustomPopupPlacement(new Point(-margin.Left, -margin.Top - popupHeight), PopupPrimaryAxis.Horizontal),
            };
        };
    }
}
