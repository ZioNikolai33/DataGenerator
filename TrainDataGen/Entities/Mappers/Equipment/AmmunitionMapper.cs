using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers.Equipment;

public class AmmunitionMapper: EquipmentMapper
{
    [BsonElement("quantity")]
    public short Quantity { get; set; }

    public AmmunitionMapper(string index, string name) : base(index, name) { }
}
