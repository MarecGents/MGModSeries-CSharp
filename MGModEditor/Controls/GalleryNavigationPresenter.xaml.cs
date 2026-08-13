using System.Windows.Controls;
using System.Windows.Threading;
using MGEditor.Helpers;
using MGEditor.Models;
using MGEditor.Services;

namespace MGEditor.Controls;

public class GalleryNavigationPresenter : Control
{
    /// <summary>Identifies the <see cref="ItemsSource"/> dependency property.</summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(object),
        typeof(GalleryNavigationPresenter),
        new PropertyMetadata(null, OnLayoutChanged)
    );

    /// <summary>Identifies the <see cref="TemplateButtonCommand"/> dependency property.</summary>
    public static readonly DependencyProperty TemplateButtonCommandProperty = DependencyProperty.Register(
        nameof(TemplateButtonCommand),
        typeof(Wpf.Ui.Input.IRelayCommand),
        typeof(GalleryNavigationPresenter),
        new PropertyMetadata(null)
    );

    /// <summary>Identifies the <see cref="CardWidth"/> dependency property（固定卡片宽，DataTemplate 经 FindAncestor 绑定；WrapPanel 按内容区自动排 2/3/4 列）。</summary>
    public static readonly DependencyProperty CardWidthProperty = DependencyProperty.Register(
        nameof(CardWidth),
        typeof(double),
        typeof(GalleryNavigationPresenter),
        new PropertyMetadata(320.0)
    );

    public object? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets the command triggered after clicking the titlebar button.
    /// </summary>
    public Wpf.Ui.Input.IRelayCommand TemplateButtonCommand =>
        (Wpf.Ui.Input.IRelayCommand)GetValue(TemplateButtonCommandProperty);

    public double CardWidth
    {
        get => (double)GetValue(CardWidthProperty);
        set => SetValue(CardWidthProperty, value);
    }

    private bool _isUpdating;
    private DispatcherOperation? _pendingUpdate;

    /// <summary>
    /// Initializes a new instance of the <see cref="GalleryNavigationPresenter"/> class.
    /// </summary>
    public GalleryNavigationPresenter()
    {
        SetValue(TemplateButtonCommandProperty, new Wpf.Ui.Input.RelayCommand<Type>(o => OnTemplateButtonClick(o)));
        SizeChanged += (_, _) => ScheduleUpdate();
        TranslationService.Instance!.LanguageChanged += ScheduleUpdate;
    }

    private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((GalleryNavigationPresenter)d).ScheduleUpdate();

    /// <summary>防重入 + Dispatcher 节流：SizeChanged/ItemsSource 变化/语言切换均触发重算。</summary>
    private void ScheduleUpdate()
    {
        if (_isUpdating || _pendingUpdate is not null)
        {
            return;
        }

        _pendingUpdate = Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(UpdateCardSizes));
    }

    private void UpdateCardSizes()
    {
        try
        {
            _isUpdating = true;
            if (ItemsSource is not IEnumerable<NavigationCard> cards)
            {
                return;
            }

            // 固定宽：320（不再按内容区比例——WrapPanel 自动按内容区宽度排 2/3/4 列，拉宽不闪烁）
            double cardWidth = CardWidth;

            // 基准高：90 保底
            const double baseHeight = 90;

            foreach (var card in cards)
            {
                // per-card 高：测量 Description 换行高度，超出基准则增高（需求 1）
                var ft = TextMeasurer.Prepare(
                    card.Description ?? string.Empty,
                    12,
                    System.Windows.FontWeights.Normal,
                    System.Windows.FontStyles.Normal,
                    TextMeasurer.GetPixelsPerDip());
                double descHeight = TextMeasurer.MeasureHeight(ft, cardWidth - 60); // 预留图标/内边距
                card.CardHeight = Math.Max(baseHeight, 22 + descHeight + 24);
            }
        }
        finally
        {
            _isUpdating = false;
            _pendingUpdate = null;   // 复位：允许下一次调度（否则后续重算被吞）
        }
    }

    private void OnTemplateButtonClick(Type? pageType)
    {
        INavigationService navigationService = App.GetRequiredService<INavigationService>();

        if (pageType is not null)
        {
            // 页面通过 ui:NavigationView.HeaderContent 附加属性提供翻译标题，
            // 导航时 WPF-UI 自动将其设为导航项 Content → BreadcrumbBar（大 Title）随语言切换
            _ = navigationService.Navigate(pageType);
        }

        System.Diagnostics.Debug.WriteLine(
            $"INFO | {nameof(GalleryNavigationPresenter)} navigated, ({pageType})",
            "Wpf.Ui.Gallery"
        );
    }
}
