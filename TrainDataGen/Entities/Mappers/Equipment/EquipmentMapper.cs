using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers.Equipment;

public class EquipmentMapper : BaseEntity
{
    [BsonElement("equipment_category")]
    public BaseEntity EquipmentCategory { get; set; }
    [BsonElement("cost")]
    public CostMapper Cost { get; set; }

    public class CostMapper
    {
        [BsonElement("quantity")]
        public byte Quantity { get; set; }
        [BsonElement("unit")]
        public string Unit { get; set; }
    }

    public EquipmentMapper(string index, string name) : base(index, name) { }
}