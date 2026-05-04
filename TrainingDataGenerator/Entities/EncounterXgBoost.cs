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
    public double HpRatio { get; set; }
    public double AcRatio { get; set; }
    public double SizeRatio { get; set; }

    // Party Size
    public int PartySize { get; set; }

    // Party Level
    public int TotalPartyLevel { get; set; }
    public int MeanPartyLevel { get; set; }
    public int MaxPartyLevel { get; set; }

    // Party HP
    public int TotalPartyHP { get; set; }
    public int MeanPartyHp { get; set; }
    public int MaxPartyHp { get; set; }

    // Party AC
    public int TotalPartyAc { get; set; }
    public int MeanPartyAc { get; set; }
    public int MaxPartyAc { get; set; }

    // Party Speed
    public int TotalPartySpeed { get; set; }
    public int MeanPartySpeed { get; set; }
    public int MaxPartySpeed { get; set; }

    // Party Initiative
    public int TotalPartyInitiative { get; set; }
    public int MeanPartyInitiative { get; set; }
    public int MaxPartyInitiative { get; set; }

    // Party Attributes
    public int TotalPartyAttributes { get; set; }
    public int TotalPartyStrength { get; set; }
    public int TotalPartyDexterity { get; set; }
    public int TotalPartyConstitution { get; set; }
    public int TotalPartyIntelligence { get; set; }
    public int TotalPartyWisdom { get; set; }
    public int TotalPartyCharisma { get; set; }
    public int MeanPartyAttributes { get; set; }
    public int MeanPartyStrength { get; set; }
    public int MeanPartyDexterity { get; set; }
    public int MeanPartyConstitution { get; set; }
    public int MeanPartyIntelligence { get; set; }
    public int MeanPartyWisdom { get; set; }
    public int MeanPartyCharisma { get; set; }
    public int MaxPartyAttributes { get; set; }
    public int MaxPartyStrength { get; set; }
    public int MaxPartyDexterity { get; set; }
    public int MaxPartyConstitution { get; set; }
    public int MaxPartyIntelligence { get; set; }
    public int MaxPartyWisdom { get; set; }
    public int MaxPartyCharisma { get; set; }

    // Party Skills
    public Dictionary<string, int> TotalPartySkills { get; set; }
    public Dictionary<string, int> MeanPartySkills { get; set; }
    public Dictionary<string, int> MaxPartySkills { get; set; }

    // Party Proficiencies
    public int TotalPartyProficiencies { get; set; }
    public int MeanPartyProficiencies { get; set; }
    public int MaxPartyProficiencies { get; set; }
    
    public Dictionary<string, int> PartyClassDistribution { get; set; }
    public Dictionary<string, int> PartySubclassDistribution { get; set; }
    public Dictionary<string, int> PartyRaceDistribution { get; set; }
    public Dictionary<string, int> PartySubraceDistribution { get; set; }
    public Dictionary<string, int> PartyTraitsDistribution { get; set; }
    public Dictionary<string, int> PartyFeaturesDistribution { get; set; }
    public Dictionary<string, int> PartyClassSpecificsDistribution { get; set; }
    public Dictionary<string, int> PartySubclassSpecificsDistribution { get; set; }
    public Dictionary<string, int> PartyFeatureSpecificsDistribution { get; set; }
    public Dictionary<string, int> PartyProficienciesDistribution { get; set; }
    public Dictionary<string, int> PartyMeleeWeaponsDistribution { get; set; }
    public Dictionary<string, int> PartyRangedWeaponsDistribution { get; set; }
    public Dictionary<string, int> PartyArmorsDistribution { get; set; }
    public Dictionary<string, int> PartySpellsDistribution { get; set; }
    public Dictionary<string, int> PartyCantripsDistribution { get; set; }
    public Dictionary<string, int> PartySpellSlotsDistribution { get; set; }
    public Dictionary<string, int> PartyVulnerabilitiesDistribution { get; set; }    
    public Dictionary<string, int> PartyResistancesDistribution { get; set; }
    public Dictionary<string, int> PartyImmunitiesDistribution { get; set; }

    // Party Vulnerabilities
    public int TotalPartyVulnerabilities { get; set; }
    public int MeanPartyVulnerabilities { get; set; }
    public int MaxPartyVulnerabilities { get; set; }

    // Party Resistances
    public int TotalPartyResistances { get; set; }
    public int MeanPartyResistances { get; set; }
    public int MaxPartyResistances { get; set; }

    // Party Immunities
    public int TotalPartyImmunities { get; set; }
    public int MeanPartyImmunities { get; set; }
    public int MaxPartyImmunities { get; set; }

    // Party Spellcasting Bonus
    public int TotalPartySpellcastingBonus { get; set; }
    public int MeanPartySpellcastingBonus { get; set; }
    public int MaxPartySpellcastingBonus { get; set; }

    // Monster Size
    public int MonsterSize { get; set; }

    // Monster CR
    public double TotalMonstersCR { get; set; }
    public double MeanMonstersCR { get; set; }
    public double MaxMonstersCR { get; set; }

    // Monster HP
    public int TotalMonstersHP { get; set; }
    public int MeanMonstersHP { get; set; }
    public int MaxMonstersHP { get; set; }

    // Monster AC
    public int TotalMonstersAC { get; set; }
    public int MeanMonstersAC { get; set; }
    public int MaxMonstersAC { get; set; }

    // Monster Speed
    public int TotalMonstersSpeed { get; set; }
    public int MeanMonstersSpeed { get; set; }
    public int MaxMonstersSpeed { get; set; }

    // Monster Initiative
    public int TotalMonstersInitiative { get; set; }
    public int MeanMonstersInitiative { get; set; }
    public int MaxMonstersInitiative { get; set; }

    // Monster Attributes
    public int TotalMonstersAttributes { get; set; }
    public int TotalMonstersStrength { get; set; }
    public int TotalMonstersDexterity { get; set; }
    public int TotalMonstersConstitution { get; set; }
    public int TotalMonstersIntelligence { get; set; }
    public int TotalMonstersWisdom { get; set; }
    public int TotalMonstersCharisma { get; set; }
    public int MeanMonstersAttributes { get; set; }
    public int MeanMonstersStrength { get; set; }
    public int MeanMonstersDexterity { get; set; }
    public int MeanMonstersConstitution { get; set; }
    public int MeanMonstersIntelligence { get; set; }
    public int MeanMonstersWisdom { get; set; }
    public int MeanMonstersCharisma { get; set; }
    public int MaxMonstersAttributes { get; set; }
    public int MaxMonstersStrength { get; set; }
    public int MaxMonstersDexterity { get; set; }
    public int MaxMonstersConstitution { get; set; }
    public int MaxMonstersIntelligence { get; set; }
    public int MaxMonstersWisdom { get; set; }
    public int MaxMonstersCharisma { get; set; }

    // Monster Skills    
    public Dictionary<string, int> TotalMonstersSkills { get; set; }
    public Dictionary<string, int> MeanMonstersSkills { get; set; }
    public Dictionary<string, int> MaxMonstersSkills { get; set; }

    // Monster Proficiencies
    public int TotalMonstersProficiencies { get; set; }
    public int MeanMonstersProficiencies { get; set; }
    public int MaxMonstersProficiencies { get; set; }

    // Monster Categorical Distributions
    public Dictionary<string, int> MonstersDistribution { get; set; }
    public Dictionary<string, int> MonstersProficienciesDistribution { get; set; }
    public Dictionary<string, int> MonstersSpecialAbilitiesDistribution { get; set; }
    public Dictionary<string, int> MonstersActionsDistribution { get; set; }
    public Dictionary<string, int> MonstersLegendaryActionsDistribution { get; set; }
    public Dictionary<string, int> MonstersReactionsDistribution { get; set; }
    public Dictionary<string, int> MonstersVulnerabilitiesDistribution { get; set; }
    public Dictionary<string, int> MonstersResistancesDistribution { get; set; }
    public Dictionary<string, int> MonstersImmunitiesDistribution { get; set; }
    public Dictionary<string, int> MonstersConditionImmunitiesDistribution { get; set; }

    // Monster Vulnerabilities
    public int TotalMonstersVulnerabilities { get; set; }
    public int MeanMonstersVulnerabilities { get; set; }
    public int MaxMonstersVulnerabilities { get; set; }

    // Monster Resistances
    public int TotalMonstersResistances { get; set; }
    public int MeanMonstersResistances { get; set; }
    public int MaxMonstersResistances { get; set; }

    // Monster Immunities
    public int TotalMonstersImmunities { get; set; }
    public int MeanMonstersImmunities { get; set; }
    public int MaxMonstersImmunities { get; set; }

    public EncounterXgBoost(Encounter encounter)
    {
        Id = encounter.Id;

        // Encounter features
        Difficulty = encounter.Difficulty;
        Outcome = encounter.Outcome.Outcome;
        HpRatio = encounter.CalculateHpRatio();
        AcRatio = encounter.CalculateAcRatio();
        SizeRatio = encounter.CalculateSizeRatio();

        // Party Size
        PartySize = encounter.PartyMembers.Count;

        // Party Level
        TotalPartyLevel = encounter.PartyMembers.Sum(pm => pm.Level);
        MeanPartyLevel = (int)encounter.PartyMembers.Average(pm => pm.Level);
        MaxPartyLevel = encounter.PartyMembers.Max(pm => pm.Level);

        // Party HP
        TotalPartyHP = encounter.PartyMembers.Sum(pm => pm.HitPoints);
        MeanPartyHp = (int)encounter.PartyMembers.Average(pm => pm.HitPoints);
        MaxPartyHp = encounter.PartyMembers.Max(pm => pm.HitPoints);

        // Party AC
        TotalPartyAc = encounter.PartyMembers.Sum(pm => pm.ArmorClass);
        MeanPartyAc = (int)encounter.PartyMembers.Average(pm => pm.ArmorClass);
        MaxPartyAc = encounter.PartyMembers.Max(pm => pm.ArmorClass);

        // Party Speed
        TotalPartySpeed = encounter.PartyMembers.Sum(pm => pm.Speed);
        MeanPartySpeed = (int)encounter.PartyMembers.Average(pm => pm.Speed);
        MaxPartySpeed = encounter.PartyMembers.Max(pm => pm.Speed);

        // Party Initiative
        TotalPartyInitiative = encounter.PartyMembers.Sum(pm => pm.Initiative);
        MeanPartyInitiative = (int)encounter.PartyMembers.Average(pm => pm.Initiative);
        MaxPartyInitiative = encounter.PartyMembers.Max(pm => pm.Initiative);

        // Party Attributes
        TotalPartyAttributes = encounter.PartyMembers.Sum(pm => pm.BaseStats);
        TotalPartyStrength = encounter.PartyMembers.Sum(pm => pm.Strength.Value);
        TotalPartyDexterity = encounter.PartyMembers.Sum(pm => pm.Dexterity.Value);
        TotalPartyConstitution = encounter.PartyMembers.Sum(pm => pm.Constitution.Value);
        TotalPartyIntelligence = encounter.PartyMembers.Sum(pm => pm.Intelligence.Value);
        TotalPartyWisdom = encounter.PartyMembers.Sum(pm => pm.Wisdom.Value);
        TotalPartyCharisma = encounter.PartyMembers.Sum(pm => pm.Charisma.Value);
        MeanPartyAttributes = (int)encounter.PartyMembers.Average(pm => pm.BaseStats);
        MeanPartyStrength = (int)encounter.PartyMembers.Average(pm => pm.Strength.Value);
        MeanPartyDexterity = (int)encounter.PartyMembers.Average(pm => pm.Dexterity.Value);
        MeanPartyConstitution = (int)encounter.PartyMembers.Average(pm => pm.Constitution.Value);
        MeanPartyIntelligence = (int)encounter.PartyMembers.Average(pm => pm.Intelligence.Value);
        MeanPartyWisdom = (int)encounter.PartyMembers.Average(pm => pm.Wisdom.Value);
        MeanPartyCharisma = (int)encounter.PartyMembers.Average(pm => pm.Charisma.Value);
        MaxPartyAttributes = encounter.PartyMembers.Max(pm => pm.BaseStats);
        MaxPartyStrength = encounter.PartyMembers.Max(pm => pm.Strength.Value);
        MaxPartyDexterity = encounter.PartyMembers.Max(pm => pm.Dexterity.Value);
        MaxPartyConstitution = encounter.PartyMembers.Max(pm => pm.Constitution.Value);
        MaxPartyIntelligence = encounter.PartyMembers.Max(pm => pm.Intelligence.Value);
        MaxPartyWisdom = encounter.PartyMembers.Max(pm => pm.Wisdom.Value);
        MaxPartyCharisma = encounter.PartyMembers.Max(pm => pm.Charisma.Value);

        // Party Skills
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

        // Party Proficiencies
        TotalPartyProficiencies = encounter.PartyMembers.Sum(pm => pm.Proficiencies.Count);
        MeanPartyProficiencies = (int)encounter.PartyMembers.Average(pm => pm.Proficiencies.Count);
        MaxPartyProficiencies = encounter.PartyMembers.Max(pm => pm.Proficiencies.Count);

        // Party Categorical Distributions
        PartyClassDistribution = encounter.PartyMembers
            .GroupBy(pm => pm.Class)
            .ToDictionary(g => g.Key, g => g.Count());
        PartySubclassDistribution = encounter.PartyMembers
            .GroupBy(pm => pm.Subclass)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyRaceDistribution = encounter.PartyMembers
            .GroupBy (pm => pm.Race)
            .ToDictionary(g => g.Key, g => g.Count());
        PartySubraceDistribution = encounter.PartyMembers
            .GroupBy(pm => pm.Subrace ?? string.Empty)
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .ToDictionary(g => g.Key, g => g.Count());
        PartyTraitsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Traits)
            .GroupBy(t => t)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyFeaturesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Features)
            .GroupBy(f => f.Name)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyClassSpecificsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.ClassSpecific?.Keys ?? Enumerable.Empty<string>())
            .GroupBy(cs => cs)
            .ToDictionary(g => g.Key, g => g.Count());
        PartySubclassSpecificsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.SubclassSpecific?.Keys ?? Enumerable.Empty<string>())
            .GroupBy(ss => ss)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyFeatureSpecificsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.FeatureSpecifics ?? Enumerable.Empty<string>())
            .GroupBy(fs => fs)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyProficienciesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Proficiencies ?? Enumerable.Empty<string>())
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyMeleeWeaponsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.MeleeWeapons.Select(w => w.Name) ?? Enumerable.Empty<string>())
            .GroupBy(mw => mw)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyRangedWeaponsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.RangedWeapons.Select(w => w.Name) ?? Enumerable.Empty<string>())
            .GroupBy(rw => rw)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyArmorsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Armors.Select(w => w.Name) ?? Enumerable.Empty<string>())
            .GroupBy(a => a)
            .ToDictionary(g => g.Key, g => g.Count());
        PartySpellsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Spells.Select(w => w.Name) ?? Enumerable.Empty<string>())
            .GroupBy(s => s)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyCantripsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Cantrips.Select(w => w.Name) ?? Enumerable.Empty<string>())
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());
        PartySpellSlotsDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.SpellSlots.GetSpellSlotCounts())
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));
        PartyVulnerabilitiesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Vulnerabilities)
            .GroupBy(v => v)
            .ToDictionary(g => g.Key, g => g.Count());
        PartyResistancesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Resistances)
            .GroupBy(r => r)
            .ToDictionary(g => g.Key, g => g.Count());        
        PartyImmunitiesDistribution = encounter.PartyMembers
            .SelectMany(pm => pm.Immunities)
            .GroupBy(i => i)
            .ToDictionary(g => g.Key, g => g.Count());

        // Party Vulnerabilities
        TotalPartyVulnerabilities = encounter.PartyMembers.Sum(pm => pm.Vulnerabilities.Count);
        MeanPartyVulnerabilities = (int)encounter.PartyMembers.Average(pm => pm.Vulnerabilities.Count);
        MaxPartyVulnerabilities = encounter.PartyMembers.Max(pm => pm.Vulnerabilities.Count);

        // Party Resistances
        TotalPartyResistances = encounter.PartyMembers.Sum(pm => pm.Resistances.Count);
        MeanPartyResistances = (int)encounter.PartyMembers.Average(pm => pm.Resistances.Count);
        MaxPartyResistances = encounter.PartyMembers.Max(pm => pm.Resistances.Count);

        // Party Immunities
        TotalPartyImmunities = encounter.PartyMembers.Sum(pm => pm.Immunities.Count);
        MeanPartyImmunities = (int)encounter.PartyMembers.Average(pm => pm.Immunities.Count);
        MaxPartyImmunities = encounter.PartyMembers.Max(pm => pm.Immunities.Count);

        // Party Spellcasting Bonus
        TotalPartySpellcastingBonus = encounter.PartyMembers.Sum(pm => UtilityMethods.GetSpellcastingModifier(pm));
        MeanPartySpellcastingBonus = (int)encounter.PartyMembers.Average(pm => UtilityMethods.GetSpellcastingModifier(pm));
        MaxPartySpellcastingBonus = encounter.PartyMembers.Max(pm => UtilityMethods.GetSpellcastingModifier(pm));

        // Monster Size
        MonsterSize = encounter.Monsters.Count;

        // Monster CR
        TotalMonstersCR = encounter.Monsters.Sum(m => m.ChallengeRating);
        MeanMonstersCR = encounter.Monsters.Average(m => m.ChallengeRating);
        MaxMonstersCR = encounter.Monsters.Max(m => m.ChallengeRating);

        // Monster HP
        TotalMonstersHP = encounter.Monsters.Sum(m => m.HitPoints);
        MeanMonstersHP = (int)encounter.Monsters.Average(m => m.HitPoints);
        MaxMonstersHP = encounter.Monsters.Max(m => m.HitPoints);

        // Monster AC
        TotalMonstersAC = encounter.Monsters.Sum(m => m.ArmorClass);
        MeanMonstersAC = (int)encounter.Monsters.Average(m => m.ArmorClass);
        MaxMonstersAC = encounter.Monsters.Max(m => m.ArmorClass);

        // Monster Speed
        TotalMonstersSpeed = encounter.Monsters.Sum(m => m.CalculateSpeedValue());
        MeanMonstersSpeed = (int)encounter.Monsters.Average(m => m.CalculateSpeedValue());
        MaxMonstersSpeed = encounter.Monsters.Max(m => m.CalculateSpeedValue());

        // Monster Initiative
        TotalMonstersInitiative = encounter.Monsters.Sum(m => m.Dexterity.Value);
        MeanMonstersInitiative = (int)encounter.Monsters.Average(m => m.Dexterity.Value);
        MaxMonstersInitiative = encounter.Monsters.Max(m => m.Dexterity.Value);

        // Monster Attributes
        TotalMonstersAttributes = encounter.Monsters.Sum(m => m.BaseStats);
        TotalMonstersStrength = encounter.Monsters.Sum(m => m.Strength.Value);
        TotalMonstersDexterity = encounter.Monsters.Sum(m => m.Dexterity.Value);
        TotalMonstersConstitution = encounter.Monsters.Sum(m => m.Constitution.Value);
        TotalMonstersIntelligence = encounter.Monsters.Sum(m => m.Intelligence.Value);
        TotalMonstersWisdom = encounter.Monsters.Sum(m => m.Wisdom.Value);
        TotalMonstersCharisma = encounter.Monsters.Sum(m => m.Charisma.Value);
        MeanMonstersAttributes = (int)encounter.Monsters.Average(m => m.BaseStats);
        MeanMonstersStrength = (int)encounter.Monsters.Average(m => m.Strength.Value);
        MeanMonstersDexterity = (int)encounter.Monsters.Average(m => m.Dexterity.Value);
        MeanMonstersConstitution = (int)encounter.Monsters.Average(m => m.Constitution.Value);
        MeanMonstersIntelligence = (int)encounter.Monsters.Average(m => m.Intelligence.Value);
        MeanMonstersWisdom = (int)encounter.Monsters.Average(m => m.Wisdom.Value);
        MeanMonstersCharisma = (int)encounter.Monsters.Average(m => m.Charisma.Value);
        MaxMonstersAttributes = encounter.Monsters.Max(m => m.BaseStats);
        MaxMonstersStrength = encounter.Monsters.Max(m => m.Strength.Value);
        MaxMonstersDexterity = encounter.Monsters.Max(m => m.Dexterity.Value);
        MaxMonstersConstitution = encounter.Monsters.Max(m => m.Constitution.Value);
        MaxMonstersIntelligence = encounter.Monsters.Max(m => m.Intelligence.Value);
        MaxMonstersWisdom = encounter.Monsters.Max(m => m.Wisdom.Value);
        MaxMonstersCharisma = encounter.Monsters.Max(m => m.Charisma.Value);

        // Monster Skills
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

        // Monster Proficiencies
        TotalMonstersProficiencies = encounter.Monsters.Sum(m => m.Proficiencies.Count);
        MeanMonstersProficiencies = (int)encounter.Monsters.Average(m => m.Proficiencies.Count);
        MaxMonstersProficiencies = encounter.Monsters.Max(m => m.Proficiencies.Count);

        // Monster Categorical Distributions
        MonstersDistribution = encounter.Monsters
            .GroupBy(m => m.Name)
            .ToDictionary(g => g.Key, g => g.Count());        
        MonstersProficienciesDistribution = encounter.Monsters
            .SelectMany(m => m.Proficiencies)
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());
        MonstersSpecialAbilitiesDistribution = encounter.Monsters
            .SelectMany(m => m.SpecialAbilities.Select(sa => sa.Name) ?? Enumerable.Empty<string>())
            .GroupBy(sa => sa)
            .ToDictionary(g => g.Key, g => g.Count());
        MonstersActionsDistribution = encounter.Monsters
            .SelectMany(m => m.Actions.Select(a => a.Name) ?? Enumerable.Empty<string>())
            .GroupBy(a => a)
            .ToDictionary(g => g.Key, g => g.Count());
        MonstersLegendaryActionsDistribution = encounter.Monsters
            .SelectMany(m => m.LegendaryActions.Select(la => la.Name) ?? Enumerable.Empty<string>())
            .GroupBy(la => la)
            .ToDictionary(g => g.Key, g => g.Count());
        MonstersReactionsDistribution = encounter.Monsters
            .SelectMany(m => m.Reactions.Select(r => r.Name) ?? Enumerable.Empty<string>())
            .GroupBy(r => r)
            .ToDictionary(g => g.Key, g => g.Count());
        MonstersVulnerabilitiesDistribution = encounter.Monsters
            .SelectMany(m => m.Vulnerabilities)
            .GroupBy(v => v)
            .ToDictionary(g => g.Key, g => g.Count());
        MonstersResistancesDistribution = encounter.Monsters
            .SelectMany(m => m.Resistances)
            .GroupBy(r => r)
            .ToDictionary(g => g.Key, g => g.Count());
        MonstersImmunitiesDistribution = encounter.Monsters
            .SelectMany(m => m.Immunities)
            .GroupBy(i => i)
            .ToDictionary(g => g.Key, g => g.Count());
        MonstersConditionImmunitiesDistribution = encounter.Monsters
            .SelectMany(m => m.ConditionImmunities.Select(c => c.Name))
            .GroupBy(ci => ci)
            .ToDictionary(g => g.Key, g => g.Count());

        // Monster Vulnerabilities
        TotalMonstersVulnerabilities = encounter.Monsters.Sum(m => m.Vulnerabilities.Count);
        MeanMonstersVulnerabilities = (int)encounter.Monsters.Average(m => m.Vulnerabilities.Count);
        MaxMonstersVulnerabilities = encounter.Monsters.Max(m => m.Vulnerabilities.Count);

        // Monster Resistances
        TotalMonstersResistances = encounter.Monsters.Sum(m => m.Resistances.Count);
        MeanMonstersResistances = (int)encounter.Monsters.Average(m => m.Resistances.Count);
        MaxMonstersResistances = encounter.Monsters.Max(m => m.Resistances.Count);

        // Monster Immunities
        TotalMonstersImmunities = encounter.Monsters.Sum(m => m.Immunities.Count);
        MeanMonstersImmunities = (int)encounter.Monsters.Average(m => m.Immunities.Count);
        MaxMonstersImmunities = encounter.Monsters.Max(m => m.Immunities.Count);
    }
}
