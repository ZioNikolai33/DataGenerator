using TrainingDataGenerator.Entities.Equip;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Entities.PartyEntities;

namespace TrainingDataGenerator.Interfaces;

public interface IEquipmentService
{
    void ManageEquipments(PartyMember member, ClassMapper classMapper);

    void EquipRandomWeapons(PartyMember member);

    void ManageArmorRequirements(PartyMember member);

    bool IsProficient(PartyMember member, Weapon weapon);

    List<BaseEntity> GetEquipmentsByCategory(string equipmentIndex);

    (List<Armor> Armors, List<MeleeWeapon> MeleeWeapons, List<RangedWeapon> RangedWeapons, List<Ammunition> Ammunitions) 
        ConvertToTypedEquipment(List<EquipmentMapper> equipments);
}