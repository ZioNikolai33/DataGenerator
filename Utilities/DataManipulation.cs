using MongoDB.Driver;
using TrainDataGen.DataBase;

namespace TrainDataGen.Utilities;

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
}