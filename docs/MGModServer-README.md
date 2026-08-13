# MGModServer — SPT 服务端 Mod

**MG-Mod 服务端 Mod 核心逻辑库** — 基于 C# 的 SPT Mod 框架

| 项 | 值 |
|---|---|
| 语言 | C# 14 |
| 运行时 | .NET 10.0 |
| 项目类型 | 类库 (Library, SDK.Web) |
| 框架依赖 | SPTarkov.Common / DI / Server.Core 4.1.2 |
| 架构模式 | 依赖注入 (DI) + 服务层 |
| 版本 | v1.0.1.040102 |

> 原仓库：`MarecGents/MG-Mod-CSharp`（已暂停维护，代码迁移至本整合仓 `MGModServer/`）

---

## 📖 简介

MGModServer 是 MG-Mod 家族的核心项目，使用 C# 编写的 SPT 服务端 Mod。它通过依赖注入（DI）架构深度集成 SPT 框架，实现对游戏服务器的全面配置与功能扩展。编译产出的 DLL 为 MG-Mod 提供全部后端逻辑支持，是 MG-Mod 功能体系的技术基石。

## 🏗️ 项目结构

```
MGModServer/
├── MGmod.cs              ← Mod 入口（IOnLoad 加载器）
├── types/
│   ├── models/           ← 数据模型定义
│   │   ├── Custom/       ← 自定义类型（配置/键映射）
│   │   ├── EFT/          ← EFT 游戏类型映射
│   │   └── Paths/        ← 路径管理
│   ├── server/           ← 服务器模块（8 个子系统）
│   ├── services/         ← 业务服务层（7 个服务）
│   └── utils/            ← 工具类
├── db/                   ← 游戏数据库覆盖
├── res/                  ← 运行时配置和资源
│   ├── res/botsystem/    ← ZDFW BotSystem 配置（AI 名字池/PMC 战术小队/地图难度/地图刷新/PMC 战吼）
│   └── res/quest/        ← 任务数据（quests.json、quest3X4.json 3X4 任务标记）
├── traders/              ← 自定义商人数据
├── images/               ← 图片资源
├── bundles/              ← 资源包（编译时输出）
└── Logg/                 ← 运行日志目录
```

## 🧩 模块体系

- **Server 层**（8 个服务器模块）：Bots / Configs / Globals / Hideout / Locales / Locations / Templates / Traders
- **Services 层**（7 个服务）：配置加载（ConfigSettingServices）、商人服务（CustomTraderServices）等
- **ZDFW BotSystem**：AI 名字池、PMC 战术小队、地图难度、地图刷新、PMC 战吼（25 语言）

## 🔨 构建与输出

```bash
dotnet build MGModServer/MGMod.csproj -c Release
```

Release 构建输出到仓库根：

```
Build/SPT_Runtime/user/mods/MGMod/
├── MGMod.dll
├── bundles/  images/  db/  res/  traders/  Logg/  bundles.json
└── MGModEditor.exe        ← MGModEditor 单文件发布（同一目录，见 docs/MGModEditor-README.md）
```

将 `Build/SPT_Runtime/user/mods/MGMod/` 整个文件夹放入 SPT 服务端的 `user/mods/` 目录即可部署。

> 注：`MGModEditor`（桌面配置编辑器）的单文件 exe 按设计同时输出到 `mods/MGMod/` 下，随 Mod 一起分发。
