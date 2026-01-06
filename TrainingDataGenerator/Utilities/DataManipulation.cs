using TrainingDataGenerator.DataBase;

namespace TrainingDataGenerator.Utilities;

public static class DataManipulation
{
    public static List<MonsterDifficulty> GetMonstersDifficultiesList(Database db)
    {
        var filteredMonsters = new List<MonsterDifficulty>();

        foreach (var item in Lists.monsters)
        {
            filteredMonsters.Add(new MonsterDifficulty
            {
                Index = item.Index,
                Name = item.Name,
                ChallengeRating = item.ChallengeRating,
                Xp = item.Xp
            });
        }

        return filteredMonsters;
    }

    public static short CalculateHitPercentage(int armorClass, int attackBonus)
    {
        var hitPercentage = 0;

        for (var roll = 1; roll <= 20; roll++)
        {
            if (roll == 1)
                continue;

            if (roll == 20)
            {
                hitPercentage += 5;
                continue;
            }

            if (roll + attackBonus >= armorClass)
                hitPercentage += 5;
        }

        return (short)hitPercentage;
    }

    public static string ConvertAbilityIndex(string index)
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

    public static int GetSpellsPowerValue(string spell)
    {
        return 0;
    }
}