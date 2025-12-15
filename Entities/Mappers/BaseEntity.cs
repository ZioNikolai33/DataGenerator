using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class BaseEntity
{
    [BsonElement("index")]
    public string Index { get; set; }
    [BsonElement("name")]
    public string Name { get; set; }

    public BaseEntity(string index, string name)
    {
        Index = index;
        Name = name;
    }
}
