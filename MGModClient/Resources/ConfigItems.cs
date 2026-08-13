using System.Collections.Generic;

namespace MGModClient.Resources;

/// <summary>
/// 配置项选项表（占位）——复用桌面版 Resources/ConfigItems.cs（含 ValueKey/ValueFormatKey）。
/// 当前为空骨架：选项值由后续编辑器功能按桌面版 ConfigItems.cs 填充。
/// </summary>
internal static class ConfigItems
{
    // 示例：public static readonly List<KeyValuePair<int, string>> RaidTime = new() { ... };
    public static readonly List<int> RaidTime = new() { 15, 30, 45, 60, 90, 120, 180 };
}
