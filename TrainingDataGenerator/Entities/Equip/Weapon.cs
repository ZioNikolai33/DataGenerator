using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.Equip;

abstract public class Weapon : Equipment
{
    public string WeaponCategory { get; set; }
    public string WeaponRange { get; set; }
    public string CategoryRange { get; set; }
    public DamageData Damage { get; set; }

    public class DamageData
    {
        public string DamageType { get; set; } = string.Empty;
        public string DamageDice { get; set; } = string.Empty;
    }

    public class RangeData
    {
        public short Normal { get; set; }
        public short? Long { get; set; }
    }

    public Weapon(EquipmentMapper equipment) : base(equipment)
    {
        WeaponCategory = equipment.WeaponCategory ?? "Melee";
        WeaponRange = equipment.WeaponRange ?? "Close";
        CategoryRange = equipment.CategoryRange ?? "Standard";
        Damage = new DamageData
        {
            DamageType = equipment.Damage?.DamageType.Index ?? "None",
            DamageDice = equipment.Damage?.DamageDice ?? "0d0"
        };
    }

    abstract public int GetWeaponPower(int strengthModifier, int dexterityModifier, int proficiencyBonus, bool isProficient);
}
