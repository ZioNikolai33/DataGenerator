using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

[BsonIgnoreExtraElements]
public class EquipmentCategoryMapper: BaseEntity
{
    [BsonElement("equipment")]
    public List<BaseEntity> Equipment { get; set; }

    public EquipmentCategoryMapper(string index, string name) : base(index, name) { }
}
