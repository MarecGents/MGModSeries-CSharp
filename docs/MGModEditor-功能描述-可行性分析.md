# MGModEditor 功能描述提示 —— 可行性分析报告

> 日期：2026-08-14　|　适用版本：MGModEditor v1.3.1.1（MGModSeries-CSharp 整合仓）　|　状态：调研完成，结论为**可行，低风险**

---

## 1. 需求概述

为 MGModEditor **每一个功能**（Frame 包成的行：左侧 Button + 右侧 ToggleSwitch/ComboBox）添加功能描述：

- 在左侧 Button 的右侧紧挨着放一个**小的圆形问号图标**；
- **鼠标悬停**或**点击**该图标时，弹出一个小窗口展示该功能的描述文字；
- 弹出效果与 Home 页跳转按钮（`ui:CardAction`）的 description 呈现方式一致（原生 ToolTip，悬停显示 10 秒）。

要求：不改动任何现有功能逻辑，纯 UI 增量；描述文本走现有 i18n 体系，随语言切换实时刷新。

---

## 2. 现状调研（关键事实，均为源码核实）

### 2.1 页面与控件结构

| 页面 | FrameGroup 分组数 | 功能行数（CheckButtonFrame/ComboxFrame） |
|---|---|---|
| `Views/Pages/Functions/RaidSystemPage.xaml` | 6 | **44**（26 开关 + 18 下拉） |
| `Views/Pages/Functions/DevelopSystemPage.xaml` | 12 | **45**（34 开关 + 11 下拉） |
| `Views/Pages/Functions/EconomySystemPage.xaml` | 2 | **13**（8 开关 + 5 下拉） |
| `Views/Pages/Functions/ExclusiveFeaturePage.xaml` | 2 | **7**（7 开关） |
| `Views/Pages/Functions/ContainerExpandPage.xaml` | 23 个容器组 | **115**（23 容器 × 5 行） |
| **合计** | | **224** |

结构规律：每个功能行是 `controls:CheckButtonFrame` 或 `controls:ComboxFrame` 控件，通过 `ButtonContent="{DynamicResource Raid.Button.X}"` 显示左侧按钮文字，右侧绑定对应配置项。`ContainerFrameGroup`（`Controls/ContainerFrameGroup.xaml`）内部固定 5 行（启用/宽/高/兼容/负重），由 23 个容器实例复用，**这 5 行的文案键是共享的**（`Container.Button.*`，5 个键）。

### 2.2 Home 页 description 的现有实现（用户指定的参照物）

`Controls/GalleryNavigationPresenter.xaml:29-30`：

```xaml
ToolTip="{Binding Description}"
ToolTipService.ShowDuration="10000"
```

即：`NavigationCard.Description`（来自 `GalleryPageAttribute`，经 `TranslationService` 翻译）通过**原生 WPF ToolTip** 呈现，悬停显示 10 秒。整仓目前仅有这一处 ToolTip，**没有**任何 Flyout/Popup 先例。

### 2.3 i18n 机制

- `TranslationService`（`Services/TranslationService.cs`）：`res/i18n/*.json`（5 语言）以 EmbeddedResource 嵌入，启动/切语言时拍平为**点号扁平键**（如 `Raid.Button.AiHealth`）→ 注入 `Application.Resources.MergedDictionaries`；
- XAML 侧全部经 `{DynamicResource}` 消费，**语言切换即时热刷新，无需重建集合**（Home 卡片、导航菜单需要重建是因为它们用了 C# 集合绑定，纯 DynamicResource 场景不受影响）；
- 缺键显示 `{Missing:Key}` 占位，便于发现遗漏；`TranslationService.ValidateKeyConsistency()` 提供键一致性校验方法（当前仅定义、未在启动时调用）；
- 现有键量：**323 键 × 5 语言**（zh-CN 为基线）。

### 2.4 WPF-UI 4.3.0 控件能力盘点（已反射核实）

| 控件 | 是否适用 | 说明 |
|---|---|---|
| `ui:InfoBadge` | ✗ | 仅静态徽标（图标/数值），无点击事件，非交互控件 |
| `ui:Flyout` | △ | 是 ContentControl 包 `PART_Popup`（Popup + Fade 动画 + 主题边框），但**内部 Popup 未绑定 PlacementTarget**——锚定点是 Flyout 自身在布局中的位置，而非目标控件；需自行改造模板或代码接线才能精确贴住问号图标 |
| 原生 `ToolTip` | ✓ | 与 Home 页完全同机制，悬停 10 秒，零风险 |
| 原生 `Popup` | ✓ | `PlacementTarget` + `Placement="Right"` 可精确锚定到图标；`StaysOpen=False` 点击外部自动关闭 |
| `ui:SymbolIcon` | ✓ | `SymbolRegular.QuestionCircle16/20` 就是"圆圈内问号"现成字形，无需美术资产 |

---

## 3. 技术可行性逐项分析

| 需求点 | 实现方式 | 可行性 |
|---|---|---|
| 圆形问号图标 | `Border`/`Ellipse` 圆形底 + `ui:SymbolIcon Symbol="QuestionCircle16"`，宽高约 18px | ✅ 高，Fluent 字形现成 |
| 悬停弹窗 | 原生 `ToolTip`（`ToolTipService.ShowDuration=10000`），与 Home 页一致 | ✅ 高，零风险 |
| 点击弹窗 | 原生 `Popup`（`StaysOpen=False`、`Placement=Right`、`PlacementTarget=图标`、Fade 动画），或改版 `ui:Flyout` | ✅ 高（原生 Popup 为主推） |
| 描述文字随语言切换 | `Description` 依赖属性绑 `{DynamicResource Raid.Desc.X}` → 语言切换时 DP 值自动更新；ToolTip 每次打开取最新值，Popup 内容绑定实时刷新 | ✅ 高，**比 Home 卡片更简单**（无需 LanguageChanged 重建） |
| 不改现有逻辑 | 全部为增量：新增依赖属性 + 模板加一个元素 + 每行加一个属性 | ✅ 无破坏性 |
| 性能 | 224 行各带 1 个 ToolTip + 1 个 Popup；Popup 未打开时不实例化内容（Popup 是惰性显示），内存与渲染开销可忽略 | ✅ 无风险 |
| 禁用态行的可读性 | 行被禁用（如"AI数量"依赖"默认选项自定义"开关）时，图标加 `ToolTipService.ShowOnDisabled="True"`，悬停仍可查看说明 | ✅ 可控 |

**唯一需注意的布局细节**：两个控件的模板中左列是 `ui:Button`（`HorizontalAlignment=Stretch` 撑满列宽，内容居中）。问号图标以**同列叠放、右对齐**的方式放在按钮右侧边缘（紧挨按钮），不改变现有列结构与按钮行为，视觉即"紧挨在按钮右侧"。

---

## 4. 实现方案对比

### 方案 A：仅 ToolTip（悬停）
- 改动最小（图标 + ToolTip），但**不支持点击**，触屏/不易悬停场景体验差。
- 不满足"点一下"的需求，仅作降级备选。

### 方案 B：ToolTip（悬停）+ 原生 Popup（点击）✅ **推荐**
- 悬停走 ToolTip（与 Home 页完全一致）；点击走 Popup 钉住展示，点击外部/再次点击自动关闭；
- 均为 WPF 原生能力，无新依赖、无第三方 API 不确定项；
- Popup 内容用主题色 Card 样式（`CardBackgroundFillColorDefaultBrush`/`CardStrokeColorDefaultBrush`），与全应用风格统一；
- 每行一个 Popup 实例（惰性），224 行可接受；如后续担心，可收敛为窗口级单例 Popup（方案 A 的演进）。

### 方案 C：`ui:Flyout`（WPF-UI 4.3.0）
- 自带主题边框、Fade 动画、`Show/Hide/IsOpen` API，视觉最"原生"；
- 但 Popup 无 PlacementTarget 绑定，需在 `OnApplyTemplate` 中手动设置 `PART_Popup.PlacementTarget = 图标`（或改造其模板），属于对第三方控件内部结构的依赖；
- 可作为 B 的视觉增强选项，风险略高，**不建议首版采用**。

> **结论：采用方案 B**。交互上"悬停 = ToolTip（10 秒）、点击 = Popup 钉住"，两者并存，完全覆盖"鼠标放上去或者点一下"的需求表述。

---

## 5. 影响面与风险

### 5.1 改动文件清单（预估）

| 类别 | 文件 | 改动 |
|---|---|---|
| 新增控件 | `Controls/InfoTip.xaml(.cs)` | 新增（问号图标 + ToolTip + Popup 的组合控件） |
| 控件代码 | `Controls/CheckButtonFrame.xaml.cs`、`Controls/ComboxFrame.xaml.cs` | 各加 1 个 `Description` 依赖属性（约 10 行） |
| 控件模板 | `Controls/CheckButtonFrame.xaml`、`Controls/ComboxFrame.xaml` | 左列加 InfoTip 元素（约 4 行） |
| 资源注册 | `App.xaml` | 注册 InfoTip.xaml 资源字典（1 行） |
| 页面接入 | 5 个功能页 XAML + `Controls/ContainerFrameGroup.xaml` | 224 处各加 `Description="{DynamicResource ...}"` |
| i18n | `res/i18n/*.json` × 5 + `Resources/Translations.cs` | 新增约 114~159 键 × 5 语言（详见翻译方案报告） |
| 可选 | `Controls/FrameGroup.xaml(.cs)` | 分组标题旁同样加说明图标 |

### 5.2 风险点与对策

| 风险 | 等级 | 对策 |
|---|---|---|
| 模板布局回归（按钮/开关位置变化） | 低 | 图标为同列叠放元素，不挤占列宽；构建后人工过一遍 5 页截图比对 |
| 224 处 XAML 手改易漏 | 低 | 用脚本批量生成 `Description` 属性插入；`{Missing:Key}` 占位会在运行时暴露遗漏 |
| 语言包 JSON 语法错误（尾随逗号等，历史踩坑） | 低 | 编辑后跑一次 JSON 解析校验（启动时 `ResolveAndLoad` 失败会自动回退 zh-CN 并打印日志） |
| 5 语言键量较大（159 键 × 5） | 中 | 分阶段：先 zh-CN + en-US，再 ru/fr/jp；键一致性用 `ValidateKeyConsistency()` 校验 |
| 禁用行无法点击弹层 | 低 | 图标强制 `IsEnabled=True` + `ToolTipService.ShowOnDisabled="True"` |
| Popup 位置贴边溢出 | 低 | `Placement=Right` + `Popup.HorizontalOffset`；必要时用 `PlacementRectangle` 边界自适应（首版不做，记录为已知限制） |

### 5.3 兼容性

- 不改动任何现有 `ButtonContent`/`IsChecked`/`SelectedValue` 绑定与配置读写逻辑；
- 不引入新 NuGet 包；
- 老版本 config.json 无影响（纯界面层）。

---

## 6. 工作量估算

| 阶段 | 内容 | 估算 |
|---|---|---|
| 控件 + 模板 + 页面接入 | InfoTip、DP、模板、224 处属性（脚本辅助） | 0.5~1 天 |
| i18n 文案（zh-CN 基线 114~159 条 + 常量） | 撰写 + 录入 | 1~1.5 天 |
| 翻译（en-US 必做；ru/fr/jp） | 三语翻译 + 录入 + 一致性校验 | 1~2 天（可并行/分包） |
| 回归验证 | 构建、5 页人工核对、语言热切换、禁用态检查 | 0.5 天 |
| **合计** | | **约 3~5 天** |

---

## 7. 结论

**可行，且为低风险纯增量改动。**

- 交互载体（ToolTip + Popup）均为 WPF 原生能力，与 Home 页现有 description 机制同源，无新依赖；
- i18n 走现有 `DynamicResource` 热切换管线，语言联动**比 Home 卡片更简单**（无需重建集合）；
- 主要工作量不在代码，而在 **224 处页面接入与 5 语言文案**（文案量约 114~159 键，详见《i18n 多语言翻译方案》）；
- 建议按《代码方案》落地，文案按《翻译方案》分阶段推进。

**下一步**：确认范围（是否包含分组标题与容器名描述 = 45 个 Tier-2 键），然后按《代码方案报告》实施。
