using System;
using BepInEx.Logging;
using EFT.UI.DragAndDrop;
using EFT.Utilities;
using UnityEngine;

namespace MGModClient.Services;

/// <summary>
/// Rig Layouts 布局注入器 —— 把自定义弹挂/背包布局预制体（ContainedGridsView）注入客户端。
///
/// 机制：Rig Layouts 走 `Resources.Load("UI/Rig Layouts/<name>")`（Unity Resources 系统），
/// 不经过 SPT 的 AssetBundle 替换链路（EasyBundlePatch 只能命中游戏本就请求的 bundle key），
/// 因此必须由本插件自行加载 bundle 并把预制体塞进 `ResourcesCache._storage`。
///
/// 本类作为「资源类型处理器」被 <see cref="ClientResourceLoader"/> 调用：
///  - 由 Loader 负责检索 bundle 文件，本类负责「给定预制体 → 注入 ResourcesCache」；
///  - 键 = "UI/Rig Layouts/" + 预制体名（须与物品 JSON 的 RigLayoutName/GridLayoutName 一致）；
///  - 用 TryAdd 防重复注入（不覆盖已存在的键）。
/// </summary>
public static class RigLayoutInjector
{
    /// <summary>
    /// 尝试注入单个布局预制体。带 ContainedGridsView 的才注入，返回是否成功注入。
    /// </summary>
    public static bool TryInjectPrefab(GameObject prefab, ManualLogSource logger)
    {
        var gridView = prefab.GetComponent<ContainedGridsView>();
        if (gridView == null)
        {
            logger.LogInfo($"[RigLayoutInjector] 跳过非布局预制体: {prefab.name}（无 ContainedGridsView）");
            return false;
        }

        // 键 = "UI/Rig Layouts/" + 预制体名（须与物品 JSON 的 RigLayoutName 一致）
        var key = "UI/Rig Layouts/" + prefab.name;
        if (ResourcesCache._storage.TryAdd(key, gridView))
        {
            logger.LogInfo($"[RigLayoutInjector] 布局已注入: {key}");
            return true;
        }
        logger.LogWarning($"[RigLayoutInjector] {key} 已存在，跳过（重复注入或与原版冲突）");
        return false;
    }
}
