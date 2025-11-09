using MongoDB.Bson.Serialization.Attributes;
using TrainDataGen.Entities.Enums;

namespace TrainDataGen.Entities.Mappers;

public class RaceMapper : BaseEntity
{
    public short Speed { get; set; }
    public List<AbilityBonus> AbilityBonuses { get; set; }
    public Size Size { get; set; }
    public List<BaseEntity> StartingProficiences { get; set; }
    public ProficiencyChoiceMapper StartingProficiencesOptions { get; set; }
    public AbilityBonusOptions AbilityOptions { get; set; }
    public List<BaseEntity> Traits { get; set; }
    public List<BaseEntity> Subraces { get; set; }

    public class AbilityScore
    {
        [BsonElement("ability_score")]
        public BaseEntity Ability { get; set; }
        [BsonElement("bonus")]
        public byte Bonus { get; set; }

        public AbilityScore(BaseEntity ability, byte bonus)
        {
            Ability = ability;
            Bonus = bonus;
        }
    }

    public class AbilityBonusOptions
    {
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("from")]
        public OptionSet From { get; set; }
    }

    public class OptionSet
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; }
        [BsonElement("options")]
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; }
        [BsonElement("ability_score")]
        public BaseEntity AbilityScore { get; set; }
        [BsonElement("bonus")]
        public byte Bonus { get; set; }
    }

    public RaceMapper(string index, string name) : base(index, name) { }

    public List<BaseEntity> GetRandomProficiency(List<BaseEntity>? proficiencies) => 
        StartingProficiencesOptions.GetRandomChoice(proficiencies);

    public List<AbilityScore> GetRandomAbility()
    {
        var random = new Random();
        var selectedAbility = new List<AbilityScore>();

        selectedAbility = this.AbilityOptions.From.Options
            .OrderBy(_ => random.Next())
            .Take(AbilityOptions.Choose)
            .Select(option => new AbilityScore(option.AbilityScore, option.Bonus))
            .ToList();

        return selectedAbility;
    }
}