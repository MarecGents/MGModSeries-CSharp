# MGGTMod — 通用自定义商人框架（服务端）

**MG General Trader Mod** — 通用自定义商人框架（独立发布版）

| 项 | 值 |
|---|---|
| 语言 | C# 14 |
| 运行时 | .NET 10.0 |
| 项目类型 | 类库 (Library, SDK.Web) |
| 框架依赖 | SPTarkov.Common / DI / Server.Core 4.1.1 |
| 版本 | v0.5.0.040102 |

> 原仓库：`MarecGents/MG-GT-Mod-CSharp`（已暂停维护，代码迁移至本整合仓 `MGGTMod/`）

---

## 📖 简介

MGGTMod（MG General Trader Mod）是一个独立发布的 SPT 自定义商人 Mod。它提供了完整的商人框架，允许用户通过简单的 JSON 配置快速创建自定义商人，而无需编写任何代码。

核心商人加载逻辑与 **MGModServer** 中的独立商人功能完全一致，是将其从 MG-Mod 主项目中剥离后的独立版本，专为只需要自定义商人功能的用户设计。

## 📖 商人制作教程

📄 [MG通用商人制作教程](./MG通用商人制作教程.md) —— 涵盖教程基础信息、文件与文件夹架构、各文件内部结构、撰写示例与自编译发布流程（含防冲突改名清单）。

教程以内置示例商人 **FlanrecGents** 为模板，并参考 **MG-Mod 的 MarecGents** 展示任务系统等进阶用法。

## 🏗️ 项目结构

```
MGGTMod/
├── MGGTmod.cs              ← Mod 入口（IOnLoad 加载器）
├── MGGTMod.csproj          ← .NET 10.0 类库项目
├── types/
│   ├── models/
│   │   ├── Custom/         ← 自定义类型定义（ConfigSettingType/KeyMapType）
│   │   ├── EFT/            ← EFT 游戏类型映射（MGGlobals、locales/locations/templetes/traders）
│   │   └── Paths/          ← 路径管理
│   ├── server/             ← 服务器模块（Bots/Configs/Globals/Hideout/Locales/Locations/Templates/Traders）
│   ├── services/           ← ConfigSettingServices / CustomTraderServices（核心商人服务）/ TestServices
│   └── utils/              ← MGUtils.cs
├── traders/                ← 自定义商人配置目录（可热加载，FlanrecGents 示例）
├── res/                    ← 资源文件（图片、TraderDB/itemsDB/locationsDB）
├── bundles/                ← 资源包（含 FG Rig Layouts bundle）
└── bundles.json            ← 资源包清单
```

## 🔨 构建与输出

```bash
dotnet build MGGTMod/MGGTMod.csproj -c Release
```

Release 构建输出到仓库根：

```
Build/SPT_Runtime/user/mods/MGGTMod/
├── MGGTMod.dll
├── traders/  res/  bundles/  bundles.json
```

部署：将 `Build/SPT_Runtime/user/mods/MGGTMod/` 放入 SPT 服务端 `user/mods/` 目录，然后在 `user/mods/MGGTMod/traders/` 下添加你的商人配置。
