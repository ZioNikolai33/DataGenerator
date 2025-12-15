using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.Equip;

public class MeleeWeapon : Weapon
{
    public RangeData? ThrowRange { get; set; }
    public DamageData? TwoHandedDamage { get; set; }

    public MeleeWeapon(EquipmentMapper equipment) : base(equipment)
    {
        ThrowRange = equipment.ThrowRange != null ? new RangeData { Normal = equipment.ThrowRange.Normal } : null;
        TwoHandedDamage = equipment.TwoHandedDamage != null ? new DamageData { DamageDice = equipment.TwoHandedDamage.DamageDice, DamageType = equipment.TwoHandedDamage.DamageType } : null;
    }
}
