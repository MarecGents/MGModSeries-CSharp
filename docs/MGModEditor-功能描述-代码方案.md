# MGModEditor 功能描述提示 —— 代码方案报告

> 日期：2026-08-14　|　适用版本：MGModEditor v1.3.1.1　|　方案：**方案 B（ToolTip 悬停 + 原生 Popup 点击）**

---

## 1. 总体设计

分层四步，全部为**增量改动**，不触碰任何现有绑定与配置读写逻辑：

```
┌─ 控件层：新增 InfoTip 组合控件（圆形问号图标 + ToolTip + Popup）────────┐
├─ 模型层：CheckButtonFrame / ComboxFrame 各新增 Description 依赖属性 ────┤
├─ 模板层：两控件模板左列叠放 InfoTip（紧挨按钮右侧）─────────────────────┤
└─ 接入层：5 个功能页 224 处 XAML 加 Description="{DynamicResource ...}" ──┘
```

交互约定：
- **悬停** → 原生 ToolTip，`ShowDuration=10000`（与 Home 页 `GalleryNavigationPresenter.xaml:30` 完全一致）；
- **点击** → Popup 钉住展示（`StaysOpen=False`，点外部或再次点击自动关闭）；
- 语言切换 → `{DynamicResource}` 自动刷新 Description 依赖属性，ToolTip 每次打开取新值、Popup 内容实时更新，**无需监听 `LanguageChanged`**（与 Home 卡片/导航菜单不同，它们因 C# 集合绑定才需要重建）。

---

## 2. 新增控件 `Controls/InfoTip`

### 2.1 代码类 `Controls/InfoTip.xaml.cs`

```csharp
namespace MGEditor.Controls;

/// <summary>
/// 圆形问号说明图标：悬停显示 ToolTip（10 秒），点击 Popup 钉住展示描述文本。
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
}
```

### 2.2 模板 `Controls/InfoTip.xaml`

```xaml
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
    xmlns:controls="clr-namespace:MGEditor.Controls">

    <Style TargetType="{x:Type controls:InfoTip}">
        <Setter Property="Focusable" Value="False" />
        <Setter Property="SnapsToDevicePixels" Value="True" />
        <Setter Property="OverridesDefaultStyle" Value="True" />
        <Setter Property="Cursor" Value="Hand" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type controls:InfoTip}">
                    <Grid>
                        <!-- 圆形问号图标：ToggleButton 语义，点击钉住 Popup -->
                        <ToggleButton x:Name="PART_Icon"
                                      Width="18" Height="18"
                                      HorizontalAlignment="Center"
                                      VerticalAlignment="Center"
                                      ToolTipService.ShowDuration="10000"
                                      ToolTipService.ShowOnDisabled="True"
                                      ToolTip="{TemplateBinding Description}"
                                      ToolTipService.InitialShowDelay="300">
                            <ToggleButton.Template>
                                <ControlTemplate TargetType="ToggleButton">
                                    <Grid Width="18" Height="18">
                                        <Ellipse Fill="{ui:ThemeResource ControlFillColorSecondaryBrush}"
                                                 Stroke="{ui:ThemeResource ControlStrokeColorDefaultBrush}"
                                                 StrokeThickness="1" />
                                        <ui:SymbolIcon
                                            Symbol="QuestionCircle16"
                                            FontSize="12"
                                            TextElement.Foreground="{ui:ThemeResource TextFillColorPrimaryBrush}" />
                                    </Grid>
                                </ControlTemplate>
                            </ToggleButton.Template>
                        </ToggleButton>

                        <!-- 点击弹出的小窗口：主题卡片样式 -->
                        <Popup x:Name="PART_Popup"
                               AllowsTransparency="True"
                               PopupAnimation="Fade"
                               StaysOpen="False"
                               Placement="Right"
                               VerticalOffset="-4"
                               IsOpen="{Binding IsChecked, ElementName=PART_Icon, Mode=TwoWay}">
                            <Border
                                MaxWidth="320"
                                Padding="12,10"
                                Background="{ui:ThemeResource CardBackgroundFillColorDefaultBrush}"
                                BorderBrush="{ui:ThemeResource CardStrokeColorDefaultBrush}"
                                BorderThickness="1"
                                CornerRadius="8">
                                <ui:TextBlock
                                    FontSize="12"
                                    Foreground="{ui:ThemeResource TextFillColorPrimaryBrush}"
                                    Text="{TemplateBinding Description}"
                                    TextWrapping="Wrap" />
                            </Border>
                        </Popup>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>
```

关键点说明：

| 要点 | 说明 |
|---|---|
| `QuestionCircle16` | Fluent 字形"圆圈内问号"，WPF-UI 4.3.0 `SymbolRegular` 已有（已反射核实），无需美术资产 |
| `ToggleButton` + Popup `IsOpen` | 点击切换钉住/收起（`Popup.IsOpen` 用 `ElementName=PART_Icon` 绑定到 `IsChecked`）；`StaysOpen=False` 保证点击任意外部区域自动关闭 |
| `ToolTipService.ShowOnDisabled="True"` + 构造函数 `IsEnabled=true` | 被联动禁用的行（如 `Raid.Button.AiAmount`）也能悬停查看说明 |
| `Placement="Right"` | Popup 从图标右侧展开；行高 36px，垂直偏移 -4 居中 |
| `MaxWidth="320"` + `TextWrapping="Wrap"` | 长描述自动换行，不会撑爆窗口 |
| `{TemplateBinding Description}` | ToolTip/Popup 内容取 InfoTip 的 Description 依赖属性值 |

**可选增强（首版可不做）**：`PlacementRectangle` 边界自适应，防止贴屏幕右侧时溢出；如需，在模板里加一个自定义 `Placement` 逻辑或用 `Popup.CustomPopupPlacementCallback`。

---

## 3. 控件模型层：新增 `Description` 依赖属性

### 3.1 `Controls/CheckButtonFrame.xaml.cs`（在 `ButtonContentProperty` 后追加）

```csharp
public static readonly DependencyProperty DescriptionProperty =
    DependencyProperty.Register(
        nameof(Description),
        typeof(string),
        typeof(CheckButtonFrame),
        new PropertyMetadata(null)
    );

public string? Description
{
    get => (string?)GetValue(DescriptionProperty);
    set => SetValue(DescriptionProperty, value);
}
```

### 3.2 `Controls/ComboxFrame.xaml.cs`（同样追加，`typeof(ComboxFrame)`）

```csharp
public static readonly DependencyProperty DescriptionProperty =
    DependencyProperty.Register(
        nameof(Description),
        typeof(string),
        typeof(ComboxFrame),
        new PropertyMetadata(null)
    );

public string? Description
{
    get => (string?)GetValue(DescriptionProperty);
    set => SetValue(DescriptionProperty, value);
}
```

> 两个控件各约 10 行，无副作用。`Description` 仅消费，不影响 `IsChecked`/`SelectedValue` 双向绑定。

---

## 4. 模板层：左列叠放 InfoTip

### 4.1 `Controls/CheckButtonFrame.xaml`

在 `<Grid.ColumnDefinitions>` 下、`ui:Button`（`Grid.Column="0"`）之后追加：

```xaml
<controls:InfoTip
    Grid.Column="0"
    HorizontalAlignment="Right"
    VerticalAlignment="Center"
    Margin="0,0,10,0"
    Description="{TemplateBinding Description}" />
```

> 布局原理：按钮 `HorizontalAlignment="Stretch"` 撑满第 0 列，InfoTip 作为**同列后声明元素**（z 序在上）右对齐，视觉上正好"紧挨按钮右侧边缘"；不挤占列宽，不改按钮行为。点击 InfoTip 落在图标上，不触发按钮的 Toggle 命令（兄弟元素，事件不冒泡到按钮）。

### 4.2 `Controls/ComboxFrame.xaml`（同样追加）

```xaml
<controls:InfoTip
    Grid.Column="0"
    HorizontalAlignment="Right"
    VerticalAlignment="Center"
    Margin="0,0,10,0"
    Description="{TemplateBinding Description}" />
```

### 4.3 `Controls/ContainerFrameGroup.xaml`（内嵌 5 行同样处理）

其中 `CheckButtonFrame`/`ComboxFrame` 各加一行 `Description="{DynamicResource Container.Desc.Enable}"` 等（详见 §6 键清单），写法同上。

---

## 5. 接入层：功能页 XAML

每个功能行在现有 `ButtonContent` 旁加一个 `Description` 属性，示例（`Views/Pages/Functions/RaidSystemPage.xaml`）：

```xaml
<!-- 改前 -->
<controls:ComboxFrame
    ButtonContent="{DynamicResource Raid.Button.AiHealth}"
    ItemsSource="{Binding ViewModel.ConfigItemList.AIHealth}"
    SelectedValue="{Binding ViewModel.ConfigJson.Bot.AIHealth}"/>

<!-- 改后 -->
<controls:ComboxFrame
    ButtonContent="{DynamicResource Raid.Button.AiHealth}"
    Description="{DynamicResource Raid.Desc.AiHealth}"
    ItemsSource="{Binding ViewModel.ConfigItemList.AIHealth}"
    SelectedValue="{Binding ViewModel.ConfigJson.Bot.AIHealth}"/>
```

- 5 个功能页共 **224 处**，键映射规则见《i18n 多语言翻译方案报告》附录（`Raid.Button.X → Raid.Desc.X`、地图行为 `Raid.Map.X → Raid.Desc.Map.X`）；
- **建议用脚本批量插入**（每行按钮键已知，可自动生成对应 `Description` 属性行），避免手改遗漏；插入后人工抽查即可。
- 容器页：`ContainerFrameGroup.xaml` 内 5 行改一次即覆盖全部 23 个容器实例（共享 `Container.Desc.*` 键）。

---

## 6. 可选增强（Tier-2，不在 224 行主范围内）

### 6.1 分组标题说明（`Controls/FrameGroup.xaml`）

在标题 `ui:TextBlock` 旁加一个 `InfoTip`（`Description="{TemplateBinding HeaderDescription}"`），`FrameGroup` 增加 `HeaderDescription` 依赖属性；页面 `FrameGroup` 标签上加 `HeaderDescription="{DynamicResource Raid.HeaderDesc.AI}"`。键量：22 个（Raid 6 / Develop 12 / Economy 2 / Feature 2）。

### 6.2 容器名说明（`Controls/ContainerFrameGroup.xaml`）

标题旁同样加 `InfoTip`，`ContainerFrameGroup` 增加 `Description` 依赖属性，在 `RefreshHeaderText()` 旁并行解析 `ContainerItem.Desc.*` 翻译键。键量：23 个（与 `ContainerItem.*` 一一对应）。

> 两组共 45 键，是否纳入由你定；不加也不影响 224 行主功能。

---

## 7. 资源注册与构建

`App.xaml` 的 `<Application.Resources>/<MergedDictionaries>` 中追加（`Controls/ContainerFrameGroup.xaml` 之后即可）：

```xaml
<ResourceDictionary Source="Controls/InfoTip.xaml" />
```

其余流程不变（csproj 已内建 AfterBuild 单文件发布，IDE 构建按钮即产出）。

---

## 8. 验证步骤

1. **构建**：`dotnet build MGModSeries-CSharp.slnx -c Release`（或 IDE 构建按钮），0 错误；
2. **功能回归**：运行单文件，逐页核对——按钮/开关/下拉的位置与行为与改前一致；
3. **提示体验**：
   - 悬停问号 → 10 秒 ToolTip；
   - 点击问号 → 右侧卡片弹出，点外部关闭；
   - 长描述换行、贴边不溢出（当前限制：贴屏幕右缘可能被裁，见 §2.2 可选增强）；
4. **语言热切换**：设置页切 5 种语言，描述文本即时刷新（验证 `{DynamicResource}` 链路）；
5. **禁用态**：关闭"默认选项自定义"后，`AI数量` 等行仍可悬停看说明；
6. **i18n 完整性**：运行时观察有无 `{Missing:...}` 占位；用 `TranslationService.ValidateKeyConsistency()`（可临时在启动处调用或写一次性测试）核对 5 语言键一致。

---

## 9. 文件变更清单（汇总）

| 文件 | 动作 |
|---|---|
| `MGModEditor/Controls/InfoTip.xaml` | **新增**：图标 + ToolTip + Popup 模板 |
| `MGModEditor/Controls/InfoTip.xaml.cs` | **新增**：InfoTip 控件类 |
| `MGModEditor/Controls/CheckButtonFrame.xaml.cs` | 追加 `Description` 依赖属性 |
| `MGModEditor/Controls/ComboxFrame.xaml.cs` | 追加 `Description` 依赖属性 |
| `MGModEditor/Controls/CheckButtonFrame.xaml` | 左列追加 InfoTip |
| `MGModEditor/Controls/ComboxFrame.xaml` | 左列追加 InfoTip |
| `MGModEditor/Controls/ContainerFrameGroup.xaml` | 内嵌 5 行追加 Description |
| `MGModEditor/App.xaml` | 注册 InfoTip.xaml 资源字典 |
| `MGModEditor/Views/Pages/Functions/*.xaml` × 5 | 224 处追加 `Description` 属性 |
| `MGModEditor/res/i18n/*.json` × 5 | 新增描述键（见翻译方案报告） |
| `MGModEditor/Resources/Translations.cs` | 新增描述键常量（可选但推荐，防漂移） |
| `MGModEditor/Controls/FrameGroup.xaml(.cs)`、`ContainerFrameGroup.xaml.cs` | **仅 Tier-2**：分组/容器名说明 |
