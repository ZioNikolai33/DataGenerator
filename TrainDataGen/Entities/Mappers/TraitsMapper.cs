namespace TrainDataGen.Entities.Mappers;

public class TraitsMapper : BaseEntity
{
    public List<BaseEntity> Races { get; set; }
    public List<BaseEntity> Subraces { get; set; }
    public List<BaseEntity> Proficiencies { get; set; }
    public BaseEntity Parent { get; set; }
    public TraitSpecific TraitSpec { get; set; }
    public ProficiencyChoiceMapper ProficiencyChoise { get; set; }

    public class TraitSpecific
    {
        public BaseEntity DamageType { get; set; }
        public BreathWeapon BreathWeapon { get; set; }
        public SubtraitOptions SubtraitOptions { get; set; }
        public SpellOptions SpellOptions { get; set; }
    }

    public class BreathWeapon
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public AreaOfEffect AreaEffect { get; set; }
        public Usage Use { get; set; }
        public DifficultyClass Dc { get; set; }
        public List<Damage> Damage { get; set; }
    }

    public class DC
    {
        public BaseEntity DcType { get; set; }
        public string SuccessType { get; set; }
    }

    public class Damage
    {
        public BaseEntity DamageType { get; set; }
        public Dictionary<byte, string> DamageAtCharacterLevel { get; set; }
    }

    public class SubtraitOptions
    {
        public byte Choose { get; set; }
        public SubtraitFrom From { get; set; }
        public string Type { get; set; }
    }

    public class SubtraitFrom
    {
        public string OptionSetType { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        public string OptionType { get; set; }
        public BaseEntity Item { get; set; }
    }

    public class SpellOptions
    {
        public byte Choose { get; set; }
        public SpellFrom From { get; set; }
        public string Type { get; set; }
    }

    public class SpellFrom
    {
        public string OptionSetType { get; set; }
        public List<Option> Options { get; set; }
    }

    public TraitsMapper(string index, string name) : base(index, name) { }
}