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
        public BaseEntity DamageType { get; set; }
        public string DamageDice { get; set; }
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
            DamageType = equipment.Damage?.DamageType,
            DamageDice = equipment.Damage?.DamageDice
        };
    }
}
