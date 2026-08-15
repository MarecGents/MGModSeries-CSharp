// 完整击杀链路模拟：CounterCreator 的 MatchIdentities + TestAll + 计数（模拟客户端 ConditionCounterManager.Test）
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EFT.Quests;

namespace questsim;

public static class KillChainSim
{
    public static void Run()
    {
        Console.WriteLine("\n=== 完整击杀链路模拟（CounterCreator 计数）===");

        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new EFT.PolymorphicConverter<ConditionSerializer, Condition, string>());
        var ser = JsonSerializer.Create(settings);

        var fgQuest = JObject.Parse(File.ReadAllText(@"E:\Workdata\Git_repositories\MGModSeries\MGModSeries-CSharp\MGGTMod\traders\FlanrecGents\templates\quests.json"));
        var mgQuest = JObject.Parse(File.ReadAllText(@"E:\Workdata\Git_repositories\MGModSeries\MGModSeries-CSharp\MGModServer\traders\MarecGents\templates\quests.json"));

        var cases = new (string Name, JObject Data, string Qid)[]
        {
            ("FGQuest1 杀5scav", fgQuest, "9cc236084000000000000000"),
            ("FGQuest2 灯塔杀10游荡者", fgQuest, "9cc236084000000000000001"),
            ("FGQuest3 实验室杀10PMC", fgQuest, "9cc236084000000000000002"),
            ("MG初面 杀10掠夺者+5PMC(实验室)", mgQuest, "8ef5b2ef4000000000000001"),
        };

        foreach (var (name, data, qid) in cases)
        {
            var q = (JObject)data[qid];
            Console.WriteLine($"\n--- {name} ---");
            foreach (var c in q["conditions"]["AvailableForFinish"].OfType<JObject>())
            {
                if (c.Value<string>("type") != "Elimination") continue;
                var ccc = (ConditionCounterCreator)c.ToObject(typeof(ConditionCounterCreator), ser);
                var subs = ccc.Conditions.ToList();
                var kills = subs.OfType<ConditionKills>().First();
                var loc = subs.OfType<ConditionLocation>().FirstOrDefault();
                string locStr = loc == null ? "任意" : string.Join(",", loc.target ?? new string[0]);

                int killsNeeded = (int)ccc.value;
                int count = 0;
                // 模拟击杀直到达到外层 value（每次击杀：客户端用目标侧的完整标签列表多次调用 CheckKillConditionCounter）
                var killPool = BuildKillPool(kills.target);
                for (int i = 0; i < 30 && count < killsNeeded; i++)
                {
                    bool counted = false;
                    foreach (var (side, role, locationId) in killPool)
                    {
                        foreach (var tag in BuildTargetTags(side))   // 客户端 BaseStatisticsManager 的 list
                        {
                            if (KillsMatch(kills, tag, role) && LocationMatch(loc, locationId))
                            {
                                count++;
                                counted = true;
                                break;
                            }
                        }
                        if (counted) break;
                    }
                    if (!counted) break;
                }
                Console.WriteLine($"  target={kills.target} loc={locStr} 需求={killsNeeded}: 模拟击杀后计数={count} -> {(count >= killsNeeded ? "任务可完成 ✓" : "无法完成 ✗")}");
            }
        }
    }

    // 生成候选击杀池：Savage 阵营(assault/assaultGroup/pmcBot/exUsec/...)、Usec、Bear
    static (string, string, string)[] BuildKillPool(string target)
    {
        var pool = new List<(string, string, string)>();
        if (target == "Savage")
        {
            foreach (var role in new[] { "assault", "assaultGroup", "pmcBot", "exUsec", "marksman", "cursedAssault" })
                pool.Add(("Savage", role, "laboratory"));
            // 灯塔场景
            pool.Add(("Savage", "exUsec", "Lighthouse"));
        }
        else if (target == "AnyPmc")
        {
            pool.Add(("Usec", "usec", "laboratory"));
            pool.Add(("Bear", "bear", "laboratory"));
            pool.Add(("Usec", "usec", "Lighthouse"));
            pool.Add(("Bear", "bear", "Lighthouse"));
        }
        else if (target == "Any")
        {
            pool.Add(("Savage", "assault", "laboratory"));
            pool.Add(("Usec", "usec", "laboratory"));
        }
        return pool.ToArray();
    }

    static string[] BuildTargetTags(string side)
    {
        // 模拟 BaseStatisticsManager 击杀事件打标签：击杀侧 -> 多个 target 调用
        return side switch
        {
            "Usec" => new[] { "Usec", "AnyPmc", "Enemy", "Any" },
            "Bear" => new[] { "Bear", "AnyPmc", "Enemy", "Any" },
            "Savage" => new[] { "Savage", "Bot", "Any" },
            _ => new[] { "Any" },
        };
    }

    static bool KillsMatch(ConditionKills cond, string side, string role)
    {
        var hit = new HitConditionCheck
        {
            TargetSide = side, Weapon = "x", WeaponMods = new string[0],
            TargetEquipment = new List<string>(), BodyPart = EBodyPart.Head,
            Value = 1, Distance = 0, ScavRole = role, CurrentHour = 12, EnemyHealthEffects = null,
        };
        return new ConditionHitProgressChecker(cond).Test(hit);
    }

    static bool LocationMatch(ConditionLocation cond, string locationId)
    {
        if (cond == null) return true;
        return new ConditionLocationProgressChecker(cond).Test(locationId);
    }
}
