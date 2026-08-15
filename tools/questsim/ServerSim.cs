// 服务端链路模拟：用 SPT Quest 模型反序列化我们的 quests.json，再序列化，检查 Elimination 条件完整性
using System.Text.Json;
using System.IO;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Common;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace questsim;

public static class ServerSim
{
    public static void Run()
    {
        Console.WriteLine("=== 服务端 Quest 模型 反序列化+序列化 模拟 ===");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false, WriteIndented = true };
        var converterType = Type.GetType("SPTarkov.Server.Core.Utils.Json.Converters.StringToNumberFactoryConverter, SPTarkov.Server.Core");
        if (converterType == null)
        {
            Console.WriteLine("[FAIL] 找不到 StringToNumberFactoryConverter");
            return;
        }
        options.Converters.Add((JsonConverter)Activator.CreateInstance(converterType));
        var mongoCvt = Type.GetType("SPTarkov.Server.Core.Utils.Json.Converters.StringToMongoIdConverter, SPTarkov.Server.Core");
        if (mongoCvt != null) options.Converters.Add((JsonConverter)Activator.CreateInstance(mongoCvt));
        var listOrT = Type.GetType("SPTarkov.Server.Core.Utils.Json.Converters.ListOrTConverter`2, SPTarkov.Server.Core");
        var eftEnum = Type.GetType("SPTarkov.Server.Core.Utils.Json.Converters.EftEnumConverter`2, SPTarkov.Server.Core");
        var listOrT1 = Type.GetType("SPTarkov.Server.Core.Utils.Json.Converters.ListOrTConverter`1, SPTarkov.Server.Core");
        string[] files =
        {
            @"E:\Workdata\Git_repositories\MGModSeries\MGModSeries-CSharp\MGGTMod\traders\FlanrecGents\templates\quests.json",
            @"E:\Workdata\Git_repositories\MGModSeries\MGModSeries-CSharp\MGModServer\traders\MarecGents\templates\quests.json",
        };
        foreach (var f in files)
        {
            Console.WriteLine($"\n=== {System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(f))} ===");
            try
            {
                var quests = JsonSerializer.Deserialize<Dictionary<MongoId, Quest>>(File.ReadAllText(f), options);
                Console.WriteLine($"  反序列化 OK: {quests.Count} 个任务");
                var check = JsonDocument.Parse(JsonSerializer.Serialize(quests, options));
                foreach (var q in check.RootElement.EnumerateObject())
                {
                    if (!q.Value.TryGetProperty("conditions", out var conds) ||
                        !conds.TryGetProperty("AvailableForFinish", out var finish))
                        continue;
                    foreach (var c in finish.EnumerateArray())
                    {
                        if (c.TryGetProperty("type", out var t) && t.GetString() == "Elimination")
                        {
                            bool hasCounter = c.TryGetProperty("counter", out var counter);
                            bool hasCondType = c.TryGetProperty("conditionType", out var ct);
                            int subCount = 0; string subs = "";
                            if (hasCounter && counter.TryGetProperty("conditions", out var condArr))
                            {
                                subCount = condArr.GetArrayLength();
                                foreach (var sc in condArr.EnumerateArray())
                                {
                                    var st = sc.TryGetProperty("conditionType", out var sct) ? sct.GetString() : "?";
                                    var sv = sc.TryGetProperty("value", out var scv) ? scv.ToString() : "?";
                                    var tg = sc.TryGetProperty("target", out var sgt) ? sgt.ToString() : "?";
                                    subs += $" [{st} v={sv} t={tg}]";
                                }
                            }
                            var v = c.TryGetProperty("value", out var cv) ? cv.ToString() : "?";
                            Console.WriteLine($"  {q.Name} Elimination: value={v} conditionType={(hasCondType ? ct.GetString() : "缺失!")} counter={(hasCounter ? "OK" : "缺失!")} 子条件数={subCount}{subs}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] {ex.Message}");
            }
        }
    }
}
