using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Entities.MonsterEntities;

public class SpecialAbility
{
    public string Name { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public Spellcasting? Spellcast { get; set; }
    public Usage? Usage { get; set; }
    public MonsterDC? Dc { get; set; }
    public List<Damage>? Damage { get; set; }

    public class Spellcasting
    {
        public int Level { get; set; }
        public BaseEntity Ability { get; set; } = new BaseEntity();
        public int Dc { get; set; }
        public int Modifier { get; set; }
        public List<string> ComponentsRequired { get; set; } = new List<string>();
        public string School { get; set; } = string.Empty;
        public Slots SpellSlots { get; set; } = new Slots();
        public List<Spell> Spells { get; set; } = new List<Spell>();
    }

    public SpecialAbility(MonsterMapper.SpecialAbility ability)
    {
        Name = ability.Name;
        Desc = ability.Desc;
        if (ability.Spellcasting != null)
        {
            Spellcast = new Spellcasting
            {
                Level = ability.Spellcasting.Level ?? 0,
                Ability = ability.Spellcasting.Ability ?? new BaseEntity(),
                Dc = ability.Spellcasting.Dc ?? 0,
                Modifier = ability.Spellcasting.Modifier ?? 0,
                ComponentsRequired = ability.Spellcasting.ComponentsRequired ?? new List<string>(),
                School = ability.Spellcasting.School ?? string.Empty,
                SpellSlots = (ability.Spellcasting.Slots != null) ? new Slots(ability.Spellcasting.Slots) : new Slots(),
                Spells = ability.Spellcasting.Spells?.Select(item => new Spell(EntitiesFinder.GetEntityByIndex(Lists.spells, new BaseEntity(item.Url.Substring(item.Url.LastIndexOf('/') + 1), item.Name)))).ToList() ?? new List<Spell>()
            };
        }
        Usage = (ability.Usage != null) ? new Usage(ability.Usage.Type, ability.Usage.Times, ability.Usage.RestTypes, ability.Usage.Dice, ability.Usage.MinValue) : null;
        Dc = (ability.Dc != null) ? new MonsterDC(ability.Dc.DcType.Index, ability.Dc.SuccessType, ability.Dc.DcValue) : null;
        Damage = (ability.Damage != null) ? ability.Damage.Select(item => new Damage(item)).ToList() : null;
    }
}
