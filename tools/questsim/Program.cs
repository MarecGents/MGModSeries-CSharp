// 客户端任务条件反序列化 + 击杀判定模拟
// 引用真实游戏 Assembly-CSharp.dll，模拟客户端 ConditionSerializer 反序列化与击杀计数
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EFT.Quests;
using EFT.HealthSystem;
using questsim;

Console.WriteLine("=== 任务击杀条件反序列化模拟（客户端链路）===");
// ServerSim.Run();  // .NET10 环境 StringToNumberConverter 兼容问题，跳过
KillChainSim.Run();

// 从游戏 Managed 目录加载依赖程序集
AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
{
    var name = new AssemblyName(e.Name).Name;
    var path = Path.Combine(@"E:\Game\EFT4.1.2\EscapeFromTarkov_Data\Managed", name + ".dll");
    return File.Exists(path) ? Assembly.LoadFrom(path) : null;
};

var fgQuest = JObject.Parse(File.ReadAllText(@"E:\Workdata\Git_repositories\MGModSeries\MGModSeries-CSharp\MGGTMod\traders\FlanrecGents\templates\quests.json"));
var mgQuest = JObject.Parse(File.ReadAllText(@"E:\Workdata\Git_repositories\MGModSeries\MGModSeries-CSharp\MGModServer\traders\MarecGents\templates\quests.json"));

var quests = new (string Tag, JObject Data, string QuestId)[]
{
    ("FGQuest1", fgQuest, "9cc236084000000000000000"),
    ("FGQuest2", fgQuest, "9cc236084000000000000001"),
    ("FGQuest3", fgQuest, "9cc236084000000000000002"),
    ("MGTest1", mgQuest, "8ef5b2ef4000000000000000"),
    ("MG初面", mgQuest, "8ef5b2ef4000000000000001"),
};

// 1) 反序列化验证：按 conditionType 映射到客户端 Condition 类
// 与客户端一致的多态条件反序列化配置（EftJsonConverters）
var settings = new JsonSerializerSettings();
settings.Converters.Add(new EFT.PolymorphicConverter<ConditionSerializer, Condition, string>());

foreach (var (tag, data, qid) in quests)
{
    var q = (JObject)data[qid];
    var finish = (JArray)q["conditions"]["AvailableForFinish"];
    Console.WriteLine($"\n--- {tag} ({qid}) ---");
    foreach (var c in finish.OfType<JObject>())
    {
        string condType = c.Value<string>("conditionType") ?? "?";
        string clsName = "EFT.Quests.Condition" + condType;
        var type = Type.GetType(clsName + ", Assembly-CSharp");
        if (type == null)
        {
            Console.WriteLine($"  [FAIL] 无法映射 conditionType={condType} -> {clsName}");
            continue;
        }
        var cond = (Condition)c.ToObject(type, JsonSerializer.Create(settings));
        Console.WriteLine($"  [OK] {condType} -> {cond.GetType().Name} | id={cond.id} | value={cond.value}");

        if (cond is ConditionCounterCreator ccc)
        {
            var subs = ccc.Conditions;
            Console.WriteLine($"       counter 子条件数: {subs?.Count ?? -1}");
            if (subs != null)
            {
                foreach (var sc in subs)
                {
                    string extra = "";
                    if (sc is ConditionKills k) extra = $"target={k.target} savageRole={(k.savageRole == null ? "null" : string.Join(",", k.savageRole))}";
                    if (sc is ConditionLocation loc) extra = $"target={string.Join(",", loc.target ?? new string[0])}";
                    Console.WriteLine($"         - {sc.GetType().Name} | isNecessary={sc.IsNecessary} | {extra}");
                }
            }
        }
    }
}

// 2) 击杀判定模拟：构造击杀事件 → ConditionHitProgressChecker.Test → CounterCreator 计数
Console.WriteLine("\n=== 击杀判定模拟 ===");
var simSettings = JsonSerializer.Create(settings);
var fg1 = (JObject)fgQuest["9cc236084000000000000000"];
var elim = fg1["conditions"]["AvailableForFinish"].OfType<JObject>().First(c => c.Value<string>("type") == "Elimination");
var elimCond = (ConditionCounterCreator)elim.ToObject(typeof(ConditionCounterCreator), simSettings);
var killsSub = (ConditionKills)elimCond.Conditions.First();

Console.WriteLine($"FGQuest1 击杀条件: target={killsSub.target} (杀 scav 5)");
SimulateKill(elimCond, killsSub, "Savage", "assault");   // 杀 scav
SimulateKill(elimCond, killsSub, "Savage", "pmcBot");    // 杀掠夺者（Savage 阵营）
SimulateKill(elimCond, killsSub, "Usec", "usec");        // 杀 PMC（不应计入）
Console.WriteLine("（模拟结束）");

void SimulateKill(ConditionCounterCreator cond, ConditionKills kills, string killedSide, string scavRole)
{
    var hit = new HitConditionCheck
    {
        TargetSide = killedSide,
        Weapon = "5447b5cf4bdc2d27728b4568",
        WeaponMods = new string[0],
        TargetEquipment = new List<string>(),
        BodyPart = EBodyPart.Head,
        Value = 1,
        Distance = 0,
        ScavRole = scavRole,
        CurrentHour = 12,
        EnemyHealthEffects = null,
    };
    var checker = new ConditionHitProgressChecker(kills);
    bool pass = checker.Test(hit);
    Console.WriteLine($"  击杀({killedSide},{scavRole}) -> Kills条件判定: {(pass ? "命中 ✓" : "不命中 ✗")}");
}
