using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class ClassMapper : BaseEntity
{
    [BsonElement("hit_die")]
    public short Hp { get; set; }
    [BsonElement("subclasses")]
    public List<BaseEntity> Subclasses { get; set; } = new List<BaseEntity>();
    [BsonElement("proficiency_choices")]
    public List<ProficiencyChoiceMapper> ProficiencyChoices { get; set; } = new List<ProficiencyChoiceMapper>();
    [BsonElement("saving_throws")]
    public List<BaseEntity> SavingThrows { get; set; } = new List<BaseEntity>();
    [BsonElement("proficiencies")]
    public List<BaseEntity> Proficiencies { get; set; } = new List<BaseEntity>();
    [BsonElement("starting_equipment")]
    public List<Equipment> StartingEquipments { get; set; } = new List<Equipment>();
    [BsonElement("starting_equipment_options")]
    public List<StartingEquipmentOptionMapper> StartingEquipmentsOptions { get; set; } = new List<StartingEquipmentOptionMapper>();
    [BsonElement("multi_classing")]
    public Multiclass Multiclassing { get; set; } = new Multiclass();
    [BsonElement("spellcasting")]
    public Spellcasting? SpellcastingAbility { get; set; }

    [BsonIgnoreExtraElements]
    public class Multiclass
    {
        [BsonElement("prerequisites")]
        public List<Prerequisite>? Prerequisites { get; set; }
        [BsonElement("prerequisite_options")]
        public PrerequisiteOptions? PrerequisiteOptions { get; set; }
        [BsonElement("proficiencies")]
        public List<BaseEntity> Proficiencies { get; set; } = new List<BaseEntity>();
        [BsonElement("proficiency_choices")]
        public List<ProficiencyChoice>? ProficiencyChoices { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Prerequisite
    {
        [BsonElement("ability_score")]
        public BaseEntity AbilityScore { get; set; } = new BaseEntity();
        [BsonElement("minimum_score")]
        public byte MinimumScore { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class PrerequisiteOptions
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("from")]
        public PrerequisiteFrom From { get; set; } = new PrerequisiteFrom();
    }

    [BsonIgnoreExtraElements]
    public class PrerequisiteFrom
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<ScorePrerequisiteOption> Options { get; set; } = new List<ScorePrerequisiteOption>();
    }

    [BsonIgnoreExtraElements]
    public class ScorePrerequisiteOption
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; } = string.Empty;
        [BsonElement("ability_score")]
        public BaseEntity AbilityScore { get; set; } = new BaseEntity();
        [BsonElement("minimum_score")]
        public byte MinimumScore { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ProficiencyChoice
    {
        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("from")]
        public ProficiencyFrom From { get; set; } = new ProficiencyFrom();
    }

    [BsonIgnoreExtraElements]
    public class ProficiencyFrom
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<ProficiencyOption> Options { get; set; } = new List<ProficiencyOption>();
    }

    [BsonIgnoreExtraElements]
    public class ProficiencyOption
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; } = string.Empty;
        [BsonElement("item")]
        public BaseEntity Item { get; set; } = new BaseEntity();
    }

    [BsonIgnoreExtraElements]
    public class Spellcasting
    {
        [BsonElement("level")]
        public byte Level { get; set; }
        [BsonElement("spellcasting_ability")]
        public BaseEntity SpellcastingAbility { get; set; } = new BaseEntity();
    }

    [BsonIgnoreExtraElements]
    public class Equipment
    {
        [BsonElement("equipment")]
        public BaseEntity Equip { get; set; }

        [BsonElement("quantity")]
        public short Quantity { get; set; }

        public Equipment(BaseEntity equip, short quantity)
        {
            Equip = equip;
            Quantity = quantity;
        }
    }

    public ClassMapper(string index, string name) : base(index, name) {  }
}
