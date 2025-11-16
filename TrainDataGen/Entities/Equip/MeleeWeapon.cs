using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities.Equip;

public class MeleeWeapon : Weapon
{
    public RangeData ThrowRange { get; set; }
    public DamageData? TwoHandedDamage { get; set; }

    public MeleeWeapon(EquipmentMapper equipment) : base(equipment)
    {
        ThrowRange = equipment.ThrowRange ?? new RangeData { Normal = 0, Long = 0 };
        TwoHandedDamage = equipment.TwoHandedDamage;
    }
}
