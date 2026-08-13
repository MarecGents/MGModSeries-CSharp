using _MGGTmod.types.models.Custom;
using Spectre.Console;
using SPTarkov.Common.Logger;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _MGGTmod.types.server;

[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class GlobalsServer(
    GlobalTable Globals,
    SptLogger<GlobalsServer> logger
    )
{
    public GlobalTable GetGlobals()
    {
        return Globals;
    }
    public void AddBuff(string buffName, List<Buff> buff)
    {
        var Buffs = Globals.Configuration.Health.Effects.Stimulator.Buffs;
        if (!Buffs.ContainsKey(buffName))
        {
            Buffs.Add(buffName, buff);
            return;
        }
        logger.LogWithColor($"针剂Buff名称：{buffName}重复！请更换其他Buff名称。", Color.Cyan);
        return;

    }
    public void AddBuffs(Dictionary<string,List<Buff>> Buffs)
    {
        foreach( var buff in Buffs)
        {
            AddBuff(buff.Key, buff.Value);
        }
    }
}