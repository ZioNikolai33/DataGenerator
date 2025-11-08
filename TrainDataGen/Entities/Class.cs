using MongoDB.Bson.Serialization.Attributes;
using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities;

public class Class
{
    public string Name { get; set; }
    public short Hp { get; set; }
    public List<BaseMapper> Subclasses { get; set; }
    public List<BaseMapper> SavingThrows { get; set; }
    public List<BaseMapper> Proficiencies { get; set; }
    public List<Feature> Features { get; set; }
    public List<Equipment> Equipment { get; set; }
    public Multiclass Multiclassing { get; set; }
    public string? SpellcastingAbility { get; set; }
    public List<Spell> Spells { get; set; }

    public Class (ClassMapper classMapper, List<Feature> features, List<Spell> spells)
    {
        Name = classMapper.Name;
        Hp = classMapper.Hp;
        Subclasses = classMapper.Subclasses;
        SavingThrows = classMapper.SavingThrows;
        Proficiencies = classMapper.Proficiencies;
        Equipment = classMapper.StartingEquipments;
        StartingEquipmentsOptions = classMapper.StartingEquipmentsOptions;
        Multiclassing = classMapper.Multiclassing;
        SpellcastingAbility = classMapper.SpellcastingAbility;
        Features = features;
        Spells = spells;

        classMapper.ProficiencyChoices.ForEach(choice => Proficiencies.AddRange(choice.GetRandomChoice(Proficiencies)));
    }

    public List<Equipment> GetEquipmentOptions(
            List<dynamic> martialMeleeWeapons,
            List<dynamic> simpleWeapons,
            List<dynamic> simpleMeleeWeapons,
            List<dynamic> martialWeapons)
    {
        var equipmentOptions = new List<Equipment>();
        var choices = new List<Choice>();
        var random = new Random();

        if (Name == "barbarian")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("greataxe", 1));
            }
            else
            {
                var listEquip = martialMeleeWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }

            selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("handaxe", 2));
            }
            else
            {
                var listEquip = simpleWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }
        }

        if (Name == "bard")
        {
            int selection = random.Next(3);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("rapier", 1));
            }
            else if (selection == 1)
            {
                equipmentOptions.Add(new Equipment("longsword", 1));
            }
            else
            {
                var listEquip = simpleWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }
        }

        if (Name == "cleric")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("mace", 1));
            }
            else
            {
                equipmentOptions.Add(new Equipment("warhammer", 1));
            }

            selection = random.Next(3);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("scale-mail", 1));
            }
            else if (selection == 1)
            {
                equipmentOptions.Add(new Equipment("leather-armor", 1));
            }
            else
            {
                equipmentOptions.Add(new Equipment("chain-mail", 1));
            }

            selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("light-crossbow", 1));
                equipmentOptions.Add(new Equipment("crossbow-bolt", 20));
            }
            else
            {
                var listEquip = simpleWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }
        }

        if (Name == "druid")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("wooden-shield", 1));
            }
            else
            {
                var listEquip = simpleWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }

            selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("scimitar", 1));
            }
            else
            {
                var listEquip = simpleMeleeWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }
        }

        if (Name == "fighter")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("chain-mail", 1));
            }
            else
            {
                equipmentOptions.Add(new Equipment("leather-armor", 1));
                equipmentOptions.Add(new Equipment("longbow", 1));
                equipmentOptions.Add(new Equipment("arrow", 20));
            }

            selection = random.Next(2);
            if (selection == 0)
            {
                var listEquip = martialWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
                equipmentOptions.Add(new Equipment("shield", 1));
            }
            else
            {
                var listEquip = martialWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(2, listEquip));
            }

            selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("light-crossbow", 1));
                equipmentOptions.Add(new Equipment("crossbow-bolt", 20));
            }
            else
            {
                equipmentOptions.Add(new Equipment("handaxe", 2));
            }
        }

        if (Name == "monk")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("shortsword", 1));
            }
            else
            {
                var listEquip = simpleWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }
        }

        if (Name == "paladin")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                var listEquip = martialWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
                equipmentOptions.Add(new Equipment("shield", 1));
            }
            else
            {
                var listEquip = martialWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(2, listEquip));
            }

            selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("javelin", 5));
            }
            else
            {
                var listEquip = simpleMeleeWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }
        }

        if (Name == "ranger")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("scale-mail", 1));
            }
            else
            {
                equipmentOptions.Add(new Equipment("leather-armor", 1));
            }

            selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("shortsword", 2));
            }
            else
            {
                var listEquip = simpleMeleeWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(2, listEquip));
            }
        }

        if (Name == "rogue")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("rapier", 1));
            }
            else
            {
                equipmentOptions.Add(new Equipment("shortsword", 1));
            }

            selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("shortbow", 1));
                equipmentOptions.Add(new Equipment("arrow", 20));
            }
            else
            {
                equipmentOptions.Add(new Equipment("shortsword", 1));
            }
        }

        if (Name == "sorcerer")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("light-crossbow", 1));
                equipmentOptions.Add(new Equipment("crossbow-bolt", 20));
            }
            else
            {
                var listEquip = simpleWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }
        }

        if (Name == "warlock")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("light-crossbow", 1));
                equipmentOptions.Add(new Equipment("crossbow-bolt", 20));
            }
            else
            {
                var listEquip = simpleWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
                choices.Add(new Choice(1, listEquip));
            }

            var listEquip2 = simpleWeapons.Select(item => new Equipment((string)item.index, 1)).ToList();
            choices.Add(new Choice(1, listEquip2));
        }

        if (Name == "wizard")
        {
            int selection = random.Next(2);
            if (selection == 0)
            {
                equipmentOptions.Add(new Equipment("quarterstaff", 1));
            }
            else
            {
                equipmentOptions.Add(new Equipment("dagger", 1));
            }
        }

        // Add choices to equipmentOptions
        if (choices.Count > 0)
        {
            foreach (var choice in choices)
            {
                equipmentOptions.AddRange(choice.GetRandomChoiceWithoutCheck());
            }
        }

        return equipmentOptions;
    }

    public Slots GetSpellSlots(int level)
    {
        var spellSlots = new Slots();

        if (new[] { "bard", "cleric", "druid", "sorcerer", "wizard" }.Contains(Name))
        {
            spellSlots = level switch
            {
                1 => new Slots(2),
                2 => new Slots(3),
                3 => new Slots(4, 2),
                4 => new Slots(4, 3),
                5 => new Slots(4, 3, 2),
                6 => new Slots(4, 3, 3),
                7 => new Slots(4, 3, 3, 1),
                8 => new Slots(4, 3, 3, 2),
                9 => new Slots(4, 3, 3, 3, 1),
                10 => new Slots(4, 3, 3, 3, 2),
                11 or 12 => new Slots(4, 3, 3, 3, 2, 1),
                13 or 14 => new Slots(4, 3, 3, 3, 2, 1, 1),
                15 or 16 => new Slots(4, 3, 3, 3, 2, 1, 1, 1),
                17 => new Slots(4, 3, 3, 3, 2, 1, 1, 1, 1),
                18 => new Slots(4, 3, 3, 3, 3, 1, 1, 1, 1),
                19 => new Slots(4, 3, 3, 3, 3, 2, 1, 1, 1),
                20 => new Slots(4, 3, 3, 3, 3, 2, 2, 1, 1),
                _ => new Slots()
            };
        }
        else if (new[] { "paladin", "ranger" }.Contains(Name))
        {
            spellSlots = level switch
            {
                2 => new Slots(2),
                3 or 4 => new Slots(3),
                5 or 6 => new Slots(4, 2),
                7 or 8 => new Slots(4, 3),
                9 or 10 => new Slots(4, 3, 2),
                11 or 12 => new Slots(4, 3, 3),
                13 or 14 => new Slots(4, 3, 3, 1),
                15 or 16 => new Slots(4, 3, 3, 2),
                17 or 18 => new Slots(4, 3, 3, 3, 1),
                19 or 20 => new Slots(4, 3, 3, 3, 2),
                _ => new Slots()
            };
        }

        return spellSlots;
    }
}