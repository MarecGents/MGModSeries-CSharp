# MGModClient（空框架）

MGModEditor 客户端版 —— BepInEx 5 插件空框架，为游戏内实时编辑 MGMod-CSharp 配置（`res/config/config.json`）打基础。

## 设计要点（依据《SPTClientMods/docs/MGModEditor客户端迁移-*》报告）

- **不打游戏逻辑补丁**：只做"配置镜像编辑 + 文件写回"，不引用 Assembly-CSharp/hollowed/spt-* → 版本脆弱性最低
- **UI**：ConfigurationManager 标准 F12 菜单（`Config.Bind` 镜像配置项）
- **写回**：`SyncService` 原子写 `config.json`（tmp + File.Replace），saveTime 版本戳，500ms 防抖，_suppress 防回环
- **i18n**：`Locales`（Radar 模式）键复用桌面版 Translations.cs 点分体系；Language 最先绑定；语言切换 = 值保真收集 → 移除重建 → `ConfigurationManager.BuildSettingList()`
- **服务端配合**：MGMod-CSharp 需新增 IOnUpdate 热重载组件 + 幂等性改造（见可行性报告 §4.3/§5）

## 目录结构

```
MGModClient/
├── MGModClient.csproj          # netstandard2.1；仅引用 BepInEx.Core + UnityEngine.Modules
├── MGModClientPlugin.cs        # [BepInPlugin] 入口；Language 最先绑定；初始化顺序骨架
├── Models/
│   └── MGConfig.cs                # 8 域配置模型（空骨架，含 EnableValue 三件套）
├── Resources/
│   ├── ConfigItems.cs             # 选项表占位（桌面版 ConfigItems.cs 移植）
│   └── Locales.cs                 # i18n 字典（Radar 模式，键复用桌面版）
├── Services/
│   ├── PathLocator.cs             # 定位 server 侧 config.json（候选路径探测）
│   ├── ConfigMirror.cs            # MGConfig ↔ ConfigEntry 双向映射（空骨架）
│   ├── SyncService.cs             # 变更写回（原子写/防回环/saveTime 版本戳）
│   └── ConfigurationManagerBridge.cs # 反射调 BuildSettingList()
└── Helpers/
    └── JsonUtils.cs               # System.Text.Json 封装（Read/WriteAtomic）
```

## 开发路径（里程碑，见落地方案 §七）

- M1 MVP：只读镜像（F12 展示 8 域配置 + EN/ZH 语言切换）
- M2 写回同步（改值 → 原子写 config.json）
- M3 服务端热重载（MGMod-CSharp 改造，本仓库之外）
- M4 完整闭环（客户端改 → 服务端生效）

## 构建

```powershell
dotnet build MGModClient.csproj
# 输出复制到 Build/BepInEx/plugins/MGModClient/
```

> 参考：SAIN / AmandsGraphics / Tyrian-Radar-Standalone / SPT-Waypoints（BepInEx 5 插件生态）；`SPTClientMods/Reference/BepInEx/` 为引用 dll 中央拷贝。
