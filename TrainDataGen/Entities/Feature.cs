using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities;

public class Feature : BaseEntity
{
    public List<string> Desc { get; set; }
    public List<BaseEntity>? FeatureSpec { get; set; }

    public Feature(FeatureMapper feature) : base(feature.Index, feature.Name)
    {
        Desc = feature.Desc;
        FeatureSpec = feature.FeatureSpec?.GetRandomChoice();
    }
}
