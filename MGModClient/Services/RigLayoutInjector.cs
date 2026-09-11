using System;
using BepInEx.Logging;
using EFT.UI.DragAndDrop;
using UnityEngine;

namespace MGModClient.Services;

/// <summary>
/// Rig Layouts 布局注入器 —— 把自定义弹挂/背包布局预制体（ContainedGridsView）注入客户端。
///
/// 机制：Rig Layouts 走 `Resources.Load("UI/Rig Layouts/<name>")`（Unity Resources 系统），
/// 不经过 SPT 的 AssetBundle 替换链路（EasyBundlePatch 只能命中游戏本就请求的 bundle key），
/// 因此必须由本插件自行加载 bundle 并把预制体塞进 `CacheResourcesPopAbstractClass.Dictionary_0`
/// （即旧版本 `ResourcesCache._storage`，4.0.13 游戏内类名已变更为 CacheResourcesPopAbstractClass）。
///
/// 本类作为「资源类型处理器」被 <see cref="ClientResourceLoader"/> 调用：
///  - 由 Loader 负责检索 bundle 文件，本类负责「给定预制体 → 注入资源缓存」；
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
        var storage = CacheResourcesPopAbstractClass.Dictionary_0;
        if (storage.TryGetValue(key, out var existing) && existing != null)
        {
            logger.LogWarning($"[RigLayoutInjector] {key} 已存在原版资源，跳过（防覆盖）");
            return false;
        }
        // 直接赋值注入：若游戏此前 Pop 未命中已把 null 缓存进字典（CacheResourcesPopAbstractClass.Pop 未命中也会 Add），
        // TryAdd 会静默失败，这里用覆盖修正；存在非空原版资源时上方已拦截
        storage[key] = gridView;
        logger.LogInfo($"[RigLayoutInjector] 布局已注入: {key}");
        return true;
    }
}
