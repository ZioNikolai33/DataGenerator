using MongoDB.Bson.Serialization.Attributes;
using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class TraitMapper : BaseEntity
{
    [BsonElement("races")]
    public List<BaseEntity> Races { get; set; } = new List<BaseEntity>();
    [BsonElement("subraces")]
    public List<BaseEntity> Subraces { get; set; } = new List<BaseEntity>();
    [BsonElement("proficiencies")]
    public List<BaseEntity> Proficiencies { get; set; } = new List<BaseEntity>();
    [BsonElement("parent")]
    public BaseEntity? Parent { get; set; }
    [BsonElement("trait_specific")]
    public TraitSpecific? TraitSpec { get; set; }
    [BsonElement("proficiency_choices")]
    public ProficiencyChoiceMapper? ProficiencyChoice { get; set; }

    [BsonIgnoreExtraElements]
    public class TraitSpecific
    {
        [BsonElement("damage_type")]
        public BaseEntity? DamageType { get; set; }
        [BsonElement("breath_weapon")]
        public BreathWeapon? BreathWeapon { get; set; }
        [BsonElement("subtrait_options")]
        public SubtraitOptions? SubtraitOptions { get; set; }
        [BsonElement("spell_options")]
        public SpellOptions? SpellOptions { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class BreathWeapon
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;
        [BsonElement("area_of_effect")]
        public Area AreaEffect { get; set; } = new Area();
        [BsonElement("usage")]
        public Usage Use { get; set; } = new Usage();
        [BsonElement("dc")]
        public DC Dc { get; set; } = new DC();
        [BsonElement("damage")]
        public List<Damage> Damage { get; set; } = new List<Damage>();
    }

    [BsonIgnoreExtraElements]
    public class DC
    {
        [BsonElement("dc_type")]
        public BaseEntity DcType { get; set; } = new BaseEntity();
        [BsonElement("success_type")]
        public string SuccessType { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class Damage
    {
        [BsonElement("damage_type")]
        public BaseEntity DamageType { get; set; } = new BaseEntity();
        [BsonElement("damage_at_character_level")]
        public Dictionary<byte, string> DamageAtCharacterLevel { get; set; } = new Dictionary<byte, string>();
    }

    [BsonIgnoreExtraElements]
    public class SubtraitOptions
    {
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("from")]
        public SubtraitFrom From { get; set; } = new SubtraitFrom();
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        public List<BaseEntity> GetRandomChoice(IRandomProvider random)
        {
            var selectedSubtrait = new List<BaseEntity>();

            if (this.From.Options.All(item => item.Item != null))
                selectedSubtrait = this.From.Options
                        .OrderBy(_ => random.Next())
                        .Take(Choose)
                        .Select(option => new BaseEntity(option.Item.Index, option.Item.Name))
                        .ToList();

            return selectedSubtrait;
        }
    }

    [BsonIgnoreExtraElements]
    public class Usage
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("times")]
        public byte? Times { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SubtraitFrom
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<Option> Options { get; set; } = new List<Option>();
    }

    [BsonIgnoreExtraElements]
    public class Option
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; } = string.Empty;
        [BsonElement("item")]
        public BaseEntity Item { get; set; } = new BaseEntity();
    }

    [BsonIgnoreExtraElements]
    public class SpellOptions
    {
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("from")]
        public SpellFrom From { get; set; } = new SpellFrom();
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        public List<BaseEntity> GetRandomChoice(IRandomProvider random)
        {
            var selectedSpell = new List<BaseEntity>();

            if (this.From.Options.All(item => item.Item != null))
                selectedSpell = this.From.Options
                        .OrderBy(_ => random.Next())
                        .Take(Choose)
                        .Select(option => new BaseEntity(option.Item.Index, option.Item.Name))
                        .ToList();

            return selectedSpell;
        }
    }

    [BsonIgnoreExtraElements]
    public class SpellFrom
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<Option> Options { get; set; } = new List<Option>();
    }

    public TraitMapper(string index, string name) : base(index, name) { }
}