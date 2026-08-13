using MGEditor.Models;
using MGEditor.Resources;
using MGEditor.Services;

namespace MGEditor.Controls;

public class ContainerFrameGroup: Control
{

    /// <summary>容器 item id → 翻译键（保留 ContainerId 逻辑，HeaderText 走 i18n）。</summary>
    private static readonly Dictionary<string, string> ContainerItemKeyById = new()
    {
        { "67600929bd0a0549d70993f6", Translations.ContainerItemBallisticPlateCase },
        { "62a09d3bcf4a99369e262447", Translations.ContainerItemGingyKeychain },
        { "5c093db286f7740a1b2617e3", Translations.ContainerItemHolodilnickThermalBag },
        { "5b6d9ce188a4501afc1b2b25", Translations.ContainerItemTHICCWeaponCase },
        { "5c0a840b86f7742ffa4f2482", Translations.ContainerItemTHICCItemCase },
        { "60b0f6c058e0b0481a09ad11", Translations.ContainerItemWZWallet },
        { "66bc98a01a47be227a5e956e", Translations.ContainerItemStreamerItemCase },
        { "5aafbcd986f7745e590fff23", Translations.ContainerItemMedicineCase },
        { "5d235bb686f77443f4331278", Translations.ContainerItemSICCPouch },
        { "5b7c710788a4506dec015957", Translations.ContainerItemLuckyScavJunkBox },
        { "5c127c4486f7745625356c13", Translations.ContainerItemMagazineCase },
        { "5aafbde786f774389d0cbc0f", Translations.ContainerItemAmmunitionCase },
        { "567143bf4bdc2d1a0f8b4567", Translations.ContainerItemPistolCase },
        { "5e2af55f86f7746d4159f07c", Translations.ContainerItemGrenadeCase },
        { "590c60fc86f77412b13fddcf", Translations.ContainerItemDocumentsCase },
        { "59fb023c86f7746d0d4b423c", Translations.ContainerItemWeaponCase },
        { "619cbf7d23893217ec30b689", Translations.ContainerItemInjectorCase },
        { "59fb042886f7746c5005a7b2", Translations.ContainerItemItemCase },
        { "5c093e3486f77430cb02e593", Translations.ContainerItemDogtagCase },
        { "5783c43d2459774bbe137486", Translations.ContainerItemSimpleWallet },
        { "619cbf9e0a7c3a1a2731940a", Translations.ContainerItemKeycardHolderCase },
        { "59fafd4b86f7745ca07e1232", Translations.ContainerItemKeyTool },
        { "59fb016586f7746d0d4b423a", Translations.ContainerItemMoneyCase },
    };

    /// <summary>Identifies the <see cref="HeaderText"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTextProperty = 
        DependencyProperty.Register(
            nameof(HeaderText),
            typeof(string),
            typeof(ContainerFrameGroup),
            new PropertyMetadata("Default HeaderText")
            );
    public static readonly DependencyProperty ContainerIdProperty =
        DependencyProperty.Register(
            nameof(ContainerId),
            typeof(string),
            typeof(ContainerFrameGroup),
            new PropertyMetadata("Default ContainerId", OnContainerIdOrExpandChanged)
            );
    public static readonly DependencyProperty ContainerExpandProperty =
        DependencyProperty.Register(
            nameof(ContainerExpand),
            typeof(Dictionary<string, MGModConfig_Templates_ContainerExpands>),
            typeof(ContainerFrameGroup),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnContainerIdOrExpandChanged)
            );
    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(
            nameof(IsExpanded),
            typeof(bool?),
            typeof(ContainerFrameGroup),
            new PropertyMetadata(null, OnIsExpandedChanged)
            );
    public static readonly DependencyProperty ExpandWidthProperty =
        DependencyProperty.Register(
            nameof(ExpandWidth),
            typeof(int?),
            typeof(ContainerFrameGroup),
            new PropertyMetadata(null, OnExpandWidthChanged)
            );
    public static readonly DependencyProperty ExpandHeightProperty =
        DependencyProperty.Register(
            nameof(ExpandHeight),
            typeof(int?),
            typeof(ContainerFrameGroup),
            new PropertyMetadata(null, OnExpandHeightChanged)
            );
    public static readonly DependencyProperty IsFilterProperty =
        DependencyProperty.Register(
            nameof(IsFilter),
            typeof(bool?),
            typeof(ContainerFrameGroup),
            new PropertyMetadata(null, OnIsFilterChanged)
            );
    public static readonly DependencyProperty IsNoWeightProperty =
        DependencyProperty.Register(
            nameof(IsNoWeight),
            typeof(bool?),
            typeof(ContainerFrameGroup),
            new PropertyMetadata(null, OnIsNoWeightChanged)
            );
    public string? HeaderText
    {
        get => (string?)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }
    public string? ContainerId
    {
        get => (string?)GetValue(ContainerIdProperty);
        set => SetValue(ContainerIdProperty, value);
    }
    public Dictionary<string, MGModConfig_Templates_ContainerExpands>? ContainerExpand
    {
        get => (Dictionary<string, MGModConfig_Templates_ContainerExpands>?)GetValue(ContainerExpandProperty);
        set => SetValue(ContainerExpandProperty, value);
    }
    public bool? IsExpanded
    {
        get => (bool?)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
    public int? ExpandWidth
    {
        get => (int?)GetValue(ExpandWidthProperty);
        set => SetValue(ExpandWidthProperty, value);
    }
    public int? ExpandHeight
    {
        get => (int?)GetValue(ExpandHeightProperty);
        set => SetValue(ExpandHeightProperty, value);
    }
    public bool? IsFilter
    {
        get => (bool?)GetValue(IsFilterProperty);
        set => SetValue(IsFilterProperty, value);
    }
    public bool? IsNoWeight
    {
        get => (bool?)GetValue(IsNoWeightProperty);
        set => SetValue(IsNoWeightProperty, value);
    }

    public List<KeyValue> ExpandWidthList { get; } = new ConfigItems().ContainerExpandWidth;
    public List<KeyValue> ExpandHeightList { get; } = new ConfigItems().ContainerExpandHeight;
    public ContainerFrameGroup()
    {
        TranslationService.Instance!.LanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        // 语言切换时刷新 HeaderText
        RefreshHeaderText();
    }

    private void RefreshHeaderText()
    {
        var translation = TranslationService.Instance;
        if (translation is null || string.IsNullOrEmpty(ContainerId) || ContainerExpand is null)
        {
            return;
        }

        if (ContainerItemKeyById.TryGetValue(ContainerId, out var key))
        {
            HeaderText = translation[key];
        }
        else if (ContainerExpand.TryGetValue(ContainerId, out var expand))
        {
            HeaderText = expand.Name;   // 未收录容器 fallback 原文
        }
    }

    private static void OnContainerIdOrExpandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ContainerFrameGroup)d;

        // 两个关键依赖属性都已赋值时再初始化
        if (!string.IsNullOrEmpty(control.ContainerId) && control.ContainerExpand != null &&
            control.ContainerExpand.TryGetValue(control.ContainerId, out var expand))
        {
            control.RefreshHeaderText();
            control.IsExpanded = expand.enable;
            control.ExpandWidth = expand.cellsH;
            control.ExpandHeight = expand.cellsV;
            control.IsFilter = expand.Filter;
            control.IsNoWeight = expand.Weight;
        }
    }
    private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ContainerFrameGroup)d;
        control.ContainerExpand[control.ContainerId].enable = (bool)e.NewValue;
    }
    private static void OnExpandWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ContainerFrameGroup)d;
        control.ContainerExpand[control.ContainerId].cellsH = (int)e.NewValue;
    }
    private static void OnExpandHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ContainerFrameGroup)d;
        control.ContainerExpand[control.ContainerId].cellsV = (int)e.NewValue;
    }
    private static void OnIsFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ContainerFrameGroup)d;
        control.ContainerExpand[control.ContainerId].Filter = (bool)e.NewValue;
    }
    private static void OnIsNoWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ContainerFrameGroup)d;
        control.ContainerExpand[control.ContainerId].Weight = (bool)e.NewValue;
    }

}
