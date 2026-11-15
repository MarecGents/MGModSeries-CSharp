# MGModSeries-CSharp

**MG Mod 系列 C# 整合仓库** — 四个 C# 项目的统一解决方案（MGModServer / MGGTMod / MGModEditor / MGModClient）

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![SPT Version](https://img.shields.io/badge/SPT-4.1.6-blue)](https://dev.sp-tarkov.com/)
[![License](https://img.shields.io/badge/License-CC%20BY--NC--ND%204.0-lightgrey)](LICENSE)
> **当前分支 `v4.1.x`**：适配 SPT 4.1.6 服务端（SPTushonka，net10.0）｜MG-Mod v1.1.5.040106 ｜ MG-GT-Mod v0.5.4.040106 ｜ MGModClient v0.2.3.040106 ｜ MGModEditor v1.3.3.0

---

## 📖 简介

本仓库将原独立维护的 MG 系列 C# 项目整合为**同一解决方案下的四个同级项目**，用一个 `.slnx` 统一管理。原仓库（`MarecGents/MG-Mod-CSharp`、`MarecGents/MG-GT-Mod-CSharp`、`MarecGents/MGModEditor`）已暂停维护，代码迁移至此。

| 项目 | 目录 | 类型 | 说明 |
|---|---|---|---|
| **MGModServer** | `MGModServer/` | SPT 服务端 Mod（net10.0） | MG-Mod 服务端核心逻辑库，DI 深度集成 SPT |
| **MGGTMod** | `MGGTMod/` | SPT 服务端 Mod（net10.0） | 通用自定义商人框架（独立发布版） |
| **MGModEditor** | `MGModEditor/` | WPF 桌面 GUI（net10.0-windows） | MG-Mod 图形化配置编辑器（36 主题 / 5 语言） |
| **MGModClient** | `MGModClient/` | BepInEx 客户端插件（netstandard2.1） | 客户端功能插件（热成像白名单 / 中文名称 / 资源注入 / 服务端 mod 探测） |

---

## 🏗️ 仓库结构

```
MGModSeries-CSharp/
├── MGModServer/              ← SPT 服务端 Mod（MG-Mod 核心）
├── MGGTMod/                  ← SPT 服务端 Mod（通用商人框架）
├── MGModEditor/              ← WPF 图形化配置编辑器
├── MGModClient/              ← BepInEx 客户端插件
├── Reference/                ← 游戏/SDK DLL（73MB，不入库，克隆后需自行补齐）
├── docs/                     ← 各项目最详细介绍 README
├── Directory.Build.props     ← 统一构建输出路径（一键构建的核心配置）
├── MGModSeries-CSharp.slnx   ← 统一解决方案（四项目）
├── .gitignore
└── LICENSE
```

---

## 🚀 一键构建

打开 `MGModSeries-CSharp.slnx`，在 VS 2026 / JetBrains Rider 中点原版 **Build Solution** 按钮，或命令行执行：

```bash
dotnet build MGModSeries-CSharp.slnx        # Debug
dotnet build MGModSeries-CSharp.slnx -c Release   # Release
```

所有项目**自动构建到仓库根 `Build\` 目录**（`Directory.Build.props` 统一配置，VS/Rider/CLI 均遵守）：

| 项目 | 构建动作 | 输出位置 |
|---|---|---|
| MGModServer | build（自动） | `Build\SPT_Runtime\user\mods\MGMod\` |
| MGGTMod | build（自动） | `Build\SPT_Runtime\user\mods\MGGTMod\` |
| MGModEditor | build + **自动 publish 单文件**（AfterBuild 目标） | `Build\SPT_Runtime\user\mods\MGMod\MGModEditor.exe` |
| MGModClient | build + 自动复制（CopyBuildOutput 目标） | `Build\BepInEx\plugins\MGModClient\` |

> `Build\` 已加入 `.gitignore`，不推送至仓库。Debug/Release 均适用，产物为最新一次构建。

### 🖱️ 交互式构建菜单（推荐）

双击 `scripts\build-menu.cmd` 打开菜单窗口，用 **↑/↓ 选择 + Enter 确认**，即可一键编译或单独编译某项目（类似 npx 的交互式流程）：

| 菜单项 | 行为 |
|---|---|
| 一键编译全部 | 先清空 `Build\` 再构建四项目（slnx，Release） |
| 单独编译 MGModServer | 删除 `mods\MGMod\` 整目录后编译（会一并移除 MGModEditor.exe，如需请再编译 MGModEditor） |
| 单独编译 MGGTMod | 删除 `mods\MGGTMod\` 整目录后编译 |
| 单独编译 MGModEditor | 不删目录（与 MGModServer 共用输出，避免误删），构建后自动 publish 单文件 |
| 单独编译 MGModClient | 删除 `BepInEx\plugins\MGModClient\` 后编译 |

命令行等价用法（非交互）：

```bash
powershell -ExecutionPolicy Bypass -File scripts/build-menu.ps1 -Run all          # 一键
powershell -ExecutionPolicy Bypass -File scripts/build-menu.ps1 -Run mgmodserver  # 单独
powershell -ExecutionPolicy Bypass -File scripts/build-menu.ps1 -Run mggtmod
powershell -ExecutionPolicy Bypass -File scripts/build-menu.ps1 -Run mgeditor
powershell -ExecutionPolicy Bypass -File scripts/build-menu.ps1 -Run mgclient
```

---

## 🧩 项目详细介绍

### 1. MGModServer — SPT 服务端 Mod（核心）

**MG-Mod 服务端核心逻辑库**，基于 C#（.NET 10.0）编写，通过依赖注入（DI）架构深度集成 SPT 框架，实现对游戏服务器的全面配置与功能扩展，是 MG-Mod 功能体系的技术基石。

- **模块体系**：Server 层 8 个子系统（Bots / Configs / Globals / Hideout / Locales / Locations / Templates / Traders）+ Services 层 7 个服务（配置加载、商人服务等）
- **ZDFW BotSystem**：AI 名字池、PMC 战术小队、地图难度、地图刷新、PMC 战吼（25 语言翻译）
- **数据资源**：`db/` 游戏数据库覆盖、`res/botsystem/` AI 配置、`res/quest/` 任务数据（含 3X4 任务标记）、`traders/` 自定义商人
- **独立商人扩展**：散落物资注入（`location/looseLoot.json`）、物品插件白名单（`res/plugins/`）、投保与供货配置
- **依赖**：SPTushonka.Common / DI / Server.Core 4.1.6

📄 详细文档：[docs/MGModServer-README.md](docs/MGModServer-README.md)

### 2. MGGTMod — 通用自定义商人框架（服务端）

**MG General Trader Mod**，独立发布的 SPT 自定义商人框架（.NET 10.0）。提供完整商人框架，用户通过 JSON 配置即可创建自定义商人，无需编写代码。核心商人加载逻辑与 MGModServer 的独立商人功能一致，是剥离后的独立版本。

- **核心服务**：`CustomTraderServices`（商人加载）、`ConfigSettingServices`（配置）
- **可热加载**：`traders/` 目录下的商人配置（内置 FlanrecGents 示例 + MarecGents 任务系统参考）
- **Rig Layouts**：`bundles/` 含 FG 弹挂布局 bundle（供客户端注入）
- **散落物资**：商人目录 `location/looseLoot.json` 可向指定地图注入自定义刷新点（与 MGModServer 同源）
- **依赖**：SPTushonka.Common / DI / Server.Core 4.1.6

📄 详细文档：[docs/MGGTMod-README.md](docs/MGGTMod-README.md) ｜ [商人制作教程](docs/MG通用商人制作教程.md)

### 3. MGModEditor — 图形化配置编辑器（桌面 GUI）

**MG-Mod 可视化配置编辑器**，C# WPF（.NET 10.0-windows）构建，让用户直观编辑 MG-Mod 全部配置文件，无需手动修改 JSON。**36 种主题配色**（18 色 × 亮/暗）、**5 种界面语言**（中/英/俄/法/日）。

- **技术栈**：WPF + WPF-UI 4.3.0、MVVM（CommunityToolkit.Mvvm 8.4.2）、DI（Microsoft.Extensions.Hosting 10.0.11）
- **结构**：App 入口（IHost DI）/ Controls 自定义控件 / Services（MGConfig、AppSetting、CustomTheme、Translation）/ Theme 36 主题 / ViewModels + Views
- **发布**：单文件发布（PublishSingleFile, win-x64），构建按钮即自动产出单文件 exe

📄 详细文档：[docs/MGModEditor-README.md](docs/MGModEditor-README.md)

### 4. MGModClient — 客户端功能插件（BepInEx 客户端插件）

**客户端功能插件**（BepInEx 5，netstandard2.1），承载必须在客户端侧生效的功能：热成像瞄具白名单补丁、中文名称支持、Rig Layout 资源注入、服务端 mod 探测。配置编辑由桌面版 **MGModEditor** 负责，客户端不再维护配置镜像/写回链路。

- **Mod 识别**：`Framework/ModRegistry` 三层识别（已知目录 / 额外目录 / marker 打分），兼容 SPT 4.1.x 布局
- **热成像白名单**：读取各 mod `res/plugins/scope_allow_in_thermal.json` + 手动列表，Harmony 补丁使指定瞄具在热成像下可用
- **中文名称支持**：放开预设名/昵称的字符校验（BMP CJK），可选 UI 字体 fallback
- **资源注入**：`ClientResourceLoader` 将 `bundles/resources/**/*.bundle` 注入游戏 Resources（当前支持 Rig Layout）
- **i18n**：客户端侧文本仅中/英（配置界面描述本地化，后续版本实现）

📄 详细文档：[docs/MGModClient-README.md](docs/MGModClient-README.md)

---

## 📦 部署

- **服务端 Mod**：将 `Build\SPT_Runtime\user\mods\MGMod\` 与 `Build\SPT_Runtime\user\mods\MGGTMod\` 放入 SPT 服务端 `user\mods\` 目录（`SPT_Runtime` 即服务端运行时布局）。
- **客户端插件**：将 `Build\BepInEx\plugins\MGModClient\MGModClient.dll` 放入游戏 `BepInEx\plugins\MGModClient\`。
- **编辑器**：直接运行 `Build\SPT_Runtime\user\mods\MGMod\MGModEditor.exe`（免安装绿色单文件）。

## 📝 更新日志

各项目发布说明见 `ReleaseNotes/`：

| 项目 | 当前版本 | 更新日志 |
|---|---|---|
| MG-Mod（MGModServer） | v1.1.5.040106 | [MGMod升级更新日志](ReleaseNotes/MGMod升级更新日志.md) |
| MG-GT-Mod（MGGTMod） | v0.5.4.040106 | [MGGTMod升级更新日志](ReleaseNotes/MGGTMod升级更新日志.md) |
| MGModClient | v0.2.3.040106 | [MGModClient升级更新日志](ReleaseNotes/MGModClient升级更新日志.md) |
| MGModEditor | v1.3.3.0 | [MGModEditor升级更新日志](ReleaseNotes/MGModEditor升级更新日志.md) |

## ⚠️ 注意事项

- `Reference/` 目录不入库（73MB 游戏/SDK DLL），克隆后需自行补齐，否则 **MGModClient 无法构建**。结构如下，DLL 来源均为通用/本地 SPT 安装目录：

```
Reference/
├── BepInEx/
│   ├── core/        ← BepInEx 5 核心 + 0Harmony（来源：SPT 发布版的 BepInEx\core\，见下方说明）
│   ├── patchers/    ← spt-prepatch.dll（来源：SPT 发布版的 BepInEx\patchers\）
│   └── plugins/spt/ ← SPT 插件：spt-common/core/custom/debugging/reflection/singleplayer.dll + ConfigurationManager（来源：SPT 发布版的 BepInEx\plugins\spt\）
└── Managed/         ← 游戏托管 DLL：Assembly-CSharp、UnityEngine.*、Newtonsoft.Json、Sirenix.* 等（来源：游戏本体目录\EscapeFromTarkov_Data\Managed\）
```

  **DLL 来源（通用说明）**：

  - **BepInEx 部分**：来自 **SPT 发布版**（`sp-tarkov/build` 的 Releases 下载的 SPT 客户端压缩包，解压即得完整 SPT 客户端）。取 SPT 客户端根目录下 `BepInEx\` 的对应子目录 DLL：`core\`（BepInEx 5 核心、0Harmony）、`patchers\`（spt-prepatch）、`plugins\spt\`（spt-common/core/custom/debugging/reflection/singleplayer、ConfigurationManager）。
  - **Managed 部分**：来自 **游戏本体目录**（`<游戏安装目录>\EscapeFromTarkov_Data\Managed\`，如 `D:\Games\SPT\EscapeFromTarkov_Data\Managed\`）——复制 `Assembly-CSharp.dll`、`UnityEngine.*.dll`、`Newtonsoft.Json.dll`、`Sirenix.*.dll` 等托管 DLL。

  **补齐方法**（在仓库根执行；把 `<SPT客户端目录>` / `<游戏本体目录>` 换成你的实际路径）：

  ```powershell
  # 1) BepInEx 部分（core 全量 + patchers + plugins/spt 全量）
  $spt = "<SPT客户端目录>"   # 例如 D:\Games\SPT
  New-Item -ItemType Directory -Force Reference\BepInEx\core, Reference\BepInEx\patchers, Reference\BepInEx\plugins\spt | Out-Null
  Copy-Item "$spt\BepInEx\core\*.dll"          Reference\BepInEx\core\
  Copy-Item "$spt\BepInEx\patchers\*.dll"      Reference\BepInEx\patchers\
  Copy-Item "$spt\BepInEx\plugins\spt\*.dll"   Reference\BepInEx\plugins\spt\
  Copy-Item "$spt\BepInEx\plugins\spt\ConfigurationManager" Reference\BepInEx\plugins\spt\ -Recurse

  # 2) Managed 部分（游戏本体托管 DLL，可全量复制或按需）
  $game = "<游戏本体目录>"   # 例如 D:\Games\SPT\EscapeFromTarkov_Data
  Copy-Item "$game\Managed\*.dll"  Reference\Managed\
  ```

  MGModClient.csproj 实际引用的 DLL：`BepInEx.dll`、`0Harmony.dll`（BepInEx\core）与 `UnityEngine.dll`、`UnityEngine.CoreModule.dll`、`UnityEngine.AssetBundleModule.dll`、`Assembly-CSharp.dll`、`Newtonsoft.Json.dll`、`Sirenix.Serialization.dll`、`Sirenix.Serialization.Config.dll`（Managed）。
- `*.bundle` 使用 Git LFS 管理：克隆后需安装 [Git LFS](https://git-lfs.com/) 并执行 `git lfs pull`，否则 `bundles/` 下只会得到指针文件、资源缺失。
- 原仓库发布帖等历史资料仍保留在原仓库（`MarecGents/*`），本仓库为新的开发主线；本仓库更新日志见上方「📝 更新日志」。
- MGGTMod 与 MGModServer 均引用 SPTushonka 4.1.6（4.1.x 命名空间仍为 SPTarkov.Server.Core.*，已对齐）。

## 📜 许可

**CC BY-NC-ND 4.0**（署名-非商业性使用-禁止演绎，见 [LICENSE](LICENSE)）
