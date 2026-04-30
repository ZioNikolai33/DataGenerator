using System.Text.Json.Serialization;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Entities;

public class EncounterXgBoost
{
    [JsonIgnore]
    public string Id { get; set; }

    // Encounter features
    public CRRatios Difficulty { get; set; }
    public Results Outcome { get; set; }
    public short TotalRounds { get; set; }

    // Party features
    public int PartySize { get; set; }
    public int TotalPartyLevel { get; set; }
    public int MeanPartyLevel { get; set; }
    public int MaxPartyLevel { get; set; }
    public int TotalPartyHP { get; set; }
    public int MeanPartyHp { get; set; }
    public int MaxPartyHp { get; set; }
    public int TotalPartyAc { get; set; }
    public int MeanPartyAc { get; set; }
    public int MaxPartyAc { get; set; }
    public int TotalPartySpeed { get; set; }
    public int MeanPartySpeed { get; set; }
    public int MaxPartySpeed { get; set; }
    public int TotalPartyInitiative { get; set; }
    public int MeanPartyInitiative { get; set; }
    public int MaxPartyInitiative { get; set; }
    public int TotalPartyProficiencyBonus { get; set; }
    public int MeanPartyProficiencyBonus { get; set; }
    public int MaxPartyProficiencyBonus { get; set; }
    public int TotalPartyAttributes { get; set; }
    public int MeanPartyAttributes { get; set; }
    public int MaxPartyAttributes { get; set; }
    public int TotalPartyStrength { get; set; }
    public int MeanPartyStrength { get; set; }
    public int MaxPartyStrength { get; set; }
    public int TotalPartyDexterity { get; set; }
    public int MeanPartyDexterity { get; set; }
    public int MaxPartyDexterity { get; set; }
    public int TotalPartyConstitution { get; set; }
    public int MeanPartyConstitution { get; set; }
    public int MaxPartyConstitution { get; set; }
    public int TotalPartyIntelligence { get; set; }
    public int MeanPartyIntelligence { get; set; }
    public int MaxPartyIntelligence { get; set; }
    public int TotalPartyWisdom { get; set; }
    public int MeanPartyWisdom { get; set; }
    public int MaxPartyWisdom { get; set; }
    public int TotalPartyCharisma { get; set; }
    public int MeanPartyCharisma { get; set; }
    public int MaxPartyCharisma { get; set; }
    public Dictionary<string, int> TotalPartySkills { get; set; }
    public Dictionary<string, int> MeanPartySkills { get; set; }
    public Dictionary<string, int> MaxPartySkills { get; set; }
    public Dictionary<string, int> ClassDistribution { get; set; }
    public Dictionary<string, int> SubclassDistribution { get; set; }
    public Dictionary<string, int> RaceDistribution { get; set; }
    public Dictionary<string, int> SubraceDistribution { get; set; }
    public Dictionary<string, int> TraitsDistribution { get; set; }
    public Dictionary<string, int> FeaturesDistribution { get; set; }
    public Dictionary<string, int> ClassSpecificsDistribution { get; set; }
    public Dictionary<string, int> SubclassSpecificsDistribution { get; set; }
    public Dictionary<string, int> FeatureSpecificsDistribution { get; set; }
    public Dictionary<string, int> ProficienciesDistribution { get; set; }
    public Dictionary<string, int> TotalPartyVulnerabilitiesDistribution { get; set; }
    public int TotalPartyVulnerabilities { get; set; }
    public Dictionary<string, int> TotalPartyResistancesDistribution { get; set; }
    public int TotalPartyResistances { get; set; }
    public Dictionary<string, int> TotalPartyImmunitiesDistribution { get; set; }
    public int TotalPartyImmunities { get; set; }
    public int TotalPartyMeleeWeapons { get; set; }
    public int MeanPartyMeleeWeapons { get; set; }
    public int MaxPartyMeleeWeapons { get; set; }
    public int TotalPartyRangedWeapons { get; set; }
    public int MeanPartyRangedWeapons { get; set; }
    public int MaxPartyRangedWeapons { get; set; }
    public int TotalPartyArmors { get; set; }
    public int MeanPartyArmors { get; set; }
    public int MaxPartyArmors { get; set; }
    public int TotalPartySpellcastingBonus { get; set; }
    public int MeanPartySpellcastingBonus { get; set; }
    public int MaxPartySpellcastingBonus { get; set; }
    public int TotalPartySpells { get; set; }
    public int MeanPartySpells { get; set; }
    public int MaxPartySpells { get; set; }
    public int TotalPartyCantrips { get; set; }
    public int MeanPartyCantrips { get; set; }
    public int MaxPartyCantrips { get; set; }
    public int TotalPartySpellsDamage { get; set; }
    public int MeanPartySpellsDamage { get; set; }
    public int MaxPartySpellsDamage { get; set; }
    public int TotalPartySpellSlots { get; set; }
    public int MeanPartySpellSlots { get; set; }
    public int MaxPartySpellSlots { get; set; }
    public int TotalPartyBaseStats { get; set; }
    public int MeanPartyBaseStats { get; set; }
    public int MaxPartyBaseStats { get; set; }
    public int TotalPartyOffensivePower { get; set; }
    public int MeanPartyOffensivePower { get; set; }
    public int MaxPartyOffensivePower { get; set; }
    public int TotalPartyHealingPower { get; set; }
    public int MeanPartyHealingPower { get; set; }
    public int MaxPartyHealingPower { get; set; }

    // Monster features
    public int MonsterSize { get; set; }
    public double TotalMonstersCR { get; set; }
    public double MeanMonstersCR { get; set; }
    public double MaxMonstersCR { get; set; }
    public int TotalMonstersHP { get; set; }
    public int MeanMonstersHP { get; set; }
    public int MaxMonstersHP { get; set; }
    public int TotalMonstersAC { get; set; }
    public int MeanMonstersAC { get; set; }
    public int MaxMonstersAC { get; set; }
    public int TotalMonstersSpeed { get; set; }
    public int MeanMonstersSpeed { get; set; }
    public int MaxMonstersSpeed { get; set; }
    public int TotalMonstersInitiative { get; set; }
    public int MeanMonstersInitiative { get; set; }
    public int MaxMonstersInitiative { get; set; }
    public int TotalMonstersProficiencyBonus { get; set; }
    public int MeanMonstersProficiencyBonus { get; set; }
    public int MaxMonstersProficiencyBonus { get; set; }
    public int TotalMonstersAttributes { get; set; }
    public int MeanMonstersAttributes { get; set; }
    public int MaxMonstersAttributes { get; set; }
    public int TotalMonstersStrength { get; set; }
    public int MeanMonstersStrength { get; set; }
    public int MaxMonstersStrength { get; set; }
    public int TotalMonstersDexterity { get; set; }
    public int MeanMonstersDexterity { get; set; }
    public int MaxMonstersDexterity { get; set; }
    public int TotalMonstersConstitution { get; set; }
    public int MeanMonstersConstitution { get; set; }
    public int MaxMonstersConstitution { get; set; }
    public int TotalMonstersIntelligence { get; set; }
    public int MeanMonstersIntelligence { get; set; }
    public int MaxMonstersIntelligence { get; set; }
    public int TotalMonstersWisdom { get; set; }
    public int MeanMonstersWisdom { get; set; }
    public int MaxMonstersWisdom { get; set; }
    public int TotalMonstersCharisma { get; set; }
    public int MeanMonstersCharisma { get; set; }
    public int MaxMonstersCharisma { get; set; }
    public Dictionary<string, int> MonstersDistribution { get; set; }
    public Dictionary<string, int> TotalMonstersSkills { get; set; }
    public Dictionary<string, int> MeanMonstersSkills { get; set; }
    public Dictionary<string, int> MaxMonstersSkills { get; set; }
    public Dictionary<string, int> TotalMonstersProficienciesDistribution { get; set; }
    public int TotalMonstersProficiencies { get; set; }
    public Dictionary<string, int> TotalMonstersVulnerabilitiesDistribution { get; set; }
    public int TotalMonstersVulnerabilities { get; set; }
    public Dictionary<string, int> TotalMonstersResistancesDistribution { get; set; }
    public int TotalMonstersResistances { get; set; }
    public Dictionary<string, int> TotalMonstersImmunitiesDistribution { get; set; }
    public int TotalMonstersImmunities { get; set; }
    public int TotalMonstersSpecialAbilities { get; set; }
    public int MeanMonstersSpecialAbilities { get; set; }
    public int MaxMonstersSpecialAbilities { get; set; }
    public int TotalMonstersActions { get; set; }
    public int MeanMonstersActions { get; set; }
    public int MaxMonstersActions { get; set; }
    public int TotalMonstersDamageActions { get; set; }
    public int MeanMonstersDamageActions { get; set; }
    public int MaxMonstersDamageActions { get; set; }
    public int TotalMonstersLegendaryActions { get; set; }
    public int MeanMonstersLegendaryActions { get; set; }
    public int MaxMonstersLegendaryActions { get; set; }
    public int TotalMonstersDamageLegendaryActions { get; set; }
    public int MeanMonstersDamageLegendaryActions { get; set; }
    public int MaxMonstersDamageLegendaryActions { get; set; }
    public int TotalMonstersReactions { get; set; }
    public int MeanMonstersReactions { get; set; }
    public int MaxMonstersReactions { get; set; }
    public int TotalMonstersBaseStats { get; set; }
    public int MeanMonstersBaseStats { get; set; }
    public int MaxMonstersBaseStats { get; set; }
    public int TotalMonstersOffensivePower { get; set; }
    public int MeanMonstersOffensivePower { get; set; }
    public int MaxMonstersOffensivePower { get; set; }
    public int TotalMonstersHealingPower { get; set; }
    public int MeanMonstersHealingPower { get; set; }
    public int MaxMonstersHealingPower { get; set; }

    public EncounterXgBoost(Encounter encounter)
    {
        Id = encounter.Id;

        // Encounter features
        Difficulty = encounter.Difficulty;
        Outcome = encounter.Outcome.Outcome;
        TotalRounds = encounter.Outcome.TotalRounds;

        // Party features
        PartySize = encounter.PartyMembers.Count;
        TotalPartyLevel = encounter.PartyMembers.Sum(pm => pm.Level);
        MeanPartyLevel = (int)encounter.PartyMembers.Average(pm => pm.Level);
        MaxPartyLevel = encounter.PartyMembers.Max(pm => pm.Level);
        TotalPartyHP = encounter.PartyMembers.Sum(pm => pm.HitPoints);
        MeanPartyHp = (int)encounter.PartyMembers.Average(pm => pm.HitPoints);
        MaxPartyHp = encounter.PartyMembers.Max(pm => pm.HitPoints);
        TotalPartyAc = encounter.PartyMembers.Sum(pm => pm.ArmorClass);
        MeanPartyAc = (int)encounter.PartyMembers.Average(pm => pm.ArmorClass);
        MaxPartyAc = encounter.PartyMembers.Max(pm => pm.ArmorClass);
        TotalPartySpeed = encounter.PartyMembers.Sum(pm => pm.Speed);
        MeanPartySpeed = (int)encounter.PartyMembers.Average(pm => pm.Speed);
        MaxPartySpeed = encounter.PartyMembers.Max(pm => pm.Speed);
        TotalPartyInitiative = encounter.PartyMembers.Sum(pm => pm.Initiative);
        MeanPartyInitiative = (int)encounter.PartyMembers.Average(pm => pm.Initiative);
        MaxPartyInitiative = encounter.PartyMembers.Max(pm => pm.Initiative);
        TotalPartyProficiencyBonus = encounter.PartyMembers.Sum(pm => pm.ProficiencyBonus);
        MeanPartyProficiencyBonus = (int)encounter.PartyMembers.Average(pm => pm.ProficiencyBonus);
        MaxPartyProficiencyBonus = encounter.PartyMembers.Max(pm => pm.ProficiencyBonus);
        TotalPartyAttributes = encounter.PartyMembers.Sum(pm => pm.BaseStats);
        MeanPartyAttributes = (int)encounter.PartyMembers.Average(pm => pm.BaseStats);
        MaxPartyAttributes = encounter.PartyMembers.Max(pm => pm.BaseStats);
        TotalPartyStrength = encounter.PartyMembers.Sum(pm => pm.Strength.Value);
        MeanPartyStrength = (int)encounter.PartyMembers.Average(pm => pm.Strength.Value);
        MaxPartyStrength = encounter.PartyMembers.Max(pm => pm.Strength.Value);
        TotalPartyDexterity = encounter.PartyMembers.Sum(pm => pm.Dexterity.Value);
        MeanPartyDexterity = (int)encounter.PartyMembers.Average(pm => pm.Dexterity.Value);
        MaxPartyDexterity = encounter.PartyMembers.Max(pm => pm.Dexterity.Value);
        TotalPartyConstitution = encounter.PartyMembers.Sum(pm => pm.Constitution.Value);
        MeanPartyConstitution = (int)encounter.PartyMembers.Average(pm => pm.Constitution.Value);
        MaxPartyConstitution = encounter.PartyMembers.Max(pm => pm.Constitution.Value);
        TotalPartyIntelligence = encounter.PartyMembers.Sum(pm => pm.Intelligence.Value);
        MeanPartyIntelligence = (int)encounter.PartyMembers.Average(pm => pm.Intelligence.Value);
        MaxPartyIntelligence = encounter.PartyMembers.Max(pm => pm.Intelligence.Value);
        TotalPartyWisdom = encounter.PartyMembers.Sum(pm => pm.Wisdom.Value);
        MeanPartyWisdom = (int)encounter.PartyMembers.Average(pm => pm.Wisdom.Value);
        MaxPartyWisdom = encounter.PartyMembers.Max(pm => pm.Wisdom.Value);
        TotalPartyCharisma = encounter.PartyMembers.Sum(pm => pm.Charisma.Value);
        MeanPartyCharisma = (int)encounter.PartyMembers.Average(pm => pm.Charisma.Value);
        MaxPartyCharisma = encounter.PartyMembers.Max(pm => pm.Charisma.Value);
        TotalPartySkills = encounter.PartyMembers
            .SelectMany(pm => pm.Skills)
            .GroupBy(s => s.Name)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.Modifier));
        MeanPartySkills = encounter.PartyMembers
            .SelectMany(pm => pm.Skills)
            .GroupBy(s => s.Name)
            .ToDictionary(g => g.Key, g => (int)g.Average(s => s.Modifier));
        MaxPartySkills = encounter.PartyMembers
            .SelectMany(pm => pm.Skills)
            .GroupBy(s => s.Name)
            .ToDictionary(g => g.Key, g => g.Max(s => (int)s.Modifier));
        ClassDistribution = encounter.PartyMembers
            .GroupBy(pm => pm.Class)
            .ToDictionary(g => g.Key, g => g.Count());
        SubclassDistribution = encounter.PartyMembers
            .GroupBy(pm => pm.Subclass)
            .ToDictionary(g => g.Key, g => g.Count());
        RaceDistribution = encounter.PartyMembers
            .GroupBy (pm => pm.Race)
            .ToDictionary(g => g.Key, g => g.Count());
        SubraceDistribution = encounter.PartyMembers
            .GroupBy(pm => pm.Subrace ?? string.Empty)
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .ToDictionary(g => g.Key, g => g.Count());
        TraitsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Traits)
            .GroupBy(t => t)
            .ToDictionary(g => g.Key, g => g.Count());
        FeaturesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Features)
            .GroupBy(f => f.Name)
            .ToDictionary(g => g.Key, g => g.Count());
        ClassSpecificsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.ClassSpecific?.Keys ?? Enumerable.Empty<string>())
            .GroupBy(cs => cs)
            .ToDictionary(g => g.Key, g => g.Count());
        SubclassSpecificsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.SubclassSpecific?.Keys ?? Enumerable.Empty<string>())
            .GroupBy(ss => ss)
            .ToDictionary(g => g.Key, g => g.Count());
        FeatureSpecificsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.FeatureSpecifics ?? Enumerable.Empty<string>())
            .GroupBy(fs => fs)
            .ToDictionary(g => g.Key, g => g.Count());
        ProficienciesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Proficiencies)
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());
        TotalPartyVulnerabilitiesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Vulnerabilities)
            .GroupBy(v => v)
            .ToDictionary(g => g.Key, g => g.Count());
        TotalPartyVulnerabilities = encounter.PartyMembers.Sum(pm => pm.Vulnerabilities.Count);
        TotalPartyResistancesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Resistances)
            .GroupBy(r => r)
            .ToDictionary(g => g.Key, g => g.Count());
        TotalPartyResistances = encounter.PartyMembers.Sum(pm => pm.Resistances.Count);
        TotalPartyImmunitiesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Immunities)
            .GroupBy(i => i)
            .ToDictionary(g => g.Key, g => g.Count());
        TotalPartyImmunities = encounter.PartyMembers.Sum(pm => pm.Immunities.Count);
        TotalPartyMeleeWeapons = encounter.PartyMembers.Sum(pm => pm.MeleeWeapons.Count);
        MeanPartyMeleeWeapons = (int)encounter.PartyMembers.Average(pm => pm.MeleeWeapons.Count);
        MaxPartyMeleeWeapons = encounter.PartyMembers.Max(pm => pm.MeleeWeapons.Count);
        TotalPartyRangedWeapons = encounter.PartyMembers.Sum(pm => pm.RangedWeapons.Count);
        MeanPartyRangedWeapons = (int)encounter.PartyMembers.Average(pm => pm.RangedWeapons.Count);
        MaxPartyRangedWeapons = encounter.PartyMembers.Max(pm => pm.RangedWeapons.Count);
        TotalPartyArmors = encounter.PartyMembers.Sum(pm => pm.Armors.Count);
        MeanPartyArmors = (int)encounter.PartyMembers.Average(pm => pm.Armors.Count);
        MaxPartyArmors = encounter.PartyMembers.Max(pm => pm.Armors.Count);
        TotalPartySpellcastingBonus = encounter.PartyMembers.Sum(pm => UtilityMethods.GetSpellcastingModifier(pm));
        MeanPartySpellcastingBonus = (int)encounter.PartyMembers.Average(pm => UtilityMethods.GetSpellcastingModifier(pm));
        MaxPartySpellcastingBonus = encounter.PartyMembers.Max(pm => UtilityMethods.GetSpellcastingModifier(pm));
        TotalPartySpells = encounter.PartyMembers.Sum(pm => pm.Spells.Count);
        MeanPartySpells = (int)encounter.PartyMembers.Average(pm => pm.Spells.Count);
        MaxPartySpells = encounter.PartyMembers.Max(pm => pm.Spells.Count);
        TotalPartyCantrips = encounter.PartyMembers.Sum(pm => pm.Cantrips.Count);
        MeanPartyCantrips = (int)encounter.PartyMembers.Average(pm => pm.Cantrips.Count);
        MaxPartyCantrips = encounter.PartyMembers.Max(pm => pm.Cantrips.Count);
        TotalPartySpellSlots = encounter.PartyMembers.Sum(pm => pm.SpellSlots.GetTotalSlots());
        MeanPartySpellSlots = (int)encounter.PartyMembers.Average(pm => pm.SpellSlots.GetTotalSlots());
        MaxPartySpellSlots = encounter.PartyMembers.Max(pm => pm.SpellSlots.GetTotalSlots());
        TotalPartyBaseStats = encounter.PartyMembers.Sum(pm => pm.BaseStats);
        MeanPartyBaseStats = (int)encounter.PartyMembers.Average(pm => pm.BaseStats);
        MaxPartyBaseStats = encounter.PartyMembers.Max(pm => pm.BaseStats);
        TotalPartyOffensivePower = encounter.PartyMembers.Sum(pm => pm.OffensivePower);
        MeanPartyOffensivePower = (int)encounter.PartyMembers.Average(pm => pm.OffensivePower);
        MaxPartyOffensivePower = encounter.PartyMembers.Max(pm => pm.OffensivePower);
        TotalPartyHealingPower = encounter.PartyMembers.Sum(pm => pm.HealingPower);
        MeanPartyHealingPower = (int)encounter.PartyMembers.Average(pm => pm.HealingPower);
        MaxPartyHealingPower = encounter.PartyMembers.Max(pm => pm.HealingPower);
        TotalPartySpellsDamage = encounter.PartyMembers.Sum(pm => pm.Spells.Count(s => s.IsDamageSpell()));
        MeanPartySpellsDamage = (int)encounter.PartyMembers.Average(pm => pm.Spells.Count(s => s.IsDamageSpell()));
        MaxPartySpellsDamage = encounter.PartyMembers.Max(pm => pm.Spells.Count(s => s.IsDamageSpell()));

        // Monster features
        MonsterSize = encounter.Monsters.Count;
        TotalMonstersCR = encounter.Monsters.Sum(m => m.ChallengeRating);
        MeanMonstersCR = encounter.Monsters.Average(m => m.ChallengeRating);
        MaxMonstersCR = encounter.Monsters.Max(m => m.ChallengeRating);
        TotalMonstersHP = encounter.Monsters.Sum(m => m.HitPoints);
        MeanMonstersHP = (int)encounter.Monsters.Average(m => m.HitPoints);
        MaxMonstersHP = encounter.Monsters.Max(m => m.HitPoints);
        TotalMonstersAC = encounter.Monsters.Sum(m => m.ArmorClass);
        MeanMonstersAC = (int)encounter.Monsters.Average(m => m.ArmorClass);
        MaxMonstersAC = encounter.Monsters.Max(m => m.ArmorClass);
        TotalMonstersSpeed = encounter.Monsters.Sum(m => m.CalculateSpeedValue());
        MeanMonstersSpeed = (int)encounter.Monsters.Average(m => m.CalculateSpeedValue());
        MaxMonstersSpeed = encounter.Monsters.Max(m => m.CalculateSpeedValue());
        TotalMonstersInitiative = encounter.Monsters.Sum(m => m.Dexterity.Value);
        MeanMonstersInitiative = (int)encounter.Monsters.Average(m => m.Dexterity.Value);
        MaxMonstersInitiative = encounter.Monsters.Max(m => m.Dexterity.Value);
        TotalMonstersProficiencyBonus = encounter.Monsters.Sum(m => m.ProficiencyBonus);
        MeanMonstersProficiencyBonus = (int)encounter.Monsters.Average(m => m.ProficiencyBonus);
        MaxMonstersProficiencyBonus = encounter.Monsters.Max(m => m.ProficiencyBonus);
        TotalMonstersAttributes = encounter.Monsters.Sum(m => m.BaseStats);
        MeanMonstersAttributes = (int)encounter.Monsters.Average(m => m.BaseStats);
        MaxMonstersAttributes = encounter.Monsters.Max(m => m.BaseStats);
        TotalMonstersStrength = encounter.Monsters.Sum(m => m.Strength.Value);
        MeanMonstersStrength = (int)encounter.Monsters.Average(m => m.Strength.Value);
        MaxMonstersStrength = encounter.Monsters.Max(m => m.Strength.Value);
        TotalMonstersDexterity = encounter.Monsters.Sum(m => m.Dexterity.Value);
        MeanMonstersDexterity = (int)encounter.Monsters.Average(m => m.Dexterity.Value);
        MaxMonstersDexterity = encounter.Monsters.Max(m => m.Dexterity.Value);
        TotalMonstersConstitution = encounter.Monsters.Sum(m => m.Constitution.Value);
        MeanMonstersConstitution = (int)encounter.Monsters.Average(m => m.Constitution.Value);
        MaxMonstersConstitution = encounter.Monsters.Max(m => m.Constitution.Value);
        TotalMonstersIntelligence = encounter.Monsters.Sum(m => m.Intelligence.Value);
        MeanMonstersIntelligence = (int)encounter.Monsters.Average(m => m.Intelligence.Value);
        MaxMonstersIntelligence = encounter.Monsters.Max(m => m.Intelligence.Value);
        TotalMonstersWisdom = encounter.Monsters.Sum(m => m.Wisdom.Value);
        MeanMonstersWisdom = (int)encounter.Monsters.Average(m => m.Wisdom.Value);
        MaxMonstersWisdom = encounter.Monsters.Max(m => m.Wisdom.Value);
        TotalMonstersCharisma = encounter.Monsters.Sum(m => m.Charisma.Value);
        MeanMonstersCharisma = (int)encounter.Monsters.Average(m => m.Charisma.Value);
        MaxMonstersCharisma = encounter.Monsters.Max(m => m.Charisma.Value);
        MonstersDistribution = encounter.Monsters
            .GroupBy(m => m.Name)
            .ToDictionary(g => g.Key, g => g.Count());
        TotalMonstersSkills = encounter.Monsters
            .SelectMany(m => m.Skills)
            .GroupBy(s => s.Name)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.Modifier));
        MeanMonstersSkills = encounter.Monsters
            .SelectMany(m => m.Skills)
            .GroupBy(s => s.Name)
            .ToDictionary(g => g.Key, g => (int)g.Average(s => s.Modifier));
        MaxMonstersSkills = encounter.Monsters
            .SelectMany(m => m.Skills)
            .GroupBy(s => s.Name)
            .ToDictionary(g => g.Key, g => g.Max(s => (int)s.Modifier));
        TotalMonstersProficienciesDistribution = encounter.Monsters
            .SelectMany(m => m.Proficiencies)
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());
        TotalMonstersProficiencies = encounter.Monsters.Sum(m => m.Proficiencies.Count);
        TotalMonstersVulnerabilitiesDistribution = encounter.Monsters
            .SelectMany(m => m.Vulnerabilities)
            .GroupBy(v => v)
            .ToDictionary(g => g.Key, g => g.Count());
        TotalMonstersVulnerabilities = encounter.Monsters.Sum(m => m.Vulnerabilities.Count);
        TotalMonstersResistancesDistribution = encounter.Monsters
            .SelectMany(m => m.Resistances)
            .GroupBy(r => r)
            .ToDictionary(g => g.Key, g => g.Count());
        TotalMonstersResistances = encounter.Monsters.Sum(m => m.Resistances.Count);
        TotalMonstersImmunitiesDistribution = encounter.Monsters
            .SelectMany(m => m.Immunities)
            .GroupBy(i => i)
            .ToDictionary(g => g.Key, g => g.Count());
        TotalMonstersImmunities = encounter.Monsters.Sum(m => m.Immunities.Count);
        TotalMonstersSpecialAbilities = encounter.Monsters.Sum(m => m.SpecialAbilities.Count);
        MeanMonstersSpecialAbilities = (int)encounter.Monsters.Average(m => m.SpecialAbilities.Count);
        MaxMonstersSpecialAbilities = encounter.Monsters.Max(m => m.SpecialAbilities.Count);
        TotalMonstersActions = encounter.Monsters.Sum(m => m.Actions.Count);
        MeanMonstersActions = (int)encounter.Monsters.Average(m => m.Actions.Count);
        MaxMonstersActions = encounter.Monsters.Max(m => m.Actions.Count);
        TotalMonstersDamageActions = encounter.Monsters.Sum(m => m.Actions.Count(a => (((a.Dc?.DcValue != 0 && !string.IsNullOrEmpty(a.Dc?.DcType)) || (a.AttackBonus.HasValue)) && a.Damage.Count > 0) || a.Actions.Count > 0));
        MeanMonstersDamageActions = (int)encounter.Monsters.Average(m => m.Actions.Count(a => (((a.Dc?.DcValue != 0 && !string.IsNullOrEmpty(a.Dc?.DcType)) || (a.AttackBonus.HasValue)) && a.Damage.Count > 0) || a.Actions.Count > 0));
        MaxMonstersDamageActions = encounter.Monsters.Max(m => m.Actions.Count(a => (((a.Dc?.DcValue != 0 && !string.IsNullOrEmpty(a.Dc?.DcType)) || (a.AttackBonus.HasValue)) && a.Damage.Count > 0) || a.Actions.Count > 0));
        TotalMonstersLegendaryActions = encounter.Monsters.Sum(m => m.LegendaryActions.Count);
        MeanMonstersLegendaryActions = (int)encounter.Monsters.Average(m => m.LegendaryActions.Count);
        MaxMonstersLegendaryActions = encounter.Monsters.Max(m => m.LegendaryActions.Count);
        TotalMonstersDamageLegendaryActions = encounter.Monsters.Sum(m => m.LegendaryActions.Count(la => m.Actions.Select(a => a.Name).Contains(la.Name) || la.Damage?.Count() > 0));
        MeanMonstersDamageLegendaryActions = (int)encounter.Monsters.Average(m => m.LegendaryActions.Count(la => m.Actions.Select(a => a.Name).Contains(la.Name) || la.Damage?.Count() > 0));
        MaxMonstersDamageLegendaryActions = encounter.Monsters.Max(m => m.LegendaryActions.Count(la => m.Actions.Select(a => a.Name).Contains(la.Name) || la.Damage?.Count() > 0));
        TotalMonstersReactions = encounter.Monsters.Sum(m => m.Reactions.Count);
        MeanMonstersReactions = (int)encounter.Monsters.Average(m => m.Reactions.Count);
        MaxMonstersReactions = encounter.Monsters.Max(m => m.Reactions.Count);
        TotalMonstersBaseStats = encounter.Monsters.Sum(m => m.BaseStats);
        MeanMonstersBaseStats = (int)encounter.Monsters.Average(m => m.BaseStats);
        MaxMonstersBaseStats = encounter.Monsters.Max(m => m.BaseStats);
        TotalMonstersOffensivePower = encounter.Monsters.Sum(m => m.OffensivePower);
        MeanMonstersOffensivePower = (int)encounter.Monsters.Average(m => m.OffensivePower);
        MaxMonstersOffensivePower = encounter.Monsters.Max(m => m.OffensivePower);
        TotalMonstersHealingPower = encounter.Monsters.Sum(m => m.HealingPower);
        MeanMonstersHealingPower = (int)encounter.Monsters.Average(m => m.HealingPower);
        MaxMonstersHealingPower = encounter.Monsters.Max(m => m.HealingPower);
    }
}
