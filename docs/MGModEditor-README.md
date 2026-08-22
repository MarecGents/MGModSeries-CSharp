# MGModEditor — MG-Mod 图形化配置编辑器

**MG-Mod 图形化配置编辑器** — 基于 WPF 的现代化桌面 GUI 工具

| 项 | 值 |
|---|---|
| 语言 | C# 13 |
| 运行时 | .NET 10.0-windows |
| UI 框架 | WPF + **WPF-UI 4.3.0**（现代化控件库） |
| 架构模式 | MVVM（CommunityToolkit.Mvvm 8.4.0） |
| DI 容器 | Microsoft.Extensions.Hosting 10.0.1 |
| 发布方式 | 单文件发布 (PublishSingleFile, win-x64) |
| 版本 | v1.3.2.1 |

> 原仓库：`MarecGents/MGModEditor`（已暂停维护，代码迁移至本整合仓 `MGModEditor/`）

---

## 📖 简介

MGModEditor 是 MG-Mod 的可视化配置编辑器，采用 **C# WPF** 技术栈构建。它提供了直观的图形界面，让用户可以轻松编辑 MG-Mod 的所有配置文件，无需手动修改 JSON。

> 🎨 支持 **36 种主题配色**（18 种颜色 × 亮/暗模式）
> 🌍 支持 **5 种界面语言**（简体中文 / English / Русский / Français / 日本語），设置页一键切换，随系统语言自动适配

## 🏗️ 项目结构

```
MGModEditor/
├── App.xaml(.cs)          ← 应用入口，IHost DI 配置
├── Controls/              ← 自定义 WPF 控件（CheckButtonFrame/ComboxFrame/FrameGroup 等）
├── ControlsLookup/        ← 页面注册与导航
├── DependencyModel/       ← DI 扩展
├── Helpers/               ← 工具类（JSON 读取、文本测量、类型转换等）
├── Models/                ← 数据模型（MGConfig/AppSetting/CustomThemeConfig）
├── Services/              ← 服务层（MGConfigService/AppSettingService/CustomThemeService/TranslationService/WindowsProviderService）
├── Theme/                 ← 36 个主题 XAML 资源字典
├── ViewModels/            ← MVVM 视图模型
├── Views/                 ← WPF 页面视图
├── res/                   ← 运行时配置（config.json、EditorSettings.json、i18n 语言包）
└── Assets/                ← 图标资源
```

## 🔨 构建与输出（单文件发布）

**推荐：直接使用 IDE 原版 Build 按钮或仓库根一键构建脚本**——csproj 内置 `AfterBuild` 目标，构建完成后自动 publish 单文件到 Build 目录：

```bash
# 方式一：一键构建（推荐，见根 README「🚀 一键构建」）
dotnet build MGModSeries-CSharp.slnx -c Release

# 方式二：手动单文件 publish（等价于 IDE 构建按钮的自动行为）
dotnet publish MGModEditor/MGModEditor.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

> 注：Git Bash 中 `/p:` 会被 MSYS 路径转换破坏，请用 `-p:`（单横线）；PowerShell 下两种写法均可。

单文件发布输出到仓库根：

```
Build/SPT_Runtime/user/mods/MGMod/
├── MGModEditor.exe         ← 单文件可执行（免安装绿色，直接运行）
└── res/                    ← 运行时配置
```

MGModEditor 为免安装绿色单文件，直接运行 `MGModEditor.exe` 即可。按设计它随 MGModServer 一同输出到 `mods/MGMod/` 目录，随 Mod 分发。
