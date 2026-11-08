using BepInEx;
using BepInEx.Configuration;
using MGModClient.Models;
using MGModClient.Resources;
using MGModClient.Services;

namespace MGModClient;

/// <summary>
/// MGModClient —— MGModEditor 客户端版（BepInEx 5 插件）。
/// 目标：游戏内实时编辑 MGMod-CSharp 的 res/config/config.json（F12 ConfigurationManager 菜单镜像）
///       + 加载 FG 自定义弹挂布局（Rig Layouts）。
///
/// 功能分派（独立存在检测，无前置关系）：
///  - MG-Mod 存在 → 执行配置编辑主功能（PathLocator/ConfigMirror/SyncService）；
///  - MGGTMod 存在 → 注入 Rig Layouts 布局预制体；
/// 两者各自独立，互不为前置。
///
/// 设计约束（见《MGModEditor客户端迁移-代码落地方案报告》§一）：
///  - 核心功能（配置编辑）不打游戏逻辑补丁 → 只做文件编辑 + Config 镜像；
///  - 【例外·临时探针】ThermalScopePatch 为验证「热成像下瞄具可用性」引入了 Harmony 补丁
///    （InitTransforms / FindAimTransforms / ThermalVision.OnPreCull），属一次性验证代码，
///    结论确定后应迁出为正式功能或独立插件；
///  - RigLayoutInjector 为加载布局预制体引用了 Assembly-CSharp（ContainedGridsView/ResourcesCache）；
///  - 唯一运行时依赖 BepInEx + 0Harmony；ConfigurationManager 通过反射调用。
/// </summary>
[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
public class MGModClientPlugin : BaseUnityPlugin
{
    // ---- 全局状态（供 Services/Resources 引用）----
    /// <summary>语言条目必须最先绑定：后续所有键名/描述经 Locales.Get() 解析（Radar 同款硬约束）。</summary>
    public static ConfigEntry<string> Language { get; private set; }

    /// <summary>【探针】热成像 × 瞄具 的工作模式（Off / Material / Global）。</summary>
    public static ConfigEntry<string> ThermalScope { get; private set; }

    private PathLocator _pathLocator;
    private ConfigMirror _mirror;
    private SyncService _syncService;
    private bool _configEditorEnabled; // MG-Mod 存在时启用配置编辑

    private void Awake()
    {
        // ① 最先绑定 Language（Locales.Get 依赖它；RadarConfig.cs:66-69 同款约束）
        Language = Config.Bind("MGModClient", "Language", "EN",
            new ConfigDescription("Menu language",
                new AcceptableValueList<string>("EN", "ZH", "RU", "KO", "FR", "JP")));
        Language.SettingChanged += OnLanguageChanged;

        // ② 独立检测 MG-Mod：存在才启用配置编辑主功能（无前置）
        _configEditorEnabled = ModDetector.IsMGModPresent();
        if (_configEditorEnabled)
        {
            _pathLocator = new PathLocator();
            _pathLocator.Resolve(ServerConfigPath?.Value);
            _mirror = new ConfigMirror();
            _mirror.BindAll(Config, LoadModelFromDisk());
            _syncService = new SyncService(_mirror, _pathLocator);
            Config.SettingChanged += _syncService.OnEntryChanged;
            Logger.LogInfo("[MGModClient] 检测到 MGMod，配置编辑功能已启用");
        }
        else
        {
            Logger.LogWarning("[MGModClient] 未检测到 MGMod，配置编辑功能跳过（仅保留布局注入）");
        }

        // ③ 客户端资源加载：检索 MGMod/MGGTMod 的 bundles/resources/ 下全部 .bundle，
        //    按资源类型分发（rig 布局等；MGMod 与 MGGTMod 各自独立存在检测）
        ClientResourceLoader.LoadAll(Logger);

        // ④ 【探针】热成像 × 瞄具：把光学材质（CW FX/Collimator 等四个 shader）的
        //    _ThermalVisionOn 按材质钉为 0，验证「按材质覆盖全局 uniform」在 v4.1.x 是否成立。
        //    属临时验证代码，结论确定后迁出为正式功能或独立插件。
        ThermalScope = Config.Bind("MGModClient", "ThermalScope Mode", "Material",
            new ConfigDescription(
                "热成像下瞄具可用性探针：Off=关闭；Material=按材质钉 0（推荐）；Global=全局钉 0（仅判别用）",
                new AcceptableValueList<string>("Off", "Material", "Global")));
        ThermalScope.SettingChanged += OnThermalScopeModeChanged;

        ThermalScopeProbeService.Mode = ThermalScope.Value;
        ThermalScopeProbeService.Logger = Logger;
        ThermalScopePatch.Install(Logger);

        Logger.LogInfo($"{PluginInfo.NAME} v{PluginInfo.VERSION} 初始化完成");
    }

    /// <summary>可选：手动指定 server 侧 config.json 路径（PathLocator 探测失败时用）。</summary>
    private ConfigEntry<string> ServerConfigPath => Config.Bind(
        "MGModClient", "Server Config Path", "",
        new ConfigDescription("Path to server-side res/config/config.json (empty = auto-detect)"));

    private MGConfig LoadModelFromDisk()
    {
        var cfg = new MGConfig();
        if (_pathLocator != null && _pathLocator.ConfigPath != null)
        {
            var onDisk = JsonUtils.Read<MGConfig>(_pathLocator.ConfigPath);
            if (onDisk != null)
            {
                cfg.CopyFrom(onDisk);
            }
            else
            {
                // 文件存在但解析失败 → 损坏；打日志警告，而非静默回退默认
                Logger.LogWarning($"[MGModClient] config.json 读取失败或损坏，已使用默认配置：{_pathLocator.ConfigPath}");
            }
        }
        return cfg;
    }

    private void OnThermalScopeModeChanged(object sender, System.EventArgs e)
    {
        ThermalScopeProbeService.Mode = ThermalScope.Value;
        ThermalScopeProbeService.ResetState();
        Logger.LogInfo($"[ThermalScope] 模式切换为 {ThermalScope.Value}");
    }

    private void OnLanguageChanged(object sender, System.EventArgs e)
    {
        // 值保真收集 → 移除重建 → Save → CM.BuildSettingList() 刷新（见落地方案 §5.3）
        Logger.LogInfo($"语言切换为 {Language.Value}（重建 ConfigEntry + 刷新菜单，待实现）");
        // TODO(M1): ConfigEntryRegistry.CollectValues → 移除重建 → Config.Save() → ConfigurationManagerBridge.BuildSettingList()
    }

    private void Update()
    {
        // 主流程仍无 Update 逻辑；热成像瞄具探针走 Harmony 补丁（见 ThermalScopePatch），不在此驱动。
    }
}
