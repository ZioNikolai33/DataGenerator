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
}
