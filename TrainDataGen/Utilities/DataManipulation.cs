using MongoDB.Driver;
using TrainDataGen.DataBase;

namespace TrainDataGen.Utilities;

public static class DataManipulation
{
    public static List<MonsterDifficulty> GetMonstersDifficultiesList(Database db)
    {
        var allMonsters = db.GetAllMonsters().ToList();
        var filteredMonsters = new List<MonsterDifficulty>();

        foreach (var item in allMonsters)
        {
            filteredMonsters.Add(new MonsterDifficulty
            {
                Index = item.GetValue("index", "").AsString,
                Name = item.GetValue("name", "").AsString,
                ChallengeRating = item.GetValue("challenge_rating", 0).ToDouble(),
                Xp = item.GetValue("xp", 0).ToInt32()
            });
        }

        return filteredMonsters;
    }
}