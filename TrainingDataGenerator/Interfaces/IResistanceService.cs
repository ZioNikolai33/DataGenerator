using TrainingDataGenerator.Entities.PartyEntities;

namespace TrainingDataGenerator.Interfaces;

public interface IResistanceService
{
    void ApplyDamageResistances(PartyMember member);

    void AddResistance(PartyMember member, string damageType);

    void AddImmunity(PartyMember member, string immunityType);

    void AddVulnerability(PartyMember member, string damageType);

    bool HasResistance(PartyMember member, string damageType);

    bool HasImmunity(PartyMember member, string immunityType);
}