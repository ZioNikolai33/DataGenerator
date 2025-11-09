using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

public class EquipmentBaseMapper
{
    [BsonElement("equipment")]
    public BaseEntity Equipment { get; set; }
    [BsonElement("quantity")]
    public byte Quantity { get; set; }
}
