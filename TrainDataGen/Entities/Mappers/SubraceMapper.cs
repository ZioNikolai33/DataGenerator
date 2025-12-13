using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

[BsonIgnoreExtraElements]
public class SubraceMapper : BaseEntity
{
    [BsonElement("race")]
    public BaseEntity Race { get; set; }
    [BsonElement("desc")]
    public string Desc { get; set; }
    [BsonElement("ability_bonuses")]
    public List<AbilityBonus> AbilityBonuses { get; set; }
    [BsonElement("starting_proficiencies")]
    public List<BaseEntity> StartingProficiencies { get; set; }
    [BsonElement("racial_traits")]
    public List<BaseEntity> RacialTraits { get; set; }

    public SubraceMapper(string index, string name) : base(index, name) { }
}