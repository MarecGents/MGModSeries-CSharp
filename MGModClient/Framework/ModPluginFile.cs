using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MGModClient.Framework;

/// <summary>单个 mod 的插件文件读取结果（含失败原因，供日志排查）。</summary>
public sealed class ModPluginReadResult
{
    public string ModId { get; set; }
    public string FilePath { get; set; }
    /// <summary>ok / empty / missing / invalid</summary>
    public string Status { get; set; }
    public string Detail { get; set; }
    public List<string> Ids { get; } = new();
    public int InvalidCount { get; set; }

    public override string ToString()
    {
        var tail = string.IsNullOrEmpty(Detail) ? "" : $" ({Detail})";
        return $"{ModId}={Ids.Count}{tail}";
    }
}

/// <summary>多 mod 合并结果。</summary>
public sealed class AggregatedPluginIds
{
    public HashSet<string> Ids { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<ModPluginReadResult> PerMod { get; } = new();
    public int SourceCount => PerMod.Count;
    public int Total => Ids.Count;
}

/// <summary>
/// 读取服务端 mod 产出的插件清单（res/plugins/*.json）并跨 mod 合并。
///
/// 数据契约（由 MGModServer / MGGTMod 的 CustomTraderServices 产出）：
///   ["8ef5b2ef0000000000000022", "8ef5b2ef0000000000000023"]
/// 容错：
///   - 文件缺失 / 空文件 / 空数组 → 贡献 0 条，状态分别标记
///   - 元素非法（非 24 位 hex）→ 跳过并计数（不影响其它元素）
///   - 同时兼容对象形态 { "scope_allow_in_thermal": [...] }（防未来结构变化）
/// </summary>
public static class ModPluginFile
{
    /// <summary>热成像瞄具白名单文件名（服务端 Paths.TraderItemsSopeAllowInThermal）。</summary>
    public const string ScopeAllowInThermal = "scope_allow_in_thermal.json";

    /// <summary>读取单个 mod 的清单文件。</summary>
    public static ModPluginReadResult ReadIdList(ModInstallation mod, string fileName)
    {
        var result = new ModPluginReadResult { ModId = mod?.Id ?? "?", FilePath = mod?.PluginsFile(fileName) };

        if (mod == null || string.IsNullOrEmpty(fileName))
        {
            result.Status = "invalid";
            result.Detail = "参数为空";
            return result;
        }

        try
        {
            if (!File.Exists(result.FilePath))
            {
                result.Status = "missing";
                return result;
            }

            var text = File.ReadAllText(result.FilePath);
            if (string.IsNullOrWhiteSpace(text))
            {
                result.Status = "empty";
                return result;
            }

            JToken root;
            try
            {
                root = JToken.Parse(StripBom(text));
            }
            catch (Exception ex)
            {
                result.Status = "invalid";
                result.Detail = $"JSON 解析失败: {ex.GetType().Name}";
                return result;
            }

            var array = ExtractArray(root, fileName);
            if (array == null)
            {
                result.Status = "invalid";
                result.Detail = "未找到 id 数组（既不是数组，也没有同名属性）";
                return result;
            }

            foreach (var token in array)
            {
                var raw = token?.Type == JTokenType.String
                    ? (string)token
                    : token?.ToString();

                var id = Normalize(raw);
                if (id == null)
                {
                    result.InvalidCount++;
                    continue;
                }
                if (!result.Ids.Contains(id)) result.Ids.Add(id);
            }

            result.Status = "ok";
            if (result.InvalidCount > 0) result.Detail = $"跳过 {result.InvalidCount} 个非法 id";
            return result;
        }
        catch (Exception ex)
        {
            result.Status = "invalid";
            result.Detail = $"{ex.GetType().Name}: {ex.Message}";
            return result;
        }
    }

    /// <summary>跨多个 mod 读取并合并（去重，忽略大小写）。</summary>
    public static AggregatedPluginIds ReadMerged(IEnumerable<ModInstallation> mods, string fileName)
    {
        var aggregated = new AggregatedPluginIds();
        if (mods == null) return aggregated;

        foreach (var mod in mods)
        {
            var one = ReadIdList(mod, fileName);
            aggregated.PerMod.Add(one);
            foreach (var id in one.Ids) aggregated.Ids.Add(id);
        }

        return aggregated;
    }

    // ==================== 内部实现 ====================

    /// <summary>取出 id 数组：根数组，或对象里与文件名同名 / ids / items 的属性。</summary>
    private static JArray ExtractArray(JToken root, string fileName)
    {
        if (root is JArray array) return array;
        if (root is not JObject obj) return null;

        var candidates = new[]
        {
            Path.GetFileNameWithoutExtension(fileName),
            "ids",
            "items",
            "list",
        };

        foreach (var name in candidates)
        {
            if (obj.TryGetValue(name, StringComparison.OrdinalIgnoreCase, out var value) && value is JArray found)
                return found;
        }

        // 只有一个数组属性时也接受
        var arrays = obj.Properties().Where(p => p.Value is JArray).Select(p => (JArray)p.Value).ToList();
        return arrays.Count == 1 ? arrays[0] : null;
    }

    /// <summary>规范化单个 id：去空白/引号，必须是 24 位十六进制（EFT MongoId）。</summary>
    private static string Normalize(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var id = raw.Trim().Trim('"', '\'').Trim();
        if (id.Length != 24) return null;
        foreach (var c in id)
        {
            var isHex = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
            if (!isHex) return null;
        }
        return id.ToLowerInvariant();
    }

    private static string StripBom(string text)
        => text.Length > 0 && text[0] == '\uFEFF' ? text.Substring(1) : text;
}
