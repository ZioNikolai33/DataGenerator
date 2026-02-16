using MongoDB.Bson.Serialization.Attributes;
using TrainingDataGenerator.Entities.PartyEntities;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class SubraceMapper : BaseEntity
{
    [BsonElement("race")]
    public BaseEntity Race { get; set; } = new BaseEntity();
    [BsonElement("desc")]
    public string Desc { get; set; } = string.Empty;
    [BsonElement("ability_bonuses")]
    public List<AbilityBonus> AbilityBonuses { get; set; } = new List<AbilityBonus>();
    [BsonElement("starting_proficiencies")]
    public List<BaseEntity> StartingProficiencies { get; set; } = new List<BaseEntity>();
    [BsonElement("racial_traits")]
    public List<BaseEntity> RacialTraits { get; set; } = new List<BaseEntity>();

    public SubraceMapper(string index, string name) : base(index, name) { }
}