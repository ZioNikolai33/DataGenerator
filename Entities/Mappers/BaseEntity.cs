using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class BaseEntity
{
    [BsonElement("index")]
    public string Index { get; set; }
    [BsonElement("name")]
    [JsonIgnore]
    public string Name { get; set; }

    public BaseEntity(string index, string name)
    {
        Index = index;
        Name = name;
    }

    public BaseEntity()
    {
        Index = string.Empty;
        Name = string.Empty;
    }
}
