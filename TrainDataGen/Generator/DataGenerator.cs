using TrainDataGen.DataBase;
using TrainDataGen.Entities;
using TrainDataGen.Utilities;

namespace TrainDataGen.Generator;

public static class DataGenerator
{
    public static void Generate(Database db)
    {
        var bucket = GetRandomDifficulty();
        var party = GetRandomParty(db);
        var monstersList = DataManipulation.GetMonstersDifficultiesList(db);
    }

    private static CRRatios GetRandomDifficulty()
    {
        var random = new Random();
        var values = Enum.GetValues(typeof(CRRatios));
        
        return (CRRatios)(values.GetValue(random.Next(values.Length)) ?? CRRatios.Normal);
    }
    private static List<Member> GetRandomParty(Database db)
    {
        var random = new Random();
        var partyLevels = new List<int>();
        var party = new List<Member>();
        var numMembers = random.Next(1, 8); // Number of party members
        var section = random.Next(1, 5); // Section of levels to randomize levels organically - 1: 1-5; 2: 6-10; 3: 11-15; 4: 16-20

        for (var i = 0; i < numMembers; i++)
            partyLevels.Add(random.Next((5 * section) - 4, (5 * section) + 1)); // Generate random levels for each member, related to section

        for (var i = 0; i < numMembers; i++)
            party.Add(new Member(i, partyLevels[i]));

        foreach (var item in party)
            Console.WriteLine(item.ToString());

        return party;
    }
}
