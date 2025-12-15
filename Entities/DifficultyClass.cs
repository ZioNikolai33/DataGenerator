using MongoDB.Bson.Serialization.Attributes;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities;

[BsonIgnoreExtraElements]
public class DifficultyClass
{
    [BsonElement("dc_type")]
    public BaseEntity DcType { get; set; }
    [BsonElement("dc_success")]
    public string DcSuccess { get; set; }

    public DifficultyClass(BaseEntity dcType, string dcSuccess)
    {
        DcType = dcType;
        DcSuccess = dcSuccess;
    }
}