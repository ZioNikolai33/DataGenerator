using System.Collections.Generic;
using TrainDataGen.Entities.Enums;
using TrainDataGen.Entities.Equip;
using TrainDataGen.Entities.Mappers;
using TrainDataGen.Utilities;

namespace TrainDataGen.Entities;

public class Member
{
    public int Id { get; set; }
    public string Name { get; set; }
    public byte Level { get; set; }
    public byte HitDie { get; set; }
    public short Hp { get; set; }
    public byte ArmorClass { get; set; }
    public BaseEntity Race { get; set; }
    public BaseEntity? Subrace { get; set; }
    public List<BaseEntity> Traits { get; set; }
    public BaseEntity Class { get; set; }
    public BaseEntity Subclass { get; set; }
    public Attribute Strength { get; set; }
    public Attribute Dexterity { get; set; }
    public Attribute Constitution { get; set; }
    public Attribute Intelligence { get; set; }
    public Attribute Wisdom { get; set; }
    public Attribute Charisma { get; set; }
    public sbyte Initiative { get; set; }
    public sbyte ProficiencyBonus { get; set; }
    public List<BaseEntity> Proficiencies { get; set; }
    public List<Equipment> Equipments { get; set; }
    public List<Feature> Features { get; set; }
    public Dictionary<string, object>? ClassSpecific { get; set; }
    public Dictionary<string, object>? SubclassSpecific { get; set; }
    public List<BaseEntity> FeatureSpecifics { get; set; }
    public List<string> Vulnerabilities { get; set; }
    public List<string> Resistances { get; set; }
    public List<string> Immunities { get; set; }
    public List<Skill> Skills { get; set; }
    public Slots SpellSlots { get; set; }
    public List<Spell> Cantrips { get; set; }
    public List<Spell> Spells { get; set; }

    public Member(int id, byte level)
    {
        var random = new Random();
        var randomRace = Lists.races.OrderBy(_ => random.Next()).First();
        var randomRaceAbilityBonus = randomRace.GetRandomAbility();
        var randomSubrace = (randomRace.Subraces.Count > 0) ? EntitiesFinder.GetEntityByIndex(Lists.subraces, new BaseEntity(randomRace.Index, randomRace.Name), randomRace.Subraces.OrderBy(_ => random.Next()).FirstOrDefault()) : null;
        var randomClass = Lists.classes.OrderBy(_ => random.Next()).First();
        var randomSubclass = EntitiesFinder.GetEntityByIndex(Lists.subclasses, new BaseEntity(randomClass.Index, randomClass.Name), randomClass.Subclasses.OrderBy(_ => random.Next()).First());
        var raceTraits = new List<TraitMapper>();

        var allEquipmentsBase = randomClass.StartingEquipments;
        var randomBaseEquipments = randomClass.StartingEquipmentsOptions.SelectMany(item => item.GetRandomEquipment()).ToList();
        allEquipmentsBase.AddRange(randomBaseEquipments);

        var allEquipmentsMapper = allEquipmentsBase.Select(item => EntitiesFinder.GetEntityByIndex(Lists.equipments, item.Equip)).Where(item => item != null).ToList();
        var allArmors = allEquipmentsMapper.Where(item => item.EquipmentCategory.Index == "armor").Select(item => new Armor(item)).ToList();
        var allMeleeWeapons = allEquipmentsMapper.Where(item => item.EquipmentCategory.Index == "weapon" && item.WeaponRange == "Melee").Select(item => new MeleeWeapon(item)).ToList();
        var allRangedWeapons = allEquipmentsMapper.Where(item => item.EquipmentCategory.Index == "weapon" && item.WeaponRange == "Ranged").Select(item => new RangedWeapon(item)).ToList();
        var allAmmunition = allEquipmentsMapper.Where(item => item.GearCategory?.Index == "ammunition").Select(item => new Ammunition(item)).ToList();

        allAmmunition.ForEach(item => item.IsEquipped = true);

        Features = new List<Feature>();
        FeatureSpecifics = new List<BaseEntity>();
        Equipments = new List<Equipment>();        
        Equipments.AddRange(allMeleeWeapons);
        Equipments.AddRange(allRangedWeapons);
        Equipments.AddRange(allAmmunition);

        EquipRandomWeapons(allMeleeWeapons, allRangedWeapons);

        if (randomRace.Traits.Count > 0)
        {
            var allTraits = randomRace.Traits;

            if (randomSubrace != null)
                allTraits.AddRange(randomSubrace.RacialTraits);

            raceTraits = allTraits.Select(item => EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(randomRace.Index, randomRace.Name), item)).Where(item => item != null && item.Parent == null).ToList();
            var raceSubtraits = new List<BaseEntity>();

            foreach (var item in raceTraits)
                if (item.TraitSpec == null || item.TraitSpec.SubtraitOptions == null)
                    continue;
                else
                    raceSubtraits = item.TraitSpec.SubtraitOptions.GetRandomChoice();

            if (raceSubtraits.Count > 0)
                raceTraits.AddRange(raceSubtraits
                    .Select(item => EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(randomRace.Index, randomRace.Name), item)));

            if (raceTraits.Count > 0)
                Traits = raceTraits.Select(item => new BaseEntity(item.Index, item.Name)).ToList();
        }        

        var levels = Lists.levels.Where(item => item.Class.Index == randomClass.Index || (item.Subclass == null || (item.Class.Index == randomClass.Index && item.Subclass.Index == Subclass?.Index)) && item.Level <= level).ToList();
        var features = Lists.features.Where(item => item.Class.Index == randomClass.Index || (item.Subclass == null || (item.Class.Index == randomClass.Index && item.Subclass.Index == Subclass?.Index)) && item.Level <= level && item.Parent == null).ToList();

        // First assume random value for attributes based on Standard Array rule
        var attributes = AssumeAttributes(randomClass, random);

        if (allArmors.Count > 0 && allArmors.Any(item => item.StrengthMinimum <= attributes[0]))
            allArmors.Where(item => item.StrengthMinimum <= attributes[0]).OrderBy(_ => random.Next()).First().IsEquipped = true;

        Equipments.AddRange(allArmors);

        // Base Information
        Id = id;
        Name = $"Member {id}";
        Level = level;
        HitDie = (byte)randomClass.Hp;
        ProficiencyBonus = (sbyte)(2 + ((Level - 1) / 4));
        Race = new BaseEntity(randomRace.Index, randomRace.Name);
        Subrace = randomSubrace != null ? new BaseEntity(randomSubrace.Index, randomSubrace.Name) : null;       
        Class = new BaseEntity(randomClass.Index, randomClass.Name);
        Subclass = new BaseEntity(randomSubclass.Index, randomSubclass.Name);
        Strength = new Attribute(attributes[0]);
        Dexterity = new Attribute(attributes[1]);
        Constitution = new Attribute(attributes[2]);
        Intelligence = new Attribute(attributes[3]);
        Wisdom = new Attribute(attributes[4]);
        Charisma = new Attribute(attributes[5]);
        ArmorClass = CalculateArmorClass(allArmors);
        Initiative = Dexterity.Modifier;
        Vulnerabilities = new List<string>();
        Resistances = new List<string>();
        Immunities = new List<string>();

        SetProficiencies(randomClass, randomRace, raceTraits);
        CreateSkills(); // Create Skills with basic Modifiers
        AddProfToSavings(randomClass); // Add Proficiency to Saving Throws
        AddProfToSkills(); // Add Proficiency to Skills based on Proficiencies list
        LevelAdvancements(levels, features); // Manage Level Advancements and related Features, Spells, ClassSpecific and SubclassSpecific

        ManageFeatureSpecific(); // Manage Feature Specifics (like Expertise)

        Hp = CalculateRandomHp(randomClass);
    }

    private byte CalculateArmorClass(List<Armor> armors)
    {
        var ac = 10 + Dexterity.Value;
        var equippedArmor = armors.Where(item => item.IsEquipped).FirstOrDefault();

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

        return (byte)ac;
    }

    private void LevelAdvancements(List<LevelMapper> levels, List<FeatureMapper> features)
    {
        for (int i = 1; i <= Level; i++)
        {
            List<LevelMapper> currentLevels = levels.Where(item => item.Class.Index == Class.Index && item.Level == i).ToList();

            foreach (var level in currentLevels)
            {
                var levelFeatures = features.Where(item => item.Level == i).ToList();

                if (levelFeatures.Count == 0)
                    continue;

                Features.AddRange(levelFeatures.Select(item => new Feature(item, Proficiencies)));
            }

            if (i == Level)
            {
                var abilityImprovements = currentLevels.OrderBy(item => item.AbilityScoreBonuses).First().AbilityScoreBonuses ?? 0;
                AbilityScoreImprovement(abilityImprovements);

                if (currentLevels.Any(item => item.Spellcasting != null))
                {
                    var spellcasting = currentLevels.First(item => item.Spellcasting != null).Spellcasting;
                    SpellSlots = new Slots(currentLevels.First(item => item.Spellcasting != null).Spellcasting);

                    switch (Class.Index)
                    {
                        case "cleric":
                            spellcasting.SpellsKnown = (byte?)((byte)Wisdom.Modifier + Level);
                            break;
                        case "druid":
                            spellcasting.SpellsKnown = (byte?)((byte)Wisdom.Modifier + Level);
                            break;
                        case "paladin":
                            spellcasting.SpellsKnown = (byte?)((byte)Charisma.Modifier + Math.Floor((double)Level/2));
                            break;
                        case "ranger":
                            spellcasting.SpellsKnown = (byte?)((byte)Wisdom.Modifier + Math.Floor((double)Level / 2));
                            break;
                        case "wizard":
                            spellcasting.SpellsKnown = (byte?)((byte)Intelligence.Modifier + Level);
                            break;
                    }

                    Spells = Lists.spells.Where(item => item.Classes.Any(x => x.Index == Class.Index) || (item.Subclasses == null || (item.Classes.Any(x => x.Index == Class.Index) && item.Subclasses.Any(x => x.Index == Subclass.Index))) && IsSpellKnown(item.Level, spellcasting))
                    .OrderBy(_ => new Random().Next())
                    .Take(spellcasting.SpellsKnown ?? 0)
                    .Select(item => new Spell(item))
                    .ToList();

                    Cantrips = Lists.spells.Where(item => item.Classes.Any(x => x.Index == Class.Index) || (item.Subclasses == null || (item.Classes.Any(x => x.Index == Class.Index) && item.Subclasses.Any(x => x.Index == Subclass.Index))) && item.Level == 0)
                        .OrderBy(_ => new Random().Next())
                        .Take(spellcasting.CantripsKnown ?? 0)
                        .Select(item => new Spell(item))
                        .ToList();
                }

                if (currentLevels.Any(item => item.ClassSpecific != null))
                    ClassSpecific = currentLevels.Where(item => item.ClassSpecific != null).Last().ClassSpecific;

                if (currentLevels.Any(item => item.SubclassSpecific != null))
                    SubclassSpecific = currentLevels.Where(item => item.SubclassSpecific != null).Last().SubclassSpecific;

                CheckFeaturesPrerequisities(features); //Check Features Prerequisities (after Features and Spells are set)
            }
        }
    }

    private void CreateSkills()
    {
        Skills = new List<Skill>
        {
             new Skill(new BaseEntity("skill-acrobatics", "Acrobatics"), Dexterity.Modifier),
             new Skill(new BaseEntity("skill-animal-handling", "Animal Handling"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-arcana", "Arcana"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-athletics", "Athletics"), Strength.Modifier),
             new Skill(new BaseEntity("skill-deception", "Deception"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-history", "History"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-insight", "Insight"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-intimidation", "Intimidation"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-investigation", "Investigation"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-medicine", "Medicine"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-nature", "Nature"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-perception", "Perception"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-performance", "Performance"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-persuasion", "Persuasion"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-religion", "Religion"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-sleight-of-hand", "Sleight of Hand"), Dexterity.Modifier),
             new Skill(new BaseEntity("skill-stealth", "Stealth"), Dexterity.Modifier),
             new Skill(new BaseEntity("skill-survival", "Survival"), Wisdom.Modifier)
        };
    }

    private void SetProficiencies(ClassMapper randomClass, RaceMapper randomRace, List<TraitMapper> raceTraits)
    {
        // Starting Proficiencies (Race & Class & Race Traits)
        Proficiencies = randomRace.StartingProficiences;
        Proficiencies.AddRange(randomClass.Proficiencies);

        if (raceTraits.Count > 0)
            foreach (var item in raceTraits)
                if (item.Proficiencies.Count > 0)
                    Proficiencies.AddRange(item.Proficiencies);

        // Random Proficiencies (Race, Class & Race Traits)
        Proficiencies.AddRange(randomRace.GetRandomProficiency(Proficiencies));

        if (randomClass.ProficiencyChoices.Count > 0)
            foreach (var item in randomClass.ProficiencyChoices)
                Proficiencies.AddRange(item.GetRandomChoice(Proficiencies));

        if (raceTraits.Count > 0)
            foreach (var item in raceTraits)
                if (item.ProficiencyChoice != null)
                    Proficiencies.AddRange(item.ProficiencyChoice.GetRandomChoice(Proficiencies));
    }

    private void ManageFeatureSpecific()
    {
        var newFeatures = new List<Feature>();

        foreach (var feature in Features)
        {
            if (feature.FeatureType != null && feature.FeatureType == FeatureSpecificTypes.Expertise)
            {
                foreach (var choice in feature.FeatureSpec)
                {
                    var skill = Skills.Where(item => item.Index == choice.Index).FirstOrDefault();

                    if (skill != null)
                        skill.SetExpertise(true, ProficiencyBonus);
                }
            }
            else if (feature.FeatureType != null && feature.FeatureType == FeatureSpecificTypes.Enemy)
                FeatureSpecifics.AddRange(feature.FeatureSpec);

            else if (feature.FeatureType != null && feature.FeatureType == FeatureSpecificTypes.Terrain)
                FeatureSpecifics.AddRange(feature.FeatureSpec);

            else if (feature.FeatureType != null && feature.FeatureType == FeatureSpecificTypes.Subfeature)
            {
                foreach (var choice in feature.FeatureSpec)
                {
                    var subfeature = Lists.features.Where(item => item.Index == choice.Index).FirstOrDefault();

                    if (subfeature != null)
                    {
                        newFeatures.Add(new Feature(subfeature, Proficiencies));
                    }
                }
            }
        }

        Features.AddRange(newFeatures);
    }

    private bool IsSpellKnown(byte level, LevelMapper.SpellcastingInfo spellcasting)
    {
        switch(level)
        {
            case 1:
                if (spellcasting.SpellSlotsLevel1 > 0)
                    return true;
                break;
            case 2:
                if (spellcasting.SpellSlotsLevel2 > 0)
                    return true;
                break;
            case 3:
                if (spellcasting.SpellSlotsLevel3 > 0)
                    return true;
                break;
            case 4:
                if (spellcasting.SpellSlotsLevel4 > 0)
                    return true;
                break;
            case 5:
                if (spellcasting.SpellSlotsLevel5 > 0)
                    return true;
                break;
            case 6:
                if (spellcasting.SpellSlotsLevel6 > 0)
                    return true;
                break;
            case 7:
                if (spellcasting.SpellSlotsLevel7 > 0)
                    return true;
                break;
            case 8:
                if (spellcasting.SpellSlotsLevel8 > 0)
                    return true;
                break;
            case 9:
                if (spellcasting.SpellSlotsLevel9 > 0)
                    return true;
                break;
        }

        return false;
    }

    private void AbilityScoreImprovement(byte numberImprovements)
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
                        Strength.AddValue(1);
                        break;
                    case 1:
                        Dexterity.AddValue(1);
                        break;
                    case 2:
                        Constitution.AddValue(1);
                        break;
                    case 3:
                        Intelligence.AddValue(1);
                        break;
                    case 4:
                        Wisdom.AddValue(1);
                        break;
                    case 5:
                        Charisma.AddValue(1);
                        break;
                }
            }
        }
    }

    private void EquipRandomWeapons(List<MeleeWeapon> meleeWeapons, List<RangedWeapon> rangedWeapons)
    {
        var random = new Random();

        if (meleeWeapons.Count > 0)
        {
            var meleeWeapon = meleeWeapons.OrderBy(_ => random.Next()).First();
            Equipments.Where(item => item.Index == meleeWeapon.Index).ToList().ForEach(item => item.IsEquipped = true);

            if (meleeWeapon.Properties.Contains(new BaseEntity("light", "light")))
            {
                var lightWeapon = meleeWeapons.Where(item => item.Properties.Contains(new BaseEntity("light", "light")) && item.Index != meleeWeapon.Index).OrderBy(_ => random.Next()).First();
                Equipments.Where(item => item.Index == lightWeapon.Index).ToList().ForEach(item => item.IsEquipped = true);
            }
            else if (meleeWeapon.Properties.Contains(new BaseEntity("two-handed", "Two-Handed")))
                if (Equipments.Any(item => item.Index == "shield"))
                    Equipments.Where(item => item.Index == "shield").ToList().ForEach(item => item.IsEquipped = false);
            else
                if (Equipments.Any(item => item.Index == "shield"))
                    Equipments.Where(item => item.Index == "shield").ToList().ForEach(item => item.IsEquipped = true);
        }

        if (rangedWeapons.Count > 0)
        {
            var rangedWeapon = rangedWeapons.OrderBy(_ => random.Next()).First();
            Equipments.Where(item => item.Index == rangedWeapon.Index).ToList().ForEach(item => item.IsEquipped = true);
        }
    }

    private List<Equipment> GetEquippedItems() => Equipments.Where(item => item.IsEquipped).ToList();

    private short CalculateRandomHp(ClassMapper classMapper)
    {
        var random = new Random();

        int hp = classMapper.Hp + Constitution.Modifier;
        for (int i = 2; i <= Level; i++)
            hp += random.Next(1, classMapper.Hp + 1) + Constitution.Modifier;

        return (short)hp;
    }

    private List<byte> AssumeAttributes(dynamic randomClass, Random random)
    {
        var attributes = new List<byte> { 15, 14, 13, 12, 10, 8 };

        return attributes.OrderBy(_ => random.Next()).ToList();
    }

    private void AddProfToSavings(ClassMapper cl)
    {
        if (cl.SavingThrows.Select(item => item.Index).Contains("str"))
            Strength.SetProficiency(true, ProficiencyBonus);
        if (cl.SavingThrows.Select(item => item.Index).Contains("dex"))
            Dexterity.SetProficiency(true, ProficiencyBonus);
        if (cl.SavingThrows.Select(item => item.Index).Contains("con"))
            Constitution.SetProficiency(true, ProficiencyBonus);
        if (cl.SavingThrows.Select(item => item.Index).Contains("int"))
            Intelligence.SetProficiency(true, ProficiencyBonus);
        if (cl.SavingThrows.Select(item => item.Index).Contains("wis"))
            Wisdom.SetProficiency(true, ProficiencyBonus);
        if (cl.SavingThrows.Select(item => item.Index).Contains("cha"))
            Charisma.SetProficiency(true, ProficiencyBonus);
    }

    private void AddProfToSkills()
    {
        foreach (var skill in Skills)
            if (Proficiencies.Select(item => item.Index).ToList().Contains(skill.Index))
                skill.SetProficiency(true, ProficiencyBonus);
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

    public override string ToString()
    {
        var subraceStr = Subrace != null ? $" {Subrace.Name}" : "";
        var str = $"{Name} | {Race.Name}{subraceStr} | Lv{Level} {Class.Name}\n";
        str += $"HP: {Hp} | Initiative: {Initiative} | Proficiency Bonus: +{ProficiencyBonus}\n";
        str += $"STR: {Strength.Value} ({Strength.Modifier}) | DEX: {Dexterity.Value} ({Dexterity.Modifier}) | CON: {Constitution.Value} ({Constitution.Modifier}) | INT: {Intelligence.Value} ({Intelligence.Modifier}) | WIS: {Wisdom.Value} ({Wisdom.Modifier}) | CHA: {Charisma.Value} ({Charisma.Modifier})\n";
        str += $"Saving Throws: STR {Strength.Save}, DEX {Dexterity.Save}, CON {Constitution.Save}, INT {Intelligence.Save}, WIS {Wisdom.Save}, CHA {Charisma.Save}\n";
        str += $"Skills: {string.Join(", ", Skills.Select(skill => $"{skill.Name} {skill.Modifier}"))}\n";
        str += $"Proficiencies: {string.Join(", ", Proficiencies.Select(p => p.Name))}\n";
        str += Traits != null ? $"Traits: {string.Join(", ", Traits.Select(t => t.Name))}\n": "Traits: None\n";
        str += $"Features: {string.Join(", ", Features.Select(f => f.Name))}\n";
        str += $"Equipments: {string.Join(", ", Equipments.Select(e => $"{e.Name.Replace("-", " ")}"))}\n";
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