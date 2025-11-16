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
    public byte Initiative { get; set; }
    public byte ProficiencyBonus { get; set; }
    public List<BaseEntity> Proficiencies { get; set; }
    public List<Equipment> Equipments { get; set; }
    public List<Feature> Features { get; set; }
    public Dictionary<string, object>? ClassSpecific { get; set; }
    public Dictionary<string, object>? SubclassSpecific { get; set; }
    public List<BaseEntity> FeatureSpecifics { get; set; }
    public List<string> Masteries { get; set; }
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
        var randomSubrace = EntitiesFinder.GetEntityByIndex(Lists.subraces, new BaseEntity(randomRace.Index, randomRace.Name), randomRace.Subraces.OrderBy(_ => random.Next()).FirstOrDefault());
        var randomClass = Lists.classes.OrderBy(_ => random.Next()).First();
        var randomSubclass = EntitiesFinder.GetEntityByIndex(Lists.subclasses, new BaseEntity(randomClass.Index, randomClass.Name), randomClass.Subclasses.OrderBy(_ => random.Next()).First());
        
        var allEquipmentsBase = randomClass.StartingEquipments;
        allEquipmentsBase.AddRange((List<ClassMapper.Equipment>)randomClass.StartingEquipmentsOptions.Select(item => item.GetRandomEquipment()));

        var allEquipmentsMapper = allEquipmentsBase.Select(item => EntitiesFinder.GetEntityByIndex(Lists.equipments, item.Equip)).Where(item => item != null).ToList();
        var allArmors = allEquipmentsMapper.Where(item => item.EquipmentCategory.Index == "armor").Select(item => new Armor(item)).ToList();
        var allMeleeWeapons = allEquipmentsMapper.Where(item => item.EquipmentCategory.Index == "weapon" && item.WeaponRange == "Melee").Select(item => new MeleeWeapon(item)).ToList();
        var allRangedWeapons = allEquipmentsMapper.Where(item => item.EquipmentCategory.Index == "weapon" && item.WeaponRange == "Ranged").Select(item => new MeleeWeapon(item)).ToList();
        var allAmmunition = allEquipmentsMapper.Where(item => item.GearCategory?.Index == "ammunition").Select(item => new Ammunition(item)).ToList();
        
        allArmors.OrderBy(_ => random.Next()).First().IsEquipped = true;
        allAmmunition.ForEach(item => item.IsEquipped = true);
        EquipRandomWeapons(allMeleeWeapons, allRangedWeapons);

        var allTraits = randomRace.Traits;
        allTraits.AddRange(randomSubrace.RacialTraits);

        var raceTraits = allTraits.Select(item => EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(randomRace.Index, randomRace.Name), item)).Where(item => item.Parent == null).ToList();
        raceTraits.AddRange((List<TraitMapper>)allTraits.Select(item => EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(randomRace.Index, randomRace.Name), item).TraitSpec?.SubtraitOptions?.GetRandomChoice()));

        var levels = Lists.levels.Where(item => item.Class.Index == randomClass.Index && (item.Subclass == null || item.Subclass.Index == Subclass?.Index) && item.Level <= level).ToList();
        var features = Lists.features.Where(item => item.Class.Index == randomClass.Index && (item.Subclass == null || item.Subclass.Index == Subclass?.Index) && item.Level <= Level && item.Parent == null).ToList();

        LevelAdvancements(levels, features);

        // First assume random value for attributes based on Standard Array rule
        var attributes = AssumeAttributes(randomClass, random);

        // Base Information
        Id = id;
        Name = $"Member {id}";
        Level = level;
        HitDie = (byte)randomClass.Hp;
        ProficiencyBonus = (byte)(2 + ((Level - 1) / 4));
        Race = new BaseEntity(randomRace.Index, randomRace.Name);
        Subrace = randomSubrace != null ? new BaseEntity(randomSubrace.Index, randomSubrace.Name) : null;
        Traits = raceTraits.Select(item => new BaseEntity(item.Index, item.Name)).ToList();
        Class = new BaseEntity(randomClass.Index, randomClass.Name);
        Subclass = new BaseEntity(randomSubclass.Index, randomSubclass.Name);
        Strength = new Attribute(attributes[0]);
        Dexterity = new Attribute(attributes[1]);
        Constitution = new Attribute(attributes[2]);
        Intelligence = new Attribute(attributes[3]);
        Wisdom = new Attribute(attributes[4]);
        Charisma = new Attribute(attributes[5]);
        Initiative = Dexterity.Modifier;

        // Starting Proficiencies (Race & Class & Race Traits)
        Proficiencies = randomRace.StartingProficiences;
        Proficiencies.AddRange(randomClass.Proficiencies);
        Proficiencies.AddRange((List<BaseEntity>)raceTraits.Select(item => item.Proficiencies));

        // Random Proficiencies (Race, Class & Race Traits)
        Proficiencies.AddRange(randomRace.GetRandomProficiency(Proficiencies));
        Proficiencies.AddRange((List<BaseEntity>)randomClass.ProficiencyChoices.Select(item => item.GetRandomChoice(Proficiencies)));
        Proficiencies.AddRange((List<BaseEntity>)raceTraits.Select(item => item.ProficiencyChoice?.GetRandomChoice(Proficiencies)));

        // Add Proficiency to Saving Throws
        AddProfToSavings(randomClass);

        Hp = CalculateRandomHp(randomClass);

        //Check Proficiencies requirements are followed
        //Check Features Prerequisities (after Features and Spells are set)
    }

    private void LevelAdvancements(List<LevelMapper> levels, List<FeatureMapper> features)
    {
        for (int i = 1; i <= Level; i++)
        {
            List<LevelMapper> currentLevels = levels.Where(item => item.Level == i).ToList();

            foreach (var level in currentLevels)
            {
                var levelFeatures = features.Where(item => item.Level == i).ToList();
                Features.AddRange(levelFeatures.Select(item => new Feature(item)));
            }

            if (i == Level)
            {
                var abilityImprovements = currentLevels.OrderBy(item => item.AbilityScoreBonuses).First().AbilityScoreBonuses ?? 0;
                AbilityScoreImprovement(abilityImprovements);

                var spellcasting = currentLevels.Where(item => item.Spellcasting != null).First().Spellcasting;
                SpellSlots = new Slots(spellcasting);

                Spells = Lists.spells.Where(item => item.Classes.Contains(Class) && (item.Subclasses == null || item.Subclasses.Contains(Subclass)) && IsSpellKnown(item.Level, spellcasting))
                    .OrderBy(_ => new Random().Next())
                    .Take(spellcasting.SpellsKnown ?? 0)
                    .Select(item => new Spell(item))
                    .ToList();

                Cantrips = Lists.spells.Where(item => item.Classes.Contains(Class) && (item.Subclasses == null || item.Subclasses.Contains(Subclass)) && item.Level == 0)
                    .OrderBy(_ => new Random().Next())
                    .Take(spellcasting.CantripsKnown ?? 0)
                    .Select(item => new Spell(item))
                    .ToList();

                ClassSpecific = currentLevels.Where(item => item.ClassSpecific != null).Last().ClassSpecific;
                SubclassSpecific = currentLevels.Where(item => item.SubclassSpecific != null).Last().SubclassSpecific;
            }
        }
    }

    public bool IsSpellKnown(byte level, LevelMapper.SpellcastingInfo spellcasting)
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

    private void EquipRandomWeapons(List<MeleeWeapon> meleeWeapons, List<MeleeWeapon> rangedWeapons)
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
        for (int i = 2; i < Level; i++)
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

    private Dictionary<string, int> GetSkillsDict()
    {
        var skills = new Dictionary<string, int>
        {
            ["acrobatics"] = Dexterity.Modifier,
            ["animal_handling"] = Wisdom.Modifier,
            ["arcana"] = Intelligence.Modifier,
            ["athletics"] = Strength.Modifier,
            ["deception"] = Charisma.Modifier,
            ["history"] = Intelligence.Modifier,
            ["insight"] = Wisdom.Modifier,
            ["intimidation"] = Charisma.Modifier,
            ["investigation"] = Intelligence.Modifier,
            ["medicine"] = Wisdom.Modifier,
            ["nature"] = Intelligence.Modifier,
            ["perception"] = Wisdom.Modifier,
            ["performance"] = Charisma.Modifier,
            ["persuasion"] = Charisma.Modifier,
            ["religion"] = Intelligence.Modifier,
            ["sleight_of_hand"] = Dexterity.Modifier,
            ["stealth"] = Dexterity.Modifier,
            ["survival"] = Wisdom.Modifier
        };

        foreach (var item in skills.Keys.ToList())
        {
            if (Proficiencies.Contains("skill-" + item))
                skills[item] += ProficiencyBonus;
        }

        foreach (var item in skills.Keys.ToList())
        {
            if (Masteries.Contains("skill-" + item))
                skills[item] += ProficiencyBonus;
        }

        return skills;
    }

    public override string ToString()
    {
        var subraceStr = Subrace != null ? $" {Subrace.Name}" : "";
        var str = $"{Name} | {Race}{subraceStr} | Lv{Level} {Classe}\n";
        str += $"HP: {Hp} | Speed: {Speed} | Initiative: {Initiative} | Proficiency Bonus: +{ProficiencyBonus}\n";
        str += $"STR: {Strength.Value} ({Strength.Modifier}) | DEX: {Dexterity.Value} ({Dexterity.Modifier}) | CON: {Constitution.Value} ({Constitution.Modifier}) | INT: {Intelligence.Value} ({Intelligence.Modifier}) | WIS: {Wisdom.Value} ({Wisdom.Modifier}) | CHA: {Charisma.Value} ({Charisma.Modifier})\n";
        str += $"Saving Throws: STR {Strength.Save}, DEX {Dexterity.Save}, CON {Constitution.Save}, INT {Intelligence.Save}, WIS {Wisdom.Save}, CHA {Charisma.Save}\n";
        str += $"Skills: {string.Join(", ", Skills.Select(kv => $"{kv.Key.Replace("_", " ").ToUpper()} {kv.Value}"))}\n";
        str += $"Proficiencies: {string.Join(", ", Proficiencies)}\n";
        if (Masteries.Count > 0)
            str += $"Masteries: {string.Join(", ", Masteries)}\n";
        str += $"Traits: {(Subrace != null && Subrace.Traits != null ? string.Join(", ", Subrace.Traits) : "None")}\n";
        str += $"Features: {string.Join(", ", Features.Select(f => f.Name))}\n";
        str += $"Equipments: {string.Join(", ", Equipments.Select(e => $"{e.Quantity} x {e.Name.Replace("-", " ").ToUpper()}"))}\n";
        if (Vulnerabilities.Count > 0)
            str += $"Vulnerabilities: {string.Join(", ", Vulnerabilities)}\n";
        if (Resistances.Count > 0)
            str += $"Resistances: {string.Join(", ", Resistances)}\n";
        if (Immunities.Count > 0)
            str += $"Immunities: {string.Join(", ", Immunities)}\n";
        str += SpellSlots != null && SpellSlots.First > 0 ? SpellSlots.ToString() : "";
        str += "\n------------------------------------------------------------------------------------------------------------------------\n";
        return str;
    }
}