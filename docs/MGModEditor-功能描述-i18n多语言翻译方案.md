# MGModEditor 功能描述 —— i18n 多语言翻译方案报告

> 日期：2026-08-14　|　适用版本：MGModEditor v1.3.1.1　|　语言：zh-CN / en-US / ru-RU / fr-FR / jp-JP

---

## 1. 现状与约束（沿用现有体系，不引入新机制）

| 项 | 现状（源码核实） |
|---|---|
| 语言包 | `res/i18n/*.json` 5 份，EmbeddedResource 嵌入（`MGEditor.i18n.<lang>.json`），csproj `EmbeddedResource` 规则已覆盖 `*.json`，新增键无需改构建 |
| 键格式 | **点号扁平键**：JSON 按 Section 分块，`"Raid": { "Button.AiHealth": "AI血量" }` 拍平为 `Raid.Button.AiHealth` |
| 消费方式 | XAML `{DynamicResource 键}` / C# `translation[key]`；热切换即时刷新 |
| 缺键表现 | `{Missing:Key}` 占位，界面可见，便于排查 |
| 一致性校验 | `TranslationService.ValidateKeyConsistency()` 已实现（以 zh-CN 为基线比对各语言多/缺键），当前未在启动调用，开发期可临时调用或写一次性检查 |
| 现有键量 | 323 键 × 5 语言，zh-CN 为基线 |

**历史踩坑提醒（本仓库经验）**：
- JSON **不允许尾随逗号**（`"a":1,` 行尾多余逗号 → 解析失败，启动回退 zh-CN 并打印日志，界面语言会"变回中文"——这是排查线索）；
- 键一律扁平点号式，**不要**在值里复用点号命名或嵌套对象；
- 新增键时同步在 `Resources/Translations.cs` 加同值常量（防手写字符串漂移，历史约定）。

---

## 2. 键命名规范

遵循现有 `<Section>.<子类>.<名称>` 惯例，描述键统一用 `Desc` 子类：

| 对象 | 键模式 | 示例 | 数量 |
|---|---|---|---|
| 功能行（Raid/Economy/Develop/Feature） | `<Section>.Desc.<X>` | `Raid.Desc.AiHealth` | 98 |
| 功能行（地图回保 11 个，按钮键为 `Raid.Map.X`） | `Raid.Desc.Map.<X>` | `Raid.Desc.Map.Customs` | 11 |
| 容器内嵌 5 行（23 容器共享） | `Container.Desc.<X>` | `Container.Desc.Enable` | 5 |
| **Tier-2** 分组标题 | `<Section>.HeaderDesc.<X>` | `Develop.HeaderDesc.WeaponOpt` | 22 |
| **Tier-2** 容器名 | `ContainerItem.Desc.<X>` | `ContainerItem.Desc.THICCWeaponCase` | 23 |
| **合计** | | | **159 键 × 5 语言 = 795 条** |

对应 JSON 结构（追加进现有 Section，无需新 Section）：

```json
"Raid": {
  "Desc.AiHealth": "调整战局内所有 AI（Scav/BOSS/PMC）的基础血量倍率……",
  "Desc.Map.Customs": "开启后海关地图内的物品可回保……"
},
"Container": {
  "Desc.Enable": "开启该容器的格子扩容……"
}
```

> 地图 11 个键用 `Raid.Desc.Map.X` 而非 `Raid.MapDesc.X`，是为了与按钮键 `Raid.Map.X` 保持"同名只换子类"的机械对应，降低脚本生成与人工核对成本。

---

## 3. 完整键清单（附录 A，按页分组，可直接当勾选清单）

### 3.1 Raid —— 44 键（33 常规 + 11 地图）

**AI 组**：`Raid.Desc.AiHealth`、`Raid.Desc.AiSpawnNumber`、`Raid.Desc.USECRatio`、`Raid.Desc.AiNamePool`、`Raid.Desc.PmcTacticalSquad`、`Raid.Desc.PmcRoar`

**战局默认选项组**：`Raid.Desc.DefaultOptions`、`Raid.Desc.AiAmount`、`Raid.Desc.AiDifficulty`、`Raid.Desc.EnableBoss`、`Raid.Desc.ScavCivilWar`、`Raid.Desc.MarkedCursed`

**战局设置组**：`Raid.Desc.RaidTime`、`Raid.Desc.BossSpawnRate`、`Raid.Desc.AirdropType`、`Raid.Desc.AlwaysExtractSwitch`、`Raid.Desc.AlwaysExtractable`、`Raid.Desc.UnlimitedExtractTime`、`Raid.Desc.ScavNoCooldown`、`Raid.Desc.ScavGearBoost`、`Raid.Desc.NoLootLoss`、`Raid.Desc.MapRefreshConfig`、`Raid.Desc.MapBotDifficulty`

**天气设置组**：`Raid.Desc.GlobalWeather`、`Raid.Desc.Cloud`、`Raid.Desc.Wind`、`Raid.Desc.Rain`、`Raid.Desc.Fog`

**资源设置组**：`Raid.Desc.GlobalLoot`、`Raid.Desc.ContainerLoot`、`Raid.Desc.GroundLoot`、`Raid.Desc.ValuableLoot`、`Raid.Desc.DisableRandomContainers`

**地图回保组**：`Raid.Desc.Map.Customs`、`Raid.Desc.Map.Factory`、`Raid.Desc.Map.Interchange`、`Raid.Desc.Map.Labs`、`Raid.Desc.Map.Lighthouse`、`Raid.Desc.Map.Reserve`、`Raid.Desc.Map.GroundZero`、`Raid.Desc.Map.Shoreline`、`Raid.Desc.Map.Streets`、`Raid.Desc.Map.Woods`、`Raid.Desc.Map.Labyrinth`

### 3.2 Develop —— 45 键

**基础属性组**：`Develop.Desc.UnloadSpeed`、`Develop.Desc.SkillExpBoost`、`Develop.Desc.SupermanMode`、`Develop.Desc.NoCarryLimit`

**武器组**：`Develop.Desc.WeaponSlotCompat`、`Develop.Desc.WeaponNoJam`、`Develop.Desc.WeaponNoDurabilityLoss`、`Develop.Desc.MagazineCapacity`

**头盔组**：`Develop.Desc.HelmetHeadsetCompat`、`Develop.Desc.HelmetNoDebuff`、`Develop.Desc.HelmetNoWeight`

**护甲弹挂组**：`Develop.Desc.RigArmorCompat`、`Develop.Desc.RigArmorNoDebuff`、`Develop.Desc.RigArmorNoWeight`、`Develop.Desc.RigArmorNoDurabilityLoss`

**插板组**：`Develop.Desc.PlateCarrierDurability`、`Develop.Desc.PlateCarrierNoDebuff`、`Develop.Desc.PlateCarrierNoWeight`

**背包组**：`Develop.Desc.BackpackCompat`、`Develop.Desc.BackpackCompact`、`Develop.Desc.BackpackNoDebuff`、`Develop.Desc.BackpackNoWeight`

**强化组**：`Develop.Desc.WeaponEnchant100`、`Develop.Desc.ArmorEnchant100`

**保险组**：`Develop.Desc.InsuranceExpand`、`Develop.Desc.InsuranceCompat`、`Develop.Desc.InsuranceNoWeight`

**物品属性组**：`Develop.Desc.T7ThermalBoost`、`Develop.Desc.KeyUnlimitedDurability`、`Develop.Desc.MedicalItemDurability`

**仓库组**：`Develop.Desc.AmmoStack`、`Develop.Desc.MoneyStack`、`Develop.Desc.FullInspect`

**任务组**：`Develop.Desc.QuestFreeReset`、`Develop.Desc.QuestOptimize`、`Develop.Desc.Quest3X4Marker`

**藏身处组**：`Develop.Desc.HideoutUpgradeTime`、`Develop.Desc.HideoutProductionTime`、`Develop.Desc.ScavCaseTime`、`Develop.Desc.HideoutUpgradeUnlimited`、`Develop.Desc.HideoutAreaBonusLevel`、`Develop.Desc.HideoutNoPower`、`Develop.Desc.GymAlwaysSucceed`、`Develop.Desc.GymNoPenalty`、`Develop.Desc.GymRewardMultiplier`

### 3.3 Economy —— 13 键

**跳蚤组**：`Economy.Desc.FleaTradeMultiplier`、`Economy.Desc.FleaSell100`、`Economy.Desc.FleaInstantSell`、`Economy.Desc.FleaBuyBoost`、`Economy.Desc.FleaBrandNew`、`Economy.Desc.FleaDisableBlacklist`、`Economy.Desc.FleaOpenLevel`、`Economy.Desc.LowTax`

**商人组**：`Economy.Desc.TraderSupplyTime`、`Economy.Desc.InsuranceReturnSpeed`、`Economy.Desc.InsuranceCost`、`Economy.Desc.InsuranceReturnChance`、`Economy.Desc.BuyWithHook`

### 3.4 Feature —— 7 键

**独立内容组**：`Feature.Desc.IndependentTrader`、`Feature.Desc.IndependentItem`、`Feature.Desc.IndependentPreset`、`Feature.Desc.IndependentSave`

**其他组**：`Feature.Desc.BulletData`、`Feature.Desc.KeyCategory`、`Feature.Desc.RealTimeFlea`

### 3.5 Container 内嵌 5 键（23 容器共享，只写一份）

`Container.Desc.Enable`、`Container.Desc.Width`、`Container.Desc.Height`、`Container.Desc.Compatible`、`Container.Desc.NoWeight`

### 3.6 Tier-2：分组标题 22 键

- Raid 6：`Raid.HeaderDesc.AI`、`Raid.HeaderDesc.DefaultOptions`、`Raid.HeaderDesc.Raid`、`Raid.HeaderDesc.Weather`、`Raid.HeaderDesc.Resources`、`Raid.HeaderDesc.MapInsurance`
- Develop 12：`Develop.HeaderDesc.BaseAttributes`、`WeaponOpt`、`HelmetOpt`、`RigArmorOpt`、`PlateCarrierOpt`、`BackpackOpt`、`EnchantOpt`、`InsuranceOpt`、`ItemAttrOpt`、`WarehouseOpt`、`QuestSystem`、`Hideout`
- Economy 2：`Economy.HeaderDesc.Flea`、`Economy.HeaderDesc.Trader`
- Feature 2：`Feature.HeaderDesc.Independent`、`Feature.HeaderDesc.Other`

### 3.7 Tier-2：容器名 23 键（与 `ContainerItem.*` 同名）

`ContainerItem.Desc.BallisticPlateCase`、`GingyKeychain`、`HolodilnickThermalBag`、`THICCWeaponCase`、`THICCItemCase`、`WZWallet`、`StreamerItemCase`、`MedicineCase`、`SICCPouch`、`LuckyScavJunkBox`、`MagazineCase`、`AmmunitionCase`、`PistolCase`、`GrenadeCase`、`DocumentsCase`、`WeaponCase`、`InjectorCase`、`ItemCase`、`DogtagCase`、`SimpleWallet`、`KeycardHolderCase`、`KeyTool`、`MoneyCase`

---

## 4. 翻译样例（每语言风格基准，覆盖全部 Section 类型）

以下 10 键 × 5 语言作为**风格基准**：后续 159 键按此口吻（简短 1~2 句、说清"开/关后的效果"、专有名词不译）撰写。

| 键 | zh-CN（基线） | en-US |
|---|---|---|
| Raid.Desc.AiHealth | 调整战局内所有 AI（Scav/BOSS/PMC）的基础血量倍率，数值越高越难击杀。 | Adjusts the base health multiplier of all AI (Scavs, bosses, PMCs) in a raid. Higher values make them harder to kill. |
| Raid.Desc.PmcRoar | 开启后 PMC 在战局内会随机发出战吼语音，增强战场氛围。 | When enabled, PMCs randomly shout battle cries in raid for a more immersive battlefield. |
| Raid.Desc.Map.Customs | 开启后海关地图内的物品可回保，死亡或丢弃后有机会由保险返还。 | Enables insurance returns for items lost on Customs; items lost on death may be returned by insurance. |
| Economy.Desc.FleaSell100 | 挂在跳蚤市场的物品必定售出，不再受买家流量与随机性影响。 | Items listed on the flea market always sell, no longer affected by buyer traffic or randomness. |
| Develop.Desc.WeaponNoJam | 关闭武器卡壳机制，射击时不再随机出现枪械故障。 | Disables weapon jamming; firearms no longer fail randomly during firefights. |
| Develop.Desc.HideoutNoPower | 藏身处设施不再消耗电力，依赖燃料的功能不再受燃料限制。 | Hideout facilities no longer consume power; fuel-dependent features are no longer gated by fuel. |
| Feature.Desc.IndependentTrader | 启用 MG-Mod 自带独立商人（麻瓜与 FG），与官方商人并存，提供专属物品与任务。 | Enables MG-Mod's independent traders (Muggle & FG), coexisting with official traders and offering exclusive items and quests. |
| Container.Desc.Enable | 开启该容器的格子扩容，按下方宽度/高度设置调整容量。 | Enables grid expansion for this container, resizing it per the width/height settings below. |
| ContainerItem.Desc.THICCWeaponCase | 大型武器箱，默认 10×70 格；开启扩容后可进一步自定义容量。 | Large weapon case (10×70 cells by default); with expansion enabled you can customize its capacity. |
| Raid.HeaderDesc.Weather | 调整战局的天气表现，包括全局模式、云量、风、雨、雾。 | Adjusts raid weather, including global mode, clouds, wind, rain, and fog. |

| 键 | ru-RU | fr-FR | jp-JP |
|---|---|---|---|
| Raid.Desc.AiHealth | Регулирует множитель базового здоровья всех AI (Scav, боссы, PMC). Чем выше значение, тем сложнее их убить. | Ajuste le multiplicateur de santé de base de tous les IA (Scavs, boss, PMC). Plus la valeur est élevée, plus ils sont difficiles à tuer. | レイド内の全AI（Scav/ボス/PMC）の基礎体力倍率を調整します。値が高いほど倒しにくくなります。 |
| Raid.Desc.PmcRoar | При включении PMC случайно выкрикивают боевые кличи в рейде для большего погружения. | Une fois activé, les PMC poussent des cris de guerre aléatoires en raid pour un champ de bataille plus immersif. | 有効にするとPMCがレイド中にランダムに戦闘叫びを発し、戦場の臨場感が高まります。 |
| Raid.Desc.Map.Customs | Включает страховой возврат предметов на карте Customs; потерянные предметы могут быть возвращены страховкой. | Active le retour d'assurance des objets sur Customs ; les objets perdus peuvent être rendus par l'assurance. | Customsマップでの保険返却を有効にします。死亡・紛失時、保険で返還される可能性があります。 |
| Economy.Desc.FleaSell100 | Предметы на барахолке продаются всегда, без влияния трафика покупателей и случайности. | Les objets mis en vente sur le marché aux puces se vendent toujours, sans être affectés par le trafic ou l'aléatoire. | フリーマーケットに出品したアイテムは必ず売れます。購入者数やランダム性の影響を受けません。 |
| Develop.Desc.WeaponNoJam | Отключает заклинивание оружия; оружие больше не даёт осечек в бою. | Désactive les enrayages ; vos armes ne tomberont plus en panne aléatoirement. | 武器のジャム機構を無効化します。射撃中にランダムに故障しなくなります。 |
| Develop.Desc.HideoutNoPower | Объекты убежища больше не потребляют электроэнергию; функции, зависящие от топлива, не ограничены им. | Les installations de la planque ne consomment plus d'électricité ; les fonctions liées au carburant ne sont plus bloquées. | 隠れ家の施設が電力を消費しなくなります。燃料依存の機能も燃料の制約を受けません。 |
| Feature.Desc.IndependentTrader | Включает собственных торговцев MG-Mod (Мугл и FG), сосуществующих с официальными и предлагающих эксклюзивные предметы и задания. | Active les marchands indépendants de MG-Mod (Muggle et FG), coexistant avec les marchands officiels et proposant objets et quêtes exclusifs. | MG-Mod独自の独立商人（マグル＆FG）を有効にします。公式商人と共存し、専用アイテムとクエストを提供します。 |
| Container.Desc.Enable | Включает расширение сетки контейнера в соответствии с настройками ширины/высоты ниже. | Active l'agrandissement de la grille de ce conteneur, selon les réglages de largeur/hauteur ci-dessous. | このコンテナのグリッド拡張を有効にします。下の幅/高さ設定に従って容量が変わります。 |
| ContainerItem.Desc.THICCWeaponCase | Большой кейс для оружия (по умолчанию 10×70 ячеек); при включённом расширении ёмкость можно настроить. | Grande caisse d'armes (10×70 cases par défaut) ; avec l'agrandissement, vous pouvez personnaliser sa capacité. | 大型武器ケース（デフォルト10×70マス）。拡張を有効にすると容量をさらにカスタマイズできます。 |
| Raid.HeaderDesc.Weather | Настройка погоды в рейде: глобальный режим, облачность, ветер, дождь, туман. | Règle la météo des raids : mode global, nuages, vent, pluie et brouillard. | レイドの天候を調整します。グローバルモード、雲量、風、雨、霧を含みます。 |

---

## 5. 术语与风格规范（翻译时强制遵守）

1. **专有名词不译**：Scav、PMC、BOSS、USEC、Bear、T7、THICC、Customs、Factory、Labs、Labyrinth、SICCPouch 等游戏内名词保持原文（现有 323 键也如此处理）；
2. **句式**：陈述效果，优先"开启后 / 调整 / 关闭"开头，1~2 句，不用感叹号；
3. **长度**：单条 ≤ 120 字符（Popup MaxWidth 320px + 自动换行，过长会显得啰嗦）；
4. **语气一致**：zh 用"你"或省略主语皆可，其余语言保持一致的人称（样例用中性直陈式）；
5. **数值用词**：倍率/概率/时长等量化描述要准确对应 `ConfigItem.*` 的选项含义（如 `Default` 组指"原版默认"）；
6. **禁用态功能**：描述里可补一句前置条件（如"需先开启‘默认选项自定义’"），帮助用户理解置灰原因；
7. **不放入口/无 markdown**：值为纯文本，不写 `**`、链接、换行符 `\n`（CreditsText 是多行长文本的孤例，功能描述不需要）；
8. **容器描述**：默认格数与可扩容是重点（`10×70` 这类数字照抄，勿翻译）。

---

## 6. 实施流程（分阶段）

### 阶段 1：键骨架（0.5 天）
1. 5 份 JSON 各加 Tier-1 的 114 个键，值先占位 `TODO`（保证 5 语言键一致，规避 `{Missing:}`）；
2. `Resources/Translations.cs` 追加 114 个常量（`RaidDescAiHealth = "Raid.Desc.AiHealth"` 等）；
3. 代码侧接入（见代码方案报告 §2~§5）。

### 阶段 2：zh-CN 基线文案（1~1.5 天）
1. 按 §3 清单逐组撰写，可参考 `docs/MGModServer-README.md` 与各功能配置含义；
2. 完成后 zh-CN 即最终基线，其余语言以此为源。

### 阶段 3：翻译（1~2 天，可并行分包）
- en-US（必做，与发布同包）；ru-RU / fr-FR / jp-JP 可分包并行；
- 每语言一人/一组负责，按 §5 规范执行，用 §4 样例校准口吻。

### 阶段 4：验收（0.5 天）
1. **键一致性**：跑 `ValidateKeyConsistency()`，5 语言零多零缺；
2. **运行时检查**：5 语言各切一遍，界面无 `{Missing:...}`、无 `TODO` 残留；
3. **JSON 合法性**：5 文件可被 `ResolveAndLoad` 正常解析（无尾随逗号）；
4. **视觉回归**：长描述（如 `Feature.Desc.IndependentTrader` 与 `Develop.Desc.HideoutNoPower`）在 320px 弹窗内正常换行；
5. **版本记录**：键量从 323 → 482（Tier-1）或 482+45=527（含 Tier-2），在发布日志注明。

---

## 7. 结论与建议

- **键量**：Tier-1 = 114 键 × 5 语言 = 570 条；含 Tier-2 共 159 键 × 5 语言 = 795 条；
- **建议**：Tier-1 随功能提示一并落地；Tier-2（分组标题 + 容器名，45 键）可二期再做，互不阻塞；
- 现有 i18n 管线（DynamicResource 热切换、缺键占位、一致性校验、JSON 嵌入）**完全支撑**本需求，无新机制、无新依赖。

> 关联文档：`MGModEditor-功能描述-可行性分析.md`、`MGModEditor-功能描述-代码方案.md`
