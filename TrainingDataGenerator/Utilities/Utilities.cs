using TrainingDataGenerator.Abstracts;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.PartyEntities;

namespace TrainingDataGenerator.Utilities;

public static class UtilityMethods
{
    public static string ConvertAbilityIndexToFullName(string index)
    {
        switch (index)
        {
            case "str":
                return "strength";
            case "dex":
                return "dexterity";
            case "con":
                return "constitution";
            case "int":
                return "intelligence";
            case "wis":
                return "wisdom";
            case "cha":
                return "charisma";
        }

        return string.Empty;
    }

    public static short GetSpellcastingModifier(PartyMember member)
    {
        return GetSpellcastingModifierFromFullName(member.SpellcastingAbility, member);
    }

    public static short GetSpellcastingModifier(Monster monster)
    {
        var ability = string.Empty;
        var spellcastingAbility = monster.SpecialAbilities
            .FirstOrDefault(item => item.Name.Equals("Spellcasting") && item.Spellcast != null && item.Spellcast.Ability != null);

        if (spellcastingAbility?.Spellcast?.Ability?.Index != null)
            ability = ConvertAbilityIndexToFullName(spellcastingAbility.Spellcast.Ability.Index);

        return GetSpellcastingModifierFromFullName(ability, monster);
    }

    private static short GetSpellcastingModifierFromFullName<T>(string ability, T creature) where T : Creature
    {
        return ability switch
        {
            "strength" => creature.Strength.Modifier,
            "dexterity" => creature.Dexterity.Modifier,
            "constitution" => creature.Constitution.Modifier,
            "intelligence" => creature.Intelligence.Modifier,
            "wisdom" => creature.Wisdom.Modifier,
            "charisma" => creature.Charisma.Modifier,
            _ => 0,
        };
    }

    public static int GetDiceValue(string dice, PartyMember member)
    {
        int value = 0;

        if (!dice.Contains("d"))
            return int.Parse(dice.Trim()) / 2;

        var diceParts = dice.Trim().Split("d");

        if (dice.Trim().Contains("+ MOD"))
            value = ((int.Parse(diceParts[0]) * int.Parse(diceParts[1].Split("+")[0])) / 2) + GetSpellcastingModifierFromFullName(member.SpellcastingAbility, member);
        else if (dice.Contains("+"))
            value = ((int.Parse(diceParts[0]) * int.Parse(diceParts[1].Split("+")[0])) / 2) + int.Parse(diceParts[1].Split("+")[0]);
        else if (dice.Trim().Contains("-"))
            value = ((int.Parse(diceParts[0]) * int.Parse(diceParts[1].Split("-")[0])) / 2) - int.Parse(diceParts[1].Split("-")[0]);
        else
            value = (int.Parse(diceParts[0]) * int.Parse(diceParts[1])) / 2;

        return value;
    }

    public static int GetDiceValue(string dice, Monster monster)
    {
        int value = 0;

        if (!dice.Contains("d"))
            return int.Parse(dice.Trim());

        var diceParts = dice.Trim().Split("d");

        if (dice.Trim().Contains("+ MOD"))
            value = ((int.Parse(diceParts[0]) * int.Parse(diceParts[1].Split("+")[0])) / 2) + GetSpellcastingModifier(monster);
        else if (dice.Contains("+"))
            value = ((int.Parse(diceParts[0]) * int.Parse(diceParts[1].Split("+")[0])) / 2) + int.Parse(diceParts[1].Split("+")[0]);
        else if (dice.Trim().Contains("-"))
            value = ((int.Parse(diceParts[0]) * int.Parse(diceParts[1].Split("-")[0])) / 2) - int.Parse(diceParts[1].Split("-")[0]);
        else
            value = (int.Parse(diceParts[0]) * int.Parse(diceParts[1])) / 2;

        return value;
    }

    public static int GetDiceValue(string dice)
    {
        int value = 0;

        if (!dice.Contains("d"))
            return int.Parse(dice.Trim());

        var diceParts = dice.Trim().Split("d");

        if (dice.Contains("+"))
            value = ((int.Parse(diceParts[0]) * int.Parse(diceParts[1].Split("+")[0])) + int.Parse(diceParts[1].Split("+")[0]));
        else if (dice.Trim().Contains("-"))
            value = ((int.Parse(diceParts[0]) * int.Parse(diceParts[1].Split("-")[0])) - int.Parse(diceParts[1].Split("-")[0]));
        else
            value = (int.Parse(diceParts[0]) * int.Parse(diceParts[1]));

        return value;
    }
}
