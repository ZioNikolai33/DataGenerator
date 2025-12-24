using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities;

[BsonIgnoreExtraElements]
public class Area
{
    [BsonElement("type")]
    public string Type { get; set; }
    [BsonElement("size")]
    public int Size { get; set; }

    public Area(string type, int size)
    {
        Type = type;
        Size = size;
    }

    public Area()
    {
        Type = string.Empty;
        Size = 0;
    }
}