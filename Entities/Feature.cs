using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Mappers;
using static TrainingDataGenerator.Entities.Mappers.FeatureMapper;

namespace TrainingDataGenerator.Entities;

public class Feature : BaseEntity
{
    public List<string> Desc { get; set; }
    public FeatureSpecificTypes? FeatureType { get; set; }
    public List<string>? FeatureSpec { get; set; }

    public Feature(FeatureMapper feature, List<string> proficiencies) : base(feature.Index, feature.Name)
    {
        Desc = feature.Desc;
        FeatureSpec = feature.FeatureSpec?.GetRandomChoice(proficiencies);

        if (feature.FeatureSpec?.ExpertiseOptions != null)
            FeatureType = FeatureSpecificTypes.Expertise;
        else if (feature.FeatureSpec?.EnemyTypeOptions != null)
            FeatureType = FeatureSpecificTypes.Enemy;
        else if (feature.FeatureSpec?.TerrainTypeOptions != null)
            FeatureType = FeatureSpecificTypes.Terrain;
        else if (feature.FeatureSpec?.SubfeatureOptions != null)
            FeatureType = FeatureSpecificTypes.Subfeature;
        else if (feature.FeatureSpec?.Invocations != null)
            FeatureType = FeatureSpecificTypes.Invocation;
    }
}
