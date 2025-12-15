using System.Text.Json;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Generator;

public static class DataGenerator
{
    public static void Generate(Database db, DateTime startDate)
    {
        try
        {
            Logger.Instance.Information("Retrieving App Settings...");
            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("appsettings.json"));

            if (config == null)
                throw new InvalidOperationException("Configuration file is missing or invalid.");

            Logger.Instance.Information("App Settings successfully retrieved");

            Logger.Instance.Information($"Start generating {config.NumberOfEncountersToGenerate} encounters");
            for (var i = 1; i <= config.NumberOfEncountersToGenerate; i++)
            {
                Logger.Instance.Information($"Creating Encounter {i.ToString().PadLeft(8, '0')}...");
                var bucket = GetRandomDifficulty();
                var party = GetRandomParty(db);
                var monstersList = DataManipulation.GetMonstersDifficultiesList(db);
                var monsters = GetRandomMonsters(bucket, party.Select(x => x.Level).ToList(), monstersList);
                var encounter = new Encounter(i, bucket, party, monsters);
                var encounterWithOutcome = CalculateOutcome(encounter);

                SaveEncounter(encounterWithOutcome, startDate);
                Logger.Instance.Information($"Random difficulty selected: {encounterWithOutcome.Id}");
            }
            Logger.Instance.Information("Data generation completed.");
        }
        catch (InvalidOperationException ex)
        {
            Logger.Instance.Error(ex, ex.Message);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private static CRRatios GetRandomDifficulty()
    {
        var random = new Random();
        var values = Enum.GetValues(typeof(CRRatios));
        var selected = (CRRatios)(values.GetValue(random.Next(values.Length)) ?? CRRatios.Normal);

        Logger.Instance.Information($"Random difficulty selected: {selected}");

        return selected;
    }

    private static List<Member> GetRandomParty(Database db)
    {
        var random = new Random();
        var partyLevels = new List<byte>();
        var party = new List<Member>();
        var numMembers = random.Next(1, 8);
        var section = random.Next(1, 5);

        for (var i = 0; i < numMembers; i++)
            partyLevels.Add((byte)random.Next((5 * section) - 4, (5 * section) + 1));

        for (var i = 0; i < numMembers; i++)
            party.Add(new Member(i, partyLevels[i]));

        Logger.Instance.Information($"Generated {numMembers} party members of {string.Join(", ", partyLevels)} level. Levels were in sector {section}");

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

        Logger.Instance.Information($"Selecting monsters: ratio {ratio} - {randomNumMonsters} monsters - multiplier {numMultiplier}");

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

        Logger.Instance.Information($"Target XP before multiplier: {targetExpBeforeMultiplier}");

        var monstersFiltered = monsters.Where(m => m.Xp <= targetExpBeforeMultiplier).ToList();

        do
            randomMonsters = monstersFiltered
                .OrderBy(_ => random.Next())
                .Take(randomNumMonsters)
                .ToList();
        while (randomMonsters.Sum(m => m.Xp) > targetExpBeforeMultiplier && randomMonsters.Sum(m => m.Xp) > 0);

        Logger.Instance.Information($"Selected monsters XP sum: {randomMonsters.Sum(m => m.Xp)}");

        List<Monster> monstersSelected = randomMonsters.Select(item => new Monster(EntitiesFinder.GetEntityByIndex(Lists.monsters, new Entities.Mappers.BaseEntity(item.Index, item.Name)))).ToList();
        Logger.Instance.Information($"Selected monsters: {string.Join(", ", monstersSelected.Select(m => m.Name))}");

        return monstersSelected;
    }

    private static Encounter CalculateOutcome(Encounter encounter)
    {
        Logger.Instance.Information($"Calculating outcome for encounter {encounter.Id}");

        return encounter;
    }

    private static void SaveEncounter(Encounter encounter, DateTime startDate)
    {
        var encounterJson = JsonSerializer.Serialize(encounter, new JsonSerializerOptions { WriteIndented = true });
        var baseFolder = Directory.GetCurrentDirectory();
        var batchFolderName = Path.Combine(baseFolder, "..", "..", "..", $"Generator", $"output", $"Batch_{startDate:yyyyMMdd_HHmmss}");

        if (!Directory.Exists(batchFolderName))
        {
            Directory.CreateDirectory(batchFolderName);
            Logger.Instance.Information($"Created batch folder");
        }

        var fileName = $"{encounter.Id}.json";
        var filePath = Path.Combine(batchFolderName, fileName);

        File.WriteAllText(filePath, encounterJson);
        Logger.Instance.Information($"Encounter {encounter.Id} written to folder");
    }
}
