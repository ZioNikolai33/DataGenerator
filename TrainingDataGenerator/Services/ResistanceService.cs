using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Services;

public class ResistanceService : IResistanceService
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;

    // Mapping of draconic ancestry to damage types
    private static readonly Dictionary<string, string> DraconicAncestryResistances = new()
    {
        ["draconic-ancestry-black"] = "acid",
        ["draconic-ancestry-blue"] = "lightning",
        ["draconic-ancestry-brass"] = "fire",
        ["draconic-ancestry-bronze"] = "lightning",
        ["draconic-ancestry-copper"] = "acid",
        ["draconic-ancestry-gold"] = "fire",
        ["draconic-ancestry-green"] = "poison",
        ["draconic-ancestry-red"] = "fire",
        ["draconic-ancestry-silver"] = "cold",
        ["draconic-ancestry-white"] = "cold"
    };

    // Available damage types for Fiendish Resilience
    private static readonly List<string> FiendishResilienceDamageTypes = new()
    {
        "bludgeoning", "slashing", "piercing", "acid", "cold", "fire", 
        "force", "lightning", "necrotic", "poison", "psychic", "radiant", "thunder"
    };

    public ResistanceService(
        ILogger logger,
        IRandomProvider random)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public void ApplyDamageResistances(PartyMember member)
    {
        _logger.Verbose($"Applying damage resistances for {member.Name}");

        var resistanceCount = 0;
        var immunityCount = 0;

        // === Racial Resistances ===
        resistanceCount += ApplyRacialResistances(member);

        // === Trait-based Resistances ===
        resistanceCount += ApplyTraitResistances(member);

        // === Class/Feature-based Immunities ===
        immunityCount += ApplyClassImmunities(member);

        // === Feature-based Resistances ===
        resistanceCount += ApplyFeatureResistances(member);

        _logger.Verbose($"Applied {resistanceCount} resistances and {immunityCount} immunities to {member.Name}");
    }

    public void AddResistance(PartyMember member, string damageType)
    {
        if (!member.Resistances.Contains(damageType))
        {
            member.Resistances.Add(damageType);
            _logger.Verbose($"Added {damageType} resistance to {member.Name}");
        }
    }

    public void AddImmunity(PartyMember member, string immunityType)
    {
        if (!member.Immunities.Contains(immunityType))
        {
            member.Immunities.Add(immunityType);
            _logger.Verbose($"Added {immunityType} immunity to {member.Name}");
        }
    }

    public void AddVulnerability(PartyMember member, string damageType)
    {
        if (!member.Vulnerabilities.Contains(damageType))
        {
            member.Vulnerabilities.Add(damageType);
            _logger.Verbose($"Added {damageType} vulnerability to {member.Name}");
        }
    }

    public bool HasResistance(PartyMember member, string damageType)
    {
        return member.Resistances.Contains(damageType);
    }

    public bool HasImmunity(PartyMember member, string immunityType)
    {
        return member.Immunities.Contains(immunityType);
    }

    #region Private Helper Methods

    private int ApplyRacialResistances(PartyMember member)
    {
        var count = 0;

        // Dragonborn - Draconic Ancestry Resistance
        if (member.Race.Equals("dragonborn", StringComparison.OrdinalIgnoreCase))
        {
            var draconicAncestry = member.Traits
                .FirstOrDefault(t => t.StartsWith("draconic-ancestry-", StringComparison.OrdinalIgnoreCase));

            if (draconicAncestry != null && DraconicAncestryResistances.TryGetValue(draconicAncestry, out var damageType))
            {
                AddResistance(member, damageType);
                count++;
                _logger.Verbose($"Applied Draconic Ancestry resistance: {damageType}");
            }
        }

        return count;
    }

    private int ApplyTraitResistances(PartyMember member)
    {
        var count = 0;

        // Dwarven Resilience - Poison Resistance
        if (member.Traits.Contains("dwarven-resilience"))
        {
            AddResistance(member, "poison");
            count++;
            _logger.Verbose("Applied Dwarven Resilience: poison resistance");
        }

        // Hellish Resistance (Tiefling) - Fire Resistance
        if (member.Traits.Contains("hellish-resistance"))
        {
            AddResistance(member, "fire");
            count++;
            _logger.Verbose("Applied Hellish Resistance: fire resistance");
        }

        return count;
    }

    private int ApplyClassImmunities(PartyMember member)
    {
        var count = 0;
        var featureIndices = member.Features.Select(f => f.Index).ToList();

        // Druid - Nature's Ward (Level 10)
        if (member.Class == "druid" && featureIndices.Contains("natures-ward"))
        {
            AddImmunity(member, "poison");
            AddImmunity(member, "disease");
            count += 2;
            _logger.Verbose("Applied Nature's Ward: poison and disease immunity");
        }

        // Monk - Purity of Body (Level 10)
        if (member.Class == "monk" && featureIndices.Contains("purity-of-body"))
        {
            AddImmunity(member, "poison");
            AddImmunity(member, "disease");
            count += 2;
            _logger.Verbose("Applied Purity of Body: poison and disease immunity");
        }

        // Paladin - Divine Health (Level 3)
        if (member.Class == "paladin" && featureIndices.Contains("divine-health"))
        {
            AddImmunity(member, "disease");
            count++;
            _logger.Verbose("Applied Divine Health: disease immunity");
        }

        return count;
    }

    private int ApplyFeatureResistances(PartyMember member)
    {
        var count = 0;
        var featureIndices = member.Features.Select(f => f.Index).ToList();

        // Warlock (The Fiend) - Fiendish Resilience (Level 10)
        if (member.Class == "warlock" && 
            member.Subclass == "the-fiend" && 
            featureIndices.Contains("fiendish-resilience"))
        {
            var randomDamageType = _random.SelectRandom(FiendishResilienceDamageTypes);
            AddResistance(member, randomDamageType);
            count++;
            _logger.Verbose($"Applied Fiendish Resilience: {randomDamageType} resistance");
        }

        // Bear Totem Warrior (Barbarian) - Resistance to all damage except psychic
        if (member.Class == "barbarian" && 
            member.Subclass == "totem-warrior" && 
            featureIndices.Contains("totem-spirit-bear"))
        {
            var damageTypes = new List<string> 
            { 
                "bludgeoning", "slashing", "piercing", "acid", "cold", "fire", 
                "force", "lightning", "necrotic", "poison", "radiant", "thunder" 
            };
            
            foreach (var damageType in damageTypes)
            {
                AddResistance(member, damageType);
                count++;
            }
            
            _logger.Verbose("Applied Bear Totem: resistance to all damage except psychic (while raging)");
        }

        return count;
    }

    #endregion
}