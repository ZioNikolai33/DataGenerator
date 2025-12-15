using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.Equip;

abstract public class Equipment : BaseEntity
{
    public BaseEntity EquipmentCategory { get; set; }
    public List<BaseEntity> Properties { get; set; }
    public bool IsEquipped { get; set; } = false;

    public Equipment(EquipmentMapper equipment): base(equipment.Index, equipment.Name)
    {
        EquipmentCategory = equipment.EquipmentCategory;
        Properties = equipment.Properties ?? new List<BaseEntity>();
    }

    public void EquipItem() => IsEquipped = true;
}
