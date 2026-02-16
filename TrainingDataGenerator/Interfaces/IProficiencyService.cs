using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Entities.PartyEntities;

namespace TrainingDataGenerator.Interfaces;

public interface IProficiencyService
{
    void SetInitialProficiencies(PartyMember member, ClassMapper classMapper, RaceMapper raceMapper, SubraceMapper? subraceMapper);

    void AddBackgroundProficiencies(PartyMember member);

    void AddAdditionalProficiencies(PartyMember member);

    void ApplySkillProficiencies(PartyMember member);

    List<string> GetAllSkillIndices(PartyMember member);

    bool HasProficiency(PartyMember member, string proficiencyIndex);
}