using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Generator;

public static class PartyGenerator
{
    public static List<PartyMember> GetRandomParty(Database db)
    {
        var random = Random.Shared;
        var partyLevels = new List<byte>();
        var party = new List<PartyMember>();
        var numMembers = random.Next(1, 8);
        var section = random.Next(1, 5);

        for (var i = 0; i < numMembers; i++)
            partyLevels.Add((byte)random.Next((5 * section) - 4, (5 * section) + 1));

        for (var i = 0; i < numMembers; i++)
            party.Add(new PartyMember(i, partyLevels[i], Lists.races.OrderBy(_ => random.Next()).First(), Lists.classes.OrderBy(_ => random.Next()).First()));

        Logger.Instance.Information($"Generated {numMembers} party members of level {string.Join(", ", partyLevels)}. Levels were in sector {section}");

        return party;
    }
}
