using System.Collections.Generic;
using BepInEx.Configuration;
using MGModClient.Models;
using MGModClient.Resources;

namespace MGModClient.Services;

/// <summary>
/// 配置镜像：MGConfig 模型 ↔ ConfigEntry（F12 菜单）双向映射（落地方案 §3.2）。
/// 规则：标量 → 1 条目；EnableValue → value + enable 两条；Dictionary → 按键展开。
/// 当前为空骨架：BindAll/ApplyEntryToModel 由后续编辑器功能按 ConfigItems.cs 选项表填充。
/// </summary>
public class ConfigMirror
{
    private readonly MGConfig _model = new();
    private readonly Dictionary<string, ConfigEntryBase> _entries = new();
    private readonly Dictionary<string, System.Action<object>> _setters = new();

    public MGConfig Model => _model;

    /// <summary>为每个叶子配置项创建 ConfigEntry。TODO(M2): 按服务端模型 + ConfigItems 选项表实现。</summary>
    public void BindAll(ConfigFile cfg, MGConfig source)
    {
        _model.CopyFrom(source);
        // 示例：Locations.RaidTime（EnableValueDouble → enable + value）
        // var raidTime = cfg.Bind("MGEditor Locations", "RaidTime",
        //     _model.Locations.RaidTime.value,
        //     new ConfigDescription(Locales.Get("Locations.RaidTime"),
        //         new AcceptableValueList<double>(ConfigItems.RaidTime.Select(x => (double)x.Key).ToArray())));
        // _setters["Locations.RaidTime"] = v => { _model.Locations.RaidTime.value = (double)v; };
        // _entries["Locations.RaidTime"] = raidTime;
    }

    public void ApplyEntryToModel(ConfigEntryBase e)
    {
        if (_setters.TryGetValue(e.Definition.Key, out var setter))
            setter(e.BoxedValue);
    }

    public void ApplyAllToModel()
    {
        foreach (var e in _entries.Values) ApplyEntryToModel(e);
    }

    /// <summary>把模型值同步回各 ConfigEntry（写回后防回环用）。TODO(M2)。</summary>
    public void SyncEntriesFromModel() { }
}
