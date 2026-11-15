using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using MGModClient.Framework;
using UnityEngine;

namespace MGModClient.Services;

/// <summary>
/// 客户端资源加载器 —— 检索**所有已识别 mod** 的 `bundles/resources/` 下全部 .bundle（含子文件夹递归），
/// 按资源类型分发到对应的注入器。
///
/// 背景：SPT 的 bundle 链路（BundleManager/EasyBundlePatch）只服务「游戏本会请求的 AssetBundle key」
/// （如物品 Prefab 路径 assets/content/...），而塔科夫 Unity 中走 **Resources 系统（Resources.Load）**
/// 的资源（UI/Rig Layouts、Slots、Prefabs/UGUI/Layouts、语音等）不经过该链路——这类资源必须由客户端
/// 插件自行 `AssetBundle.LoadFromFile` 加载后注入 `ResourcesCache._storage`。
///
/// 目录约定（与 SPT 链路完全隔离）：
///  - 服务端 mod 的客户端资源统一放 `{mod}/bundles/resources/`（对应 Unity Resources 系统语义）；
///  - 资源按类型分子文件夹：`resources/rig/`（弹挂/背包布局）、`resources/slots/`（槽位图标，预留）、
///    `resources/voices/`（语音，预留）等；
///  - 本加载器递归遍历 `resources/` 下所有 .bundle（不限定子目录/文件名），新增类型无需改扫描逻辑，
///    只需在 <see cref="ProcessBundle"/> 里追加对应类型的处理分支。
///
/// mod 来源统一取自 <see cref="ModRegistry"/>，本类不再出现任何具体 mod 名。
/// </summary>
public static class ClientResourceLoader
{
    /// <summary>客户端资源根目录名（{mod}/bundles/ 下，对应 Unity Resources 系统）。</summary>
    public const string ResourcesDirName = ModInstallation.ResourcesDirName;

    /// <summary>尝试加载并注入全部客户端资源（遍历所有声称具备该能力的已装 mod）。失败仅日志不崩溃。</summary>
    public static bool LoadAll(ManualLogSource logger)
    {
        int totalLoaded = 0;
        var mods = ModRegistry.With(ModCapabilities.ClientResources).ToList();

        if (mods.Count == 0)
        {
            logger.LogWarning("[ClientResourceLoader] 未识别到提供客户端资源的 mod，跳过资源加载");
            return false;
        }

        foreach (var mod in mods)
        {
            var resourcesDir = mod.ClientResourcesDir;
            if (string.IsNullOrEmpty(resourcesDir) || !Directory.Exists(resourcesDir))
            {
                logger.LogInfo($"[ClientResourceLoader] [{mod.Id}] 无资源目录，跳过: {resourcesDir}");
                continue;
            }

            logger.LogInfo($"[ClientResourceLoader] [{mod.Id}] 检索资源目录: {resourcesDir}");

            // 递归枚举 resources/ 下全部 .bundle（含子文件夹，按资源类型分子目录）
            string[] bundleFiles;
            try
            {
                bundleFiles = Directory.GetFiles(resourcesDir, "*.bundle", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                logger.LogError($"[ClientResourceLoader] [{mod.Id}] 枚举资源目录失败: {ex.GetType().Name}: {ex.Message}");
                continue;
            }

            logger.LogInfo($"[ClientResourceLoader] [{mod.Id}] 下找到 {bundleFiles.Length} 个 .bundle");
            foreach (var bundlePath in bundleFiles)
            {
                if (ProcessBundle(bundlePath, logger)) totalLoaded++;
            }
        }

        logger.LogInfo($"[ClientResourceLoader] 完成：共处理 {totalLoaded} 个 bundle");
        return totalLoaded > 0;
    }

    /// <summary>加载单个 bundle 并按资源类型分发（rig 布局已实现；Slots/Voices 等预留）。</summary>
    private static bool ProcessBundle(string bundlePath, ManualLogSource logger)
    {
        var bundleName = Path.GetFileName(bundlePath);
        logger.LogInfo($"[ClientResourceLoader] 加载 AssetBundle: {bundleName} ({new FileInfo(bundlePath).Length} bytes)");
        var bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null)
        {
            logger.LogError($"[ClientResourceLoader] AssetBundle 加载失败: {bundleName} —— 文件损坏或非 Unity AssetBundle 格式");
            return false;
        }
        logger.LogInfo($"[ClientResourceLoader] AssetBundle 加载成功: {bundleName}");

        bool any = false;
        // ① 布局预制体（ContainedGridsView → UI/Rig Layouts/）——弹挂/背包自定义布局
        foreach (var prefab in bundle.LoadAllAssets<GameObject>())
        {
            if (RigLayoutInjector.TryInjectPrefab(prefab, logger)) any = true;
        }
        // ② 预留：槽位图标（Sprite → Slots/）、语音（AudioClip → Voices/）等
        //    var sprites = bundle.LoadAllAssets<Sprite>();   → SlotIconInjector.Inject(sprites, logger)
        //    var clips   = bundle.LoadAllAssets<AudioClip>(); → VoiceInjector.Inject(clips, logger)

        bundle.Unload(false); // 资源已注入 ResourcesCache，卸载 bundle 本体保留资源
        return any;
    }
}
