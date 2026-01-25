using System;
using TrainingDataGenerator.Abstracts;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Equip;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Entities;

public class PartyMember : Creature, ICombatCalculator
{
    public byte Level { get; set; }
    public byte HitDie { get; set; }
    public byte ArmorClass { get; set; }
    public string Class { get; set; }
    public string Race { get; set; }
    public short Speed { get; set; }
    public string? Subrace { get; set; }        
    public string Subclass { get; set; }
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

    public PartyMember(int id, byte level, RaceMapper randomRace, ClassMapper randomClass)
    {
        Logger.Instance.Information($"Generating Member {id} at Level {level}. Class {randomClass.Index} and Race {randomRace.Index}");

        var random = new Random();
        var randomRaceAbilityBonus = randomRace.GetRandomAbility();
        var randomSubrace = (randomRace.Subraces.Count > 0) ? EntitiesFinder.GetEntityByIndex(Lists.subraces, new BaseEntity(randomRace.Index, randomRace.Name), randomRace.Subraces.OrderBy(_ => random.Next()).FirstOrDefault() ?? new BaseEntity()) : null;
        var randomSubclass = EntitiesFinder.GetEntityByIndex(Lists.subclasses, new BaseEntity(randomClass.Index, randomClass.Name), randomClass.Subclasses.OrderBy(_ => random.Next()).FirstOrDefault() ?? new BaseEntity());
        var levels = Lists.levels.Where(item => item.Level <= level && ((item.Class.Index == randomClass.Index && item.Subclass == null) || (item.Class.Index == randomClass.Index && item.Subclass?.Index == Subclass))).ToList();
        var features = Lists.features.Where(item => item.Level <= level && item.Parent == null && ((item.Class.Index == randomClass.Index && item.Subclass == null) || (item.Class.Index == randomClass.Index && item.Subclass?.Index == Subclass))).ToList();

        // Base Information
        Index = id.ToString();
        Name = $"Member {id}";
        Level = level;
        HitDie = (byte)randomClass.Hp;
        ProficiencyBonus = (sbyte)(2 + ((Level - 1) / 4));
        Race = randomRace.Index;
        Speed = randomRace.Speed;
        Size = randomRace.Size.ToString();
        Subrace = randomSubrace != null ? randomSubrace.Index : null;
        Class = randomClass.Index;
        Subclass = randomSubclass.Index;
        SpellcastingAbility = randomClass.SpellcastingAbility != null ? UtilityMethods.ConvertAbilityIndexToFullName(randomClass.SpellcastingAbility.SpellcastingAbility.Index) : string.Empty;

        SetAttributes(randomClass, randomRaceAbilityBonus, features); // Set Attributes with Racial Bonuses and Level Improvements
        ManageEquipments(randomClass); // Manage starting Equipments
        ManageTraitSpecifics(randomRace, randomSubrace); // Manage Trait Specifics
        SetProficiencies(randomClass, randomRace, randomSubrace);
        LevelAdvancements(levels, features); // Manage Level Advancements and related Features, Spells, ClassSpecific and SubclassSpecific
        CreateSkills(); // Create Skills with basic Modifiers
        AddProfToSavings(randomClass); // Add Proficiency to Saving Throws
        SetFeatures(); // Set Features effects
        SetAdditionalProficiencies(); // Set Additional Proficiencies from Traits & Features
        AddProfToSkills(); // Add Proficiency to Skills based on Proficiencies list
        SetDamageResistances(); // Set Damage Vulnerabilities, Resistances and Immunities
        ManageFeatureSpecific(); // Manage Feature Specifics (like Expertise)

        ArmorClass = CalculateArmorClass();
        HitPoints = CalculateRandomHp(randomClass);
        Initiative = Dexterity?.Modifier ?? 0;

        Cantrips = Cantrips.GroupBy(c => c.Index).Select(g => g.First()).ToList();
        Spells = Spells.GroupBy(s => s.Index).Select(g => g.First()).ToList();
    }

    #region features

    private void SetFeatures()
    {
        if (Features.Select(item => item.Index).ToList().Contains("fast-movement"))
            Speed += 10;

        if (Features.Select(item => item.Index).ToList().Contains("fighting-style-defense") || Features.Select(item => item.Index).ToList().Contains("ranger-fighting-style-defense"))
            if (Armors.Any(item => item.IsEquipped && item.Index != "shield"))
                ArmorClass += 1;

        if (Class == "monk")
            SetMonkFeatures();
    }

    private void SetSpellsFromFeatures()
    {
        if (Features.Select(item => item.Index).ToList().Contains("additional-magical-secrets"))
        {
            var spellsOptions = Lists.spells.Where(item => item.Level <= 3).ToList();
            var spellsChosen = spellsOptions.OrderBy(_ => new Random().Next()).Take(2).Select(item => new Spell(item)).ToList();

            foreach (var spell in spellsChosen)
                if (spell.Level == 0)
                    Cantrips.Add(spell);
                else
                    Spells.Add(spell);
        }

        if (Features.Select(item => item.Index).ToList().Contains("magical-secrets-1"))
        {
            var spellsOptions = Lists.spells.Where(item => item.Level <= 5).ToList();
            var spellsChosen = spellsOptions.OrderBy(_ => new Random().Next()).Take(2).Select(item => new Spell(item)).ToList();

            foreach (var spell in spellsChosen)
                if (spell.Level == 0)
                    Cantrips.Add(spell);
                else
                    Spells.Add(spell);
        }

        if (Features.Select(item => item.Index).ToList().Contains("magical-secrets-2"))
        {
            var spellsOptions = Lists.spells.Where(item => item.Level <= 7).ToList();
            var spellsChosen = spellsOptions.OrderBy(_ => new Random().Next()).Take(2).Select(item => new Spell(item)).ToList();

            foreach (var spell in spellsChosen)
                if (spell.Level == 0)
                    Cantrips.Add(spell);
                else
                    Spells.Add(spell);
        }

        if (Features.Select(item => item.Index).ToList().Contains("magical-secrets-3"))
        {
            var spellsOptions = Lists.spells.Where(item => item.Level <= 9).ToList();
            var spellsChosen = spellsOptions.OrderBy(_ => new Random().Next()).Take(2).Select(item => new Spell(item)).ToList();

            foreach (var spell in spellsChosen)
                if (spell.Level == 0)
                    Cantrips.Add(spell);
                else
                    Spells.Add(spell);
        }

        if (Features.Select(item => item.Index).ToList().Contains("pact-of-the-tome"))
            Cantrips.AddRange(Lists.spells.Where(item => item.Level == 0).OrderBy(_ => new Random().Next()).Take(3).Select(item => new Spell(item)).ToList());

        if (Features.Select(item => item.Index).ToList().Contains("mystic-arcanum-6th-level"))
            Spells.Add(new Spell(Lists.spells.Where(item => item.Level == 6).OrderBy(_ => new Random().Next()).First(), "1 per Long Rest"));

        if (Features.Select(item => item.Index).ToList().Contains("mystic-arcanum-7th-level"))
            Spells.Add(new Spell(Lists.spells.Where(item => item.Level == 7).OrderBy(_ => new Random().Next()).First(), "1 per Long Rest"));

        if (Features.Select(item => item.Index).ToList().Contains("mystic-arcanum-8th-level"))
            Spells.Add(new Spell(Lists.spells.Where(item => item.Level == 8).OrderBy(_ => new Random().Next()).First(), "1 per Long Rest"));

        if (Features.Select(item => item.Index).ToList().Contains("mystic-arcanum-9th-level"))
            Spells.Add(new Spell(Lists.spells.Where(item => item.Level == 9).OrderBy(_ => new Random().Next()).First(), "1 per Long Rest"));

        if (Features.Select(item => item.Index).ToList().Contains("bonus-cantrip") && Class == "druid")
            Cantrips.Add(new Spell(Lists.spells.Where(item => item.Classes.Select(c => c.Index).Contains("druid") && item.Level == 0).OrderBy(_ => new Random().Next()).First()));

        if (Class == "druid")
            SetCircleSpells();

        if (Class == "paladin")
            SetOathSpells();

        if (Class == "cleric")
            SetDomainSpells();
    }

    private void SetMonkFeatures()
    {
        if (Features.Select(item => item.Index).ToList().Contains("unarmored-defense-monk"))
            ArmorClass = (byte)(10 + Dexterity.Modifier + Wisdom.Modifier);

        if (Features.Select(item => item.Index).ToList().Contains("unarmored-movement-1") && Armors.All(item => !item.IsEquipped))
            Speed += 10;

        if (Features.Select(item => item.Index).ToList().Contains("diamond-soul"))
        {
            Strength.Save += ProficiencyBonus;
            Constitution.Save += ProficiencyBonus;
            Intelligence.Save += ProficiencyBonus;
            Charisma.Save += ProficiencyBonus;
        }
    }

    private void SetOathSpells()
    {
        if (Features.Select(item => item.Index).ToList().Contains("oath-spells") && Level >= 3)
            switch (Subclass)
            {
                case "devotion":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "protection-from-evil-and-good").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "sanctuary").First()));
                    break;
                case "ancients":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "ensnaring-strike").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "speak-with-animals").First()));
                    break;
                case "vengeance":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "bane").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "hunters-mark").First()));
                    break;
                default:
                    break;
            }

        if (Features.Select(item => item.Index).ToList().Contains("oath-spells") && Level >= 5)
            switch (Subclass)
            {
                case "devotion":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "lesser-restoration").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "zone-of-truth").First()));
                    break;
                case "ancients":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "moonbeam").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "misty-step").First()));
                    break;
                case "vengeance":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "hold-person").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "misty-step").First()));
                    break;
                default:
                    break;
            }

        if (Features.Select(item => item.Index).ToList().Contains("oath-spells") && Level >= 9)
            switch (Subclass)
            {
                case "devotion":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "beacon-of-hope").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "dispel-magic").First()));
                    break;
                case "ancients":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "plant-growth").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "protection-from-energy").First()));
                    break;
                case "vengeance":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "haste").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "protection-from-energy").First()));
                    break;
                default:
                    break;
            }

        if (Features.Select(item => item.Index).ToList().Contains("oath-spells") && Level >= 13)
            switch (Subclass)
            {
                case "devotion":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "freedom-of-movement").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "guardian-of-faith").First()));
                    break;
                case "ancients":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "ice-storm").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "stoneskin").First()));
                    break;
                case "vengeance":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "banishment").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "dimension-door").First()));
                    break;
                default:
                    break;
            }

        if (Features.Select(item => item.Index).ToList().Contains("oath-spells") && Level >= 17)
            switch (Subclass)
            {
                case "devotion":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "commune").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "flame-strike").First()));
                    break;
                case "ancients":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "commune-with-nature").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "tree-stride").First()));
                    break;
                case "vengeance":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "hold-monster").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "scrying").First()));
                    break;
                default:
                    break;
            }
    }

    private void SetDomainSpells()
    {
        if (Features.Select(item => item.Index).ToList().Contains("domain-spells-1"))
            switch (Subclass)
            {
                case "knowledge":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "command").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "identify").First()));
                    break;
                case "life":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "bless").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "cure-wounds").First()));
                    break;
                case "light":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "burning-hands").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "faerie-fire").First()));
                    break;
                case "nature":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "animal-friendship").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "entangle").First()));
                    break;
                case "tempest":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "fog-cloud").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "thunderwave").First()));
                    break;
                case "trickery":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "charm-person").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "disguise-self").First()));
                    break;
                case "war":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "divine-favor").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "shield-of-faith").First()));
                    break;
                default:
                    break;
            }

        if (Features.Select(item => item.Index).ToList().Contains("domain-spells-2"))
            switch (Subclass)
            {
                case "knowledge":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "augury").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "suggestion").First()));
                    break;
                case "life":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "lesser-restoration").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "spiritual-weapon").First()));
                    break;
                case "light":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "flaming-sphere").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "scorching-ray").First()));
                    break;
                case "nature":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "barkskin").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "spike-growth").First()));
                    break;
                case "tempest":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "gust-of-wind").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "shatter").First()));
                    break;
                case "trickery":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "mirror-image").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "pass-without-trace").First()));
                    break;
                case "war":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "magic-weapon").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "spiritual-weapon").First()));
                    break;
                default:
                    break;
            }

        if (Features.Select(item => item.Index).ToList().Contains("domain-spells-3"))
            switch (Subclass)
            {
                case "knowledge":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "nondetection").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "speak-with-dead").First()));
                    break;
                case "life":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "beacon-of-hope").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "revivify").First()));
                    break;
                case "light":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "daylight").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "fireball").First()));
                    break;
                case "nature":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "plant-growth").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "wind-wall").First()));
                    break;
                case "tempest":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "call-lightning").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "sleet-storm").First()));
                    break;
                case "trickery":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "blink").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "dispel-magic").First()));
                    break;
                case "war":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "crusaders-mantle").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "spirit-guardians").First()));
                    break;
                default:
                    break;
            }

        if (Features.Select(item => item.Index).ToList().Contains("domain-spells-4"))
            switch (Subclass)
            {
                case "knowledge":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "arcane-eye").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "confusion").First()));
                    break;
                case "life":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "death-ward").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "guardian-of-faith").First()));
                    break;
                case "light":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "guardian-of-faith").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "wall-of-fire").First()));
                    break;
                case "nature":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "dominate-beast").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "grasping-vine").First()));
                    break;
                case "tempest":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "control-water").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "ice-storm").First()));
                    break;
                case "trickery":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "dimension-door").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "polymorph").First()));
                    break;
                case "war":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "freedom-of-movement").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "stoneskin").First()));
                    break;
                default:
                    break;
            }

        if (Features.Select(item => item.Index).ToList().Contains("domain-spells-5"))
            switch (Subclass)
            {
                case "knowledge":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "legend-lore").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "scrying").First()));
                    break;
                case "life":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "mass-cure-wounds").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "raise-dead").First()));
                    break;
                case "light":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "flame-strike").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "scrying").First()));
                    break;
                case "nature":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "insect-plague").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "tree-stride").First()));
                    break;
                case "tempest":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "destructive-wave").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "insect-plague").First()));
                    break;
                case "trickery":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "dominate-person").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "modify-person").First()));
                    break;
                case "war":
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "flame-strike").First()));
                    Spells.Add(new Spell(Lists.spells.Where(item => item.Index == "hold-monster").First()));
                    break;
                default:
                    break;
            }
    }

    private void SetCircleSpells()
    {
        if (Features.Select(item => item.Index).Contains("circle-spells-1"))
        {
            switch (Subclass)
            {
                case "arctic":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "hold-person")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "spike-growth")));
                    break;
                case "coast":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "mirror-image")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "misty-step")));
                    break;
                case "desert":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "blur")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "silence")));
                    break;
                case "forest":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "barkskin")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "spider-climb")));
                    break;
                case "grassland":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "invisibility")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "pass-without-trace")));
                    break;
                case "mountain":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "spider-climb")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "spike-growth")));
                    break;
                case "swamp":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "darkness")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "melfs-acid-arrow")));
                    break;
                case "underdark":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "spider-climb")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "web")));
                    break;
            }
        }
        if (Features.Select(item => item.Index).Contains("circle-spells-2"))
        {
            switch (Subclass)
            {
                case "arctic":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "sleet-storm")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "slow")));
                    break;
                case "coast":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "water-breathing")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "water-walk")));
                    break;
                case "desert":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "create-food-and-water")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "protection-from-energy")));
                    break;
                case "forest":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "call-lightning")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "plant-growth")));
                    break;
                case "grassland":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "daylight")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "haste")));
                    break;
                case "mountain":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "lightning-bolt")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "meld-into-stone")));
                    break;
                case "swamp":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "water-walk")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "stinking-cloud")));
                    break;
                case "underdark":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "gaseous-form")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "stinking-cloud")));
                    break;
            }
        }
        if (Features.Select(item => item.Index).Contains("circle-spells-3"))
        {
            switch (Subclass)
            {
                case "arctic":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "freedom-of-movement")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "ice-storm")));
                    break;
                case "coast":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "control-water")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "freedom-of-movement")));
                    break;
                case "desert":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "blight")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "hallucinatory-terrain")));
                    break;
                case "forest":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "divination")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "freedom-of-movement")));
                    break;
                case "grassland":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "divination")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "freedom-of-movement")));
                    break;
                case "mountain":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "stone-shape")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "stoneskin")));
                    break;
                case "swamp":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "freedom-of-movement")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "locate-creature")));
                    break;
                case "underdark":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "greater-invisibility")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "stone-shape")));
                    break;
            }
        }
        if (Features.Select(item => item.Index).Contains("circle-spells-4"))
        {
            switch (Subclass)
            {
                case "arctic":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "commune-with-nature")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "cone-of-cold")));
                    break;
                case "coast":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "conjure-elemental")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "scrying")));
                    break;
                case "desert":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "insect-plague")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "wall-of-stone")));
                    break;
                case "forest":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "commune-with-nature")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "tree-stride")));
                    break;
                case "grassland":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "dream")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "insect-plague")));
                    break;
                case "mountain":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "passwall")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "wall-of-stone")));
                    break;
                case "swamp":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "insect-plague")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "scrying")));
                    break;
                case "underdark":
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "cloudkill")));
                    Spells.Add(new Spell(Lists.spells.First(item => item.Index == "insect-plague")));
                    break;
            }
        }
    }

    private void CheckFeaturesPrerequisities(List<FeatureMapper> features)
    {
        foreach (var feature in features)
        {
            if (feature.Prerequisites != null)
            {
                foreach (var prereq in feature.Prerequisites)
                {
                    bool meetsPrereq = false;

                    if (prereq.Type == "feature")
                    {
                        if (Features.Any(item => item.Index == prereq.Feature?.Split('/').Last()))
                            meetsPrereq = true;
                    }
                    else if (prereq.Type == "spell")
                    {
                        if (Spells.Any(item => item.Index == prereq.Feature?.Split('/').Last()) || Cantrips.Any(item => item.Index == prereq.Feature?.Split('/').Last()))
                            meetsPrereq = true;
                    }
                    if (!meetsPrereq)
                        Features = Features.Select(item => item).Where(item => item.Index != feature.Index).ToList();

                }
            }
        }
    }

    #endregion

    #region traits

    private void ManageTraitSpecifics(RaceMapper randomRace, SubraceMapper? randomSubrace)
    {
        var raceTraits = new List<TraitMapper>();

        if (randomRace.Traits.Count > 0)
        {
            var allTraits = randomRace.Traits;

            if (randomSubrace != null)
                allTraits.AddRange(randomSubrace.RacialTraits);

            raceTraits = allTraits.Select(item => EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(randomRace.Index, randomRace.Name), new BaseEntity(randomSubrace?.Index ?? string.Empty, randomSubrace?.Name ?? string.Empty), item)).Where(item => item != null && item.Parent == null).ToList();
            var raceSubtraits = new List<BaseEntity>();

            foreach (var item in raceTraits)
                if (item.TraitSpec == null || item.TraitSpec.SubtraitOptions == null)
                    continue;
                else
                    raceSubtraits = item.TraitSpec.SubtraitOptions.GetRandomChoice();

            if (raceSubtraits.Count > 0)
                raceTraits.AddRange(raceSubtraits
                    .Select(item => EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(randomRace.Index, randomRace.Name), new BaseEntity(randomSubrace?.Index ?? string.Empty, randomSubrace?.Name ?? string.Empty), item)));

            if (raceTraits.Count > 0)
                Traits = raceTraits.Select(item => item.Index).ToList();
        }
    }

    private void SetSpellsFromTraits()
    {
        if (Traits.Contains("high-elf-cantrip"))
        {
            var cantripOptions = Lists.spells.Where(item => item.Level == 0 && item.Classes.Any(x => x.Index == "wizard")).ToList();
            var cantripChosen = cantripOptions.OrderBy(_ => new Random().Next()).First();

            Cantrips.Add(new Spell(cantripChosen));
        }

        if (Traits.Contains("infernal-legacy"))
        {
            var cantripOptions = Lists.spells.Where(item => item.Index == "thaumaturgy").ToList();
            var spellOptions = Lists.spells.Where(item => (Level >= 3 && item.Index == "hellish-rebuke") || (Level >= 5 && item.Index == "darkness")).ToList();

            foreach (var cantrip in cantripOptions)
            {
                var cantripChosen = cantripOptions.Where(item => item.Index == cantrip.Index).FirstOrDefault();

                if (cantripChosen != null)
                    Cantrips.Add(new Spell(cantripChosen));
            }

            foreach (var spell in spellOptions)
            {
                var spellChosen = spellOptions.Where(item => item.Index == spell.Index).FirstOrDefault();

                if (spellChosen != null)
                    Spells.Add(new Spell(spellChosen, "1 per Long Rest"));
            }
        }
    }

    #endregion

    #region attributes

    private void SetAttributes(ClassMapper randomClass, List<AbilityBonus> randomRaceAbilityBonus, List<FeatureMapper> features)
    {
        var random = new Random();
        var attributes = AssumeAttributes(randomClass, random); // First assume random value for attributes based on Standard Array rule

        if (randomRaceAbilityBonus.Count > 0)
            AddAttributesBonuses(attributes, randomRaceAbilityBonus);

        var abilityImprovements = (byte)features.Count(item => item.Index.Contains("ability-score-improvement"));
        AbilityScoreImprovement(attributes, abilityImprovements);

        if (Class == "barbarian")
            if (Features.Select(item => item.Index).ToList().Contains("primal-champion"))
            {
                attributes[0] += 4;
                attributes[2] += 4;
            }

        Strength = new Attribute(attributes[0]);
        Dexterity = new Attribute(attributes[1]);
        Constitution = new Attribute(attributes[2]);
        Intelligence = new Attribute(attributes[3]);
        Wisdom = new Attribute(attributes[4]);
        Charisma = new Attribute(attributes[5]);
    }

    private List<byte> AssumeAttributes(dynamic randomClass, Random random)
    {
        var attributes = new List<byte> { 15, 14, 13, 12, 10, 8 };

        return attributes.OrderBy(_ => random.Next()).ToList();
    }

    private void AddAttributesBonuses(List<byte> attributes, List<AbilityBonus> randomRaceAbilityBonus)
    {
        foreach (var bonus in randomRaceAbilityBonus)
        {
            if (bonus.Ability.Index == "str")
                attributes[0] += (byte)bonus.Bonus;
            if (bonus.Ability.Index == "dex")
                attributes[1] += (byte)bonus.Bonus;
            if (bonus.Ability.Index == "con")
                attributes[2] += (byte)bonus.Bonus;
            if (bonus.Ability.Index == "int")
                attributes[3] += (byte)bonus.Bonus;
            if (bonus.Ability.Index == "wis")
                attributes[4] += (byte)bonus.Bonus;
            if (bonus.Ability.Index == "cha")
                attributes[5] += (byte)bonus.Bonus;
        }
    }

    private void AddProfToSavings(ClassMapper cl)
    {
        foreach (var item in cl.SavingThrows)
        {
            if (item.Index == "str")
                Strength.SetProficiency(true, ProficiencyBonus);
            if (item.Index == "dex")
                Dexterity.SetProficiency(true, ProficiencyBonus);
            if (item.Index == "con")
                Constitution.SetProficiency(true, ProficiencyBonus);
            if (item.Index == "int")
                Intelligence.SetProficiency(true, ProficiencyBonus);
            if (item.Index == "wis")
                Wisdom.SetProficiency(true, ProficiencyBonus);
            if (item.Index == "cha")
                Charisma.SetProficiency(true, ProficiencyBonus);
        }
    }

    #endregion

    #region skills

    private List<string> GetAllSkills() =>
        Skills.Select(item => item.Index).ToList();

    private void SetProficiencies(ClassMapper randomClass, RaceMapper randomRace, SubraceMapper? randomSubrace)
    {
        var raceTraits = randomRace.Traits.Select(item => EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(randomRace.Index, randomRace.Name), new BaseEntity(randomSubrace?.Index ?? string.Empty, randomSubrace?.Name ?? string.Empty), item)).Where(item => item != null && item.Parent == null).ToList();

        // Starting Proficiencies (Race & Class & Race Traits)
        Proficiencies = randomRace.StartingProficiences.Select(item => item.Index).ToList();
        Proficiencies.AddRange(randomClass.Proficiencies.Select(item => item.Index).ToList());

        if (raceTraits.Count > 0)
            foreach (var item in raceTraits)
                if (item.Proficiencies.Count > 0)
                    Proficiencies.AddRange(item.Proficiencies.Select(item => item.Index).ToList());

        // Random Proficiencies (Race, Class & Race Traits)
        Proficiencies.AddRange(randomRace.GetRandomProficiency(Proficiencies));

        if (randomClass.ProficiencyChoices.Count > 0)
            foreach (var item in randomClass.ProficiencyChoices)
                Proficiencies.AddRange(item.GetRandomChoice(Proficiencies));

        if (raceTraits.Count > 0)
            foreach (var item in raceTraits)
                if (item.ProficiencyChoice != null)
                    Proficiencies.AddRange(item.ProficiencyChoice.GetRandomChoice(Proficiencies));

        Proficiencies.ToHashSet().ToList(); // Remove Duplicates
    }

    private void SetAdditionalProficiencies()
    {
        if (Traits.Count > 0)
        {
            if (Traits.Contains("keen-senses"))
                Proficiencies.Add("skill-perception");

            if (Traits.Contains("elf-weapon-training"))
                Proficiencies.AddRange(new List<string>() { "shortsword", "longsword", "shortbow", "longbow" });

            if (Traits.Contains("dwarven-combat-training"))
                Proficiencies.AddRange(new List<string>() { "battleaxe", "handaxe", "light-hammer", "warhammer" });

            if (Traits.Contains("menacing"))
                Proficiencies.Add("skill-intimidation");
        }

        if (Features.Count > 0)
            if (Class == "cleric" && Subclass == "life" && Features.Select(item => item.Index).ToList().Contains("bonus-proficiency"))
                Proficiencies.Add("heavy-armor");
    }

    private void AddProfToSkills()
    {
        if (Traits.Contains("skill-versatility"))
            Proficiencies.AddRange(GetAllSkills().OrderBy(_ => new Random().Next()).Take(2).ToList());

        if (Features.Count > 0)
            if (Class == "bard" && Features.Select(item => item.Index).ToList().Contains("bonus-proficiencies"))
                Proficiencies.AddRange(GetAllSkills().OrderBy(_ => new Random().Next()).Take(3).ToList());

        foreach (var skill in Skills)
            if (Proficiencies.Select(item => item).ToList().Contains(skill.Index))
                skill.SetProficiency(true, ProficiencyBonus);

        if (Features.Select(item => item.Index).ToList().Contains("jack-of-all-trades"))
            foreach (var skill in Skills.Where(skill => !skill.IsProficient).ToList())
                skill.Modifier += (sbyte)Math.Floor(ProficiencyBonus / 2.0);

        if (Features.Select(item => item.Index).ToList().Contains("remarkable-athlete"))
            foreach (var skill in Skills.Where(skill => !skill.IsProficient && new List<string>() { "skill-acrobatics", "skill-athletics", "skill-sleight-of-hand", "skill-stealth" }.Contains(skill.Index)).ToList())
                skill.Modifier += (sbyte)Math.Floor(ProficiencyBonus / 2.0);
    }

    #endregion

    #region equipment

    private void ManageEquipments(ClassMapper randomClass)
    {
        var allEquipmentsBase = randomClass.StartingEquipments;
        var randomBaseEquipments = randomClass.StartingEquipmentsOptions.SelectMany(item => item.GetRandomEquipment()).ToList();
        allEquipmentsBase.AddRange(randomBaseEquipments);

        var allEquipmentsMapper = allEquipmentsBase.Select(item => EntitiesFinder.GetEntityByIndex(Lists.equipments, item.Equip)).Where(item => item != null).ToList();
        var allArmors = allEquipmentsMapper.Where(item => item.EquipmentCategory.Index == "armor").Select(item => new Armor(item)).ToList();
        MeleeWeapons = allEquipmentsMapper.Where(item => item.EquipmentCategory.Index == "weapon" && item.WeaponRange == "Melee").Select(item => new MeleeWeapon(item)).ToList();
        RangedWeapons = allEquipmentsMapper.Where(item => item.EquipmentCategory.Index == "weapon" && item.WeaponRange == "Ranged").Select(item => new RangedWeapon(item)).ToList();
        Ammunitions = allEquipmentsMapper.Where(item => item.GearCategory?.Index == "ammunition").Select(item => new Ammunition(item)).ToList();

        Ammunitions.ForEach(item => item.IsEquipped = true);

        EquipRandomWeapons(allArmors);
        ManageArmorRequirements(allArmors);
    }

    private void ManageArmorRequirements(List<Armor> allArmors)
    {
        var random = new Random();

        if (allArmors.Where(item => item.Index != "shield").ToList().Count > 0 && allArmors.Any(item => item.StrengthMinimum <= Strength?.Value))
        {
            var armorsEquippable = allArmors.Where(item => item.StrengthMinimum <= Strength?.Value && item.Index != "shield").ToList();

            if (armorsEquippable.Count > 0)
                armorsEquippable.OrderBy(_ => random.Next()).First().IsEquipped = true;
        }

        Armors = allArmors;
    }

    #endregion

    #region utilities

    public double GetSavePercentage(string index, int saveDc)
    {
        switch (index)
        {
            case "str":
                return CombatCalculator.CalculateRollPercentage(saveDc, Strength.Save);
            case "dex":
                return CombatCalculator.CalculateRollPercentage(saveDc, Dexterity.Save);
            case "con":
                return CombatCalculator.CalculateRollPercentage(saveDc, Constitution.Save);
            case "int":
                return CombatCalculator.CalculateRollPercentage(saveDc, Intelligence.Save);
            case "wis":
                return CombatCalculator.CalculateRollPercentage(saveDc, Wisdom.Save);
            case "cha":
                return CombatCalculator.CalculateRollPercentage(saveDc, Charisma.Save);
            default:
                return 0;
        }
    }

    private byte CalculateArmorClass()
    {
        var ac = 10 + Dexterity.Modifier;
        var equippedArmor = Armors.Where(item => item.IsEquipped && item.Index != "shield").FirstOrDefault();

        if (equippedArmor != null)
        {
            ac = equippedArmor.ArmorClass.Base;

            if (equippedArmor.ArmorClass.HasDexBonus)
            {
                ac += Dexterity.Modifier;

                if (equippedArmor.ArmorClass.MaxDexBonus != null)
                    if (Dexterity.Modifier > equippedArmor.ArmorClass.MaxDexBonus)
                        ac = equippedArmor.ArmorClass.Base + equippedArmor.ArmorClass.MaxDexBonus.Value;
            }
        }

        if (Features.Select(item => item.Index).ToList().Contains("barbarian-unarmored-defense") && Armors.Where(item => item.Index != "shield" && item.IsEquipped).Count() == 0)
            ac = 10 + Dexterity.Modifier + Constitution.Modifier;

        if (Features.Select(item => item.Index).ToList().Contains("draconic-resilience") && Armors.Where(item => item.Index != "shield" && item.IsEquipped).Count() == 0)
            ac = 13 + Dexterity.Modifier;

        if (Armors.Any(item => item.Index == "shield" && item.IsEquipped))
            ac += 2;

        return (byte)ac;
    }

    private void LevelAdvancements(List<LevelMapper> levels, List<FeatureMapper> features)
    {
        for (int i = 1; i <= Level; i++)
        {
            List<LevelMapper> currentLevels = levels.Where(item => item.Class.Index == Class && item.Level == i).ToList();

            foreach (var level in currentLevels)
            {
                var levelFeatures = features.Where(item => item.Level == i).ToList();

                if (levelFeatures.Count == 0)
                    continue;

                Features.AddRange(levelFeatures.Select(item => new Feature(item, Proficiencies)));
            }

            if (i == Level)
            {
                SetSpellsFromTraits(); // Set Spells from Traits (like High Elf Cantrip)
                SetSpellsFromFeatures(); // Set Spells from Features (like Infernal Legacy)

                if (currentLevels.Any(item => item.Spellcasting != null))
                {
                    var spellcasting = currentLevels.First(item => item.Spellcasting != null).Spellcasting;

                    if (spellcasting != null)
                    {
                        SpellSlots = new Slots(spellcasting);

                        switch (Class)
                        {
                            case "cleric":
                                spellcasting.SpellsKnown = (byte?)((byte)Wisdom.Modifier + Level);
                                break;
                            case "druid":
                                spellcasting.SpellsKnown = (byte?)((byte)Wisdom.Modifier + Level);
                                break;
                            case "paladin":
                                spellcasting.SpellsKnown = (byte?)((byte)Charisma.Modifier + Math.Floor((double)Level / 2));
                                break;
                            case "ranger":
                                spellcasting.SpellsKnown = (byte?)((byte)Wisdom.Modifier + Math.Floor((double)Level / 2));
                                break;
                            case "wizard":
                                spellcasting.SpellsKnown = (byte?)((byte)Intelligence.Modifier + Level);
                                break;
                        }

                        Spells.AddRange(Lists.spells.Where(item => SpellSlots.GetSlotsLevelAvailable() >= item.Level && 
                            (item.Classes.Any(x => x.Index == Class) || 
                            (item.Subclasses == null || (item.Classes.Any(x => x.Index == Class) && item.Subclasses.Any(x => x.Index == Subclass))) && 
                            (Spells == null || !Spells.Any(s => s.Index == item.Index))))
                                .OrderBy(_ => new Random().Next())
                                .Take(spellcasting.SpellsKnown ?? 0)
                                .Select(item => new Spell(item))
                                .ToList());

                        Cantrips.AddRange(Lists.spells.Where(item => item.Level == 0 && 
                            (item.Classes.Any(x => x.Index == Class) || 
                            (item.Subclasses == null || (item.Classes.Any(x => x.Index == Class) && item.Subclasses.Any(x => x.Index == Subclass))) && 
                            (Cantrips == null || !Cantrips.Any(s => s.Index == item.Index))))
                                .OrderBy(_ => new Random().Next())
                                .Take(spellcasting.CantripsKnown ?? 0)
                                .Select(item => new Spell(item))
                                .ToList());
                    }
                }

                if (currentLevels.Any(item => item.ClassSpecific != null))
                    ClassSpecific = currentLevels.Where(item => item.ClassSpecific != null).Last().ClassSpecific;

                if (currentLevels.Any(item => item.SubclassSpecific != null))
                    SubclassSpecific = currentLevels.Where(item => item.SubclassSpecific != null).Last().SubclassSpecific;

                CheckFeaturesPrerequisities(features); //Check Features Prerequisities (after Features and Spells are set)
            }
        }
    }

    private void ManageFeatureSpecific()
    {
        var newFeatures = new List<Feature>();

        foreach (var feature in Features)
        {
            if (feature.FeatureType != null && feature.FeatureType == FeatureSpecificTypes.Expertise)
            {
                foreach (var choice in feature.FeatureSpec ?? new List<string>())
                {
                    var skill = Skills.Where(item => item.Index == choice).FirstOrDefault();

                    if (skill != null)
                        skill.SetExpertise(true, ProficiencyBonus);
                }
            }
            else if (feature.FeatureType != null && feature.FeatureType == FeatureSpecificTypes.Enemy)
                FeatureSpecifics.AddRange(feature.FeatureSpec ?? new List<string>());

            else if (feature.FeatureType != null && feature.FeatureType == FeatureSpecificTypes.Terrain)
                FeatureSpecifics.AddRange(feature.FeatureSpec ?? new List<string>());

            else if (feature.FeatureType != null && feature.FeatureType == FeatureSpecificTypes.Subfeature)
            {
                foreach (var choice in feature.FeatureSpec ?? new List<string>())
                {
                    var subfeature = Lists.features.Where(item => item.Index == choice).FirstOrDefault();

                    if (subfeature != null)
                    {
                        newFeatures.Add(new Feature(subfeature, Proficiencies));
                    }
                }
            }
        }

        Features.AddRange(newFeatures);
    }

    private void AbilityScoreImprovement(List<byte> attributes, byte numberImprovements)
    {
        for (byte j = 0; j < numberImprovements; j++)
        {
            for (int k = 0; k < 2; k++)
            {
                var random = new Random();
                var attributeIndex = random.Next(0, 6);

                switch (attributeIndex)
                {
                    case 0:
                        attributes[0] += 1;
                        if (attributes[0] > 20)
                        {
                            attributes[0] -= 1;
                            k--;
                            continue;
                        }
                        break;
                    case 1:
                        attributes[1] += 1;
                        if (attributes[1] > 20)
                        {
                            attributes[1] -= 1;
                            k--;
                            continue;
                        }
                        break;
                    case 2:
                        attributes[2] += 1;
                        if (attributes[2] > 20)
                        {
                            attributes[2] -= 1;
                            k--;
                            continue;
                        }
                        break;
                    case 3:
                        attributes[3] += 1;
                        if (attributes[3] > 20)
                        {
                            attributes[3] -= 1;
                            k--;
                            continue;
                        }
                        break;
                    case 4:
                        attributes[4] += 1;
                        if (attributes[4] > 20)
                        {
                            attributes[4] -= 1;
                            k--;
                            continue;
                        }
                        break;
                    case 5:
                        attributes[5] += 1;
                        if (attributes[5] > 20)
                        {
                            attributes[5] -= 1;
                            k--;
                            continue;
                        }
                        break;
                }
            }
        }
    }

    private void EquipRandomWeapons(List<Armor> allArmors)
    {
        var random = new Random();

        if (MeleeWeapons.Count > 0)
        {
            var meleeWeapon = MeleeWeapons.OrderBy(_ => random.Next()).First();
            meleeWeapon.IsEquipped = true;

            if (meleeWeapon.Properties.Contains("light"))
                if (MeleeWeapons.Any(item => item.Properties.Contains("light") && item.IsEquipped == false))
                    MeleeWeapons.Where(item => item.Properties.Contains("light") && item.IsEquipped == false).OrderBy(_ => random.Next()).First().IsEquipped = true;
                else if (meleeWeapon.Properties.Contains("two-handed"))
                    if (allArmors.Any(item => item.Index == "shield"))
                        allArmors.Where(item => item.Index == "shield").First().IsEquipped = false;
                    else if (allArmors.Any(item => item.Index == "shield"))
                        allArmors.Where(item => item.Index == "shield").First().IsEquipped = true;
        }

        if (RangedWeapons.Count > 0)
            RangedWeapons.OrderBy(_ => random.Next()).First().IsEquipped = true;
    }

    private int CalculateRandomHp(ClassMapper classMapper)
    {
        var random = new Random();

        int hp = classMapper.Hp + Constitution.Modifier;
        for (int i = 2; i <= Level; i++)
            hp += random.Next(1, classMapper.Hp + 1) + Constitution.Modifier;

        if (Traits.Contains("dwarven-toughness"))
            hp += Level;

        if (Features.Select(item => item.Index).Contains("draconic-resilience"))
            hp += Level;

        return hp;
    }

    private void SetDamageResistances()
    {
        if (Race == "dragonborn")
            switch (Traits.Where(item => item.Contains("dragon-ancestry")).FirstOrDefault())
            {
                case "dragon-ancestry-black":
                    Resistances.Add("acid");
                    break;
                case "dragon-ancestry-blue":
                    Resistances.Add("lightning");
                    break;
                case "dragon-ancestry-brass":
                    Resistances.Add("fire");
                    break;
                case "dragon-ancestry-bronze":
                    Resistances.Add("lightning");
                    break;
                case "dragon-ancestry-copper":
                    Resistances.Add("acid");
                    break;
                case "dragon-ancestry-gold":
                    Resistances.Add("fire");
                    break;
                case "dragon-ancestry-green":
                    Resistances.Add("poison");
                    break;
                case "dragon-ancestry-red":
                    Resistances.Add("fire");
                    break;
                case "dragon-ancestry-silver":
                    Resistances.Add("cold");
                    break;
                case "dragon-ancestry-white":
                    Resistances.Add("cold");
                    break;
            }

        if (Traits.Contains("dwarven-resilience"))
            Resistances.Add("poison");

        if (Traits.Contains("hellish-resistance"))
            Resistances.Add("fire");

        if (Class == "druid" && Features.Select(item => item.Index).ToList().Contains("natures-ward"))
            Immunities.AddRange(new List<string>() { "poison", "disease" });

        if (Class == "monk" && Features.Select(item => item.Index).ToList().Contains("purity-of-body"))
            Immunities.AddRange(new List<string>() { "poison", "disease" });

        if (Class == "paladin" && Features.Select(item => item.Index).ToList().Contains("divine-health"))
            Immunities.Add("disease");

        if (Class == "warlock" && Subclass == "the-fiend" && Features.Select(item => item.Index).ToList().Contains("fiendish-resilience"))
            Resistances.Add(new List<string>() { "bludgeoning", "slashing", "piercing", "acid", "cold", "fire", "force", "lightning", "necrotic", "poison", "psychic", "radiant", "thunder" }.OrderBy(_ => new Random().Next()).First());
    }

    private bool IsProficient(Weapon weapon)
    {
        if (Proficiencies.Contains(weapon.Index))
            return true;
        else if (Proficiencies.Contains(weapon.WeaponCategory.ToLower() + "-" + weapon.WeaponRange.ToLower() + "-" + "weapons"))
            return true;
        else if (Proficiencies.Contains(weapon.WeaponCategory.ToLower() + "-" + "weapons"))
            return true;
        else if (Proficiencies.Contains(weapon.WeaponRange.ToLower() + "-" + "weapons"))
            return true;

        return false;
    }

    #endregion

    #region outcome

    public int CalculateBaseStats()
    {
        var totalBaseStats = 0;

        totalBaseStats += CalculateSpeedValue();
        totalBaseStats += CalculateStatsValue();
        totalBaseStats += CalculateSkillsValue();

        Logger.Instance.Information($"Total Base Stats for {Name}: {totalBaseStats}");

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

        Logger.Instance.Information($"Total Healing Power for {Name}: {healingPower}");
        return healingPower;
    }

    public double CalculateSpellUsagePercentage(Spell spell, CRRatios difficulty)
    {
        if (spell == null || SpellSlots == null)
            return 0.0;

        if (spell.Level == 0)
            return 1.0;

        if (SpellSlots.GetSlotsLevelAvailable() == 0 || (SpellSlots.GetSlotsLevelAvailable() < spell.Level && spell.Uses == ""))
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
        var averageMonsterAc = (int)monsters.Average(item => item.AC.Average(x => x.Value));
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

        Logger.Instance.Information($"Offensive Power for {Name}: {(int)offensivePower}");
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
                var totalPower = (hitPercentage * spellPower) * usagePercentage;

                if (spell.Dc != null && spell.Dc.DcType != null)
                    if (spell.Dc.DcSuccess.Equals("half", StringComparison.OrdinalIgnoreCase))
                        totalPower += (hitPercentage * (spellPower / 2)) * usagePercentage;

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

        Logger.Instance.Information($"Spells Power for {Name}: {(int)offensivePower}");
        return (int)offensivePower;
    }

    #endregion

    public override string ToString()
    {
        var subraceStr = Subrace != null ? $" {Subrace}" : "";
        var str = $"{Name} | {Race}{subraceStr} | Lv{Level} {Class}\n";
        str += $"HP: {HitPoints} | Initiative: {Initiative} | Proficiency Bonus: +{ProficiencyBonus}\n";
        str += $"STR: {Strength.Value} ({Strength.Modifier}) | DEX: {Dexterity.Value} ({Dexterity.Modifier}) | CON: {Constitution.Value} ({Constitution.Modifier}) | INT: {Intelligence.Value} ({Intelligence.Modifier}) | WIS: {Wisdom.Value} ({Wisdom.Modifier}) | CHA: {Charisma.Value} ({Charisma.Modifier})\n";
        str += $"Saving Throws: STR {Strength.Save}, DEX {Dexterity.Save}, CON {Constitution.Save}, INT {Intelligence.Save}, WIS {Wisdom.Save}, CHA {Charisma.Save}\n";
        str += $"Skills: {string.Join(", ", Skills.Select(skill => $"{skill.Name} {skill.Modifier}"))}\n";
        str += $"Proficiencies: {string.Join(", ", Proficiencies.Select(p => p))}\n";
        str += Traits != null ? $"Traits: {string.Join(", ", Traits.Select(t => t))}\n": "Traits: None\n";
        str += $"Features: {string.Join(", ", Features.Select(f => f.Name))}\n";
        str += $"Weapons: {string.Join(", ", MeleeWeapons.Where(w => w.IsEquipped).Select(w => w.Name))} (Melee), {string.Join(", ", RangedWeapons.Where(w => w.IsEquipped).Select(w => w.Name))} (Ranged)\n";
        str += $"Armors: {string.Join(", ", Armors.Where(a => a.IsEquipped).Select(a => a.Name))}\n";
        if (Vulnerabilities.Count > 0)
            str += $"Vulnerabilities: {string.Join(", ", Vulnerabilities)}\n";
        if (Resistances.Count > 0)
            str += $"Resistances: {string.Join(", ", Resistances)}\n";
        if (Immunities.Count > 0)
            str += $"Immunities: {string.Join(", ", Immunities)}\n";
        str += SpellSlots != null && SpellSlots.First > 0 ? SpellSlots.ToString() : "";
        if (Cantrips != null && Cantrips.Count > 0)
            str += $"Cantrips: {string.Join(", ", Cantrips.Select(c => c.Name))}\n";
        if (Spells != null && Spells.Count > 0)
            str += $"Spells: {string.Join(", ", Spells.Select(s => s.Name))}\n";
        str += "\n------------------------------------------------------------------------------------------------------------------------\n";
        return str;
    }
}