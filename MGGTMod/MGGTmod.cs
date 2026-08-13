using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Helpers;
using _MGGTmod.types.services;
using _MGGTmod.types.utils;
using SPTarkov.Common.Logger;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Helpers.Server;

namespace _MGGTmod;

public record ModMetadata : IModMetadata
{
	public string ModGuid { get; init; } = "com.marecgents.tarkovmod.mggtmod";
	public string Name { get; init; } = "MGGTMod";
	public string Author { get; init; } = "MarecGents";
	public List<string>? Contributors { get; init; } = ["MarecGents"];
	public SemanticVersioning.Version Version { get; init; } = new("0.5.0");
	public SemanticVersioning.Range SptVersion { get; init; } = new("4.1.2");
    public bool HasPrepatcher { get; init; } = false;
	public List<string>? Incompatibilities { get; init; }
	public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; } = "https://github.com/MarecGents/MG-GT-Mod-CSharp/";
	public string? License { get; init; } = "CC BY-NC-ND 4.0";
}

[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class MGGTmod(
    SptLogger<MGGTmod> logger,
    ModHelper modHelper,
    ConfigSettingServices configSettingServices,
    MGUtils  mGUtils
    ) : IOnLoad
{
    public async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        await configSettingServices.ModSetting();
    }
}

// [Injectable(TypePriority = OnLoadOrder.Preload + 1)]
// public class PreMGmodLoad(
//     SptLogger<PreMGmodLoad> logger
//     ) : IOnLoad
// {
//     public Task OnLoadAsync(CancellationToken cancellationToken)
//     {
//         return Task.CompletedTask;
//     }
// }