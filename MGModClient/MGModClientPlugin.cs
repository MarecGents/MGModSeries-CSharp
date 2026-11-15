using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using MGModClient.Framework;
using MGModClient.Models;
using MGModClient.Resources;
using MGModClient.Services;

namespace MGModClient;

/// <summary>
/// 客户端基础库（BepInEx 5 插件）。作为可复用的 base library 设计：
/// 其它商人作者可直接 clone 仓库，改 <c>MGModClient/Framework/ModRegistry.cs</c> 的 <c>Known</c> 数组
/// （以及 csproj 的 PluginGuid/PluginName）即可完成改名，业务代码无需改动。
///
/// 功能（各 mod 之间无前置关系，按能力自动分派）：
///  - 配置编辑：对「提供 res/config/config.json 的 mod」做文件编辑 + F12 配置镜像；
///  - 客户端资源：加载「提供 bundles/resources/ 的 mod」的 rig 布局等资源并注入 ResourcesCache；
///  - 热成像瞄具：读取「提供 res/plugins/scope_allow_in_thermal.json 的 mod」的白名单并合并，
///    仅对白名单内的瞄具在热成像下恢复可用（游戏原版瞄具不受影响）。
///
/// 设计约束：
///  - 核心功能（配置编辑）不打游戏逻辑补丁 → 只做文件编辑 + Config 镜像；
///  - 【例外】ThermalScopePatch 为「热成像下瞄具可用」引入 Harmony 补丁；
///  - 唯一运行时依赖 BepInEx + 0Harmony；ConfigurationManager 通过反射调用。
/// </summary>
[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
public class MGModClientPlugin : BaseUnityPlugin
{
    /// <summary>BepInEx 配置分区名（= 插件名，改名时随 csproj 的 PluginName 一起变）。</summary>
    private const string Section = PluginInfo.NAME;

    private const string LogPrefix = "[" + PluginInfo.NAME + "]";

    // ---- 全局状态（供 Services/Resources 引用）----
    /// <summary>语言条目必须最先绑定：后续所有键名/描述经 Locales.Get() 解析（Radar 同款硬约束）。</summary>
    public static ConfigEntry<string> Language { get; private set; }

    /// <summary>【探针】热成像 × 瞄具 的工作模式（Off / ForwardAlpha / Global / Material）。</summary>
    public static ConfigEntry<string> ThermalScope { get; private set; }

    /// <summary>
    /// 【探针】手动追加白名单（逗号分隔的瞄具 TemplateId）。
    /// 默认空：正常情况由服务端插件产物（res/plugins/scope_allow_in_thermal.json）自动提供；
    /// 此项仅用于临时调试 / 服务端未部署时的验证。
    /// </summary>
    public static ConfigEntry<string> ThermalScopeAllowList { get; private set; }

    /// <summary>是否启用「服务端插件白名单」（关闭后仅用上面的手动列表）。</summary>
    public static ConfigEntry<bool> ThermalScopeUseServerPlugins { get; private set; }

    /// <summary>允许预设名 / 玩家昵称输入中文（默认开启）。</summary>
    public static ConfigEntry<bool> ChineseNamesEnable { get; private set; }

    /// <summary>当前语言非中文时，把 SimSun 挂为所有 UI 字体的 fallback（默认开启）。</summary>
    public static ConfigEntry<bool> ChineseNamesForceFontFallback { get; private set; }

    // ---- mod 识别配置 ----
    private ConfigEntry<string> _modsRootOverride;
    private ConfigEntry<string> _extraModDirectories;
    private ConfigEntry<bool> _autoDiscoverMods;
    private ConfigEntry<string> _serverConfigPath;

    private ServerConfigLocator _configLocator;
    private ConfigMirror _mirror;
    private SyncService _syncService;

    private void Awake()
    {
        // ① 最先绑定 Language（Locales.Get 依赖它；RadarConfig.cs:66-69 同款约束）
        Language = Config.Bind(Section, "Language", "EN",
            new ConfigDescription("Menu language",
                new AcceptableValueList<string>("EN", "ZH", "RU", "KO", "FR", "JP")));
        Language.SettingChanged += OnLanguageChanged;

        // ② mod 识别：Known 清单（ModRegistry.cs 单点） + 配置追加 + marker 自动发现
        BindModDetectionConfigs();
        ModRegistry.Resolve(
            modsRootOverride: _modsRootOverride.Value,
            extraDirNames: _extraModDirectories.Value,
            autoDiscover: _autoDiscoverMods.Value,
            log: Logger.LogInfo,
            warn: Logger.LogWarning);

        Logger.LogInfo($"{LogPrefix} mods 根: {ModRegistry.ModsRoot}（layout={ModRegistry.LayoutId}）");
        Logger.LogInfo($"{LogPrefix} 已识别: {ModRegistry.Describe()}");

        // ③ 配置编辑主链路：自动挑选「提供 config.json 的 mod」
        _configLocator = new ServerConfigLocator();
        _configLocator.Resolve(_serverConfigPath.Value);
        if (_configLocator.ConfigPath != null)
        {
            _mirror = new ConfigMirror();
            _mirror.BindAll(Config, LoadModelFromDisk());
            _syncService = new SyncService(_mirror, _configLocator);
            Config.SettingChanged += _syncService.OnEntryChanged;
            Logger.LogInfo($"{LogPrefix} 配置编辑已启用: {_configLocator.ConfigPath}" +
                           $"（来源={_configLocator.LastReason}, mod={_configLocator.ModId ?? "手动路径"}）");
        }
        else
        {
            Logger.LogWarning($"{LogPrefix} 未找到服务端 config.json，配置编辑跳过：{_configLocator.LastReason}");
        }

        // ④ 客户端资源加载：遍历所有提供 bundles/resources/ 的 mod
        ClientResourceLoader.LoadAll(Logger);

        // ⑤ 热成像 × 瞄具：白名单 = 服务端插件产物 ∪ 手动追加
        BindThermalScopeConfigs();
        ThermalScopeProbeService.Logger = Logger;
        ThermalScopeProbeService.Mode = ThermalScope.Value;

        ThermalScopeAllowListProvider.Init(Logger);
        ThermalScopeAllowListProvider.Changed += OnEffectiveAllowListChanged;
        ThermalScopeAllowListProvider.SetManual(ThermalScopeAllowList.Value);
        ThermalScopeAllowListProvider.SetUseServerPlugins(ThermalScopeUseServerPlugins.Value);
        ThermalScopeAllowListProvider.ReloadServerPlugins(force: true);

        ThermalScopePatch.Install(Logger);

        // ⑥ 中文名称支持：枪械/装备/弹匣预设名 + 玩家昵称
        BindChineseNameConfigs();
        ChineseNameSupport.Install(Logger);

        Logger.LogInfo($"{LogPrefix} v{PluginInfo.VERSION} 初始化完成");
    }

    // ==================== 配置绑定 ====================

    private void BindModDetectionConfigs()
    {
        _modsRootOverride = Config.Bind(Section, "Mods Root Override", "",
            new ConfigDescription(
                "直接指定服务端 mods 根目录（空 = 按 SPT 版本布局自动探测）。\n" +
                "布局候选见 Framework/SptLayout.cs：SPT 4.1.x → SPT_Runtime/user/mods；4.0.x → SPT/user/mods；3.x → user/mods。"));

        _extraModDirectories = Config.Bind(Section, "Extra Mod Directories", "",
            new ConfigDescription(
                "额外识别的 mod 目录名（逗号分隔，位于 mods 根下）。\n" +
                "用于框架分叉后改名、且不想重新编译客户端的场景；已知清单见 Framework/ModRegistry.cs。"));

        _autoDiscoverMods = Config.Bind(Section, "Auto Discover Mods", true,
            new ConfigDescription(
                "自动发现同框架 mod：扫描 mods 根下一层，按特征文件（res/plugins、res/services/TraderDB、traders+bundles.json 等）判定。\n" +
                "误判无害：只会在该目录内寻找已知文件；关闭后仅识别已知清单 + 上面的额外目录。"));

        _serverConfigPath = Config.Bind(Section, "Server Config Path", "",
            new ConfigDescription("手动指定服务端 res/config/config.json 的完整路径（空 = 自动探测）。"));
    }

    private void BindChineseNameConfigs()
    {
        ChineseNamesEnable = Config.Bind(Section, "Chinese Names Enable", true,
            new ConfigDescription(
                "允许枪械/装备/弹匣预设名与玩家昵称输入中文（客户端补丁；服务端无需改动）。\n" +
                "关闭后恢复游戏原版规则：仅允许 0-9 A-Z a-z 空格 - _。"));

        ChineseNamesForceFontFallback = Config.Bind(Section, "Chinese Names Force Font Fallback", true,
            new ConfigDescription(
                "当前游戏语言不是简体中文时，把所有 UI 字体挂上 SimSun 中文字形 fallback。\n" +
                "语言=简体中文时游戏自身已挂好（本项不会重复处理）；仅在英文/俄文等语言下才需要。"));

        ChineseNamesEnable.SettingChanged += (_, __) => ApplyChineseNameSettings();
        ChineseNamesForceFontFallback.SettingChanged += (_, __) => ApplyChineseNameSettings();
        ApplyChineseNameSettings();
    }

    private void ApplyChineseNameSettings()
    {
        ChineseNameSupport.RegexEnabled = ChineseNamesEnable.Value;
        ChineseNameSupport.FontFallbackEnabled = ChineseNamesForceFontFallback.Value;
        Logger.LogInfo($"{LogPrefix} ChineseNames: 字符校验={(ChineseNamesEnable.Value ? "放开" : "原版")}, " +
                       $"字体fallback={(ChineseNamesForceFontFallback.Value ? "开" : "关")}");
    }

    private void BindThermalScopeConfigs()
    {        ThermalScopeUseServerPlugins = Config.Bind(Section, "ThermalScope Use Server Plugins", true,
            new ConfigDescription(
                "启用服务端插件白名单：读取各已识别 mod 的 res/plugins/scope_allow_in_thermal.json 并合并。\n" +
                "物品是否允许热成像下使用由服务端物品配置（plugin.scope_allow_in_thermal）决定。"));

        ThermalScopeAllowList = Config.Bind(Section, "ThermalScope AllowList", "",
            new ConfigDescription(
                "手动追加白名单（逗号分隔的瞄具 TemplateId / 物品 _id）。默认空 —— 正常由服务端插件列表提供。\n" +
                "此项仅用于临时调试；与服务端列表取并集。留空且服务端列表为空 = 功能完全关闭（原版行为完全保留）。"));

        ThermalScope = Config.Bind(Section, "ThermalScope Mode", "ForwardAlpha",
            new ConfigDescription(
                "热成像下瞄具的工作模式：Off=关闭；ForwardAlpha=前向透明阶段前把全局置 0（推荐，已验证有效）；" +
                "Global=全局置 0（影响整帧，仅诊断用）；Material=按材质写（已验证无效，仅作对照）",
                new AcceptableValueList<string>("Off", "ForwardAlpha", "Global", "Material")));

        // 变更回调
        ThermalScopeAllowList.SettingChanged += OnThermalScopeAllowListChanged;
        ThermalScopeUseServerPlugins.SettingChanged += OnThermalScopeUseServerPluginsChanged;
        ThermalScope.SettingChanged += OnThermalScopeModeChanged;
    }

    private MGConfig LoadModelFromDisk()
    {
        var cfg = new MGConfig();
        if (_configLocator?.ConfigPath == null) return cfg;

        var onDisk = JsonUtils.Read<MGConfig>(_configLocator.ConfigPath);
        if (onDisk != null)
        {
            cfg.CopyFrom(onDisk);
        }
        else
        {
            // 文件存在但解析失败 → 损坏；打日志警告，而非静默回退默认
            Logger.LogWarning($"{LogPrefix} config.json 读取失败或损坏，已使用默认配置：{_configLocator.ConfigPath}");
        }
        return cfg;
    }

    // ==================== 事件回调 ====================

    /// <summary>白名单生效集合变化（手动/服务端/热重载）→ 推送给探针服务。</summary>
    private void OnEffectiveAllowListChanged(IReadOnlyCollection<string> ids)
    {
        ThermalScopeProbeService.SetAllowListSet(ids);
        ThermalScopeProbeService.ResetState();
    }

    private void OnThermalScopeModeChanged(object sender, EventArgs e)
    {
        ThermalScopeProbeService.Mode = ThermalScope.Value;
        ThermalScopeProbeService.ResetState();
        Logger.LogInfo($"[ThermalScope] 模式切换为 {ThermalScope.Value}");
    }

    private void OnThermalScopeAllowListChanged(object sender, EventArgs e)
    {
        ThermalScopeAllowListProvider.SetManual(ThermalScopeAllowList.Value);
    }

    private void OnThermalScopeUseServerPluginsChanged(object sender, EventArgs e)
    {
        ThermalScopeAllowListProvider.SetUseServerPlugins(ThermalScopeUseServerPlugins.Value);
    }

    private void OnLanguageChanged(object sender, EventArgs e)
    {
        // 值保真收集 → 移除重建 → Save → CM.BuildSettingList() 刷新（见落地方案 §5.3）
        Logger.LogInfo($"语言切换为 {Language.Value}（重建 ConfigEntry + 刷新菜单，待实现）");
        // TODO(M1): ConfigEntryRegistry.CollectValues → 移除重建 → Config.Save() → ConfigurationManagerBridge.BuildSettingList()
    }

    private void Update()
    {
        // 主流程无 Update 逻辑；热成像瞄具探针走 Harmony 补丁（见 ThermalScopePatch），不在此驱动。
    }
}
