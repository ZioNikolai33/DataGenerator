using System.Text.Json;
using TrainDataGen.DataBase;
using TrainDataGen.Entities;
using TrainDataGen.Utilities;

namespace TrainDataGen.Generator;

public static class DataGenerator
{
    public static void Generate(Database db, DateTime startDate)
    {
        for (var i = 1; i <= JsonSerializer.Deserialize<Config>(File.ReadAllText("appsettings.json")).NumberOfEncountersToGenerate; i++)
        {
            var bucket = GetRandomDifficulty();
            var party = GetRandomParty(db);
            var monstersList = DataManipulation.GetMonstersDifficultiesList(db);
            var monsters = GetRandomMonsters(bucket, party.Select(x => x.Level).ToList(), monstersList);
            var encounter = new Encounter(i, bucket, party, monsters);
            var encounterWithOutcome = CalculateOutcome(encounter);

            SaveEncounter(encounterWithOutcome, startDate);
        }        
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
        var partyLevels = new List<byte>();
        var party = new List<Member>();
        var numMembers = random.Next(1, 8); // Number of party members
        var section = random.Next(1, 5); // Section of levels to randomize levels organically - 1: 1-5; 2: 6-10; 3: 11-15; 4: 16-20

        for (var i = 0; i < numMembers; i++)
            partyLevels.Add((byte)random.Next((5 * section) - 4, (5 * section) + 1)); // Generate random levels for each member, related to section

        for (var i = 0; i < numMembers; i++)
            party.Add(new Member(i, partyLevels[i]));

        return party;
    }

    private static List<Monster> GetRandomMonsters(CRRatios ratio, List<byte> levels, List<MonsterDifficulty> monsters)
    {
        var random = new Random();
        var expThresholds = ExpOperations.CalculateDifficultiesExp(levels);
        var randomNumMonsters = random.Next(1, 16);
        var randomMonsters = new List<MonsterDifficulty>();
        var numMultiplier = ExpOperations.MultiplierList.FirstOrDefault(entry => entry.Number == randomNumMonsters).Number;
        var targetExpBeforeMultiplier = 0;

        switch(ratio)
        {
            case CRRatios.Easy:
                targetExpBeforeMultiplier = (int)Math.Floor((double)expThresholds.Easy / numMultiplier);
                break;
            case CRRatios.Normal:
                targetExpBeforeMultiplier = (int)Math.Floor((double)expThresholds.Medium / numMultiplier);
                break;
            case CRRatios.Hard:
                targetExpBeforeMultiplier = (int)Math.Floor((double)expThresholds.Hard / numMultiplier);
                break;
            case CRRatios.Deadly:
                targetExpBeforeMultiplier = (int)Math.Floor((double)expThresholds.Deadly / numMultiplier);
                break;
        }

        var monstersFiltered = monsters.Where(m => m.Xp <= targetExpBeforeMultiplier).ToList();

        do
            randomMonsters = monstersFiltered
                .OrderBy(_ => random.Next())
                .Take(randomNumMonsters)
                .ToList();
        while (randomMonsters.Sum(m => m.Xp) > targetExpBeforeMultiplier && randomMonsters.Sum(m => m.Xp) > 0);

        List<Monster> monstersSelected = randomMonsters.Select(item => new Monster(EntitiesFinder.GetEntityByIndex(Lists.monsters, new Entities.Mappers.BaseEntity(item.Index, item.Name)))).ToList();

        return monstersSelected;
    }

    private static Encounter CalculateOutcome(Encounter encounter)
    {
        // Algorithm to calculate outcome of encounter based on party and monsters stats
        return encounter;
    }

    private static void SaveEncounter(Encounter encounter, DateTime startDate)
    {
        var encounterJson = JsonSerializer.Serialize(encounter, new JsonSerializerOptions { WriteIndented = true });
        var baseFolder = Directory.GetCurrentDirectory();
        var batchFolderName = Path.Combine(baseFolder, "..", "..", "..", $"Generator", $"output", $"Batch_{startDate:yyyyMMdd_HHmmss}");

        if (!Directory.Exists(batchFolderName))
            Directory.CreateDirectory(batchFolderName);

        var fileName = $"{encounter.Id}.json";
        var filePath = Path.Combine(batchFolderName, fileName);

        File.WriteAllText(filePath, encounterJson);
    }
}
