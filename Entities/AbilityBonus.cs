using MongoDB.Bson.Serialization.Attributes;
using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities;

[BsonIgnoreExtraElements]
public class AbilityBonus
{
    [BsonElement("ability_score")]
    public BaseEntity Ability { get; set; }
    [BsonElement("bonus")]
    public int Bonus { get; set; }

    public AbilityBonus(BaseEntity ability, int bonus)
    {
        Ability = new BaseEntity(ability.Index, ability.Name);
        Bonus = bonus;
    }
}