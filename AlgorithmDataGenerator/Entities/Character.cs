using TrainingDataGenerator.Entities;

namespace AlgorithmDataGenerator.Entities;

public class Character
{
    public string Name { get; set; } = string.Empty;    
    public short Level { get; set; }
    public string Race { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int BaseStats { get; set; }

    public Character(string name, short level, string race, string @class, int baseStats)
    {
        Name = name;
        Level = level;
        Race = race;
        Class = @class;
        BaseStats = baseStats;
    }

    public Character(PartyMember member, int baseStats)
    {
        Name = member.Name;
        Level = member.Level;
        Race = member.Race;
        Class = member.Class;
        BaseStats = baseStats;
    }

    public override string ToString() =>
        $"{Name} - Level: {Level}, Race: {Race}, Class: {Class}, BaseStats: {BaseStats}";
}