using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Abstracts;

public abstract class Creature
{
    public string Index { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int HitPoints { get; set; }
    public byte ArmorClass { get; set; }
    public byte ProficiencyBonus { get; set; }
    public Entities.Attribute Strength { get; set; } = new Entities.Attribute();
    public Entities.Attribute Dexterity { get; set; } = new Entities.Attribute();
    public Entities.Attribute Constitution { get; set; } = new Entities.Attribute();
    public Entities.Attribute Intelligence { get; set; } = new Entities.Attribute();
    public Entities.Attribute Wisdom { get; set; } = new Entities.Attribute();
    public Entities.Attribute Charisma { get; set; } = new Entities.Attribute();
    public List<Skill> Skills { get; set; } = new List<Skill>();
    public List<string> Proficiencies { get; set; } = new List<string>();
    public List<string> Vulnerabilities { get; set; } = new List<string>();
    public List<string> Resistances { get; set; } = new List<string>();
    public List<string> Immunities { get; set; } = new List<string>();

    protected void CreateSkills()
    {
        Skills = new List<Skill>
        {
             new Skill(new BaseEntity("skill-acrobatics", "Acrobatics"), Dexterity.Modifier),
             new Skill(new BaseEntity("skill-animal-handling", "Animal Handling"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-arcana", "Arcana"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-athletics", "Athletics"), Strength.Modifier),
             new Skill(new BaseEntity("skill-deception", "Deception"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-history", "History"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-insight", "Insight"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-intimidation", "Intimidation"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-investigation", "Investigation"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-medicine", "Medicine"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-nature", "Nature"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-perception", "Perception"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-performance", "Performance"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-persuasion", "Persuasion"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-religion", "Religion"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-sleight-of-hand", "Sleight of Hand"), Dexterity.Modifier),
             new Skill(new BaseEntity("skill-stealth", "Stealth"), Dexterity.Modifier),
             new Skill(new BaseEntity("skill-survival", "Survival"), Wisdom.Modifier)
        };
    } 
}
