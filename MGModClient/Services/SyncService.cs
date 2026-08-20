using System;
using BepInEx.Configuration;
using MGModClient.Models;

namespace MGModClient.Services;

/// <summary>
/// 写回服务：ConfigEntry 变更 → 原子写回 server 侧 config.json（落地方案 §3.3）。
/// - 500ms 防抖（拖动滑块不频繁写盘）
/// - 写前重读磁盘（防覆盖外部修改），saveTime 作版本戳
/// - _suppress 防回环
/// </summary>
public class SyncService
{
    private readonly ConfigMirror _mirror;
    private readonly PathLocator _pathLocator;
    private bool _suppress;
    private string _lastSavedTime;

    public SyncService(ConfigMirror mirror, PathLocator pathLocator)
    {
        _mirror = mirror;
        _pathLocator = pathLocator;
    }

    public void OnEntryChanged(object sender, SettingChangedEventArgs e)
    {
        if (_suppress) return;
        _mirror.ApplyEntryToModel(e.ChangedSetting);
        // TODO(M2): 500ms 防抖后 WriteBack()
        WriteBack();
    }

    private void WriteBack()
    {
        try
        {
            if (_pathLocator.ConfigPath == null) return;

            // ① 重读磁盘现有文件（防覆盖桌面版/其他进程的修改）
            var onDisk = JsonUtils.Read<MGConfig>(_pathLocator.ConfigPath);
            if (onDisk == null)
            {
                // 文件缺失或损坏：不写回，避免用默认空模型覆盖 config.json
                UnityEngine.Debug.LogWarning($"[MGModClient][SyncService] config.json 读取失败或损坏，跳过写回：{_pathLocator.ConfigPath}");
                return;
            }
            if (onDisk.saveTime != _lastSavedTime)
                _mirror.Model.CopyFrom(onDisk); // 以磁盘为事实源合并外部变更（简化版）

            // ② 更新版本戳并原子写
            _mirror.Model.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            JsonUtils.WriteAtomic(_pathLocator.ConfigPath, _mirror.Model);
            _lastSavedTime = _mirror.Model.saveTime;

            // ③ 防回环：把模型值同步回 ConfigEntry
            _suppress = true;
            try { _mirror.SyncEntriesFromModel(); }
            finally { _suppress = false; }
        }
        catch (Exception ex)
        {
            // 写盘失败（文件锁/磁盘错误）不应波及 F12 操作，仅日志
            UnityEngine.Debug.LogError($"[MGModClient][SyncService] 写回 config.json 失败: {ex.Message}");
        }
    }
}
