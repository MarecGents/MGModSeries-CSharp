# MGModClient — 客户端功能插件（BepInEx）

**客户端侧功能插件** — BepInEx 5 插件，承载必须在客户端生效的功能（热成像白名单、中文名称、资源注入、服务端 mod 探测）。

| 项 | 值 |
|---|---|
| 语言 | C#（LangVersion latest） |
| 运行时 | netstandard2.1（BepInEx 5 生态） |
| 依赖 | BepInEx / 0Harmony / UnityEngine.* / Newtonsoft.Json / Assembly-CSharp / Unity.TextMeshPro（本地引用 `Reference/`） |
| 插件 GUID | com.mgmod.client |
| 版本 | v0.2.3.040106 |

> 配置编辑由桌面版 **MGModEditor** 负责；客户端**不再维护**"F12 配置镜像 + 写回 config.json"链路。

---

## 📖 简介

MGModClient 是 MG 系列的客户端侧插件，只做必须在客户端生效的事情：

- **热成像瞄具白名单**：读取各服务端 mod 产出的 `res/plugins/scope_allow_in_thermal.json` + 手动白名单，通过 Harmony 补丁让指定瞄具在热成像下仍可用；
- **中文名称支持**：放开武器/装备/弹匣预设名与玩家昵称的字符校验（BMP CJK 范围），可选挂载中文字体 fallback；
- **Rig Layout 资源注入**：把服务端 mod `bundles/resources/**/*.bundle` 注入游戏 Resources；
- **服务端 mod 探测**：识别 MGMod / MGGTMod 的安装位置，供上述功能定位资源与配置。

---

## 设计要点

- **只改内存态**：字符校验、字体 fallback、热成像判定均为内存补丁，不写任何配置文件；
- **逐补丁容错**：每个 Harmony 补丁独立 try/catch，单个补丁失败不影响其余功能；
- **Mod 识别**：`Framework/ModRegistry` 三层识别（已知目录 / 额外目录 / marker 打分），兼容 SPT 4.1.x `SPT_Runtime/user/mods` 布局；
- **白名单契约**：兼容根数组 / 同名属性 / `ids` / `items` / `list` 等结构，并强校验 24 位 hex `MongoId`；
- **配置项**：BepInEx 配置仅用于开关与路径（语言、mods 根、热成像、中文名称），不镜像服务端 8 域配置。

## 🏗️ 项目结构

```
MGModClient/
├── MGModClientPlugin.cs      # [BepInPlugin] 入口：配置绑定 + 各功能安装
├── Framework/
│   ├── ModDescriptor.cs      # mod 描述
│   ├── ModInstallation.cs    # 安装位置与能力标记
│   ├── ModPluginFile.cs      # res/plugins/*.json 解析
│   ├── ModRegistry.cs        # mod 识别（三层）
│   ├── ServerConfigLocator.cs# 定位服务端 config.json
│   └── SptLayout.cs          # SPT 4.1.x 目录布局规则
├── Models/
│   └── MGConfig.cs           # 服务端配置镜像模型（当前仅用于读取/探测，不写回）
├── Resources/
│   ├── ConfigItems.cs        # 占位（配置编辑链未启用）
│   └── Locales.cs            # 客户端 i18n 占位（中/英，后续版本实现）
├── Services/
│   ├── ChineseNameSupport.cs          # 中文名称支持（字符校验 + 字体 fallback）
│   ├── ClientResourceLoader.cs        # resources 资源注入调度
│   ├── RigLayoutInjector.cs           # Rig Layout 注入
│   ├── ThermalScopeAllowListProvider.cs # 白名单合并（服务端 + 手动）
│   ├── ThermalScopePatch.cs           # 热成像判定 Harmony 补丁
│   ├── ThermalScopeProbeService.cs    # 本地玩家 PWA 探针
│   ├── ConfigMirror.cs                # 历史遗留（配置编辑链取消，不再启用）
│   ├── SyncService.cs                 # 历史遗留（同上）
│   └── ConfigurationManagerBridge.cs  # 历史遗留（同上）
└── Helpers/
    └── JsonUtils.cs          # JSON 读写封装（原子写 tmp + File.Replace）
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

构建需要仓库根 `Reference/` 目录（BepInEx/Managed 游戏 DLL）。该目录已加入 `.gitignore` 不入库，克隆仓库后需从本地 SPT 安装目录复制补齐：

- `Reference/BepInEx/core|patchers|plugins/spt` ← **SPT 发布版**（`sp-tarkov/build` Releases 下载的 SPT 客户端压缩包，解压即得完整 SPT 客户端）根目录下 `BepInEx\` 对应子目录（BepInEx 5 核心、spt-prepatch、spt-common/core/custom/debugging/reflection/singleplayer + ConfigurationManager）
- `Reference/Managed` ← **游戏本体目录**`\<游戏安装目录>\EscapeFromTarkov_Data\Managed\`（Assembly-CSharp、UnityEngine.*、Unity.TextMeshPro、Newtonsoft.Json、Sirenix.* 等）

复制命令与变量说明见根 README「⚠️ 注意事项 → Reference/ 目录不入库」一节。

## 后续计划

- **中英双语切换**：仅做配置界面描述的中/英切换（BepInEx `ConfigDescription` 在绑定后不可改，计划采用"启动期应用 + 切换后提示重启"；方案见 `docs/方案-MGModClient语言切换-中英双语-2026-11-15-2030.md`）。
- **资源注入扩展**：Slots / Voices 注入目前仅预留，后续按需启用。
