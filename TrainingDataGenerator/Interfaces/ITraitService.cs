using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Interfaces;

public interface ITraitService
{
    void ManageTraitSpecifics(PartyMember member, RaceMapper raceMapper, SubraceMapper? subraceMapper);

    List<TraitMapper> GetRaceTraits(RaceMapper raceMapper, SubraceMapper? subraceMapper);

    List<BaseEntity> SelectSubtraits(List<TraitMapper> traits);

    bool HasTrait(PartyMember member, string traitIndex);
}