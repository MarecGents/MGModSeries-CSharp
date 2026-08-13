using _MGGTmod.types.models.Custom;
using _MGGTmod.types.utils;
using SPTarkov.Common.Logger;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _MGGTmod.types.server;

[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class LocationsServer(
    LocationTable Locations,
    SptLogger<LocationsServer> logger,
    MGUtils mGUtils
    )
{
    private MGUtils mGUtils;
    
    public Dictionary<string, Location> GetLocations()
    {
        return Locations.GetDictionary();
    }
}
