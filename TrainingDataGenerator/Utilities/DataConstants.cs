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
}
