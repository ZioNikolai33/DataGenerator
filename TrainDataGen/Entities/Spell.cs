namespace TrainDataGen.Entities;

public class Spell
{
    public string Name { get; set; }
    public string Range { get; set; }
    public bool Ritual { get; set; }
    public int Level { get; set; }
    public string Duration { get; set; }
    public bool Concentration { get; set; }
    public string CastingTime { get; set; }
    public Dictionary<string, string>? HealAtSlotLevel { get; set; }
    public string School { get; set; }
    public List<string> Classes { get; set; }
    public List<string>? Subclasses { get; set; }
    public Area? AreaEffect { get; set; }
    public DifficultyClass? Dc { get; set; }
    public SpellDamage? Damage { get; set; }
    public string? AttackType { get; set; }

    public Spell(dynamic spell)
    {
        Name = spell.index;
        Range = spell.range;
        Ritual = spell.ritual;
        Level = spell.level;
        Duration = spell.duration;
        Concentration = spell.concentration;
        CastingTime = spell.casting_time;
        HealAtSlotLevel = spell.heal_at_slot_level != null ? spell.heal_at_slot_level as Dictionary<string, string> : null;
        School = spell.school.index;
        Classes = new List<string>();
        foreach (var item in spell.classes)
            Classes.Add(item.index);
        Subclasses = spell.subclasses != null ? new List<string>(spell.subclasses.Select(s => s.index)) : null;
        AreaEffect = spell.area_of_effect != null ? new Area(spell.area_of_effect) : null;
        Dc = spell.dc != null ? new DifficultyClass(spell.dc) : null;
        Damage = spell.damage != null ? new SpellDamage(spell.damage) : null;
        AttackType = spell.attack_type != null ? spell.attack_type : null;
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