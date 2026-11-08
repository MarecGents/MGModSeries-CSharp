using SPTarkov.Server.Core.Models.Common;

namespace _MGMod.types.models.EFT.hideout;

public class CustomProduction
{
    public MongoId Id { get; set; }
    public MongoId TargetProductId { get; set; }
    public int Count { get; set; }
    public double ProductionTime { get; set; }
    public int AreaType { get; set; }
    public int RequiredLevel { get; set; }

    public Dictionary<MongoId, RequirementItemProperty> RequirementList { get; set; } =
        new Dictionary<MongoId, RequirementItemProperty>();
}

public class RequirementItemProperty
{
    public int? Count { get; set; }
    public List<string>? GameVersions { get; set; }
    public string Type { get; set; } = "";
}