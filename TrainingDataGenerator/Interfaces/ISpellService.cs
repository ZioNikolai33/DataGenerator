using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Interfaces;

public interface ISpellService
{
    void ApplySpellsFromFeatures(PartyMember member);
    void ApplySpellsFromTraits(PartyMember member);
    void ApplyCircleSpells(PartyMember member);
    void ApplyOathSpells(PartyMember member);
    void ApplyDomainSpells(PartyMember member);
}
