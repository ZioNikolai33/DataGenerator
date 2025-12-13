using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

[BsonIgnoreExtraElements]
public class TraitMapper : BaseEntity
{
    [BsonElement("races")]
    public List<BaseEntity> Races { get; set; }
    [BsonElement("subraces")]
    public List<BaseEntity> Subraces { get; set; }
    [BsonElement("proficiencies")]
    public List<BaseEntity> Proficiencies { get; set; }
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
        public string Name { get; set; }
        [BsonElement("desc")]
        public string Desc { get; set; }
        [BsonElement("area_of_effect")]
        public Area AreaEffect { get; set; }
        [BsonElement("usage")]
        public Usage Use { get; set; }
        [BsonElement("dc")]
        public DC Dc { get; set; }
        [BsonElement("damage")]
        public List<Damage> Damage { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class DC
    {
        [BsonElement("dc_type")]
        public BaseEntity DcType { get; set; }
        [BsonElement("success_type")]
        public string SuccessType { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Damage
    {
        [BsonElement("damage_type")]
        public BaseEntity DamageType { get; set; }
        [BsonElement("damage_at_character_level")]
        public Dictionary<byte, string> DamageAtCharacterLevel { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SubtraitOptions
    {
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("from")]
        public SubtraitFrom From { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }

        public List<BaseEntity> GetRandomChoice()
        {
            var random = new Random();
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
        public string Type { get; set; }
        [BsonElement("times")]
        public byte? Times { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SubtraitFrom
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
        [BsonElement("item")]
        public BaseEntity Item { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SpellOptions
    {
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("from")]
        public SpellFrom From { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }

        public List<BaseEntity> GetRandomChoice()
        {
            var random = new Random();
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
        public string OptionSetType { get; set; }
        [BsonElement("options")]
        public List<Option> Options { get; set; }
    }

    public TraitMapper(string index, string name) : base(index, name) { }
}