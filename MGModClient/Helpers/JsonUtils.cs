using System;
using System.IO;
using Newtonsoft.Json;

namespace MGModClient.Services;

/// <summary>
/// JSON 读写封装（Newtonsoft.Json，与游戏/桌面版生态一致）。
/// 原子写：写 .tmp 后 File.Replace，避免半写文件被服务端读到（落地方案 §3.3）。
/// </summary>
public static class JsonUtils
{
    private static readonly JsonSerializerSettings Settings = new() { Formatting = Formatting.Indented };

    public static T Read<T>(string path) where T : class, new()
    {
        try
        {
            if (!File.Exists(path)) return null;
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), Settings);
        }
        catch (Exception)
        {
            return null; // 解析失败返回 null，不崩溃（调用方决定回退）
        }
    }

    public static void WriteAtomic<T>(string path, T value)
    {
        var tmp = path + ".tmp";
        File.WriteAllText(tmp, JsonConvert.SerializeObject(value, Settings));
        File.Replace(tmp, path, null);
    }
}
