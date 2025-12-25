using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.Equip;

public class RangedWeapon : Weapon
{
    public RangeData Range { get; set; }

    public RangedWeapon(EquipmentMapper equipment) : base(equipment)
    {
        Range = new RangeData {
            Normal = equipment.Range?.Normal ?? 0,
            Long = equipment.Range?.Long ?? 0
        };
    }

    public override int GetWeaponPower(int strengthModifier, int dexterityModifier, int proficiencyBonus, bool isProficient)
    {
        var weaponPower = 0;
        var damageParts = Damage.DamageDice.Split('d');
        var averageDamage = (int.Parse(damageParts[0]) * (int.Parse(damageParts[1]) + 1)) / 2;
        var totalDamage = averageDamage + dexterityModifier;

        if (isProficient)
            totalDamage += proficiencyBonus;

        weaponPower = totalDamage;

        return weaponPower;
    }
}
