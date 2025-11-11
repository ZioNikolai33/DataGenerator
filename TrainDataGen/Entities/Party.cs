using TrainDataGen.Utilities;

namespace TrainDataGen.Entities;

public class Member
{
    public int Id { get; set; }
    public string Name { get; set; }
    public byte Level { get; set; }
    public Race Race { get; set; }
    public Subrace Subrace { get; set; }
    public short Speed { get; set; }
    public Class Class { get; set; }
    public Attribute Strength { get; set; }
    public Attribute Dexterity { get; set; }
    public Attribute Constitution { get; set; }
    public Attribute Intelligence { get; set; }
    public Attribute Wisdom { get; set; }
    public Attribute Charisma { get; set; }
    public short Hp { get; set; }
    public string Subclass { get; set; }
    public byte Initiative { get; set; }
    public byte ProficiencyBonus { get; set; }
    public List<string> Proficiencies { get; set; }
    public List<Equipment> Equipments { get; set; }
    public List<Feature> Features { get; set; }
    public List<string> FeatureSpecifics { get; set; }
    public List<string> Masteries { get; set; }
    public List<string> Vulnerabilities { get; set; }
    public List<string> Resistances { get; set; }
    public List<string> Immunities { get; set; }
    public Skills Skills { get; set; }
    public Slots SpellSlots { get; set; }
    public List<Spell> Spells { get; set; }

    public Member(int id, byte level)
    {
        var random = new Random();
        var randomRace = Lists.races.OrderBy(_ => random.Next()).First();
        var randomClass = Lists.classes.OrderBy(_ => random.Next()).First();
        var attributes = AddAbilityScores(randomRace, AssumeAttributes(randomClass, random));

        Id = id;
        Name = $"Member {id}";
        Level = level;
        Race = new Race(randomRace);
        Subrace = (randomRace.Subraces.Count > 0) ? randomRace.Subraces[random.Next(randomRace.Subraces.Count)] : null;
        Speed = randomRace.Speed;
        Class = randomClass.Name;
        Strength = new Attribute(attributes[0]);
        Dexterity = new Attribute(attributes[1]);
        Constitution = new Attribute(attributes[2]);
        Intelligence = new Attribute(attributes[3]);
        Wisdom = new Attribute(attributes[4]);
        Charisma = new Attribute(attributes[5]);
        Hp = CalculateRandomHp(randomClass.Hp, random);
        Subclass = randomClass.Subclasses[random.Next(randomClass.Subclasses.Count)];
        Initiative = Dexterity.Modifier;
        ProficiencyBonus = GetProfBonus();
        Proficiencies = new List<string>(randomClass.Proficiencies);
        Proficiencies.AddRange(randomRace.Proficiencies);

        foreach (var item in randomClass.ProficiencyChoices)
        {
            var chosen = item.GetRandomChoice(Proficiencies);
            Proficiencies.AddRange(chosen);
        }

        Equipments = new List<Equipment>(randomClass.StartingEquipments);
        if (randomClass.StartingEquipmentsOptions.Count > 0)
        {
            foreach (var item in randomClass.StartingEquipmentsOptions)
                Equipments.Add(item);
        }

        Features = randomClass.Features.Where(item => item.Level == Level).ToList();

        FeatureSpecifics = new List<string>();
        Masteries = new List<string>();
        Vulnerabilities = new List<string>();
        Resistances = new List<string>();
        Immunities = new List<string>();

        foreach (var item in randomClass.ProficiencyChoices)
        {
            var sampled = GetRandomSample(item.Choices, item.Number, random);
            Proficiencies.AddRange(sampled);
        }

        SetFeatureSpecifics(random, Features);
        AddProfToSavings(randomClass);
        Skills = GetSkillsDict();
        SpellSlots = randomClass.GetSpellSlots(Level);
        Spells = randomClass.Spells.Where(item => SpellSlots.HasEnoughSlots(item.Level, 0)).ToList();
    }

    private int CalculateRandomHp(int hitDie, Random random)
    {
        int hp = hitDie + Constitution.Modifier;
        for (int i = 2; i < Level; i++)
            hp += random.Next(1, hitDie + 1) + Constitution.Modifier;
        return hp;
    }

    private List<int> AssumeAttributes(dynamic randomClass, Random random)
    {
        var attributes = new List<int> { 15, 14, 13, 12, 10, 8 };
        attributes = attributes.OrderBy(_ => random.Next()).ToList();
        return attributes;
    }

    private int GetProfBonus()
    {
        return 2 + ((Level - 1) / 4);
    }

    private void AddProfToSavings(dynamic classe)
    {
        Strength.Save += classe.SavingThrows.Contains("str") ? ProficiencyBonus : 0;
        Dexterity.Save += classe.SavingThrows.Contains("dex") ? ProficiencyBonus : 0;
        Constitution.Save += classe.SavingThrows.Contains("con") ? ProficiencyBonus : 0;
        Intelligence.Save += classe.SavingThrows.Contains("int") ? ProficiencyBonus : 0;
        Wisdom.Save += classe.SavingThrows.Contains("wis") ? ProficiencyBonus : 0;
        Charisma.Save += classe.SavingThrows.Contains("cha") ? ProficiencyBonus : 0;
    }

    private List<int> AddAbilityScores(dynamic race, List<int> attributes)
    {
        foreach (var item in race.AbilityBonuses)
        {
            switch (item.Name)
            {
                case "str": attributes[0] += item.Bonus; break;
                case "dex": attributes[1] += item.Bonus; break;
                case "con": attributes[2] += item.Bonus; break;
                case "int": attributes[3] += item.Bonus; break;
                case "wis": attributes[4] += item.Bonus; break;
                case "cha": attributes[5] += item.Bonus; break;
            }
        }
        return attributes;
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

    private void SetFeatureSpecifics(Random random, List<Feature> features)
    {
        foreach (var feature in features)
        {
            foreach (var item in feature.FeatureSpecificChoices)
            {
                var choices = new List<string>(item.Choices);
                switch (feature.FeatureSpecificType)
                {
                    case "subfeature_options":
                    case "enemy_type_options":
                    case "terrain_type_options":
                    case "invocations":
                        choices = choices.Except(FeatureSpecifics).ToList();
                        FeatureSpecifics.AddRange(GetRandomSample(choices, item.Number, random));
                        break;
                    case "expertise":
                        choices = choices.Except(Masteries).ToList();
                        Masteries.AddRange(GetRandomSample(choices, item.Number, random));
                        break;
                }
            }
        }
    }

    private List<string> GetRandomSample(List<string> source, int count, Random random)
    {
        return source.OrderBy(_ => random.Next()).Take(count).ToList();
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