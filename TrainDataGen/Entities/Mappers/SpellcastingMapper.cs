using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

public class SpellcastingMapper
{
    [BsonElement("level")]
    public byte Level { get; set; }
    [BsonElement("spellcasting_ability")]
    public BaseEntity SpellcastingAbility { get; set; }
}
