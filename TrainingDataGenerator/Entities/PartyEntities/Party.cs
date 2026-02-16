using TrainingDataGenerator.Abstracts;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Equip;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Entities.PartyEntities;

public class PartyMember : Creature, ICombatCalculator
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;

    public byte Level { get; set; }
    public byte HitDie { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public short Speed { get; set; }
    public string? Subrace { get; set; }        
    public string Subclass { get; set; } = string.Empty;
    public sbyte Initiative { get; set; }
    public List<string> Traits { get; set; } = new List<string>();
    public List<Feature> Features { get; set; } = new List<Feature>();
    public List<MeleeWeapon> MeleeWeapons { get; set; } = new List<MeleeWeapon>();
    public List<RangedWeapon> RangedWeapons { get; set; } = new List<RangedWeapon>();
    public List<Armor> Armors { get; set; } = new List<Armor>();
    public List<Ammunition> Ammunitions { get; set; } = new List<Ammunition>();    
    public Dictionary<string, object>? ClassSpecific { get; set; }
    public Dictionary<string, object>? SubclassSpecific { get; set; }
    public List<string> FeatureSpecifics { get; set; } = new List<string>();
    public string SpellcastingAbility { get; set; } = string.Empty;
    public Slots SpellSlots { get; set; } = new Slots();
    public List<Spell> Cantrips { get; set; } = new List<Spell>();
    public List<Spell> Spells { get; set; } = new List<Spell>();

    public PartyMember(
        int id,
        byte level,
        RaceMapper randomRace,
        ClassMapper randomClass,
        ILogger logger,
        IRandomProvider random,
        IAttributeService attributeService,
        IEquipmentService equipmentService,
        ISpellService spellService,
        IFeatureService featureService,
        IProficiencyService proficiencyService,
        ITraitService traitService,
        IResistanceService resistanceService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));

        _logger.Verbose($"Generating Member {id} at Level {level}. Class {randomClass.Index} and Race {randomRace.Index}");

        // Select random subrace and subclass
        var randomRaceAbilityBonus = randomRace.GetRandomAbility(_random);
        var randomSubrace = SelectRandomSubrace(randomRace);
        var randomSubclass = SelectRandomSubclass(randomClass);

        // Get levels and features for this character
        var levels = GetLevelsForCharacter(level, randomClass.Index, randomSubclass.Index);
        var features = GetFeaturesForCharacter(level, randomClass.Index, randomSubclass.Index);

        // Initialize base information
        InitializeBaseInfo(id, level, randomRace, randomClass, randomSubrace, randomSubclass);

        // Services to set up character
        attributeService.SetAttributes(this, randomClass, randomRaceAbilityBonus, features); // Set attributes with racial bonuses and level improvements
        equipmentService.ManageEquipments(this, randomClass); // Manage equipment
        traitService.ManageTraitSpecifics(this, randomRace, randomSubrace); // Manage racial traits and subtraits
        proficiencyService.SetInitialProficiencies(this, randomClass, randomRace, randomSubrace); // Set proficiencies
        
        LevelAdvancements(levels, features, spellService); // Process level advancements (features, spells, class specifics)
        CreateSkills(); // Create skills with basic modifiers
        
        attributeService.AddSavingThrowProficiencies(this, randomClass); // Add proficiency to saving throws
        featureService.ApplyFeatureEffects(this); // Apply feature effects
        proficiencyService.AddBackgroundProficiencies(this); // Add background proficiencies
        proficiencyService.AddAdditionalProficiencies(this); // Add additional proficiencies from traits & features
        proficiencyService.ApplySkillProficiencies(this); // Apply skill proficiencies
        resistanceService.ApplyDamageResistances(this); // Apply damage resistances, immunities, and vulnerabilities
        featureService.ApplyFeatureSpecifics(this); // Manage feature specifics (expertise, favored enemy/terrain, subfeatures)

        // Final calculations
        ArmorClass = CalculateArmorClass();
        HitPoints = CalculateRandomHp(randomClass);
        AddAdditionalHpFromTraits();
        Initiative = Dexterity?.Modifier ?? 0;

        // Deduplicate spells and cantrips
        Cantrips = Cantrips.GroupBy(c => c.Index).Select(g => g.First()).ToList();
        Spells = Spells.GroupBy(s => s.Index).Select(g => g.First()).ToList();

        _logger.Verbose($"Successfully generated {Name} - {Race} {Class} Level {Level}");
    }

    #region Initialization Helper Methods

    private void InitializeBaseInfo(int id, byte level, RaceMapper randomRace, ClassMapper randomClass, SubraceMapper? randomSubrace, SubclassMapper randomSubclass)
    {
        Index = id.ToString();
        Name = $"Member {id}";
        Level = level;
        HitDie = (byte)randomClass.Hp;
        ProficiencyBonus = (byte)(2 + (Level - 1) / 4);
        Race = randomRace.Index;
        Speed = randomRace.Speed;
        Size = randomRace.Size.ToString();
        Subrace = randomSubrace?.Index;
        Class = randomClass.Index;
        Subclass = randomSubclass.Index;
        SpellcastingAbility = randomClass.SpellcastingAbility != null
            ? UtilityMethods.ConvertAbilityIndexToFullName(randomClass.SpellcastingAbility.SpellcastingAbility.Index)
            : string.Empty;

        _logger.Verbose($"Initialized base info: {Name} - {Race} {Class} Level {Level}");
    }

    private SubraceMapper? SelectRandomSubrace(RaceMapper race)
    {
        if (race.Subraces.Count == 0)
            return new SubraceMapper(string.Empty, string.Empty);

        var randomSubraceEntity = _random.SelectRandom(race.Subraces);
        var subrace = EntitiesFinder.GetEntityByIndex(
            Lists.subraces.ToList(),
            new BaseEntity(race.Index, race.Name),
            randomSubraceEntity);

        if (subrace != null)
            _logger.Verbose($"Selected subrace: {subrace.Name}");

        return subrace;
    }

    private SubclassMapper SelectRandomSubclass(ClassMapper classMapper)
    {
        var randomSubclassEntity = _random.SelectRandom(classMapper.Subclasses);
        var subclass = EntitiesFinder.GetEntityByIndex(
            Lists.subclasses.ToList(),
            new BaseEntity(classMapper.Index, classMapper.Name),
            randomSubclassEntity);

        _logger.Verbose($"Selected subclass: {subclass.Name}");
        return subclass;
    }

    private List<LevelMapper> GetLevelsForCharacter(byte level, string classIndex, string subclassIndex)
    {
        var levels = Lists.levels
            .Where(l => l.Level <= level &&
                       l.Class.Index == classIndex &&
                       (l.Subclass == null || l.Subclass.Index == subclassIndex))
            .ToList();

        _logger.Verbose($"Retrieved {levels.Count} level entries for character");
        return levels;
    }

    private List<FeatureMapper> GetFeaturesForCharacter(byte level, string classIndex, string subclassIndex)
    {
        var features = Lists.features
            .Where(f => f.Level <= level &&
                       f.Parent == null &&
                       f.Class.Index == classIndex &&
                       (f.Subclass == null || f.Subclass.Index == subclassIndex))
            .ToList();

        _logger.Verbose($"Retrieved {features.Count} features for character");
        return features;
    }

    #endregion

    #region Level Advancement

    private void LevelAdvancements(List<LevelMapper> levels, List<FeatureMapper> features, ISpellService spellService)
    {
        _logger.Verbose($"Processing level advancements up to level {Level}");

        for (int i = 1; i <= Level; i++)
        {
            var currentLevels = levels
                .Where(l => l.Class.Index == Class && 
                           l.Level == i && 
                           (l.Subclass == null || l.Subclass.Index == Subclass))
                .ToList();

            var levelFeatures = features
                .Where(f => f.Level == i && 
                           (f.Subclass == null || f.Subclass.Index == Subclass))
                .ToList();

            // Add features for this level
            if (levelFeatures.Count > 0)
            {
                Features.AddRange(levelFeatures.Select(f => new Feature(f, Proficiencies, _random)));
                _logger.Verbose($"Added {levelFeatures.Count} features at level {i}");
            }

            // Process final level specifics
            if (i == Level)
            {
                ProcessFinalLevel(currentLevels, features, spellService);
            }
        }
    }

    private void ProcessFinalLevel(List<LevelMapper> currentLevels, List<FeatureMapper> features, ISpellService spellService)
    {
        _logger.Verbose($"Processing final level specifics for level {Level}");

        // Warlock Invocations
        if (Class.Equals("warlock") && Level > 1)
        {
            ApplyWarlockInvocations(currentLevels, features);
        }

        // Apply spells from traits and features
        spellService.ApplySpellsFromTraits(this);
        spellService.ApplySpellsFromFeatures(this);

        // Handle spellcasting
        if (currentLevels.Any(l => l.Spellcasting != null))
        {
            ProcessSpellcasting(currentLevels);
        }

        // Set class and subclass specific data
        if (currentLevels.Any(l => l.ClassSpecific != null))
        {
            ClassSpecific = currentLevels.Last(l => l.ClassSpecific != null).ClassSpecific;
        }

        if (currentLevels.Any(l => l.SubclassSpecific != null))
        {
            SubclassSpecific = currentLevels.Last(l => l.SubclassSpecific != null).SubclassSpecific;
        }

        // Check feature prerequisites
        CheckFeaturePrerequisites(features);
    }

    private void ApplyWarlockInvocations(List<LevelMapper> currentLevels, List<FeatureMapper> features)
    {
        var numInvocations = currentLevels
            .Where(l => l.ClassSpecific != null && l.ClassSpecific.ContainsKey("invocations_known"))
            .Select(l => Convert.ToInt32(l.ClassSpecific?["invocations_known"]))
            .FirstOrDefault();

        if (numInvocations == 0)
            return;

        var availableInvocations = features
            .Where(f => f.Index.Equals("eldritch-invocations"))
            .SelectMany(f => f.FeatureSpec?.Invocations ?? new List<BaseEntity>())
            .ToList();

        var featureInvocations = Lists.features
            .Where(f => availableInvocations.Select(a => a.Index).Contains(f.Index))
            .ToList();

        var selectedInvocations = _random.SelectRandom(featureInvocations, numInvocations)
            .Select(f => new Feature(f, Proficiencies, _random))
            .ToList();

        Features.AddRange(selectedInvocations);
        _logger.Verbose($"Added {selectedInvocations.Count} Warlock invocations");
    }

    private void ProcessSpellcasting(List<LevelMapper> currentLevels)
    {
        var spellcasting = currentLevels.First(l => l.Spellcasting != null).Spellcasting;
        if (spellcasting == null)
            return;

        SpellSlots = new Slots(spellcasting);

        // Calculate spells known based on class
        spellcasting.SpellsKnown = Class switch
        {
            "cleric" => (byte?)(Wisdom.Modifier + Level),
            "druid" => (byte?)(Wisdom.Modifier + Level),
            "paladin" => (byte?)(Charisma.Modifier + Math.Floor((double)Level / 2)),
            "ranger" => (byte?)(Wisdom.Modifier + Math.Floor((double)Level / 2)),
            "wizard" => (byte?)(Intelligence.Modifier + Level),
            _ => spellcasting.SpellsKnown
        };

        _logger.Verbose($"Spells known: {spellcasting.SpellsKnown}, Cantrips known: {spellcasting.CantripsKnown}");

        // Add random spells
        AddRandomSpells(spellcasting);
        AddRandomCantrips(spellcasting);
    }

    private void AddRandomSpells(LevelMapper.SpellcastingInfo spellcasting)
    {
        var availableSpells = Lists.spells
            .Where(s => SpellSlots.GetSlotsLevelAvailable() >= s.Level && 
                       (s.Classes.Any(c => c.Index == Class) || 
                        s.Subclasses != null && s.Classes.Any(c => c.Index == Class) && 
                         s.Subclasses.Any(sc => sc.Index == Subclass)) &&
                       !Spells.Any(existing => existing.Index == s.Index))
            .ToList();

        var spellsToAdd = Math.Min(availableSpells.Count, spellcasting.SpellsKnown ?? 0);
        if (spellsToAdd == 0)
            return;

        var selectedSpells = _random.SelectRandom(availableSpells, spellsToAdd)
            .Select(s => new Spell(s))
            .ToList();

        Spells.AddRange(selectedSpells);
        _logger.Verbose($"Added {selectedSpells.Count} random spells");
    }

    private void AddRandomCantrips(LevelMapper.SpellcastingInfo spellcasting)
    {
        var availableCantrips = Lists.spells
            .Where(s => s.Level == 0 && 
                       (s.Classes.Any(c => c.Index == Class) || 
                        s.Subclasses != null && s.Classes.Any(c => c.Index == Class) && 
                         s.Subclasses.Any(sc => sc.Index == Subclass)) &&
                       !Cantrips.Any(existing => existing.Index == s.Index))
            .ToList();

        var cantripsToAdd = Math.Min(availableCantrips.Count, spellcasting.CantripsKnown ?? 0);
        if (cantripsToAdd == 0)
            return;

        var selectedCantrips = _random.SelectRandom(availableCantrips, cantripsToAdd)
            .Select(s => new Spell(s))
            .ToList();

        Cantrips.AddRange(selectedCantrips);
        _logger.Verbose($"Added {selectedCantrips.Count} random cantrips");
    }

    private void CheckFeaturePrerequisites(List<FeatureMapper> allFeatures)
    {
        var featuresToRemove = new List<string>();

        foreach (var feature in allFeatures)
        {
            if (feature.Prerequisites == null || feature.Prerequisites.Count == 0)
                continue;

            foreach (var prereq in feature.Prerequisites)
            {
                bool meetsPrereq = false;

                if (prereq.Type == "feature")
                {
                    var requiredFeature = prereq.Feature?.Split('/').Last();
                    if (Features.Any(f => f.Index == requiredFeature))
                        meetsPrereq = true;
                }
                else if (prereq.Type == "spell")
                {
                    var requiredSpell = prereq.Feature?.Split('/').Last();
                    if (Spells.Any(s => s.Index == requiredSpell) || 
                        Cantrips.Any(c => c.Index == requiredSpell))
                        meetsPrereq = true;
                }

                if (!meetsPrereq)
                {
                    featuresToRemove.Add(feature.Index);
                    _logger.Warning($"Feature '{feature.Index}' does not meet prerequisites");
                }
            }
        }

        if (featuresToRemove.Count > 0)
        {
            Features = Features.Where(f => !featuresToRemove.Contains(f.Index)).ToList();
            _logger.Verbose($"Removed {featuresToRemove.Count} features due to unmet prerequisites");
        }
    }

    #endregion

    #region Calculations

    public byte CalculateArmorClass()
    {
        var ac = 10 + Dexterity.Modifier;
        var equippedArmor = Armors.FirstOrDefault(a => a.IsEquipped && a.Index != "shield");

        if (equippedArmor != null)
        {
            ac = equippedArmor.ArmorClass.Base;

            if (equippedArmor.ArmorClass.HasDexBonus)
            {
                if (equippedArmor.ArmorClass.MaxDexBonus != null)
                    ac += Math.Min(Dexterity.Modifier, equippedArmor.ArmorClass.MaxDexBonus.Value);
                else
                    ac += Dexterity.Modifier;
            }
        }

        // Barbarian Unarmored Defense
        if (Features.Any(f => f.Index == "barbarian-unarmored-defense") && 
            !Armors.Any(a => a.Index != "shield" && a.IsEquipped))
        {
            ac = 10 + Dexterity.Modifier + Constitution.Modifier;
        }

        // Draconic Resilience
        if (Features.Any(f => f.Index == "draconic-resilience") && 
            !Armors.Any(a => a.Index != "shield" && a.IsEquipped))
        {
            ac = Math.Max(ac, 13 + Dexterity.Modifier);
        }

        // Monk Unarmored Defense
        if (Features.Any(f => f.Index == "monk-unarmored-defense") && 
            !Armors.Any(a => a.Index != "shield" && a.IsEquipped))
        {
            ac = 10 + Dexterity.Modifier + Wisdom.Modifier;
        }

        // Fighting Style: Defense
        var defenseStyles = new[] { "fighter-fighting-style-defense", "ranger-fighting-style-defense", "fighting-style-defense" };
        if (Features.Any(f => defenseStyles.Contains(f.Index)) && 
            Armors.Any(a => a.IsEquipped && a.Index != "shield"))
        {
            ac += 1;
        }

        // Shield bonus
        if (Armors.Any(a => a.Index == "shield" && a.IsEquipped))
        {
            ac += 2;
        }

        _logger.Verbose($"Calculated AC: {ac}");
        return (byte)ac;
    }

    private int CalculateRandomHp(ClassMapper classMapper)
    {
        int hp = classMapper.Hp + Constitution.Modifier;
        
        for (int i = 2; i <= Level; i++)
        {
            hp += _random.Next(1, classMapper.Hp + 1) + Constitution.Modifier;
        }

        _logger.Verbose($"Calculated HP: {hp}");
        return hp;
    }

    private void AddAdditionalHpFromTraits()
    {
        var additionalHp = 0;

        // Dwarven Toughness
        if (Traits.Contains("dwarven-toughness"))
        {
            additionalHp += Level;
            _logger.Verbose($"Added {Level} HP from Dwarven Toughness");
        }

        // Draconic Resilience
        if (Features.Any(f => f.Index == "draconic-resilience"))
        {
            additionalHp += Level;
            _logger.Verbose($"Added {Level} HP from Draconic Resilience");
        }

        if (additionalHp > 0)
        {
            HitPoints += additionalHp;
            _logger.Verbose($"Final HP: {HitPoints}");
        }
    }

    public double GetSavePercentage(string index, int saveDc)
    {
        return index.ToLower() switch
        {
            "str" => CombatCalculator.CalculateRollPercentage(saveDc, Strength.Save),
            "dex" => CombatCalculator.CalculateRollPercentage(saveDc, Dexterity.Save),
            "con" => CombatCalculator.CalculateRollPercentage(saveDc, Constitution.Save),
            "int" => CombatCalculator.CalculateRollPercentage(saveDc, Intelligence.Save),
            "wis" => CombatCalculator.CalculateRollPercentage(saveDc, Wisdom.Save),
            "cha" => CombatCalculator.CalculateRollPercentage(saveDc, Charisma.Save),
            _ => 0
        };
    }

    #endregion

    #region Outcome Calculations

    public int CalculateBaseStats()
    {
        var totalBaseStats = 0;

        totalBaseStats += CalculateSpeedValue();
        totalBaseStats += CalculateStatsValue();
        totalBaseStats += CalculateSkillsValue();

        _logger.Verbose($"Total Base Stats for {Name}: {totalBaseStats}");

        return totalBaseStats;
    }

    public int CalculateSpeedValue()
    {
        var speedValue = (int)Speed;

        if (Speed > 30)
        {
            var extra = Speed - 30;
            speedValue += extra * 2;
        }

        return speedValue;
    }

    public int CalculateStatsValue() => Strength.Value + Dexterity.Value + Constitution.Value + Intelligence.Value + Wisdom.Value + Charisma.Value;

    public int CalculateSkillsValue() => Skills.Sum(item => item.Modifier);

    public int CalculateOffensivePower<T>(List<T> monsters, CRRatios difficulty) where T : ICombatCalculator
    {
        var offensivePower = 0;

        offensivePower += CalculateWeaponsPower(monsters.Cast<Monster>().ToList());
        offensivePower += CalculateSpellsPower(monsters.Cast<Monster>().ToList(), difficulty);

        return offensivePower;
    }

    public int CalculateHealingPower()
    {
        var healingPower = 0;

        foreach (var spell in Spells)
            if (spell.IsHealingSpell())
                healingPower += spell.GetHealingPower(SpellSlots, this);

        _logger.Verbose($"Total Healing Power for {Name}: {healingPower}");
        return healingPower;
    }

    public double CalculateSpellUsagePercentage(Spell spell, CRRatios difficulty)
    {
        if (spell == null || SpellSlots == null)
            return 0.0;

        if (spell.Level == 0)
            return 1.0;

        if (SpellSlots.GetSlotsLevelAvailable() == 0 || SpellSlots.GetSlotsLevelAvailable() < spell.Level && spell.Uses == "")
            return 0.0;

        if (spell.Uses != "")
            return 1 / (int)difficulty;

        var totalAvailableSlots = SpellSlots.GetTotalSlots(spell.Level);
        var numCompetingSpells = Spells.Count(s => s.Level > 0 && s.Level <= spell.Level && s.Uses == "");

        for (var level = 0; level < DataConstants.MaxSpellLevel; level++)
            if (level < spell.Level && SpellSlots.GetSlotsLevelAvailable() >= level)
                numCompetingSpells += Spells.Count(s => s.Level > 0 && s.Level <= level && s.Uses == "");

        if (numCompetingSpells == 0)
            return 0.0;

        var avgSlotsPerSpell = (double)totalAvailableSlots / Math.Max(1, Spells.Count(s => s.Level > 0 && s.Uses == ""));
        var usagePercentage = Math.Min(1.0, avgSlotsPerSpell / (int)difficulty);

        return usagePercentage;
    }

    private int CalculateWeaponsPower(List<Monster> monsters)
    {
        var offensivePower = 0.0;
        var averageMonsterAc = (int)monsters.Average(item => item.ArmorClass);
        var meleeWeaponsEquipped = MeleeWeapons.Where(item => item.IsEquipped).ToList();
        var rangedWeaponsEquipped = RangedWeapons.Where(item => item.IsEquipped).ToList();
        var maxMeleePower = 0.0;
        var maxRangedPower = 0.0;

        foreach (var weapon in meleeWeaponsEquipped)
        {
            var meleePower = weapon.GetWeaponPower(Strength.Modifier, Dexterity.Modifier);
            var attackBonus = weapon.GetAttackBonus(Strength.Modifier, Dexterity.Modifier, ProficiencyBonus, IsProficient(weapon));
            var attackPower = CombatCalculator.CalculateRollPercentage(averageMonsterAc, attackBonus) * meleePower;

            CombatCalculator.ApplyDefenses(monsters,
                r => r.Resistances,
                i => i.Immunities,
                v => v.Vulnerabilities,
                weapon.Damage.DamageType,
                ref attackPower);

            if (attackPower > maxMeleePower)
                maxMeleePower = attackPower;
        }

        foreach (var weapon in rangedWeaponsEquipped)
        {
            var rangedPower = weapon.GetWeaponPower(Strength.Modifier, Dexterity.Modifier);
            var attackBonus = weapon.GetAttackBonus(Strength.Modifier, Dexterity.Modifier, ProficiencyBonus, IsProficient(weapon));
            var attackPower = CombatCalculator.CalculateRollPercentage(averageMonsterAc, attackBonus) * rangedPower;

            CombatCalculator.ApplyDefenses(monsters,
                r => r.Resistances,
                i => i.Immunities,
                v => v.Vulnerabilities,
                weapon.Damage.DamageType,
                ref attackPower);

            if (attackPower > maxRangedPower)
                maxRangedPower = attackPower;
        }

        offensivePower = Math.Max(maxMeleePower, maxRangedPower);

        if (Features.Any(f => f.Index.Contains("extra-attack")))
            offensivePower *= 1 + Features.Where(f => f.Index.Contains("extra-attack")).Count();

        _logger.Verbose($"Offensive Power for {Name}: {(int)offensivePower}");
        return (int)offensivePower;
    }

    private int CalculateSpellsPower(List<Monster> monsters, CRRatios difficulty)
    {
        var offensivePower = 0;

        if (Spells.Count(item => item.IsDamageSpell()) == 0)
            return 0;

        foreach (var spell in Spells)
        {
            if (spell.IsDamageSpell())
            {
                var spellPower = spell.GetSpellPower(this);
                var hitPercentage = spell.GetSpellPercentage(this, monsters);
                var usagePercentage = CalculateSpellUsagePercentage(spell, difficulty);
                var totalPower = hitPercentage * spellPower * usagePercentage;

                if (spell.Dc != null && spell.Dc.DcType != null)
                    if (spell.Dc.DcSuccess.Equals("half", StringComparison.OrdinalIgnoreCase))
                        totalPower += hitPercentage * (spellPower / 2) * usagePercentage;

                CombatCalculator.ApplyDefenses(monsters,
                    r => r.Resistances,
                    i => i.Immunities,
                    v => v.Vulnerabilities,
                    spell.Damage?.DamageType ?? "",
                    ref totalPower);

                offensivePower += (int)totalPower;
            }
        }

        offensivePower /= Spells.Count(item => item.IsDamageSpell());

        _logger.Verbose($"Spells Power for {Name}: {offensivePower}");
        return offensivePower;
    }

    private bool IsProficient(Weapon weapon)
    {
        if (Proficiencies.Contains(weapon.Index))
            return true;

        var categoryRangeProficiency = $"{weapon.WeaponCategory.ToLower()}-{weapon.WeaponRange.ToLower()}-weapons";
        if (Proficiencies.Contains(categoryRangeProficiency))
            return true;

        var categoryProficiency = $"{weapon.WeaponCategory.ToLower()}-weapons";
        if (Proficiencies.Contains(categoryProficiency))
            return true;

        var rangeProficiency = $"{weapon.WeaponRange.ToLower()}-weapons";
        if (Proficiencies.Contains(rangeProficiency))
            return true;

        return false;
    }

    #endregion

    #region ToString

    public override string ToString()
    {
        var subraceStr = Subrace != null ? $" {Subrace}" : "";
        var str = $"{Name} | {Race}{subraceStr} | Lv{Level} {Class}\n";
        str += $"HP: {HitPoints} | Initiative: {Initiative} | Proficiency Bonus: +{ProficiencyBonus}\n";
        str += $"STR: {Strength.Value} ({Strength.Modifier:+#;-#;0}) | DEX: {Dexterity.Value} ({Dexterity.Modifier:+#;-#;0}) | " +
               $"CON: {Constitution.Value} ({Constitution.Modifier:+#;-#;0}) | INT: {Intelligence.Value} ({Intelligence.Modifier:+#;-#;0}) | " +
               $"WIS: {Wisdom.Value} ({Wisdom.Modifier:+#;-#;0}) | CHA: {Charisma.Value} ({Charisma.Modifier:+#;-#;0})\n";
        str += $"Saving Throws: STR {Strength.Save:+#;-#;0}, DEX {Dexterity.Save:+#;-#;0}, CON {Constitution.Save:+#;-#;0}, " +
               $"INT {Intelligence.Save:+#;-#;0}, WIS {Wisdom.Save:+#;-#;0}, CHA {Charisma.Save:+#;-#;0}\n";
        str += $"Skills: {string.Join(", ", Skills.Select(s => $"{s.Name} {s.Modifier:+#;-#;0}"))}\n";
        str += $"Proficiencies: {string.Join(", ", Proficiencies)}\n";
        str += Traits.Count > 0 ? $"Traits: {string.Join(", ", Traits)}\n" : "Traits: None\n";
        str += $"Features: {string.Join(", ", Features.Select(f => f.Name))}\n";
        
        var meleeWeapons = MeleeWeapons.Where(w => w.IsEquipped).Select(w => w.Name);
        var rangedWeapons = RangedWeapons.Where(w => w.IsEquipped).Select(w => w.Name);
        str += $"Weapons: {string.Join(", ", meleeWeapons)} (Melee), {string.Join(", ", rangedWeapons)} (Ranged)\n";
        str += $"Armors: {string.Join(", ", Armors.Where(a => a.IsEquipped).Select(a => a.Name))}\n";
        
        if (Vulnerabilities.Count > 0)
            str += $"Vulnerabilities: {string.Join(", ", Vulnerabilities)}\n";
        if (Resistances.Count > 0)
            str += $"Resistances: {string.Join(", ", Resistances)}\n";
        if (Immunities.Count > 0)
            str += $"Immunities: {string.Join(", ", Immunities)}\n";
        
        if (SpellSlots != null && SpellSlots.First > 0)
            str += SpellSlots.ToString();
        
        if (Cantrips.Count > 0)
            str += $"Cantrips: {string.Join(", ", Cantrips.Select(c => c.Name))}\n";
        if (Spells.Count > 0)
            str += $"Spells: {string.Join(", ", Spells.Select(s => s.Name))}\n";
        
        str += "\n" + new string('-', 120) + "\n";
        return str;
    }

    #endregion
}