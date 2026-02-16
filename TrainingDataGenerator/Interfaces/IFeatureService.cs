using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Entities.PartyEntities;

namespace TrainingDataGenerator.Interfaces;

public interface IFeatureService
{
    void ApplyFeatureEffects(PartyMember member);

    void ApplyFeatureSpecifics(PartyMember member);

    void CheckFeaturePrerequisites(PartyMember member, List<FeatureMapper> allFeatures);
}