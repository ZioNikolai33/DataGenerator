using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Services;

public class SpellService : ISpellService
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;

    public SpellService(
        ILogger logger,
        IRandomProvider random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ApplySpellsFromFeatures(PartyMember member)
    {
        _logger.Verbose($"Applying spells from features for {member.Name}");

        var featureIndices = member.Features.Select(f => f.Index).ToList();

        // Magical Secrets (Bard)
        ApplyMagicalSecrets(member, featureIndices);

        // Pact of the Tome (Warlock)
        if (featureIndices.Contains("pact-of-the-tome"))
        {
            var cantripsToAdd = Lists.spells
                .Where(s => s.Level == 0)
                .OrderBy(_ => _random.Next())
                .Take(3)
                .Select(s => new Spell(s))
                .ToList();

            member.Cantrips.AddRange(cantripsToAdd);
            _logger.Verbose($"Added 3 cantrips from Pact of the Tome to {member.Name}");
        }

        // Mystic Arcanum (Warlock)
        ApplyMysticArcanum(member, featureIndices);

        // Bonus Cantrip (Druid - Circle of the Land)
        if (featureIndices.Contains("bonus-cantrip") && member.Class == "druid")
        {
            var druidCantrip = Lists.spells
                .Where(s => s.Level == 0 && s.Classes.Any(c => c.Index == "druid"))
                .OrderBy(_ => _random.Next())
                .FirstOrDefault();

            if (druidCantrip != null)
            {
                member.Cantrips.Add(new Spell(druidCantrip));
                _logger.Verbose($"Added bonus Druid cantrip to {member.Name}");
            }
        }

        // Class-specific spell lists
        if (member.Class == "druid")
            ApplyCircleSpells(member);

        if (member.Class == "paladin")
            ApplyOathSpells(member);

        if (member.Class == "cleric")
            ApplyDomainSpells(member);
    }

    public void ApplySpellsFromTraits(PartyMember member)
    {
        _logger.Verbose($"Applying spells from traits for {member.Name}");

        // High Elf Cantrip
        if (member.Traits.Contains("high-elf-cantrip"))
        {
            var wizardCantrip = Lists.spells
                .Where(s => s.Level == 0 && s.Classes.Any(c => c.Index == "wizard"))
                .OrderBy(_ => _random.Next())
                .FirstOrDefault();

            if (wizardCantrip != null)
            {
                member.Cantrips.Add(new Spell(wizardCantrip));
                _logger.Verbose($"Added High Elf cantrip '{wizardCantrip.Name}' to {member.Name}");
            }
        }

        // Infernal Legacy (Tiefling)
        if (member.Traits.Contains("infernal-legacy"))
        {
            var thaumaturgy = Lists.spells.FirstOrDefault(s => s.Index == "thaumaturgy");
            if (thaumaturgy != null)
            {
                member.Cantrips.Add(new Spell(thaumaturgy));
                _logger.Verbose($"Added Thaumaturgy cantrip from Infernal Legacy to {member.Name}");
            }

            if (member.Level >= 3)
            {
                var hellishRebuke = Lists.spells.FirstOrDefault(s => s.Index == "hellish-rebuke");
                if (hellishRebuke != null)
                {
                    member.Spells.Add(new Spell(hellishRebuke, "1 per Long Rest"));
                    _logger.Verbose($"Added Hellish Rebuke from Infernal Legacy to {member.Name}");
                }
            }

            if (member.Level >= 5)
            {
                var darkness = Lists.spells.FirstOrDefault(s => s.Index == "darkness");
                if (darkness != null)
                {
                    member.Spells.Add(new Spell(darkness, "1 per Long Rest"));
                    _logger.Verbose($"Added Darkness from Infernal Legacy to {member.Name}");
                }
            }
        }
    }

    public void ApplyCircleSpells(PartyMember member)
    {
        if (member.Class != "druid")
            return;

        _logger.Verbose($"Applying Circle spells for {member.Name} - Subclass: {member.Subclass}");

        var featureIndices = member.Features.Select(f => f.Index).ToList();

        // Circle Spells Level 1 (3rd level)
        if (featureIndices.Contains("circle-spells-1"))
            AddCircleSpellsByTier(member, member.Subclass, 1);

        // Circle Spells Level 2 (5th level)
        if (featureIndices.Contains("circle-spells-2"))
            AddCircleSpellsByTier(member, member.Subclass, 2);

        // Circle Spells Level 3 (7th level)
        if (featureIndices.Contains("circle-spells-3"))
            AddCircleSpellsByTier(member, member.Subclass, 3);

        // Circle Spells Level 4 (9th level)
        if (featureIndices.Contains("circle-spells-4"))
            AddCircleSpellsByTier(member, member.Subclass, 4);
    }

    public void ApplyOathSpells(PartyMember member)
    {
        if (member.Class != "paladin")
            return;

        _logger.Verbose($"Applying Oath spells for {member.Name} - Subclass: {member.Subclass}");

        var hasOathSpells = member.Features.Any(f => f.Index == "oath-spells");
        if (!hasOathSpells)
            return;

        if (member.Level >= 3)
            AddOathSpellsByLevel(member, member.Subclass, 3);

        if (member.Level >= 5)
            AddOathSpellsByLevel(member, member.Subclass, 5);

        if (member.Level >= 9)
            AddOathSpellsByLevel(member, member.Subclass, 9);

        if (member.Level >= 13)
            AddOathSpellsByLevel(member, member.Subclass, 13);

        if (member.Level >= 17)
            AddOathSpellsByLevel(member, member.Subclass, 17);
    }

    public void ApplyDomainSpells(PartyMember member)
    {
        if (member.Class != "cleric")
            return;

        _logger.Verbose($"Applying Domain spells for {member.Name} - Subclass: {member.Subclass}");

        var featureIndices = member.Features.Select(f => f.Index).ToList();

        if (featureIndices.Contains("domain-spells-1"))
            AddDomainSpellsByTier(member, member.Subclass, 1);

        if (featureIndices.Contains("domain-spells-2"))
            AddDomainSpellsByTier(member, member.Subclass, 2);

        if (featureIndices.Contains("domain-spells-3"))
            AddDomainSpellsByTier(member, member.Subclass, 3);

        if (featureIndices.Contains("domain-spells-4"))
            AddDomainSpellsByTier(member, member.Subclass, 4);

        if (featureIndices.Contains("domain-spells-5"))
            AddDomainSpellsByTier(member, member.Subclass, 5);
    }

    #region Private Helper Methods

    private void ApplyMagicalSecrets(PartyMember member, List<string> featureIndices)
    {
        // Additional Magical Secrets (Lore Bard - Level 6)
        if (featureIndices.Contains("additional-magical-secrets"))
        {
            AddMagicalSecretsSpells(member, maxLevel: 3, count: 2, "Additional Magical Secrets");
        }

        // Magical Secrets Level 1 (Level 10)
        if (featureIndices.Contains("magical-secrets-1"))
        {
            AddMagicalSecretsSpells(member, maxLevel: 5, count: 2, "Magical Secrets 1");
        }

        // Magical Secrets Level 2 (Level 14)
        if (featureIndices.Contains("magical-secrets-2"))
        {
            AddMagicalSecretsSpells(member, maxLevel: 7, count: 2, "Magical Secrets 2");
        }

        // Magical Secrets Level 3 (Level 18)
        if (featureIndices.Contains("magical-secrets-3"))
        {
            AddMagicalSecretsSpells(member, maxLevel: 9, count: 2, "Magical Secrets 3");
        }
    }

    private void AddMagicalSecretsSpells(PartyMember member, int maxLevel, int count, string featureName)
    {
        var spellsToAdd = Lists.spells
            .Where(s => s.Level <= maxLevel)
            .OrderBy(_ => _random.Next())
            .Take(count)
            .ToList();

        foreach (var spellMapper in spellsToAdd)
        {
            var spell = new Spell(spellMapper);
            
            if (spell.Level == 0)
                member.Cantrips.Add(spell);
            else
                member.Spells.Add(spell);
        }

        _logger.Verbose($"Added {count} spells from {featureName} to {member.Name}");
    }

    private void ApplyMysticArcanum(PartyMember member, List<string> featureIndices)
    {
        var arcanumLevels = new Dictionary<string, int>
        {
            { "mystic-arcanum-6th-level", 6 },
            { "mystic-arcanum-7th-level", 7 },
            { "mystic-arcanum-8th-level", 8 },
            { "mystic-arcanum-9th-level", 9 }
        };

        foreach (var (featureIndex, spellLevel) in arcanumLevels)
        {
            if (featureIndices.Contains(featureIndex))
            {
                var arcanumSpell = Lists.spells
                    .Where(s => s.Level == spellLevel)
                    .OrderBy(_ => _random.Next())
                    .FirstOrDefault();

                if (arcanumSpell != null)
                {
                    member.Spells.Add(new Spell(arcanumSpell, "1 per Long Rest"));
                    _logger.Verbose($"Added Mystic Arcanum level {spellLevel} spell '{arcanumSpell.Name}' to {member.Name}");
                }
            }
        }
    }

    private void AddCircleSpellsByTier(PartyMember member, string subclass, int tier)
    {
        var spellPairs = GetCircleSpellsForTier(subclass, tier);

        foreach (var spellIndex in spellPairs)
        {
            var spell = Lists.spells.FirstOrDefault(s => s.Index == spellIndex);
            if (spell != null)
            {
                member.Spells.Add(new Spell(spell));
            }
        }

        _logger.Verbose($"Added {spellPairs.Count} Circle spells (Tier {tier}) to {member.Name}");
    }

    private List<string> GetCircleSpellsForTier(string subclass, int tier)
    {
        var circleSpells = new Dictionary<string, Dictionary<int, List<string>>>
        {
            ["arctic"] = new()
            {
                [1] = new() { "hold-person", "spike-growth" },
                [2] = new() { "sleet-storm", "slow" },
                [3] = new() { "freedom-of-movement", "ice-storm" },
                [4] = new() { "commune-with-nature", "cone-of-cold" }
            },
            ["coast"] = new()
            {
                [1] = new() { "mirror-image", "misty-step" },
                [2] = new() { "water-breathing", "water-walk" },
                [3] = new() { "control-water", "freedom-of-movement" },
                [4] = new() { "conjure-elemental", "scrying" }
            },
            ["desert"] = new()
            {
                [1] = new() { "blur", "silence" },
                [2] = new() { "create-food-and-water", "protection-from-energy" },
                [3] = new() { "blight", "hallucinatory-terrain" },
                [4] = new() { "insect-plague", "wall-of-stone" }
            },
            ["forest"] = new()
            {
                [1] = new() { "barkskin", "spider-climb" },
                [2] = new() { "call-lightning", "plant-growth" },
                [3] = new() { "divination", "freedom-of-movement" },
                [4] = new() { "commune-with-nature", "tree-stride" }
            },
            ["grassland"] = new()
            {
                [1] = new() { "invisibility", "pass-without-trace" },
                [2] = new() { "daylight", "haste" },
                [3] = new() { "divination", "freedom-of-movement" },
                [4] = new() { "dream", "insect-plague" }
            },
            ["mountain"] = new()
            {
                [1] = new() { "spider-climb", "spike-growth" },
                [2] = new() { "lightning-bolt", "meld-into-stone" },
                [3] = new() { "stone-shape", "stoneskin" },
                [4] = new() { "passwall", "wall-of-stone" }
            },
            ["swamp"] = new()
            {
                [1] = new() { "darkness", "melfs-acid-arrow" },
                [2] = new() { "water-walk", "stinking-cloud" },
                [3] = new() { "freedom-of-movement", "locate-creature" },
                [4] = new() { "insect-plague", "scrying" }
            },
            ["underdark"] = new()
            {
                [1] = new() { "spider-climb", "web" },
                [2] = new() { "gaseous-form", "stinking-cloud" },
                [3] = new() { "greater-invisibility", "stone-shape" },
                [4] = new() { "cloudkill", "insect-plague" }
            }
        };

        return circleSpells.GetValueOrDefault(subclass)?.GetValueOrDefault(tier) ?? new List<string>();
    }

    private void AddOathSpellsByLevel(PartyMember member, string subclass, int level)
    {
        var spellIndices = GetOathSpellsForLevel(subclass, level);

        foreach (var spellIndex in spellIndices)
        {
            var spell = Lists.spells.FirstOrDefault(s => s.Index == spellIndex);
            if (spell != null)
            {
                member.Spells.Add(new Spell(spell));
            }
        }

        _logger.Verbose($"Added {spellIndices.Count} Oath spells (Level {level}) to {member.Name}");
    }

    private List<string> GetOathSpellsForLevel(string subclass, int level)
    {
        var oathSpells = new Dictionary<string, Dictionary<int, List<string>>>
        {
            ["devotion"] = new()
            {
                [3] = new() { "protection-from-evil-and-good", "sanctuary" },
                [5] = new() { "lesser-restoration", "zone-of-truth" },
                [9] = new() { "beacon-of-hope", "dispel-magic" },
                [13] = new() { "freedom-of-movement", "guardian-of-faith" },
                [17] = new() { "commune", "flame-strike" }
            },
            ["ancients"] = new()
            {
                [3] = new() { "ensnaring-strike", "speak-with-animals" },
                [5] = new() { "moonbeam", "misty-step" },
                [9] = new() { "plant-growth", "protection-from-energy" },
                [13] = new() { "ice-storm", "stoneskin" },
                [17] = new() { "commune-with-nature", "tree-stride" }
            },
            ["vengeance"] = new()
            {
                [3] = new() { "bane", "hunters-mark" },
                [5] = new() { "hold-person", "misty-step" },
                [9] = new() { "haste", "protection-from-energy" },
                [13] = new() { "banishment", "dimension-door" },
                [17] = new() { "hold-monster", "scrying" }
            }
        };

        return oathSpells.GetValueOrDefault(subclass)?.GetValueOrDefault(level) ?? new List<string>();
    }

    private void AddDomainSpellsByTier(PartyMember member, string subclass, int tier)
    {
        var spellIndices = GetDomainSpellsForTier(subclass, tier);

        foreach (var spellIndex in spellIndices)
        {
            var spell = Lists.spells.FirstOrDefault(s => s.Index == spellIndex);
            if (spell != null)
            {
                member.Spells.Add(new Spell(spell));
            }
        }

        _logger.Verbose($"Added {spellIndices.Count} Domain spells (Tier {tier}) to {member.Name}");
    }

    private List<string> GetDomainSpellsForTier(string subclass, int tier)
    {
        var domainSpells = new Dictionary<string, Dictionary<int, List<string>>>
        {
            ["knowledge"] = new()
            {
                [1] = new() { "command", "identify" },
                [2] = new() { "augury", "suggestion" },
                [3] = new() { "nondetection", "speak-with-dead" },
                [4] = new() { "arcane-eye", "confusion" },
                [5] = new() { "legend-lore", "scrying" }
            },
            ["life"] = new()
            {
                [1] = new() { "bless", "cure-wounds" },
                [2] = new() { "lesser-restoration", "spiritual-weapon" },
                [3] = new() { "beacon-of-hope", "revivify" },
                [4] = new() { "death-ward", "guardian-of-faith" },
                [5] = new() { "mass-cure-wounds", "raise-dead" }
            },
            ["light"] = new()
            {
                [1] = new() { "burning-hands", "faerie-fire" },
                [2] = new() { "flaming-sphere", "scorching-ray" },
                [3] = new() { "daylight", "fireball" },
                [4] = new() { "guardian-of-faith", "wall-of-fire" },
                [5] = new() { "flame-strike", "scrying" }
            },
            ["nature"] = new()
            {
                [1] = new() { "animal-friendship", "entangle" },
                [2] = new() { "barkskin", "spike-growth" },
                [3] = new() { "plant-growth", "wind-wall" },
                [4] = new() { "dominate-beast", "grasping-vine" },
                [5] = new() { "insect-plague", "tree-stride" }
            },
            ["tempest"] = new()
            {
                [1] = new() { "fog-cloud", "thunderwave" },
                [2] = new() { "gust-of-wind", "shatter" },
                [3] = new() { "call-lightning", "sleet-storm" },
                [4] = new() { "control-water", "ice-storm" },
                [5] = new() { "destructive-wave", "insect-plague" }
            },
            ["trickery"] = new()
            {
                [1] = new() { "charm-person", "disguise-self" },
                [2] = new() { "mirror-image", "pass-without-trace" },
                [3] = new() { "blink", "dispel-magic" },
                [4] = new() { "dimension-door", "polymorph" },
                [5] = new() { "dominate-person", "modify-memory" }
            },
            ["war"] = new()
            {
                [1] = new() { "divine-favor", "shield-of-faith" },
                [2] = new() { "magic-weapon", "spiritual-weapon" },
                [3] = new() { "crusaders-mantle", "spirit-guardians" },
                [4] = new() { "freedom-of-movement", "stoneskin" },
                [5] = new() { "flame-strike", "hold-monster" }
            }
        };

        return domainSpells.GetValueOrDefault(subclass)?.GetValueOrDefault(tier) ?? new List<string>();
    }

    #endregion
}