using System.Collections.Generic;
using System.Linq;

namespace TrainDataGen.Entities;

public class Subrace
{
    public string Name { get; set; }
    public string Race { get; set; }
    public List<AbilityBonus> AbilityScores { get; set; }
    public List<string> Proficiencies { get; set; }
    public List<string> Traits { get; set; }

    public Subrace(dynamic subrace)
    {
        Name = subrace.index;
        Race = subrace.race.index;
        AbilityScores = new List<AbilityBonus>();
        foreach (var item in subrace.ability_bonuses)
            AbilityScores.Add(new AbilityBonus(item));
        Proficiencies = new List<string>();
        foreach (var item in subrace.starting_proficiencies)
            Proficiencies.Add(item.index);
        Traits = new List<string>();
        foreach (var item in subrace.racial_traits)
            Traits.Add(item.index);
    }
}

public class Race
{
    public string Name { get; set; }
    public List<AbilityBonus> AbilityBonuses { get; set; }
    public List<string> Proficiencies { get; set; }
    public List<string> Traits { get; set; }
    public List<Subrace> Subraces { get; set; }
    public short Speed { get; set; }

    public Race(dynamic race, List<Subrace> subraceStats)
    {
        Name = race.index;
        AbilityBonuses = new List<AbilityBonus>();
        foreach (var item in race.ability_bonuses)
            AbilityBonuses.Add(new AbilityBonus(item));
        Proficiencies = new List<string>();
        foreach (var item in race.starting_proficiencies)
            Proficiencies.Add(item.index);
        Traits = new List<string>();
        foreach (var item in race.traits)
            Traits.Add(item.index);
        Subraces = subraceStats.Where(item => item.Race == Name).ToList();
        Speed = race.speed;
    }
}