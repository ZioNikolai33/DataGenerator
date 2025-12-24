using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class EquipmentCategoryMapper: BaseEntity
{
    [BsonElement("equipment")]
    public List<BaseEntity> Equipment { get; set; } = new List<BaseEntity>();

    public EquipmentCategoryMapper(string index, string name) : base(index, name) { }
}
