using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Interfaces;

public interface IAttributeService
{
    void SetAttributes(PartyMember member, ClassMapper classMapper, List<AbilityBonus> raceAbilityBonuses, List<FeatureMapper> features);

    List<byte> GenerateStandardArray();

    void ApplyRacialBonuses(List<byte> attributes, List<AbilityBonus> raceAbilityBonuses);

    void ApplyAbilityScoreImprovements(List<byte> attributes, byte numberOfImprovements);

    void ApplyClassSpecificBonuses(List<byte> attributes, string classIndex, List<FeatureMapper> features);

    void AddSavingThrowProficiencies(PartyMember member, ClassMapper classMapper);

    int GetAttributeModifier(byte attributeValue);
}