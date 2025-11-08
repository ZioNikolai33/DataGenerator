using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

public class BaseMapper
{
    [BsonElement("index")]
    public string Index { get; set; }
    [BsonElement("name")]
    public string Name { get; set; }

    public BaseMapper(string index, string name)
    {
        Index = index;
        Name = name;
    }
}
