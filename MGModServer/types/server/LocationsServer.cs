using _MGMod.types.models.Custom;
using _MGMod.types.models.EFT.locations;
using _MGMod.types.models.EFT.templetes;
using _MGMod.types.models.Paths;
using _MGMod.types.services;
using _MGMod.types.utils;
using SPTarkov.Common.Logger;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Tables;
using Color = Spectre.Console.Color;

namespace _MGMod.types.server;

[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class LocationsServer(
    DatabaseServices databaseServices,
    MGUtils mGUtils
    )
{
    public Dictionary<string, Location> GetLocations()
    {
        return databaseServices.GetLocations().GetDictionary();
    }
    
    public Location? GetLocation(string mapId)
    {
        return databaseServices.GetLocations().GetLocation(mapId);
    }
    
    public int AddLooseLoot(PathType path)
    {
        Dictionary<string, CustomLooseLoot>? looseLootDict;
        try
        {
            looseLootDict = mGUtils.GetJsonDataFromFile<Dictionary<string, CustomLooseLoot>>(path);
        }
        catch (Exception ex)
        {
            // 单个商人的散落物资文件损坏，不应影响该商人其余内容，也不应中断后续商人加载。
            Log($"散落物资：looseLoot.json 解析失败，已跳过注入：{ex.GetType().Name}: {ex.Message}", Color.Red);
            return 0;
        }

        if (looseLootDict is null || looseLootDict.Count == 0) return 0;

        int registered = 0;
        foreach (var (mapId, looseLoot) in looseLootDict)
        {
            if (looseLoot is null || string.IsNullOrWhiteSpace(mapId)) continue;
            registered += AddLooseLootPositions(mapId, looseLoot);
        }
        return registered;
    }
    
    public int AddLooseLootPositions(string mapId, CustomLooseLoot looseLoot)
    {
        Location? location = GetLocation(mapId);
        if (location is null)
        {
            Log($"散落物资：地图[{mapId}]不存在于游戏数据库，已跳过。", Color.Cyan);
            return 0;
        }

        if (location.LooseLoot is null)
        {
            // 该地图在 SPT_Data/database/locations/<map>/ 下没有 looseLoot.json
            // （develop / hideout / privatearea / suburbs / terminal / town）。
            Log($"散落物资：地图[{mapId}]没有 looseLoot 数据（该地图不支持散落物资），已跳过。", Color.Cyan);
            return 0;
        }

        List<Spawnpoint> dynamicPoints = CollectValidSpawnpoints(mapId, looseLoot.spawnpoints, true);
        List<Spawnpoint> forcedPoints = CollectValidSpawnpoints(mapId, looseLoot.spawnpointsForced, false);
        if (dynamicPoints.Count == 0 && forcedPoints.Count == 0) return 0;

        location.LooseLoot.AddTransformer(loot =>
        {
            if (loot is null) return null;

            loot.Spawnpoints = MergeSpawnpoints(loot.Spawnpoints, dynamicPoints);
            loot.SpawnpointsForced = MergeSpawnpoints(loot.SpawnpointsForced, forcedPoints);
            return loot;
        });

        // Log($"散落物资：地图[{mapId}]已注册注入（动态 {dynamicPoints.Count} 个 / 强制 {forcedPoints.Count} 个）。", Color.Yellow);
        return dynamicPoints.Count + forcedPoints.Count;
    }
    
    private List<Spawnpoint> CollectValidSpawnpoints(string mapId, List<Spawnpoint>? source, bool requireItemDistribution)
    {
        List<Spawnpoint> valid = new List<Spawnpoint>();
        if (source is null) return valid;

        foreach (Spawnpoint spawnpoint in source)
        {
            if (spawnpoint?.Template is null || string.IsNullOrWhiteSpace(spawnpoint.Template.Id))
            {
                Log($"散落物资：地图[{mapId}]存在缺少 template.Id 的刷新点，已丢弃。", Color.Cyan);
                continue;
            }

            if (spawnpoint.Template.Items is null || !spawnpoint.Template.Items.Any())
            {
                Log($"散落物资：地图[{mapId}]刷新点[{spawnpoint.Template.Id}]没有 Items，已丢弃。", Color.Cyan);
                continue;
            }

            if (!requireItemDistribution)
            {
                valid.Add(spawnpoint);
                continue;
            }

            // 动态刷新点：SPT 生成时会遍历 ItemDistribution，并要求 composedKey 能在 Items[].composedKey 中命中。
            HashSet<string> composedKeys = spawnpoint.Template.Items
                .Select(item => item.ComposedKey)
                .Where(key => !string.IsNullOrEmpty(key))
                .Select(key => key!)
                .ToHashSet();

            bool hasValidDistribution = spawnpoint.ItemDistribution is not null
                && spawnpoint.ItemDistribution.Any(dist =>
                    dist?.ComposedKey?.Key is { } compKey && composedKeys.Contains(compKey));

            if (!hasValidDistribution)
            {
                Log($"散落物资：地图[{mapId}]刷新点[{spawnpoint.Template.Id}]的 itemDistribution 与 Items 的 composedKey 不匹配，已丢弃（否则开局时会被静默跳过）。", Color.Cyan);
                continue;
            }

            if ((spawnpoint.Probability ?? 0) <= 0)
            {
                Log($"散落物资：地图[{mapId}]刷新点[{spawnpoint.Template.Id}]的 probability 为空或 0，SPT 会视为永不抽取，已丢弃。", Color.Cyan);
                continue;
            }

            valid.Add(spawnpoint);
        }

        return valid;
    }
    
    private IEnumerable<Spawnpoint> MergeSpawnpoints(IEnumerable<Spawnpoint>? target, List<Spawnpoint> additions)
    {
        List<Spawnpoint> merged = target?.ToList() ?? new List<Spawnpoint>();

        foreach (Spawnpoint addition in additions)
        {
            string? additionId = addition.Template?.Id;
            Spawnpoint? exist = merged.FirstOrDefault(point => point?.Template?.Id == additionId);

            if (exist?.Template is null)
            {
                merged.Add(CloneSpawnpoint(addition));
                continue;
            }

            exist.Template.Items = (exist.Template.Items ?? Enumerable.Empty<SptLootItem>())
                .Union(addition.Template?.Items ?? Enumerable.Empty<SptLootItem>());
            exist.ItemDistribution = (exist.ItemDistribution ?? Enumerable.Empty<LooseLootItemDistribution>())
                .Union(addition.ItemDistribution ?? Enumerable.Empty<LooseLootItemDistribution>());
        }

        return merged;
    }
    
    private Spawnpoint CloneSpawnpoint(Spawnpoint spawnpoint)
    {
        return mGUtils.Deserialize<Spawnpoint>(mGUtils.Serialize(spawnpoint)) ?? spawnpoint;
    }
    
    public void MGmodLocations(MGModConfig_Locations LocationsSetting)
    {
        var Locations_ = GetLocations();
        Dictionary<string,MapBotDifficulty> easyMapJson = mGUtils.GetJsonDataFromFile<Dictionary<string,MapBotDifficulty>>(BotSystemPathsType.EasyMapPath);
        Dictionary<string,MapBotDifficulty> hardMapJson = mGUtils.GetJsonDataFromFile<Dictionary<string,MapBotDifficulty>>(BotSystemPathsType.HardMapPath);
        Dictionary<string, List<string>> mapZoneFull =
            mGUtils.GetJsonDataFromFile<Dictionary<string, List<string>>>(Paths.MapZoneFullJson);
        PmcTacticalSquadType pmcTacticalSquad =
            mGUtils.GetJsonDataFromFile<PmcTacticalSquadType>(BotSystemPathsType.PmcTacticalSquadPath);
        string[] Exclude = [ "Develop", "Hideout", "PrivateArea", "Suburbs", "Terminal", "Town"];
        List<string> hardLevelMapList = ["Laboratory", "Labyrinth", "SandboxHigh", "TarkovStreets"];
        List<string> smallMapList = ["Bigmap", "Factory4Day", "Factory4Night", "Laboratory", "Sandbox", "SandboxHigh"]; 
        List<string> bossNames = [
            "bossBully",
            "bossKnight",
            "bossPartisan",
            "bossTagilla",
            "bossKilla",
            "bossZryachiy",
            "bossGluhar",
            "bossSanitar",
            "bossBoar",
            "bossBoarSniper",
            "bossKolontay",
            "bossTagillaAgro",
            "tagillaHelperAgro",
            "bossKojaniy",
            "bossKillaAgro"
        ];
        
        foreach (var mapName  in Locations_.Keys)
        {
            if (Exclude.Contains(mapName)) continue;
            var locationBase = Locations_[mapName].Base;
            // 功能：战局时长(分钟) RaidTime
            if (LocationsSetting.RaidTime.enable)
            {
                locationBase.EscapeTimeLimit = LocationsSetting.RaidTime.value;
            }
            // 功能：BOSS刷新率 BOSSSpwanChance
            if (LocationsSetting.BOSSSpwanChance.enable && locationBase.BossLocationSpawn.Count > 0)
            {
                foreach(var Bzone in locationBase.BossLocationSpawn)
                {
                    if (!bossNames.Contains(Bzone.BossName)) continue;
                    Bzone.BossChance = LocationsSetting.BOSSSpwanChance.value;
                }
            }
            // 功能：100%可拉闸  功能：100%可撤离
            if ((LocationsSetting.Pass100 || LocationsSetting.Escape100) && locationBase.Exits.Any())
            {
                foreach(var exit in locationBase.Exits)
                {
                    if (exit.PassageRequirement == null) continue;
                    if (exit.PassageRequirement == RequirementState.WorldEvent && LocationsSetting.Pass100)
                    {
                        exit.Chance = 100;
                    }
                    else if (LocationsSetting.Escape100)
                    {
                        exit.Chance = 100;
                    }

                }
            }
            // 功能：地图是否回保 MapInsurance
            if (LocationsSetting.MapInsurance.ContainsKey(mapName))
            {
                locationBase.Insurance = LocationsSetting.MapInsurance[mapName];
                locationBase.IsSecret = !LocationsSetting.MapInsurance[mapName];
            }
            // 功能：地图AI难度分布 MapBotDifficulty : easy, normal, hard, impossible
            if (LocationsSetting.BotSystem.MapBotDifficulty != "default")
            {
                  string difficulty = LocationsSetting.BotSystem.MapBotDifficulty;
                  foreach(var Bzone in locationBase.BossLocationSpawn)
                  {
                      Bzone.BossDifficulty = difficulty;
                      Bzone.BossEscortDifficulty = difficulty;
                  }
                  MapBotDifficulty mapBotDifficulty = easyMapJson[difficulty];
                  if (hardLevelMapList.Contains(mapName))
                  {
                      mapBotDifficulty = hardMapJson[difficulty];
                  }
                  locationBase.BotEasy = mapBotDifficulty.BotEasy;
                  locationBase.BotNormal = mapBotDifficulty.BotNormal;
                  locationBase.BotHard = mapBotDifficulty.BotHard;
                  locationBase.BotImpossible = mapBotDifficulty.BotImpossible;
            }
            // 功能：PMC战术小队 PmcTacticalSquad
            if (LocationsSetting.BotSystem.PmcTacticalSquad)
            {
                List<string> zoneList = new List<string>();
                foreach (var zones in mapZoneFull[mapName])
                {
                    if(zones.Contains("Snipe")) continue;
                    zoneList.Add(zones);
                }

                BossLocationSpawn usecSpawn = mGUtils.Deserialize<BossLocationSpawn>(mGUtils.Serialize(pmcTacticalSquad.USEC));
                BossLocationSpawn bearSpawn = mGUtils.Deserialize<BossLocationSpawn>(mGUtils.Serialize(pmcTacticalSquad.BEAR));
                string zoneString = string.Join(",", zoneList);
                usecSpawn.BossZone = zoneString;
                bearSpawn.BossZone = zoneString;
                locationBase.BossLocationSpawn.Add(usecSpawn);
                locationBase.BossLocationSpawn.Add(bearSpawn);
            }
            // 功能：地图刷新参数优化 MapRefershConfig
            if (LocationsSetting.BotSystem.MapRefershConfig)
            {
                bool flag = false;
                if (smallMapList.Contains(mapName)) flag = true;
                // locationBase.SavSummonSeconds;
                // locationBase.MatchingMinSeconds;
                // locationBase.UsersSpawnSecondsN;
                // locationBase.UsersSpawnSecondsN2;
                locationBase.MaxBotPerZone = flag?3:7;
                locationBase.MaxDistToFreePoint = flag?400:800;
                locationBase.MinDistToFreePoint = flag?30:60;
                locationBase.BotSpawnTimeOffMax = flag?30:40;
                locationBase.BotSpawnTimeOffMin = flag?20:30;
                locationBase.BotSpawnTimeOnMax = flag?10:50;
                locationBase.BotSpawnTimeOnMin = 0;
                // locationBase.BotMaxTimePlayer;
                // locationBase.BotMaxPlayer;
                locationBase.BotStart = 0;
                locationBase.BotStop = 60*(int)locationBase.EscapeTimeLimit;
            }
        }
    }
    
    private void Log(string data, Color textColor)
    {
        mGUtils.Log("LocationsServer", data, textColor);
    }
}
