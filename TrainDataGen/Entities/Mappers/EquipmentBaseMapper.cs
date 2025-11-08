using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

public class EquipmentBaseMapper
{
    [BsonElement("equipment")]
    public BaseMapper Equipment { get; set; }
    [BsonElement("quantity")]
    public byte Quantity { get; set; }
}
