using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Utilities;

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

    public override int GetWeaponPower(int strengthModifier, int dexterityModifier)
    {
        if (!Damage.DamageDice.Contains("d"))
            return int.Parse(Damage.DamageDice.Trim());

        var weaponPower = 0;
        var averageDamage = DataManipulation.GetDiceValue(Damage.DamageDice);
        var totalDamage = averageDamage + dexterityModifier;

        weaponPower = totalDamage;

        return weaponPower;
    }

    public override int GetAttackBonus(int strengthModifier, int dexterityModifier, int proficiencyBonus, bool isProficient)
    {
        var attackBonus = dexterityModifier;

        if (isProficient)
            attackBonus += proficiencyBonus;

        return attackBonus;
    }
}
