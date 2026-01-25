using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class MonsterMapper : BaseEntity
{
    [BsonElement("desc")]
    public string Desc { get; set; } = string.Empty;

    [BsonElement("size")]
    public string Size { get; set; } = string.Empty;

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("subtype")]
    public string Subtype { get; set; } = string.Empty;

    [BsonElement("alignment")]
    public string Alignment { get; set; } = string.Empty;

    [BsonElement("armor_class")]
    public List<ArmorClass> AC { get; set; } = new List<ArmorClass>();

    [BsonElement("hit_points")]
    public int HitPoints { get; set; }

    [BsonElement("hit_dice")]
    public string HitDice { get; set; } = string.Empty;

    [BsonElement("hit_points_roll")]
    public string HitPointsRoll { get; set; } = string.Empty;

    [BsonElement("speed")]
    public BsonDocument Speed { get; set; } = new BsonDocument();

    [BsonElement("strength")]
    public int Strength { get; set; }

    [BsonElement("dexterity")]
    public int Dexterity { get; set; }

    [BsonElement("constitution")]
    public int Constitution { get; set; }

    [BsonElement("intelligence")]
    public int Intelligence { get; set; }

    [BsonElement("wisdom")]
    public int Wisdom { get; set; }

    [BsonElement("charisma")]
    public int Charisma { get; set; }

    [BsonElement("proficiencies")]
    public List<ProficiencyWrapper> Proficiencies { get; set; } = new List<ProficiencyWrapper>();

    [BsonElement("damage_vulnerabilities")]
    public List<string> DamageVulnerabilities { get; set; } = new List<string>();

    [BsonElement("damage_resistances")]
    public List<string> DamageResistances { get; set; } = new List<string>();

    [BsonElement("damage_immunities")]
    public List<string> DamageImmunities { get; set; } = new List<string>();

    [BsonElement("condition_immunities")]
    public List<BaseEntity> ConditionImmunities { get; set; } = new List<BaseEntity>();

    [BsonElement("senses")]
    public BsonDocument Senses { get; set; } = new BsonDocument();

    [BsonElement("languages")]
    public string Languages { get; set; } = string.Empty;

    [BsonElement("challenge_rating")]
    public double ChallengeRating { get; set; }

    [BsonElement("proficiency_bonus")]
    public int ProficiencyBonus { get; set; }

    [BsonElement("xp")]
    public int Xp { get; set; }

    [BsonElement("special_abilities")]
    public List<SpecialAbility> SpecialAbilities { get; set; } = new List<SpecialAbility>();

    [BsonElement("actions")]
    public List<NormalAction> Actions { get; set; } = new List<NormalAction>();

    [BsonElement("legendary_actions")]
    public List<LegendaryAction> LegendaryActions { get; set; } = new List<LegendaryAction>();

    [BsonElement("reactions")]
    public List<Reaction> Reactions { get; set; } = new List<Reaction>();

    [BsonIgnoreExtraElements]
    public class ArmorClass
    {
        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("value")]
        public int Value { get; set; }

        [BsonElement("armor")]
        public List<BaseEntity> Armor { get; set; } = new List<BaseEntity>();

        [BsonElement("spell")]
        public BaseEntity Spell { get; set; } = new BaseEntity();

        [BsonElement("condition")]
        public BaseEntity Condition { get; set; } = new BaseEntity();
    }

    [BsonIgnoreExtraElements]
    public class ProficiencyWrapper
    {
        [BsonElement("value")]
        public int Value { get; set; }

        [BsonElement("proficiency")]
        public BaseEntity Proficiency { get; set; } = new BaseEntity();
    }

    [BsonIgnoreExtraElements]
    public class SpecialAbility
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;
        [BsonElement("spellcasting")]
        public Spellcasting? Spellcasting { get; set; }

        [BsonElement("usage")]
        public Usage? Usage { get; set; }

        [BsonElement("dc")]
        public Dc? Dc { get; set; }

        [BsonElement("damage")]
        public List<Damage>? Damage { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Spellcasting
    {
        [BsonElement("level")]
        public int? Level { get; set; }

        [BsonElement("ability")]
        public BaseEntity? Ability { get; set; }

        [BsonElement("dc")]
        public int? Dc { get; set; }

        [BsonElement("modifier")]
        public int? Modifier { get; set; }

        [BsonElement("components_required")]
        public List<string>? ComponentsRequired { get; set; }

        [BsonElement("school")]
        public string? School { get; set; }

        [BsonElement("slots")]
        public SpellSlots? Slots { get; set; }

        [BsonElement("spells")]
        public List<SpellReference>? Spells { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SpellSlots
    {
        [BsonElement("1")]
        public byte? _1 { get; set; }
        [BsonElement("2")]
        public byte? _2 { get; set; }
        [BsonElement("3")]
        public byte? _3 { get; set; }
        [BsonElement("4")]
        public byte? _4 { get; set; }
        [BsonElement("5")]
        public byte? _5 { get; set; }
        [BsonElement("6")]
        public byte? _6 { get; set; }
        [BsonElement("7")]
        public byte? _7 { get; set; }
        [BsonElement("8")]
        public byte? _8 { get; set; }
        [BsonElement("9")]
        public byte? _9 { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SpellReference
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("level")]
        public int? Level { get; set; }

        [BsonElement("usage")]
        public Usage? Usage { get; set; }

        [BsonElement("notes")]
        public string? Notes { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class Usage
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("times")]
        public byte? Times { get; set; }
        [BsonElement("dice")]
        public string? Dice { get; set; }
        [BsonElement("min_value")]
        public byte? MinValue { get; set; }
        [BsonElement("rest_types")]
        public List<string>? RestTypes { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Dc
    {
        [BsonElement("dc_type")]
        public BaseEntity DcType { get; set; } = new BaseEntity();

        [BsonElement("dc_value")]
        public int DcValue { get; set; }

        [BsonElement("success_type")]
        public string SuccessType { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class Damage
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("damage_type")]
        public BaseEntity DamageType { get; set; } = new BaseEntity();

        [BsonElement("damage_dice")]
        public string DamageDice { get; set; } = string.Empty;

        [BsonElement("dc")]
        public Dc? Dc { get; set; }

        [BsonElement("choose")]
        public byte? Choose { get; set; }

        [BsonElement("from")]
        public DamageOptionSet From { get; set; } = new DamageOptionSet();
    }

    [BsonIgnoreExtraElements]
    public class DamageOptionSet
    {
        [BsonElement("option_set_type")]
        public string Option_Set_Type { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<DamageOption> Options { get; set; } = new List<DamageOption>();
    }

    [BsonIgnoreExtraElements]
    public class DamageOption
    {
        [BsonElement("option_type")]
        public string Option_Type { get; set; } = string.Empty;
        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
        [BsonElement("damage_type")]
        public BaseEntity Damage_Type { get; set; } = new BaseEntity();
        [BsonElement("damage_dice")]
        public string Damage_Dice { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class NormalAction
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;

        [BsonElement("attack_bonus")]
        public int? AttackBonus { get; set; }

        [BsonElement("damage")]
        public List<Damage> Damage { get; set; } = new List<Damage>();

        [BsonElement("dc")]
        public Dc Dc { get; set; } = new Dc();

        [BsonElement("usage")]
        public Usage Usage { get; set; } = new Usage();

        [BsonElement("multiattack_type")]
        public string MultiattackType { get; set; } = string.Empty;
        [BsonElement("actions")]
        public List<SubAction> Actions { get; set; } = new List<SubAction>();

        [BsonElement("action_options")]
        public ActionOptions ActionOptions { get; set; } = new ActionOptions();

        [BsonElement("options")]
        public Options Options { get; set; } = new Options();

        [BsonElement("attacks")]
        public List<AttackVariant> Attacks { get; set; } = new List<AttackVariant>();
    }

    [BsonIgnoreExtraElements]
    public class LegendaryAction
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;

        [BsonElement("attack_bonus")]
        public int? AttackBonus { get; set; }

        [BsonElement("dc")]
        public Dc Dc { get; set; } = new Dc();

        [BsonElement("damage")]
        public List<Damage> Damage { get; set; } = new List<Damage>();
    }

    [BsonIgnoreExtraElements]
    public class Reaction
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;

        [BsonElement("attack_bonus")]
        public byte? AttackBonus { get; set; }

        [BsonElement("dc")]
        public Dc Dc { get; set; } = new Dc();

        [BsonElement("damage")]
        public List<Damage> Damage { get; set; } = new List<Damage>();
    }

    [BsonIgnoreExtraElements]
    public class SubAction
    {
        [BsonElement("action_name")]
        public string ActionName { get; set; } = string.Empty;

        [BsonElement("count")]
        public object? Count { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class ActionOptions
    {
        [BsonElement("choose")]
        public int Choose { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("from")]
        public OptionSet From { get; set; } = new OptionSet();
    }

    [BsonIgnoreExtraElements]
    public class Options
    {
        [BsonElement("choose")]
        public int Choose { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("from")]
        public OptionSet From { get; set; } = new OptionSet();
    }

    [BsonIgnoreExtraElements]
    public class OptionSet
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;

        [BsonElement("options")]
        public List<Option> Options { get; set; } = new List<Option>();
    }

    [BsonIgnoreExtraElements]
    public class Option
    {
        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;
        [BsonElement("option_type")]
        public string OptionType { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
        [BsonElement("action_name")]
        public string ActionName { get; set; } = string.Empty;

        [BsonElement("count")]
        public int? Count { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("items")]
        public List<Option> Items { get; set; } = new List<Option>();

        [BsonElement("dc")]
        public Dc Dc { get; set; } = new Dc();

        [BsonElement("damage")]
        public List<Damage> Damage { get; set; } = new List<Damage>();
    }

    [BsonIgnoreExtraElements]
    public class AttackVariant
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("dc")]
        public Dc Dc { get; set; } = new Dc();
        [BsonElement("damage")]
        public List<Damage> Damage { get; set; } = new List<Damage>();
    }

    public MonsterMapper(string index, string name) : base(index, name)
    {
    }
}