using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Interfaces;

public interface IFeatureService
{
    void ApplyFeatureEffects(PartyMember member);

    void ApplyFeatureSpecifics(PartyMember member);

    void CheckFeaturePrerequisites(PartyMember member, List<FeatureMapper> allFeatures);
}