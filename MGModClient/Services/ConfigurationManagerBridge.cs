using System;
using BepInEx;
using BepInEx.Bootstrap;

namespace MGModClient.Services;

/// <summary>
/// ConfigurationManager 桥：反射调用 BuildSettingList()（语言切换后刷新 F12 菜单）。
/// 不硬引用插件 dll（其可能缺省/升级），通过字符串名反射定位（落地方案 §5.3）。
/// </summary>
public static class ConfigurationManagerBridge
{
    public static void BuildSettingList()
    {
        object cm = null;
        if (Chainloader.PluginInfos.TryGetValue("BepInEx.cfg", out var info))
            cm = info.Instance;
        if (cm == null)
            cm = UnityEngine.Object.FindObjectOfType(
                Type.GetType("ConfigurationManager.ConfigurationManager, ConfigurationManager"));
        cm?.GetType().GetMethod("BuildSettingList")?.Invoke(cm, null);
    }
}
