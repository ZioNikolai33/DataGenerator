using MongoDB.Bson.Serialization.Attributes;
using TrainingDataGenerator.Entities.Enums;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class RaceMapper : BaseEntity
{
    [BsonElement("speed")]
    public short Speed { get; set; }
    [BsonElement("size")]
    public Size Size { get; set; }
    [BsonElement("ability_bonuses")]
    public List<AbilityBonus> AbilityBonuses { get; set; }
    [BsonElement("starting_proficiencies")]
    public List<BaseEntity> StartingProficiences { get; set; }
    [BsonElement("starting_proficiency_options")]
    public ProficiencyChoiceMapper? StartingProficiencesOptions { get; set; }
    [BsonElement("ability_bonus_options")]
    public AbilityBonusOptions? AbilityOptions { get; set; }
    [BsonElement("traits")]
    public List<BaseEntity> Traits { get; set; }
    [BsonElement("subraces")]
    public List<BaseEntity> Subraces { get; set; }

    [BsonIgnoreExtraElements]
    public class AbilityBonusOptions
    {
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("from")]
        public OptionSet From { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class OptionSet
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; }
        [BsonElement("options")]
        public List<Option> Options { get; set; }
    }

    [BsonIgnoreExtraElements]
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

    public List<string> GetRandomProficiency(List<string>? proficiencies) => 
        (StartingProficiencesOptions != null) ? StartingProficiencesOptions.GetRandomChoice(proficiencies) : new List<string>();

    public List<AbilityBonus> GetRandomAbility()
    {
        var random = new Random();
        var selectedAbility = new List<AbilityBonus>();

        if (AbilityOptions == null)
            return selectedAbility;

        selectedAbility = this.AbilityOptions.From.Options
            .OrderBy(_ => random.Next())
            .Take(AbilityOptions.Choose)
            .Select(option => new AbilityBonus(option.AbilityScore, option.Bonus))
            .ToList();

        return selectedAbility;
    }
}