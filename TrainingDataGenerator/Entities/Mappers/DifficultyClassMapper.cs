using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class DifficultyClassMapper
{
    [BsonElement("dc_type")]
    public BaseEntity DcType { get; set; } = new BaseEntity();
    [BsonElement("dc_success")]
    public string DcSuccess { get; set; } = string.Empty;
}