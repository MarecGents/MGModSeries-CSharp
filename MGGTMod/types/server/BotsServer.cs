using _MGGTmod.types.models.Custom;
using _MGGTmod.types.services;
using _MGGTmod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _MGGTmod.types.server;

[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class BotsServer(
    DatabaseServices databaseServices,
    MGUtils mGUtils
)
{
    public BotTable GetBots()
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
}