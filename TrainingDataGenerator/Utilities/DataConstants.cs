namespace TrainingDataGenerator.Utilities;

public static class DataConstants
{
    public const short MaxSpellLevel = 9;
    public const short MinSpellLevel = 0;
    public const short MaxCharacterLevel = 20;
    public const short MinCharacterLevel = 1;
    public const short MinPartySize = 1;
    public const short MaxPartySize = 6;
    public const short MinLevelSection = 1;
    public const short MaxLevelSection = 5;

    public static readonly List<string> AbilityScores = [ "strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma" ];
    public static readonly List<string> Classes = [ "barbarian", "bard", "cleric", "druid", "fighter", "monk", "paladin", "ranger", "rogue", "sorcerer", "warlock", "wizard" ];
    public static readonly List<string> Races = [ "dwarf", "elf", "halfling", "human", "dragonborn", "gnome", "half-elf", "half-orc", "tiefling" ];
    public static readonly Dictionary<byte, int> ValueToModifiers = new()
    {
        { 1, -5 },
        { 2, -4 },
        { 3, -4 },
        { 4, -3 },
        { 5, -3 },
        { 6, -2 },
        { 7, -2 },
        { 8, -1 },
        { 9, -1 },
        { 10, 0 },
        { 11, 0 },
        { 12, 1 },
        { 13, 1 },
        { 14, 2 },
        { 15, 2 },
        { 16, 3 },
        { 17, 3 },
        { 18, 4 },
        { 19, 4 },
        { 20, 5 },
        { 21, 5 },
        { 22, 6 },
        { 23, 6 },
        { 24, 7 },
        { 25, 7 },
        { 26, 8 },
        { 27, 8 },
        { 28, 9 },
        { 29, 9 },
        { 30, 10 }
    };
}
