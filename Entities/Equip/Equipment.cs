using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.Equip;

abstract public class Equipment : BaseEntity
{
    public string EquipmentCategory { get; set; }
    public List<string> Properties { get; set; }
    public bool IsEquipped { get; set; } = false;

    public Equipment(EquipmentMapper equipment): base(equipment.Index, equipment.Name)
    {
        EquipmentCategory = equipment.EquipmentCategory.Index;
        Properties = equipment.Properties?.Select(item => item.Index).ToList() ?? new List<string>();
    }

    public void EquipItem() => IsEquipped = true;
}
