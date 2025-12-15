using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities.Equip;

public class Ammunition : Equipment
{
    public short Quantity { get; set; }

    public Ammunition(EquipmentMapper equipment) : base(equipment)
    {
        Quantity = equipment.Quantity ?? 0;
    }
}
