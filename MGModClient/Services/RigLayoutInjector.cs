using System;
using System.IO;
using BepInEx.Logging;
using EFT.UI.DragAndDrop;
using EFT.Utilities;
using UnityEngine;

namespace MGModClient.Services;

/// <summary>
/// Rig Layouts 布局注入器 —— 把 FG_Alpha/FG_RBAV 自定义弹挂布局预制体注入客户端。
///
/// 机制：Rig Layouts 走 `Resources.Load("UI/Rig Layouts/<name>")`（Unity Resources 系统），
/// 不经过 SPT 的 AssetBundle 替换链路（EasyBundlePatch 只能命中游戏本就请求的 bundle key），
/// 因此必须由本插件自行 `AssetBundle.LoadFromFile` 加载 bundle，并把预制体塞进
/// `ResourcesCache._storage`。（WTT-ClientCommonLib/RigLayoutManager、EternalCycleClient 同款做法）
///
/// 加载策略（与 SPT 的 bundles.json 链路完全隔离）：
///  - 遍历 MGGTMod/bundles/ 下指定分类目录里**全部 *.bundle**（不做文件名硬编码）；
///  - 每个 bundle 用 `LoadAllAssets&lt;GameObject&gt;` 取出所有预制体，
///    只注入带 `ContainedGridsView` 组件的（键 = "UI/Rig Layouts/" + 预制体名，
///    预制体名须与物品 JSON 的 RigLayoutName 一致）；
///  - 用 TryAdd 防重复注入（不覆盖已存在的键）。
///
/// 日志：全程 LogInfo/LogWarning/LogError 输出（目录定位/文件枚举/bundle 加载/资源统计/注入结果），
/// 可在游戏内 BepInEx 控制台（~ 键）查看。
/// </summary>
public static class RigLayoutInjector
{
    /// <summary>rig 布局 bundle 所在分类子目录（MGGTMod/bundles/ 下）。</summary>
    public const string BundleDirSubPath = "rig";

    /// <summary>尝试加载并注入 FG 布局预制体。失败仅日志不崩溃。</summary>
    public static bool TryInject(ManualLogSource logger)
    {
        try
        {
            // ① 定位分类目录（ModDetector 基于游戏目录 + 可选覆盖）
            var bundleDir = ModDetector.GetMGGTBundleDir(BundleDirSubPath);
            logger.LogInfo($"[RigLayoutInjector] 定位 bundle 目录: {bundleDir ?? "(null)"}");
            if (string.IsNullOrEmpty(bundleDir) || !Directory.Exists(bundleDir))
            {
                logger.LogWarning($"[RigLayoutInjector] bundle 目录不存在: {bundleDir}（服务端可能未下发，或 MGGTMod 未构建发布新 bundle）");
                return false;
            }

            // ② 遍历目录下全部 .bundle（不绑定具体文件名，新增布局无需改代码）
            var bundleFiles = Directory.GetFiles(bundleDir, "*.bundle", SearchOption.TopDirectoryOnly);
            logger.LogInfo($"[RigLayoutInjector] {bundleDir} 下找到 {bundleFiles.Length} 个 .bundle");
            if (bundleFiles.Length == 0)
            {
                logger.LogWarning($"[RigLayoutInjector] {bundleDir} 下没有 .bundle 文件");
                return false;
            }

            int injected = 0;
            foreach (var bundlePath in bundleFiles)
            {
                var bundleName = Path.GetFileName(bundlePath);
                logger.LogInfo($"[RigLayoutInjector] 尝试加载 AssetBundle: {bundleName} ({new FileInfo(bundlePath).Length} bytes)");
                var bundle = AssetBundle.LoadFromFile(bundlePath);
                if (bundle == null)
                {
                    logger.LogError($"[RigLayoutInjector] AssetBundle 加载失败（返回 null）: {bundleName} —— 文件损坏或非 Unity AssetBundle 格式");
                    continue;
                }
                logger.LogInfo($"[RigLayoutInjector] AssetBundle 加载成功: {bundleName}");

                // ③ 取出 bundle 内全部 GameObject 预制体，只注入带 ContainedGridsView 的
                var gameObjects = bundle.LoadAllAssets<GameObject>();
                logger.LogInfo($"[RigLayoutInjector] {bundleName} 内 GameObject 资源数: {gameObjects.Length}");
                if (gameObjects.Length == 0)
                {
                    logger.LogWarning($"[RigLayoutInjector] {bundleName} 内没有任何 GameObject —— bundle 可能是空包（预制体未被打入）");
                    continue;
                }

                foreach (var prefab in gameObjects)
                {
                    var gridView = prefab.GetComponent<ContainedGridsView>();
                    if (gridView == null)
                    {
                        logger.LogInfo($"[RigLayoutInjector]   跳过非布局预制体: {prefab.name}（无 ContainedGridsView）");
                        continue; // 非布局预制体，跳过
                    }

                    // 键 = "UI/Rig Layouts/" + 预制体名（须与物品 JSON 的 RigLayoutName 一致）
                    var key = "UI/Rig Layouts/" + prefab.name;
                    if (ResourcesCache._storage.TryAdd(key, gridView))
                    {
                        injected++;
                        logger.LogInfo($"[RigLayoutInjector] 布局已注入: {key}（来自 {bundleName}）");
                    }
                    else
                    {
                        logger.LogWarning($"[RigLayoutInjector] {key} 已存在，跳过（重复注入或与原版冲突）");
                    }
                }
            }

            logger.LogInfo($"[RigLayoutInjector] 完成：共注入 {injected} 个布局");
            return injected > 0;
        }
        catch (Exception ex)
        {
            logger.LogError($"[RigLayoutInjector] 注入异常: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }
}
