# MG 通用商人（MGGTMod）制作教程

> 基于 **MGModSeries-CSharp** 整合仓的 **MGGTMod** 项目源码与其内置 **FlanrecGents** 示例商人，
> 并参考同仓 **MGModServer** 项目中的 **MarecGents** 示例商人编写。
> 适用于 SPT 4.1.x + .NET 10.0。

---

## 🎯 难度标识（先看这里）

本教程各章节按难度分级，你可以按需跳读：

| 标识 | 难度 | 内容 | 适合 |
|:---:|:---:|------|------|
| ⭐ | **基础** | 文件架构、商人核心信息、商品、物品、本地化、跳蚤分类 | 所有人都要掌握，是"添加商人"的全部所需 |
| ⭐⭐ | **进阶** | 任务系统（击杀/收集/上交条件、奖励、文本注册、任务解锁商品） | 想给商人加任务链、剧情、任务奖励时再看 |
| ⭐⭐⭐ | **高级** | 自编译发布、改名防冲突、把 MGGTMod 变成"你自己的 Mod" | 想打包发布给别人时才需要 |

> **新手建议路径**：⭐ 全部 → 先用「4.1 复制模板」做出第一个商人 → 玩熟后再学 ⭐⭐ 任务系统；
> ⭐⭐⭐ 只在发布前需要。**绝大多数"做一个自己的商人"只需要 ⭐ 部分，不需要编译。**

---

## 一、教程基础信息

### 1.1 本教程讲什么

本教程教你**在 MGGTMod 中制作一个自定义商人**，包括：

- MGGTMod 的文件与文件夹架构，以及每个文件的作用；
- 商人文件夹内各 JSON 文件的内部结构（逐字段说明）；
- 从复制模板到从零撰写的完整示例；
- 如何修改源码、自编译并发布成**你自己的 Mod**（改名防止与原版冲突）。

### 1.2 MGGTMod 是什么

MGGTMod（MG General Trader Mod）是一个 **SPT 服务端自定义商人框架**：
把"商人"做成 `traders/` 下的一个个**文件夹**，每个文件夹放若干 JSON 配置，
服务端启动时自动扫描加载——**添加新商人只需写 JSON，不需要写一行 C# 代码**。

它与 MG-Mod 的关系：

```
MGModServer 项目（MG-Mod 迁移，完整版，含战局/经济/养成/容器/特色功能）
└── 独立商人功能  ← 与本项目共享同一套商人加载逻辑
MGGTMod 项目（独立版，仅保留自定义商人框架）
└── 商人加载逻辑与 MG-Mod 完全一致（两项目 CustomTraderServices.cs 逐行相同）
```

因此本教程内容**同样适用于 MG-Mod（MGModServer 项目）** 的独立商人功能。

### 1.3 系统要求

| 项目 | 要求 |
|------|------|
| 游戏服务端 | SPT **4.1.0+**（代码中 `SptVersion` 声明为 4.1.2） |
| 运行时 | .NET 10.0（Windows x64） |
| 仅改 JSON | 无需任何开发环境，直接改 `user/mods/MGGTMod/traders/` 即可 |
| 自编译发布 | 需要 .NET 10 SDK（Visual Studio 2022 / Rider / dotnet CLI） |

### 1.4 两种用法，先分清

| 用法 | 做什么 | 是否需要编译 |
|------|--------|:----:|
| **直接添加商人** | 复制 `traders/FlanrecGents/` 改成你的商人 | ❌ 不需要，改 JSON 即可 |
| **自编译发布** | 把整个 MGGTMod 改成"你的 Mod"再发布 | ✅ 需要，见第五章 |

> 绝大多数场景只需要第一种。只有当你想**把 MGGTMod 当作自己 Mod 的源码基础**、
> 改名打包发布给别人时，才需要第五章的流程。

### 1.5 术语约定

- **商人（Trader）**：`traders/` 下的一个文件夹，内含该商人全部配置；
- **商人 ID（`_id`）**：`traderInfo.json` 中的 24 位 MongoId，是商人在游戏数据库中的唯一标识；
- **物品 ID**：`items/*.json` 中 `item._id`，同样要求 MongoId；
- **商品条目 ID**：`assort.json` 中 `items[]._id`，可以用任意字符串（加载时会自动重写为 MongoId，见 3.4）。

---

## 二、文件和文件夹架构和用途 ⭐

### 2.1 整体目录结构

```
MGModSeries-CSharp/                   ← 整合仓根目录（四个 C# 项目 + 统一构建）
├── README.md / LICENSE               ← 项目说明与许可证（CC BY-NC-ND 4.0）
├── MGModSeries-CSharp.slnx           ← 解决方案（MGGTMod / MGModServer / MGModEditor / MGModClient）
├── Directory.Build.props             ← 统一构建输出配置（决定 Build\ 输出位置）
├── MGGTMod/                          ← ★ 本教程对象：MGGTMod 项目（下文以此为根）
│   ├── MGGTmod.cs                    ← Mod 入口（ModMetadata 元数据 + IOnLoad 加载器）
│   ├── MGGTMod.csproj                ← 项目文件（.NET 10 类库，AssemblyName=MGGTMod → MGGTMod.dll）
│   ├── types/                        ← ★ C# 源码层（改这里需要重新编译）
│   │   ├── models/
│   │   │   ├── Custom/               ← 自定义配置类型（ConfigSettingType.cs 等）
│   │   │   ├── EFT/                  ← EFT 数据模型映射
│   │   │   │   ├── MGGlobals.cs              （CustomGlobals：Buffs/ItemPresets）
│   │   │   │   ├── locales/                  （TraderDesc/ItemsDesc/QuestDesc 等）
│   │   │   │   ├── locations/                （CustomLooseLoot.cs）
│   │   │   │   ├── templetes/                （CustomTraderItems/TemplateItem 相关）
│   │   │   │   └── traders/                  （CustomTraderInfo/CustomTraderInsurance 等）
│   │   │   └── Paths/Paths.cs        ← 全部路径常量（./traders、./res/services/TraderDB 等）
│   │   ├── server/                   ← 服务器模块封装（8 个）
│   │   │   ├── TradersServer.cs      ← 商人表（GetTraders/GetTrader）
│   │   │   ├── TemplatesServer.cs    ← 物品表/跳蚤分类/任务/过滤器（IsItemExists/AddFilters 等）
│   │   │   ├── LocalesServer.cs      ← 本地化（AddTraderInfo/AddItemInfo/AddQuestInfo）
│   │   │   ├── GlobalsServer.cs      ← 全局参数（AddBuffs 等）
│   │   │   ├── ConfigsServer.cs      ← 游戏配置（保险/跳蚤/更新时间/重复任务白名单）
│   │   │   ├── LocationsServer.cs    ← 地图表
│   │   │   ├── BotsServer.cs         ← AI 机器人
│   │   │   └── HideoutServer.cs      ← 藏身处
│   │   ├── services/                 ← 业务服务层
│   │   │   ├── ConfigSettingServices.cs  ← 入口调度（ModSetting）
│   │   │   ├── CustomTraderServices.cs   ← ★ 核心商人服务（商人加载全流程）
│   │   │   └── TestServices.cs           ← 测试用（不影响正式流程）
│   │   └── utils/MGUtils.cs          ← 工具类（读 JSON/文件/路径/日志）
│   ├── traders/                      ← ★ 商人配置目录（数据层，改 JSON 即生效）
│   │   └── FlanrecGents/             ← 内置示例商人（可整目录复制当模板）
│   ├── res/                          ← 资源目录
│   │   └── services/TraderDB/        ← 商人基础数据模板
│   │       ├── base.json             ← ★ 商人基础模板（所有商人共用，代码自动填充）
│   │       ├── assort.json / questassort.json / dialogue.json / services.json / suits.json
│   ├── bundles/                      ← 资源包目录（含 FG Rig Layouts bundle）
│   ├── bundles.json                  ← 资源包清单（运行时由代码重写）
│   └── obj/                          ← 编译产物（Release 输出见 5.5）
├── MGModServer/                      ← 兄弟项目（MG-Mod 迁移，含 MarecGents 商人模板）
├── MGModEditor/  MGModClient/        ← 兄弟项目（图形化编辑器 / 客户端插件）
├── docs/                             ← 各项目 README 与本教程
└── scripts/build-menu.cmd / .ps1     ← 交互式构建菜单
```

### 2.2 一句话理解各层

```
┌─────────────────────────────────────────────────────┐
│ 数据层（traders/、res/）  ← 你主要修改的地方，改 JSON 即可 │
│        │ 启动时被 CustomTraderServices 扫描加载         │
│        ▼                                              │
│ 代码层（types/、MGGTmod.cs、csproj） ← 框架本身，通常不动  │
│        需要改 Mod 名称/发布时才会碰                       │
└─────────────────────────────────────────────────────┘
```

- **`traders/`**：一个子文件夹 = 一个商人。**新增/删除/修改商人全靠这里**。
- **`res/services/TraderDB/base.json`**：商人的"骨架"模板（血量货币、可买卖分类、忠诚度等），
  代码加载 `traderInfo.json` 后用它拼装出完整的商人对象，一般**不需要改**。
- **`types/`**：C# 源码。只有要发布成独立 Mod 时才需要理解并改名。

### 2.3 商人加载流程（理解了这个就理解了一切）

服务端启动时，`MGGTmod`（入口）→ `ConfigSettingServices.ModSetting()` → `CustomTraderServices.Start()`，流程如下：

```
扫描 ./traders/ 下每个子文件夹（每个 = 一个商人）
  ├─ ① 读取 traderInfo.json
  │     ├─ enable=false 或 _id 与已有商人冲突 → 跳过并提示
  │     ├─ 用 res/services/TraderDB/base.json 生成商人基础信息
  │     ├─ 注册头像（{_id}.jpg）→ /files/trader/avatar/
  │     ├─ 注册保险/修理/忠诚度/对话/折扣等
  │     ├─ 写入保险返回率、重复任务白名单、跳蚤、更新时间、初始忠诚度
  │     └─ 读取 traderData/assort.json + questassort.json（商品与任务关联）
  ├─ ② images/quests/ 下的任务图标 → /files/quest/icon/
  ├─ ③ items/*.json        → 注册自定义物品（重名则跳过并警告）
  ├─ ④ locales/            → itemsdescription.json（物品名）+ mail.json（任务文本）
  ├─ ⑤ templates/          → handbook.json（跳蚤分类条目）+ quests.json（任务定义）
  ├─ ⑥ globals.json        → ItemPresets + Buffs（针剂效果）
  └─ ⑦ bundles.json        → 收集后重写 mod 根目录 bundles.json
输出日志：商人【X】已添加。
```

> 记不住没关系，只要知道：**一个商人文件夹里能放哪些文件、每个文件装什么**，就足够了。

---

## 三、各个文件夹下的特定文件的内部结构 ⭐

### 3.1 商人文件夹标准结构（以 FlanrecGents 为例）

```
traders/FlanrecGents/
├── 9cc23608f000000000000000.jpg   ← 商人头像（文件名必须 = 商人 _id）
├── traderInfo.json                ← ★ 商人核心信息（必须）
├── globals.json                   ← 针剂 Buff / 物品预设
├── bundles.json                   ← 资源包清单（一般留空）
├── items/                         ← 自定义物品（可选）
│   ├── FG-Endurance1.json
│   └── FG-Zagustin.json
├── locales/                       ← 本地化文本（可选）
│   ├── itemsdescription.json      ← 物品名称/描述
│   └── mail.json                  ← 任务/邮件文本
├── templates/                     ← 模板数据（可选）
│   ├── handbook.json              ← 跳蚤分类条目
│   └── quests.json                ← 任务定义
├── traderData/                    ← 交易数据（必须，可只放最小内容）
│   ├── assort.json                ← ★ 出售商品清单
│   └── questassort.json           ← 任务与商品关联
└── location/                      ← 战利品（可选，当前框架未启用）
    └── looseLoot.json
```

| 文件 | 必须 | 作用 |
|------|:----:|------|
| `traderInfo.json` | ✅ | 商人 ID、名称、描述、位置、保险、修理、忠诚度等 |
| `{_id}.jpg` 头像 | ✅ | 商人头像（缺了会报"你把我的头像放哪了"警告，商人仍可加载） |
| `traderData/assort.json` | ✅ | 出售商品、价格、忠诚度等级 |
| `globals.json` | ✅ | 针剂 Buff（若物品用到 `StimulatorBuffs` 则必须） |
| `items/*.json` | ❌ | 自定义物品定义 |
| `locales/*.json` | ❌ | 物品名/描述、任务文本 |
| `templates/*.json` | ❌ | 跳蚤分类条目、任务 |
| `traderData/questassort.json` | ❌ | 任务关联商品 |
| `location/*.json` | ❌ | 战利品（当前未生效） |
| `images/quests/*.jpg` | ❌ | 任务图标（会被注册为 `/files/quest/icon/文件名`；FlanrecGents 未使用，见 MarecGents） |
| `bundles.json` | ❌ | 资源包清单（无自定义 bundle 则留空） |

---

### 3.2 `traderInfo.json` —— 商人核心信息

以 FlanrecGents 为例，逐字段说明：

```jsonc
{
  "enable": true,                    // 是否启用；false 则该商人被跳过
  "name": "FlanrecGents",            // 逻辑名（建议与文件夹同名）
  "_id": "9cc23608f000000000000000", // ★ 商人 ID：24 位 MongoId（0-9a-f），必须全局唯一！
  "locales": {                       // 商人名称与描述（游戏内显示，可中文）
    "FullName": "FG商人",            //   全名
    "FirstName": "FG商人",           //   名
    "Nickname": "FG商人",            //   昵称（重复任务白名单等也会用到）
    "Location": "MG实验室",          //   所在地
    "Description": "FG是MG商人的好朋友……" // 简介
  },
  "insurance": {                     // 保险服务
    "enable": true,                  //   是否提供保险
    "minreturnTime": 0,              //   最短返还小时
    "maxreturnTime": 0,              //   最长返还小时
    "pay": 0,                        //   保险费用
    "chance": 100,                   //   返还概率（0-100）
    "storageTime": 100,              //   最长保管时间
    "Message": {                     //   保险各状态对话（每项是字符串数组）
      "insuranceStart":     ["真是的，别的事情不来找我……"],
      "insuranceFound":     ["你的东西找到了呢……"],
      "insuranceFailed":    ["该死的，一定是MG在半路……"],
      "insuranceExpired":   ["小子，我这么惦记你……"],
      "insuranceComplete":  ["好了好了，我们之间两清了呢……"],
      "insuranceFailedLabs":[]       //   实验室地图失败消息（可留空数组）
    }
  },
  "repair": {                        // 修理服务
    "enable": false,                 //   是否提供修理
    "coefficient": 1,                //   价格系数
    "quality": 3                     //   修理质量
  },
  "loyaltyLevels": {                 // 忠诚度等级（共 4 级，数组长度 4）
    "description": {                 //   仅注释用途，代码不读取（模板自带说明）
      "main": "用于自定义商人的好感、等级、消费额等关系（此字段无用）",
      "minLevel": "解锁商人等级最低玩家等级",
      "minSalesSum": "解锁商人等级所需消费额",
      "minStanding": "解锁商人等级最低好感",
      "buy_price_coef": "购买物品的优惠,正数变贵，负数打折",
      "repair_price_coef": "修理折扣 同购买",
      "insurance_price_coef": "投保优惠 同购买",
      "exchange_price_coef": "不知道是干什么的",
      "heal_price_coef": "治疗优惠 同购买"
    },
    "range": [                       //   4 个等级，逐级解锁
      { "minLevel": 1,  "minSalesSum": 0,       "minStanding": 0, "buy_price_coef": 0, "repair_price_coef": -50, "insurance_price_coef": 30, "exchange_price_coef": 0, "heal_price_coef": 100 },
      { "minLevel": 15, "minSalesSum": 0,       "minStanding": 0, "buy_price_coef": 0, "repair_price_coef": -50, "insurance_price_coef": 20, "exchange_price_coef": 0, "heal_price_coef": 50  },
      { "minLevel": 30, "minSalesSum": 0,       "minStanding": 0, "buy_price_coef": 0, "repair_price_coef": -50, "insurance_price_coef": 10, "exchange_price_coef": 0, "heal_price_coef": 0   },
      { "minLevel": 45, "minSalesSum": 0,       "minStanding": 0, "buy_price_coef": 0, "repair_price_coef": 0,  "insurance_price_coef": 5,  "exchange_price_coef": 0, "heal_price_coef": -50 }
    ]
  },
  "discount": 0,                     // 出售物品的价格折扣：不为 0 时价格降低，为 0 时价格最高、获利最多
  "medic": false,                    // 是否为医疗商人
  "updateTime": { "min": 600, "max": 600 }, // 商品刷新间隔（秒），min/max 之间随机
  "unlockedDefault": true,           // 是否默认解锁（false 则需先完成任务解锁）
  "items_buy": {                     // ★ 收购配置（玩家可以把物品**卖给你这个商人**）
    "category": [                    //   可收购的「物品大类」（物品的 _parent 分类 id）
      "5448e5284bdc2dcb718b4567",    //     Vest 弹挂/胸挂
      "5448e54d4bdc2dcc718b4568",    //     Armor 护甲
      "5448f3a64bdc2d60728b456a",    //     Stimulator 医疗/针剂
      "65649eb40bf0ed77b8044453",    //     BuiltInInserts 内置插板
      "5447b5cf4bdc2d27728b4568",    //     Weapon 武器
      "5485a8684bdc2da71d8b4567",    //     Ammo 弹药
      "5448bf274bdc2dfc2f8b456a"     //     Medical 医疗用品
    ],
    "id_list": []                    //   额外指定可收购的「单个物品 id」（留空 = 不额外指定）
  }
}
```

> ⚠️ **没有 `items_buy` 会怎样**：SPT 的出售校验要求物品大类必须在该商人的 `items_buy.category` 里，
> 否则**玩家无法把任何东西卖给你这个商人**（出售界面不显示/交易失败）。
> 想"玩家能把护甲/弹挂甲/武器等卖给我"，就把对应大类 id 加进 `category`。
> 大类 id（`_parent`）可在 SPT 数据库 `templates/items.json` 或原版同类物品的 `_parent` 字段查到。
> 不配置时框架会自动给一个覆盖上述 7 类的默认值（见 `CustomTraderServices.cs`），所以漏写也不会完全失效——但显式配置更清晰可控。

> ⚠️ **`_id` 是重中之重**：不能与游戏已有商人、也不能与其他 Mod 商人重复。
> 重复时加载会提示：`商人【X】的Id:xxx已存在于游戏中,请修改.` 并跳过该商人。
> 建议用随机生成的 24 位十六进制（如在线 MongoId 生成器）。
>
> 💡 **强烈建议：用「你自己的独特前缀」做 ID 开头**。24 位 MongoId = 24 个十六进制字符（0-9a-f），
> 你可以用**前几位做你的专属标签**，后几位做编号（如 `你的前缀` + 递增编号），
> 这样你的商人/物品 ID 天然全局唯一、不会与任何人撞车、也便于管理。
> 例如作者本人用 QQ 号转 16 进制作为前缀（`9cc23608...` 是 `9xxx` 的 QQ 号转换而来）。
> **你的商人 `_id`、物品 `_id`、任务 `_id`、Buff 组名，全部都要用你自己的前缀**，不要照抄示例里的任何 ID！

---

### 3.3 头像文件 `{_id}.jpg`

- 放在**商人文件夹根目录**，文件名 = `traderInfo.json` 里的 `_id`，例如 `9cc23608f000000000000000.jpg`；
- 加载时自动注册到 `/files/trader/avatar/` 路由；
- 缺失时提示：`XXX：混蛋，你把我的头像放哪了！快还给我！`（商人仍会加载，只是没头像）。

---

### 3.4 `traderData/assort.json` —— 出售商品清单

```jsonc
{
  "items": [                         // 商品条目列表
    {
      "_id": "FGTrader_default_assort_item_01", // ★ 商品条目 ID（见下方说明）
      "_tpl": "9cc236080000000000000000",       // ★ 物品模板 ID（items.json 中存在的物品）
      "parentId": "hideout",         // 固定 "hideout"
      "slotId": "hideout",           // 固定 "hideout"
      "upd": {                       // 可选：商品属性
        "UnlimitedCount": true,      //   不限库存
        "StackObjectsCount": 9999999 //   库存数量
      }
    },
    { "_id": "…02", "_tpl": "9cc236080000000000000001", "parentId": "hideout", "slotId": "hideout", "upd": { "UnlimitedCount": true, "StackObjectsCount": 9999999 } }
  ],
  "barter_scheme": {                 // ★ 价格表：商品条目 ID → 支付方案
    "FGTrader_default_assort_item_01": [
      [ { "count": 50000, "_tpl": "5449016a4bdc2d6f028b456f" } ] // 50000 卢布
    ],
    "FGTrader_default_assort_item_02": [
      [ { "count": 50000, "_tpl": "5449016a4bdc2d6f028b456f" } ]
    ]
  },
  "loyal_level_items": {             // ★ 购买所需商人等级：商品条目 ID → 1~4
    "FGTrader_default_assort_item_01": 1,
    "FGTrader_default_assort_item_02": 1
  }
}
```

要点：

- **`_tpl` 填什么**：必须是游戏 `items.json` 中存在的物品 ID。出售**自定义物品**时填 `items/*.json` 里定义的 `_id`；出售**原版物品**时填原版物品 ID。
- **`_id` 可以用任意字符串**（如 `FGTrader_default_assort_item_01`）：加载时若检测到它不是 MongoId，会自动生成新的 MongoId，并把 `barter_scheme`、`questassort.json` 中所有引用一并替换——所以三个文件里的商品条目 ID 保持一致即可，不用自己生成 MongoId。
- **货币 ID**：卢布 `5449016a4bdc2d6f028b456f`、美元 `5696686a4bdc2da3298b456a`、欧元 `569668774bdc2da2298b4568`。
- 想做成"以物易物"：在 `barter_scheme` 的数组里放多组 `{ "count": 数量, "_tpl": 物品ID }`，如 `[ [ {…卢布}, {…物品} ] ]`。

---

### 3.5 `traderData/questassort.json` —— 任务关联

```jsonc
{
  "started": {},   // 任务进行中解锁的商品：{ 商品条目ID: 任务ID }
  "success": {},   // 任务完成后解锁的商品
  "fail": {}       // 任务失败后解锁的商品
}
```

不涉及任务时可留空 `{}`。FlanrecGents 的该文件即三个空对象。

---

### 3.6 `items/*.json` —— 自定义物品

一个文件定义一个物品，结构为 `{ item, origin, type }`：

```jsonc
{
  "item": {                          // ★ 完整物品定义（与游戏 items.json 同构）
    "_id": "9cc236080000000000000000",       // ★ 物品 ID（MongoId，全局唯一）
    "_name": "FG-Endurance1",                // 内部名
    "_parent": "5448f3a64bdc2d60728b456a",   // ★ 物品父分类（5448f3a6… = 医疗用品）
    "_type": "Item",
    "_props": {                      // ★ 属性（可只写要覆盖的字段）
      "Name": "FG-Endurance1",       //   内部名称（显示名见 locales/itemsdescription.json）
      "ShortName": "FG-Endurance1",
      "Weight": 0.05,                //   重量
      "BackgroundColor": "orange",   //   背景色
      "Width": 1, "Height": 1,       //   占格
      "StackMaxSize": 1,
      "ItemSound": "med_stimulator",
      "Prefab": { "path": "assets/content/weapons/usable_items/item_syringe/item_stimulator_sj6_loot.bundle", "rcid": "" },      // 掉落模型
      "UsePrefab": { "path": "assets/content/weapons/usable_items/item_syringe/item_stimulator_sj6_container.bundle", "rcid": "" }, // 使用模型
      "ExaminedByDefault": false,
      "CanSellOnRagfair": true,
      "StimulatorBuffs": "Buffs_FG-Endurance1", // ★ 针剂效果名（对应 globals.json 里的 Buff 组）
      "effects_health": [],          //   健康效果
      "effects_damage": {}           //   伤害效果（止血/止痛等，见 FG-Zagustin.json 示例）
      // ……其余属性可参照原版同类物品
    },
    "_proto": "544fb3f34bdc2d03748b456a" // ★ 原型物品 ID（544fb3f3… = 刺激剂），未写的属性继承自它
  },
  "origin": "5c0e531d86f7747fa23f4d42", // 可选：参考的原版物品 ID（MongoId 格式才生效），用于继承其插槽/过滤器兼容性
  "type": []                             // 可选：预留字段，示例均为空数组
}
```

要点：

- **`_proto`（原型）**：新物品未在 `_props` 中声明的属性，全部从原型物品继承——想快速做"某原版物品的魔改版"，把 `_proto` 指向它、`_props` 只写要改的字段即可。
- **`_id` 冲突**：若与游戏已有物品重复，加载会提示 `独立商人物品【id:xxx】已存在，请酌情修改id，本次不执行添加操作。`
- **`origin`**：参考的原版物品 ID（需 MongoId 格式）。填写后，代码会把新物品自动加入"所有允许该原版物品的插槽过滤器"（弹匣、枪械槽位等），即"新物品能装进原版物品能装的地方"；**留空**则表明这是一个全新的自定义物品，各种属性（含模型文件等）全部由作者自定义设置。
- **针剂类物品**（`_proto` 为刺激剂）：`_props.StimulatorBuffs` 指定的名字必须与 `globals.json` 中的 Buff 组名一致，否则针剂没有效果。

参考：MarecGents 的 `items/MG-Propital.json`（止痛针，带 `effects_damage.Pain`）、FlanrecGents 的 `items/FG-Zagustin.json`（止血剂，带 `LightBleeding`/`HeavyBleeding` 治愈效果）。

---

### 3.7 `globals.json` —— 针剂 Buff 与物品预设

```jsonc
{
  "ItemPresets": { },                // 可选：物品预设（武器改装配件组合），一般留空
  "Buffs": {                         // ★ 针剂效果组（键名 = items/*.json 里的 StimulatorBuffs）
    "Buffs_FG-Endurance1": [         // 组名必须与物品引用一致，且全局唯一
      {
        "AbsoluteValue": true,       // 是否为绝对值（false 则为百分比）
        "BuffType": "MaxStamina",    // 效果类型（MaxStamina/StaminaRate/SkillRate/HealthRate/EnergyRate/HydrationRate/RemoveAllBloodLosses…）
        "Chance": 1,                 // 触发概率（0~1）
        "Delay": 1,                  // 延迟（秒）
        "Duration": 300,             // 持续时间（秒）
        "SkillName": "",             // BuffType 为 SkillRate 时的技能名（如 "Strength"）
        "Value": 50                  // 数值
      }
      // ……一个组可放多个 Buff（正面/负面副作用都行，参考 FG-Endurance1.json 的 10 条）
    ],
    "Buffs_FGZagustin": [ /* … */ ]
  }
}
```

要点：

- **Buff 组名全局唯一**：重复时提示 `针剂Buff名称：xxx重复！请更换其他Buff名称.`
- `SkillRate` 需配合 `SkillName` 指定技能（如 `Strength`/`Vitality`/`Health`/`Metabolism`）；
- 想加"副作用"就在同一组里加 `Chance` 较低、`Value` 为负的条目（参考 FlanrecGents 的 `HealthRate:-1`）。

---

### 3.8 `locales/itemsdescription.json` —— 物品名称与描述

```jsonc
{
  "9cc236080000000000000000": {      // ★ 键 = items/*.json 里的物品 _id
    "Name": "FG耐力针",              // 游戏内名称
    "ShortName": "耐力针",           // 简称
    "Description": "FG趁MG不在实验室的时候偷偷从试验台上拿走的半成品……"
  },
  "9cc236080000000000000001": {
    "Name": "FG止血剂",
    "ShortName": "止血剂",
    "Description": "FG蛮横不讲理般地向MG索要来的……"
  }
}
```

> 只对**自定义物品**写即可；原版物品已有文本。键写错（与物品 `_id` 不一致）会导致游戏内显示为空/英文占位。

---

### 3.9 `locales/mail.json` —— 任务/邮件文本

键 = 任务 `_id`，值 = 任务文本（`QuestDesc`）：

```jsonc
{
  "8ef5b2ef4000000000000000": {      // ★ 键 = templates/quests.json 里的任务 _id
    "name": "任务名",
    "description": "任务描述",
    "startedMessageText": "接取时的消息",
    "successMessageText": "完成时的消息",
    "failMessageText": "失败时的消息",
    "changeQuestMessageText": "改任务时的消息",
    "acceptPlayerMessage": "接受后发给玩家的消息",
    "completePlayerMessage": "完成后发给玩家的消息",
    "declinePlayerMessage": "拒绝时的消息"
    // "other": { "其他本地化键": "文本" }  // 可选，额外键直接写入本地化表
  }
}
```

FlanrecGents 未用任务，该文件为空 `{}`；MarecGents 有完整示例（见其 `locales/mail.json`）。

---

### 3.10 `templates/handbook.json` —— 跳蚤分类条目

```jsonc
[
  {
    "Id": "9cc236080000000000000000",        // ★ 物品 ID
    "ParentId": "5b47574386f77428ca22b33a",  // ★ 跳蚤分类 ID（5b475743…b33a = 医药）
    "Price": 50000                            // ★ 价格（卢布）
  },
  { "Id": "9cc236080000000000000001", "ParentId": "5b47574386f77428ca22b33a", "Price": 50000 }
]
```

- 自定义物品若要**能在跳蚤出售/有参考价**，应添加跳蚤分类条目；
- `ParentId` 决定物品在跳蚤分类中的归属（医疗/弹药/枪械等），可参考原版同类物品的跳蚤分类 ID（MarecGents 中任务箱用的分类是 `5b47574386f77428ca22b345`）。

> ⚠️ **「跳蚤上架」与 handbook 的因果关系（重要）**：物品能否在跳蚤市场挂单出售，
> **由是否注册了 handbook 条目决定**（客户端图鉴/上架检查依赖它）——只把物品加进 `assort.json` 出售、
> 却忘了 handbook 条目，玩家**无法在跳蚤上架**这件物品（甚至图鉴里也不显示）。
> 所以：**凡是你打算让玩家能在跳蚤买卖的自定义物品，都要在 `templates/handbook.json` 里加一条**。
> 注意这里说的"出售"指**给玩家的购买渠道**（assort 决定"商人卖不卖"，handbook 决定"跳蚤能不能上架/图鉴有没有"），两者都要配齐。

---

### 3.11 `templates/quests.json` —— 任务定义 ⭐⭐

键 = 任务 `_id`，值 = 完整任务对象（结构与原版 `quests.json` 一致）：

> ⚠️ 任务对象必须**严格参照原版 `quests.json` 的任务格式**，缺失字段会导致报错。

```jsonc
{
  "8ef5b2ef4000000000000000": {      // ★ 任务 ID
    "QuestName": "TestQuest1",
    "_id": "8ef5b2ef4000000000000000",
    "traderId": "8ef5b2eff000000000000000", // ★ 所属商人 _id（必须 = traderInfo.json 的 _id）
    "image": "/files/quest/icon/MGQuest1.jpg", // 任务图标（对应 images/quests/ 下的文件）
    "location": "Any",
    "description": "任务描述",
    "conditions": {                  // 任务条件
      "AvailableForStart": [],
      "AvailableForFinish": [ /* 击杀/上交物品等条件 */ ],
      "Fail": []
    },
    "rewards": {                     // 奖励
      "Started": [],
      "Success": [ /* 物品/好感/技能/经验奖励 */ ],
      "Fail": []
    },
    "side": "Pmc",
    "type": "Elimination",
    // ……其余字段参照原版任务结构
  }
}
```

> 完整可抄的示例见 MarecGents 的 `MGModServer/traders/MarecGents/templates/quests.json`
> （`TestQuest1`：击杀 1 人 + 上交 3 个 `6275303a9f372d6ea97f9ec7` + 10 个 `5c0e874186f7745dc7616606`，奖励物品/好感/技能）。
> 任务文本放 `locales/mail.json`，任务图标放 `images/quests/`（会被注册为 `/files/quest/icon/文件名`）。
>
> 📖 **任务系统完整写法（击杀/收集/上交条件、奖励带子物品、文本注册、任务解锁商品、任务链）见 4.3 节（⭐⭐ 进阶）**。

---

### 3.12 `location/looseLoot.json` —— 地图散落战利品

```jsonc
{ }
```

当前框架中 `AddTraderLocationToDB` 为**空实现**（尚未启用），此文件放什么都不会生效，留空即可。MG-Mod 的 MarecGents 模板中该文件虽有内容，但同样未被加载逻辑使用。

---

### 3.13 `bundles.json` —— 资源包清单

```jsonc
{ "manifest": [] }
```

- **用途**：放置商人作者的**皮肤、贴图、模型**等资源文件信息；对应的皮肤/贴图/新模型文件需放到 **mod 根目录下的 `bundles/` 文件夹**中；
- **说明**：这部分 MG 作者并不熟悉，但制作商人皮肤的作者们很清楚该如何编写（按常规 bundle 清单格式填写即可）；
- 没有自定义资源包时保持 `{ "manifest": [] }`；
- 若商人需要加载自定义 bundle（自定义模型/贴图），在此声明并放入 `bundles/`；
- 注意：mod 根目录的 `bundles.json` 会在启动时**被代码用各商人的 manifest 重写**，不要手工维护它。

---

### 3.14 `res/services/TraderDB/base.json` —— 商人基础模板（了解即可）

所有商人共用的"骨架"，代码加载 `traderInfo.json` 后将其字段覆盖进 `base.json` 对应位置（ID/名称/位置/保险/修理/忠诚度/折扣等），生成完整商人对象。内容包括：

- 商人余额（`balance_rub/dol/eur`）、货币（`currency: "RUB"`）、`gridHeight`（商店界面格子高度）；
- `items_buy`（收购分类白名单）、`items_buy_prohibited`（禁止收购）、`items_sell`（按等级出售分类）、`sell_category`（可出售分类大全）——决定商人"收什么、卖什么"的底子；
- 默认 `loyaltyLevels`、`insurance`、`repair`、`unlockedByDefault` 等。

一般**不需要修改**。若想改变"所有商人共有的收购/出售底子"，可以在这里改。

---

## 四、撰写示例 ⭐

### 4.1 方案一：复制 FlanrecGents 模板改造（新手推荐）

以"新建商人 **MyTrader**（卖一把魔改 AK）"为例：

**步骤 1：复制模板文件夹**

```
复制 traders/FlanrecGents/ → traders/MyTrader/
（不要复制 items/ 和 locales/ 中 FG 的物品，除非你要保留它们）
```

> ⚠️⚠️ **三遍强调：不要"复制 FG 商人然后只改个名字"就完事！**
> 1. **商人 `_id`、物品 `_id`、任务 `_id`、Buff 组名必须全部换成你自己的**（用你的独特前缀，见 3.2 的 💡 提示）。
>    示例里所有 ID（`9cc23608...`、`8ef5b2ef...`）都是作者（FG/MG）的专属前缀，
>    直接复制会与作者发布的 Mod **ID 冲突**，加载时直接报错跳过（`已存在于游戏中` / `已存在，不执行添加`）。
> 2. **头像文件名**必须改成「你的 `_id`.jpg」（见步骤 3），否则显示旧商人头像。
> 3. 复制时**只保留你要的文件**：不要的 FG 物品、任务、图片一并删掉，避免残留无用数据。

**步骤 2：改 `traderInfo.json`**

- 生成你自己的 24 位 MongoId（建议用你的独特前缀开头，见 3.2 的 💡 提示），填入 `_id`；
- `name` 改为 `MyTrader`；
- `locales` 改为你自己的名字/描述/位置；
- 按需调整保险、修理、忠诚度、`items_buy` 收购。

**步骤 3：换头像**

把新头像图片命名为 `你的_id.jpg`，替换文件夹根目录的旧 jpg（删掉旧的 FG 头像）。

**步骤 4：定义物品（可选）**

在 `items/` 下新建 `MyAK.json`：

```jsonc
{
  "item": {
    "_id": "你生成的新物品ID(24位MongoId)",
    "_name": "MyAK",
    "_parent": "5447e0e74bdc2d3c308b4567",   // 突击步枪分类
    "_type": "Item",
    "_props": { "Name": "MyAK", "ShortName": "MyAK", "Weight": 3.2 },
    "_proto": "5447a9cd4bdc2dbd668b4567"      // 原型：AK-74N
  },
  "origin": "5447a9cd4bdc2dbd668b4567",
  "type": []
}
```

> 💡 示例中的原版物品 ID（`_parent`/`_proto`/`origin`）请**以你服务端 SPT 数据库中的实际 ID 为准**
> （如 `AK-74N` 的 `5447a9cd4bdc2dbd668b4567` 在部分版本可能不同）。可在 SPT 安装目录的
> 数据库文件（`SPT_Data/Server/database/templates/items.json`）或 Mod 开发工具中查询确认。

**步骤 5：写本地化**

在 `locales/itemsdescription.json` 里给新物品 ID 加名称/描述。

**步骤 6：上架商品**

在 `traderData/assort.json` 的 `items` 加一条（`_tpl` = 新物品 `_id`），
在 `barter_scheme` 与 `loyal_level_items` 里加对应条目。

**步骤 7：加跳蚤分类条目与 Buff（按需）**

- `templates/handbook.json` 加 `{ "Id": 新物品ID, "ParentId": 枪械分类ID, "Price": 价格 }`；
- 若物品是针剂且 `_props.StimulatorBuffs` 有值，在 `globals.json` 的 `Buffs` 里加同名效果组。

**步骤 8：重启服务端验证**

启动后应看到日志 `商人【MyTrader】已添加。`。进游戏确认商人出现、商品正确、物品显示正常。

> 完成以上即"添加商人"全流程。**全程不需要编译**——文件在 `user/mods/MGGTMod/traders/` 下改完重启即可。

### 4.2 方案二：从零写一个最小商人（MyTrader 完整 JSON）

按 3.1 的结构，最小可行集合 = `traderInfo.json` + 头像 + `traderData/assort.json`（+ `globals.json`）。下面给出一套可直接抄的最小模板（卖原版物品 5 万卢布）：

`traders/MyTrader/traderInfo.json`：

```jsonc
{
  "enable": true,
  "name": "MyTrader",
  "_id": "a1b2c3d4e5f60718293a4b5c",   // ← 换成你自己的 24 位 MongoId
  "locales": {
    "FullName": "我的商人",
    "FirstName": "我的商人",
    "Nickname": "我的商人",
    "Location": "我的实验室",
    "Description": "这是我制作的第一个商人。"
  },
  "insurance": { "enable": false, "minreturnTime": 0, "maxreturnTime": 0, "pay": 0, "chance": 100, "storageTime": 100, "Message": {} },
  "repair": { "enable": false, "coefficient": 1, "quality": 3 },
  "loyaltyLevels": {
    "description": { "main": "", "minLevel": "", "minSalesSum": "", "minStanding": "", "buy_price_coef": "", "repair_price_coef": "", "insurance_price_coef": "", "exchange_price_coef": "", "heal_price_coef": "" },
    "range": [
      { "minLevel": 1, "minSalesSum": 0, "minStanding": 0, "buy_price_coef": 0, "repair_price_coef": -50, "insurance_price_coef": 0, "exchange_price_coef": 0, "heal_price_coef": 100 },
      { "minLevel": 1, "minSalesSum": 0, "minStanding": 0, "buy_price_coef": 0, "repair_price_coef": -50, "insurance_price_coef": 0, "exchange_price_coef": 0, "heal_price_coef": 100 },
      { "minLevel": 1, "minSalesSum": 0, "minStanding": 0, "buy_price_coef": 0, "repair_price_coef": -50, "insurance_price_coef": 0, "exchange_price_coef": 0, "heal_price_coef": 100 },
      { "minLevel": 1, "minSalesSum": 0, "minStanding": 0, "buy_price_coef": 0, "repair_price_coef": -50, "insurance_price_coef": 0, "exchange_price_coef": 0, "heal_price_coef": 100 }
    ]
  },
  "discount": 0,
  "medic": false,
  "updateTime": { "min": 600, "max": 600 },
  "unlockedDefault": true,
  "items_buy": {
    "category": [
      "5448e5284bdc2dcb718b4567",
      "5448e54d4bdc2dcc718b4568",
      "5448f3a64bdc2d60728b456a",
      "65649eb40bf0ed77b8044453",
      "5447b5cf4bdc2d27728b4568",
      "5485a8684bdc2da71d8b4567",
      "5448bf274bdc2dfc2f8b456a"
    ],
    "id_list": []
  }
}
```

`traders/MyTrader/traderData/assort.json`（出售 100 发 5.45 弹药 `54527ac44bdc2d36668b4567`，5 万卢布/发）：

```jsonc
{
  "items": [
    { "_id": "MyTrader_assort_01", "_tpl": "54527ac44bdc2d36668b4567", "parentId": "hideout", "slotId": "hideout", "upd": { "UnlimitedCount": true, "StackObjectsCount": 9999999 } }
  ],
  "barter_scheme": {
    "MyTrader_assort_01": [ [ { "count": 50000, "_tpl": "5449016a4bdc2d6f028b456f" } ] ]
  },
  "loyal_level_items": { "MyTrader_assort_01": 1 }
}
```

`traders/MyTrader/globals.json`：

```jsonc
{ "ItemPresets": {}, "Buffs": {} }
```

`traders/MyTrader/bundles.json`：

```jsonc
{ "manifest": [] }
```

头像：`traders/MyTrader/a1b2c3d4e5f60718293a4b5c.jpg`。

重启服务端 → 日志出现 `商人【MyTrader】已添加。` 即成功。

### 4.3 进阶：任务系统（完整教程）⭐⭐

> 本节教你在商人上加**任务链**：玩家接任务 → 完成（击杀/收集/上交）→ 领奖励 → 解锁商品。
> 涉及文件：`templates/quests.json`（任务定义）+ `locales/mail.json`（任务文本）+ `traderData/questassort.json`（任务解锁商品）+ `images/quests/`（任务图标）。
> 完整可抄示例：`MGModServer/traders/MarecGents/`（任务 + 奖励 + 解锁全套）。

#### 4.3.1 任务系统全景

| 能力 | 文件 | 说明 |
|------|------|------|
| 任务定义 | `templates/quests.json` | 完整任务对象（条件 `conditions` / 奖励 `rewards`） |
| 任务文本 | `locales/mail.json` | 任务名、描述、各状态消息 |
| 任务图标 | `images/quests/*.jpg` | 自动注册为 `/files/quest/icon/文件名` |
| 任务解锁商品 | `traderData/questassort.json` | `started/success/fail` 三阶段解锁商品 |
| 商人加入重复任务池 | 代码自动 | 加载商人时自动把 `Nickname` 加入重复任务白名单 |
| 多任务串行 | `quests.json` 的 `AvailableForStart` | 前置任务完成后才解锁下一任务 |

一个任务的生命周期：

```
接取（AcceptQuest）→ 状态 Started → 完成条件达成 → 领取奖励（CompleteQuest）→ 任务结束
     │                        │                          │
     └─ questassort.started   └─ questassort.success     └─ rewards.Success
        （接取即解锁的商品）      （完成后解锁的商品）        （完成任务发放的奖励）
```

#### 4.3.2 任务定义 `templates/quests.json`

键 = 任务 `_id`（**必须用你自己的前缀**，全局唯一），值 = 完整任务对象。最小结构：

```jsonc
{
  "你的前缀0000000000000001": {        // ★ 任务 _id（MongoId，全局唯一）
    "QuestName": "MyFirstQuest",       // 任务逻辑名（自定义）
    "_id": "你的前缀0000000000000001",  // 与键一致
    "traderId": "你的商人_id",          // ★ 所属商人（必须 = traderInfo.json 的 _id）
    "image": "/files/quest/icon/MyQuest1.jpg",  // 任务图标（对应 images/quests/ 下文件名）
    "location": "Any",                // 任务展示用地图（地图 id，可 "Any"）
    "description": "任务描述",
    "conditions": {                   // ★ 任务条件（见 4.3.3）
      "AvailableForStart": [],        //   接取前置条件（空 = 可直接接）
      "AvailableForFinish": [ /* 完成条件 */ ],
      "Fail": []
    },
    "rewards": {                      // ★ 奖励（见 4.3.5）
      "Started": [],                  //   接取即发（如送道具）
      "Success": [ /* 完成奖励 */ ],
      "Fail": []
    },
    "side": "Pmc",
    "type": "PickUp",                 // 任务类型（PickUp=收集类 / Elimination=击杀类）
    // ……其余字段参照原版任务结构（isKey/restartable/secretQuest 等，可抄 MarecGents）
  }
}
```

> ⚠️ 任务对象**必须严格参照原版 `quests.json` 格式**，缺失字段会导致加载报错。最稳妥：复制 MarecGents 的完整任务对象再改。

#### 4.3.3 完成条件：击杀 / 收集 / 上交

`AvailableForFinish` 是**条件数组**，可混用多种条件（任务要求同时满足全部）：

**① 收集/上交物品（简单，⭐）**

```jsonc
{
  "conditionType": "FindItem",          // 找到并持有（在战局里找到）
  "target": ["原版或自定义物品_id"],      // 可多个（满足其一）
  "value": 5,                            // 数量
  "id": "你的前缀...0001"                // 条件 id（唯一）
},
{
  "conditionType": "HandoverItem",       // 上交（交给商人）
  "target": ["物品_id"],
  "value": 5,
  "id": "你的前缀...0002"
}
```

**② 击杀（⭐⭐，最容易写错！）**

击杀条件由**外层 `Elimination` + `counter` 子条件**组成，结构固定：

```jsonc
{
  "completeInSeconds": 0,
  "conditionType": "CounterCreator",     // 固定
  "type": "Elimination",                 // 固定
  "value": 5,                            // ★ 击杀总数（5 个）
  "counter": {
    "id": "你的前缀...0003",             // counter id（唯一）
    "conditions": [
      {
        "conditionType": "Kills",
        "target": "Savage",              // ★ 击杀目标（见下方 target 表）
        "savageRole": [],                //   精确到 bot 类型（可选，见下方）
        "value": 1,                      // ★★ 必须 = 1！！（见下方红色警告）
        "id": "你的前缀...0004"
      },
      {
        "conditionType": "Location",     // 可选：限制击杀地图
        "target": ["laboratory"],        //   地图名（laboratory/Lighthouse/woods…）
        "id": "你的前缀...0005"
      }
    ]
  },
  "id": "你的前缀...0006"                // 外层条件 id（唯一）
}
```

> 🔴🔴 **`Kills` 子条件的 `value` 必须 = 1，千万不能填成击杀总数！**
> 客户端判定是 `1 >= Kills.value`（每次击杀按 1 计数），写成 5/10 会**永远不命中、任务永远无法完成**。
> **击杀总数只写在外层 `Elimination` 的 `value`**（上面的例子：外层 5 + Kills 1 = 杀 5 个）。
> 这是本项目踩过的最深的坑（MG 商人《初次见面》曾因此击杀无效）。

**`target` 填什么（击杀对象）**：

| target | 击杀对象 | 说明 |
|--------|----------|------|
| `"Savage"` | scav 阵营全体 | 普通 scav、掠夺者（pmcBot）、游荡者（exUsec）都算 |
| `"AnyPmc"` | 任意 PMC | usec + bear 都算，最常用 |
| `"Usec"` / `"Bear"` | 指定阵营 PMC | 只算对应阵营 |
| `"Any"` | 任意目标 | 什么都能算 |

**精确到某类 bot（可选）**：在 `savageRole` 里填 bot 的**内部类型名**（大小写敏感），
如 `["pmcBot"]`（实验室掠夺者）、`["exUsec"]`（灯塔游荡者 Rogue）、`["bossKilla"]`（Killa）——
与 `target:"Savage"` 搭配使用，只统计该类型的 Savage 阵营 bot。
> ⚠️ 游荡者（exUsec）和掠夺者（pmcBot）**不是 PMC**，击杀标签是 `"Savage"` 不是 `"Usec"`，
> 想"杀游荡者"必须 `target:"Savage" + savageRole:["exUsec"]`（+ `Location:["Lighthouse"]`），
> 写成 `target:"Usec"` 只会算 AI PMC，杀游荡者不计。

**限制地图**：在 `counter.conditions` 里加一个 `conditionType:"Location"` 子条件，
`target` 填**地图名**（`laboratory`/`Lighthouse`/`woods`/`interchange`…）。
> ⚠️ 任务级 `location` 字段只是**展示用**（填地图 id），**不限制击杀计数**；限制击杀地点必须靠 `Location` 子条件。

#### 4.3.4 任务文本注册 `locales/mail.json`

任务的显示文本（任务名、描述、消息）**必须注册**，否则游戏里显示空/英文占位。键 = 任务 `_id`：

```jsonc
{
  "你的前缀0000000000000001": {
    "name": "《我的第一个任务》",
    "description": "任务描述……（可含 <color=#ff0000><b>红字高亮</b></color>）",
    "startedMessageText": "接取时弹出的消息",
    "successMessageText": "完成时弹出的消息",
    "failMessageText": "失败消息",
    "changeQuestMessageText": "",
    "acceptPlayerMessage": "",
    "completePlayerMessage": "",
    "declinePlayerMessage": "",
    "other": {
      "你的前缀...0001": "<color=#ff0000><b>在战局中找到5件xxx</b></color>",
      "你的前缀...0002": "<color=#ff0000><b>上交5件xxx</b></color>",
      "你的前缀...0006": "<color=#ff0000><b>在任意地点击杀5名scav</b></color>"
    }
  }
}
```

> ⚠️ **条件文本 key 填什么**：`other` 里的键 = **`quests.json` 里每个条件的 `id`**！
> 客户端显示条件时按 `条件id.Localized()` 查文本——**新加的条件（尤其击杀条件）必须同步在这里注册**，
> 否则任务详情里该条件显示空白。收集/上交/击杀条件都要注册（击杀注册**外层** Elimination 条件的 id，不是 Kills 子条件 id）。

#### 4.3.5 奖励：物品 / 好感 / 经验 + 带子物品

`rewards.Success`（完成奖励）可放多种奖励：

```jsonc
"Success": [
  {
    "id": "你的前缀...0010",
    "type": "Experience",            // 经验
    "value": 10000
  },
  {
    "id": "你的前缀...0011",
    "type": "TraderStanding",        // 好感
    "value": "0.2",
    "target": "你的商人_id"
  },
  {
    "id": "你的前缀...0012",
    "type": "Item",                  // 物品
    "items": [
      { "_id": "你的前缀...0013", "_tpl": "你的物品_id", "upd": { "StackObjectsCount": 1 } }
    ],
    "target": "你的前缀...0013",
    "value": 1
  }
]
```

**奖励物品带子物品（进阶）**：想给一件护甲/弹挂甲"出厂自带插板、软甲"，
在 `items` 数组里给主物品挂子物品（`parentId` = 主物品 `_id`，`slotId` = 槽位名）：

```jsonc
"items": [
  { "_id": "主物品_id", "_tpl": "护甲_id", "upd": { "StackObjectsCount": 1 } },
  { "_id": "子物品1_id", "_tpl": "插板_id",  "parentId": "主物品_id", "slotId": "Front_plate" },
  { "_id": "子物品2_id", "_tpl": "软甲_id",  "parentId": "主物品_id", "slotId": "Soft_armor_front" }
  // ……一个槽位一条；槽位名必须是该物品 Slots 里真实存在的（Front_plate/Back_plate/Soft_armor_front 等）
]
```

> ⚠️ 子物品的 `slotId` 必须匹配物品**真实槽位名**、`_tpl` 必须能装进该槽（槽位过滤器），
> 否则装备不完整/装不上。参考 MarecGents 的护甲奖励（插板 SAPI/SSAPI + 芳纶软甲全套）。

**接取即送**：把物品放 `rewards.Started`（接取任务立刻发放，界面显示橙色背景），如 MarecGents 的《初次见面》送弹挂。

#### 4.3.6 任务解锁商品 `traderData/questassort.json`

任务完成后（或接取时）解锁某商品购买权。键值方向：**商品条目 id → 任务 id**：

```jsonc
{
  "started": {},                      // 接取任务后解锁的商品
  "success": {
    "MyTrader_assort_02": "你的前缀0000000000000001"  // ★ 商品条目id → 任务id
  },
  "fail": {}
}
```

> ⚠️ 键值方向别写反：**左边 = `assort.json` 里的商品条目 `_id`，右边 = `quests.json` 的任务 `_id`**。
> 写反会报 `KeyNotFoundException` 导致服务端启动崩溃。
> 商品条目 id 用普通字符串即可（加载时自动重写为 MongoId 并同步替换此处引用，见 3.4）。

#### 4.3.7 任务图标与前置任务链

- **图标**：`images/quests/你的图.jpg` → `quests.json` 的 `image` 写 `/files/quest/icon/你的图.jpg`；
- **前置任务链**：后一个任务的 `AvailableForStart` 加：

```jsonc
"AvailableForStart": [
  {
    "conditionType": "Quest",
    "target": "前置任务_id",
    "status": [4, 5],                // 4=已完成 5=失败可重接（照抄即可）
    "id": "你的前缀...0020"
  }
]
```

这样玩家必须完成前置任务才能接后一个——三任务串行任务链就是这么做的（FlanrecGents 三任务互锁 + 任务解锁装备购买）。

---


## 五、自编译发布流程 ⭐⭐⭐

### 5.1 什么时候需要自编译

- 你想**基于 MGGTMod 做自己的 Mod**，改名打包发布给别人；
- 你修改了 `types/` 下的 C# 代码（框架本身），需要重新编译。

> 只加商人（改 JSON）不需要编译，跳过本章。

### 5.2 前置条件

- **.NET 10 SDK**（`dotnet --list-sdks` 确认）；
- IDE（Visual Studio 2022 / JetBrains Rider）或直接用命令行；
- 首次构建需联网还原 NuGet 包：`SPTarkov.Common`、`SPTarkov.DI`、`SPTarkov.Server.Core`（版本 4.1.2）。

### 5.3 改名清单（防止与原版冲突）

> ⚠️ **重要提示**：以下"同名冲突"**未经实际测试**，属于预防性建议。
> 原版作者自己就是这样区分两个 Mod 的（MGMod 用 `MGMod.dll`/`_MGMod`/类 `MGmod`，
> MGGTMod 用 `MGGTMod.dll`/`_MGGTmod`/类 `MGGTmod`，二者可共存），
> 说明**改名是作者既有的防冲突实践**。为避免风险，发布前建议全部替换。

| # | 位置 | 原值 | 建议改为 | 说明 |
|---|------|------|----------|------|
| 1 | 项目文件夹名 / 解决方案 | `MGGTMod` / `MGModSeries-CSharp.slnx` | 你的 Mod 名 | 整合仓内项目目录名（不影响运行，但利于管理） |
| 2 | `csproj` → 项目名（文件名） | `MGGTMod.csproj` | `MyTraderMod.csproj` | 决定 `AssemblyName`（dll 名） |
| 3 | `csproj` → `<RootNamespace>` | `_MGGTMod` | `_MyTraderMod` | C# 根命名空间（源码文件实际用 `_MGGTmod.*`） |
| 4 | `csproj` → `<Version>` | `0.5.0.040102` | `0.1.0`（你的版本） | 版本号（SemVer 前三位 + 构建号） |
| 5 | 全部 `.cs` → `namespace` / `using` | `_MGGTmod` | `_MyTraderMod` | **全局替换**（`types/` 下所有文件 + 入口） |
| 6 | `MGGTmod.cs` → 主类名 | `MGGTmod` | `MyTraderMod` | `[Injectable]` 注册的加载器类 |
| 7 | `MGGTmod.cs` → `ModMetadata.ModGuid` | `com.marecgents.tarkovmod.mggtmod` | `com.你的域名.你的mod名` | 唯一 GUID，建议改 |
| 8 | `MGGTmod.cs` → `ModMetadata.Name` | `MGGTMod` | `MyTraderMod` | Mod 显示名 |
| 9 | `MGGTmod.cs` → `ModMetadata.Author` | `MarecGents` | 你的名字 | |
| 10 | `MGGTmod.cs` → `ModMetadata.Version` | `0.5.0` | 你的版本 | |
| 11 | `MGGTmod.cs` → `ModMetadata.Url` / `License` | 作者信息 | 你的信息（可选） | |
| 12 | `MGUtils.cs` → 日志前缀（可选） | `[MG通用商人框架]` | 你的框架名 | 纯显示用 |

> **整合仓构建注意**：`MGGTMod.csproj` 的 `OutputPath` 写死为 `$(MGModModsOutputDir)MGGTMod\`（由根目录 `Directory.Build.props` 的 `MGModModsOutputDir` 决定 = `Build\SPT_Runtime\user\mods\`）。改名 Mod 后需把其中 `MGGTMod\` 同步改为 `<你的Mod名>\`，输出仍统一落在 `Build\SPT_Runtime\user\mods\` 下。

> **为什么改这些**：SPT 按 `user/mods/` 下各 Mod 的程序集（dll）与 DI 注册类型加载。
> 若你的 Mod 与原版**dll 同名、主类同名、命名空间同名、`ModMetadata.Name/ModGuid` 相同**，
> 可能造成程序集/元数据/DI 注册互相干扰。**商人 `_id`、物品 `_id`、Buff 组名**则不同——
> 这三类是**运行时必定检测并明确报错**的（见 3.2/3.6/3.7），务必改。
> ⚠️ 上述"可能干扰"未经实测，最稳妥做法：按表全部改掉。

### 5.4 编译命令

在 `MGModSeries-CSharp/`（整合仓根目录）下执行：

```powershell
dotnet build MGGTMod/MGGTMod.csproj -c Release
```

也可以一键构建整个整合仓（四项目）：

```powershell
dotnet build MGModSeries-CSharp.slnx -c Release
```

或使用交互式构建菜单：`scripts/build-menu.cmd -Run mggtmod`（用 IDE 时：选择 `Release` 配置 → 生成解决方案）。

### 5.5 输出与部署

输出路径由根目录 `Directory.Build.props` 的 `MGModModsOutputDir` 决定（各项目 csproj 的 `OutputPath` 引用它），MGGTMod 输出到：

```
Build\SPT_Runtime\user\mods\MGGTMod\
├── MGGTMod.dll            ← ★ 编译出的 Mod 程序集
├── MGGTMod.pdb
├── MGGTMod.deps.json
├── bundles.json           ← 资源包清单（启动时被重写）
├── bundles/               ← 资源包（含 FG Rig Layouts bundle）
├── res/                   ← 资源（TraderDB 等）
└── traders/               ← ★ 你的商人配置（可后续直接改）
```

部署步骤：

1. 将 `Build\SPT_Runtime\user\mods\MGGTMod\` 整个文件夹（或其内容）复制到服务端的 `user/mods/<你的Mod名>/`；
2. 启动服务端，看到 `[MG通用商人框架][商人系统]：加载完毕。` 及 `商人【X】已添加。` 即成功；
3. 以后只改商人 JSON 时，直接改 `user/mods/<你的Mod名>/traders/` 下的文件并重启即可，无需重新编译；
4. **（可选）** 后续想改商人默认内容，回到源码 `MGGTMod/traders/` 修改后重新 build。

> 部署结构参考 `docs/MGGTMod-README.md`：`将 MGGTMod 文件夹放入 user/mods/ 目录`。

### 5.6 常见问题速查

| 现象 | 原因 | 解决 |
|------|------|------|
| 日志 `商人【X】不存在配置文件"traderInfo.json"` | 文件夹里缺 `traderInfo.json` | 补上，或删除该文件夹 |
| 日志 `商人【X】的Id:xxx已存在于游戏中,请修改.` | `_id` 与已有商人重复 | 换新的 24 位 MongoId |
| 日志 `商人【X】已添加。` 但游戏里没有 | `enable=false`；或头像缺失只影响显示 | 检查 `enable`；`_id` 是否 24 位 MongoId |
| 日志 `独立商人物品【id:xxx】已存在…不执行添加` | 物品 `_id` 与已有物品重复 | 换物品 ID |
| 日志 `针剂Buff名称：xxx重复！` | `globals.json` 的 Buff 组名与别处重复 | 换 Buff 组名，并同步改物品的 `StimulatorBuffs` |
| 游戏内物品没名字/描述 | `locales/itemsdescription.json` 的键与物品 `_id` 不一致 | 对齐键名 |
| 商品在商人界面不显示 | `assort.json` 的 `_tpl` 物品 ID 不存在 | 确认 `_tpl` 是原版物品 ID 或自定义物品 `_id` |
| 针剂没效果 | `StimulatorBuffs` 与 `globals.json` Buff 组名不一致 | 对齐名字 |
| 任务图标 404 | `images/quests/` 文件名与 `quests.json` 的 `image` 不一致 | 对齐文件名 |
| 击杀条件不计数（任务无法完成） | `Kills` 子条件 `value` 填成了击杀总数（应恒为 1） | 把 Kills 子条件 `value` 改回 1，总数只写外层 Elimination 的 `value`（见 4.3.3） |
| 玩家无法把物品卖给你商人 | `traderInfo.json` 缺 `items_buy` 或大类不在 `category` | 配 `items_buy.category`（见 3.2） |
| 自定义物品无法在跳蚤上架 | 缺 `templates/handbook.json` 条目 | 补 handbook 条目（见 3.10） |
| 启动报 dll/程序集相关错误 | 与其它 Mod 同名 dll / 同名类型冲突（未实测） | 按 5.3 改名清单全部替换后重新编译 |

---

## 附：本教程依据

- `MGModSeries-CSharp/MGGTMod/` 源码（入口 `MGGTmod.cs`、核心服务 `types/services/CustomTraderServices.cs`、路径常量 `types/models/Paths/Paths.cs`）；
- 内置示例商人 `MGGTMod/traders/FlanrecGents/`（最小完整模板）；
- `MGModSeries-CSharp/MGModServer/traders/MarecGents/`（任务系统等进阶示例）；
- 发布仓库 `MG-GT-Mod/` 的 README（安装/部署方式）。

> 许可证提示：本项目采用 **CC BY-NC-ND 4.0**（禁止商业用途、禁止修改后重新发布）。
> 请自行确认你的二次开发与发布是否符合作者授权。
