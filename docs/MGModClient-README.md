# MGModClient — 游戏内配置编辑器（BepInEx 客户端插件）

**MGModEditor 客户端版** — BepInEx 5 插件，为游戏内实时编辑服务端配置打基础

| 项 | 值 |
|---|---|
| 语言 | C#（LangVersion latest） |
| 运行时 | netstandard2.1（BepInEx 5 生态） |
| 依赖 | BepInEx / 0Harmony / UnityEngine.* / Newtonsoft.Json（本地引用 Reference/） |
| 插件 GUID | com.mgmod.client |
| 版本 | v0.1.0 |

> 原位置：`SPTClientMods/MGEditor-Client/`（非 git 仓库，原名「MGEditor-Client」；代码迁移至本整合仓 `MGModClient/` 并全链路改名为 MGModClient）
> 引用 DLL 中央拷贝：仓库根 `Reference/`（游戏/SDK DLL，73MB，不入库，克隆后需自行补齐）

---

## 📖 简介

MGModClient 是 MGModEditor 的客户端版 —— BepInEx 5 插件。目标是在游戏内（F12 ConfigurationManager 菜单）实时编辑 MGModServer 的 `res/config/config.json`，实现"游戏内改配置 → 写回文件 → 服务端生效"的闭环。

## 设计要点

- **不打游戏逻辑补丁**：只做"配置镜像编辑 + 文件写回"，不引用 Assembly-CSharp/hollowed/spt-* → 版本脆弱性最低
- **UI**：ConfigurationManager 标准 F12 菜单（`Config.Bind` 镜像配置项）
- **写回**：`SyncService` 原子写 `config.json`（tmp + File.Replace），saveTime 版本戳，500ms 防抖，_suppress 防回环
- **i18n**：`Locales`（Radar 模式）键复用桌面版 Translations.cs 点分体系；Language 最先绑定；语言切换 = 值保真收集 → 移除重建 → `ConfigurationManager.BuildSettingList()`
- **Rig Layouts 注入**：检测到 MGGTMod 时注入 FG 弹挂布局（AssetBundle → ResourcesCache）
- **服务端配合**：MGModServer 需新增 IOnUpdate 热重载组件 + 幂等性改造（见原可行性报告）

## 🏗️ 项目结构

```
MGModClient/
├── MGModClientPlugin.cs      # [BepInPlugin] 入口；Language 最先绑定；功能分派（MGMod→配置编辑 / MGGTMod→布局注入）
├── Models/
│   └── MGConfig.cs           # 8 域配置模型 + saveTime 版本戳 + EnableValue 双值类型
├── Resources/
│   ├── ConfigItems.cs        # 配置项选项表
│   └── Locales.cs            # i18n 字典（EN/ZH，缺键回退）
├── Services/
│   ├── ModDetector.cs        # 服务端 mod 存在性检测（MGMod/MGGTMod）
│   ├── PathLocator.cs        # 定位 server 侧 config.json
│   ├── ConfigMirror.cs       # MGConfig ↔ F12 菜单 ConfigEntry 双向映射
│   ├── SyncService.cs        # 变更写回（原子写/防抖/防回环）
│   ├── ConfigurationManagerBridge.cs  # 反射刷新 ConfigurationManager 菜单
│   └── RigLayoutInjector.cs  # FG 弹挂布局注入（AssetBundle → ResourcesCache）
└── Helpers/
    └── JsonUtils.cs          # JSON 读写封装（原子写 tmp+File.Replace）
```

## 🔨 构建与输出

```bash
dotnet build MGModClient/MGModClient.csproj -c Release
```

构建输出（CopyBuildOutput 目标自动复制）到仓库根：

```
Build/BepInEx/plugins/MGModClient/MGModClient.dll
```

部署：将 `MGModClient.dll` 放入 BepInEx 插件目录 `BepInEx/plugins/MGModClient/`。

## ⚠️ 前置依赖

构建需要仓库根 `Reference/` 目录（BepInEx/Managed 游戏 DLL）。该目录已加入 `.gitignore` 不入库，克隆仓库后需从原 `SPTClientMods/Reference/` 复制补齐。

## 开发路径（里程碑）

- M1 MVP：只读镜像（F12 展示 8 域配置 + EN/ZH 语言切换）
- M2 写回同步（改值 → 原子写 config.json）
- M3 服务端热重载（MGModServer 改造）
- M4 完整闭环（客户端改 → 服务端生效）
