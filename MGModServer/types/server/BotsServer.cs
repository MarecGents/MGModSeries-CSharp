using _MGMod.types.models.Custom;
using _MGMod.types.models.Paths;
using _MGMod.types.services;
using _MGMod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Bots;

namespace _MGMod.types.server;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class BotsServer(
    DatabaseServices databaseServices,
    MGUtils mGUtils
    )
{
    public Bots GetBots()
    {
        return databaseServices.GetBots();
    }

    public BotType GetBot(string key)
    {
        if (GetBots().Types.TryGetValue(key, out var botType))
        {
            return botType;
        }
        return null;
    }
    
    public void SetBotsHealth(int rate, string? botType = null)
    {
        var bots = GetBots();
        foreach(var key in bots.Types.Keys)
        {
            if( string.IsNullOrEmpty(botType) || key == botType)
            {
                var bodyPart = bots.Types[key].BotHealth.BodyParts.ElementAtOrDefault(0);
				bodyPart.Chest.Max *= rate;
                bodyPart.Chest.Min *= rate;
                bodyPart.Head.Max *= rate;
                bodyPart.Head.Min *= rate;
                bodyPart.LeftLeg.Max *= rate;
                bodyPart.LeftLeg.Min *= rate;
                bodyPart.LeftArm.Max *= rate;
                bodyPart.LeftArm.Min *= rate;
                bodyPart.RightLeg.Max *= rate;
                bodyPart.RightLeg.Min *= rate;
                bodyPart.RightArm.Max *= rate;
                bodyPart.RightArm.Min *= rate;
                bodyPart.Stomach.Max *= rate;
                bodyPart.Stomach.Min *= rate;
            }
        }
    }
    public void MGmodBots(MGModConfig_Bot? BotSetting)
    {
        // 功能：AI血量 AIHealth
        if ( BotSetting?.AIHealth != 1)
        {
            SetBotsHealth(BotSetting.AIHealth);
        }
        
        // 功能：AI名字池 BotNameAdd
        if (BotSetting.BotSystem.BotNameAdd)
        {
            List<string> botNameList = mGUtils.GetJsonDataFromFile<List<string>>(BotSystemPathsType.PmcNamePath);
            foreach (var key in new List<string>(["pmcusec", "pmcbear"]))
            {
                var botType = GetBot(key);
                if (botType == null) continue;
                botType.FirstNames.AddRange(botNameList);
            }
        }
    }
}