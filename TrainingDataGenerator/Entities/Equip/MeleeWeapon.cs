using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.Equip;

public class MeleeWeapon : Weapon
{
    public RangeData? ThrowRange { get; set; }
    public DamageData? TwoHandedDamage { get; set; }

    public MeleeWeapon(EquipmentMapper equipment) : base(equipment)
    {
        ThrowRange = equipment.ThrowRange != null ? new RangeData { Normal = equipment.ThrowRange.Normal } : null;
        TwoHandedDamage = equipment.TwoHandedDamage != null ? new DamageData { DamageDice = equipment.TwoHandedDamage.DamageDice, DamageType = equipment.TwoHandedDamage.DamageType.Index } : null;
    }

    public override int GetWeaponPower(int strengthModifier, int dexterityModifier)
    {
        if (!Damage.DamageDice.Contains("d"))
            return int.Parse(Damage.DamageDice.Trim());

        var weaponPower = 0;
        var damageParts = Damage.DamageDice.Split('d');
        var averageDamage = (int.Parse(damageParts[0]) * (int.Parse(damageParts[1]) + 1)) / 2;
        var totalDamage = averageDamage;

        if (Properties.Contains("finesse") || Properties.Contains("thrown"))
            totalDamage += Math.Max(strengthModifier, dexterityModifier);
        else
            totalDamage += strengthModifier;

        weaponPower = totalDamage;

        return weaponPower;
    }

    public override int GetAttackBonus(int strengthModifier, int dexterityModifier, int proficiencyBonus, bool isProficient)
    {
        var attackBonus = 0;

        if (Properties.Contains("finesse") || Properties.Contains("thrown"))
            attackBonus += Math.Max(strengthModifier, dexterityModifier);
        else
            attackBonus += strengthModifier;

        if (isProficient)
            attackBonus += proficiencyBonus;

        return attackBonus;
    }
}
