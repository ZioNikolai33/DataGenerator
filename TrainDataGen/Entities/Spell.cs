using TrainDataGen.Entities.Enums;
using TrainDataGen.Entities.Mappers;
using TrainDataGen.Utilities;

namespace TrainDataGen.Entities;

public class Spell: BaseEntity
{
    public string Range { get; set; }
    public byte Level { get; set; }
    public string Duration { get; set; }
    public bool Concentration { get; set; }
    public string CastingTime { get; set; }
    public Dictionary<string, string>? HealAtSlotLevel { get; set; }
    public BaseEntity School { get; set; }
    public List<BaseEntity> Classes { get; set; }
    public List<BaseEntity>? Subclasses { get; set; }
    public Area? AreaEffect { get; set; }
    public DifficultyClass? Dc { get; set; }
    public SpellDamage? Damage { get; set; }
    public string? AttackType { get; set; }

    public Spell(SpellMapper spell): base(spell.Index, spell.Name)
    {
        Index = spell.Index;
        Name = spell.Name;
        Range = spell.Range;
        Level = spell.Level;
        Duration = spell.Duration;
        Concentration = spell.Concentration;
        CastingTime = spell.CastingTime;
        HealAtSlotLevel = spell.HealAtSlotLevel;
        School = spell.School;
        Classes = spell.Classes;
        Subclasses = spell.Subclasses;
        AreaEffect = spell.AreaOfEffect;
        Dc = spell.Dc;
        Damage = spell.Damage != null ? new SpellDamage(spell.Damage) : null;
        AttackType = spell.AttackType;
    }
}

public class SpellDamage
{
    public string? DamageType { get; set; }
    public Dictionary<string, string>? DamageSlots { get; set; }
    public Dictionary<string, string>? DamageAtCharacterLevel { get; set; }

    public SpellDamage(dynamic damage)
    {
        DamageType = damage.damage_type != null ? damage.damage_type.index : null;
        DamageSlots = damage.damage_at_slot_level != null ? damage.damage_at_slot_level as Dictionary<string, string> : null;
        DamageAtCharacterLevel = damage.damage_at_character_level != null ? damage.damage_at_character_level as Dictionary<string, string> : null;
    }
}