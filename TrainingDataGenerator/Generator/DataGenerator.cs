using System.Text.Json;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Generator;

public static class DataGenerator
{
    public static async Task GenerateAsync(Database db, DateTime startDate)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        try
        {
            Logger.Instance.Verbose("Retrieving App Settings...");
            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("appsettings.json"));

            if (config == null)
                throw new InvalidOperationException("Configuration file is missing or invalid.");

            Logger.Instance.Verbose("App Settings successfully retrieved");

            Logger.Instance.Verbose($"Start generating {config.NumberOfCycles} encounters\n");
            for (var i = 1; i <= config.NumberOfCycles; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Logger.Instance.Information($"Creating Encounter {i.ToString().PadLeft(8, '0')}...");
                var bucket = GetRandomDifficulty();
                var party = PartyGenerator.GetRandomParty(db);
                var monstersList = DataManipulation.GetMonstersDifficultiesList(db);
                var monsters = MonsterGenerator.GetRandomMonsters(bucket, party.Select(x => x.Level).ToList(), monstersList);
                var encounter = new Encounter(i, bucket, party, monsters);
                var encounterWithOutcome = OutcomeCalculator.CalculateOutcome(encounter);
                await SaveEncounter(encounterWithOutcome, startDate);
            }
            Logger.Instance.Information("Data generation completed.");
        }
        catch (InvalidOperationException ex)
        {
            Logger.Instance.Error(ex, ex.Message);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static CRRatios GetRandomDifficulty()
    {
        var random = Random.Shared;
        var values = Enum.GetValues(typeof(CRRatios));
        var selected = (CRRatios)(values.GetValue(random.Next(values.Length)) ?? CRRatios.Normal);

        Logger.Instance.Information($"Random difficulty selected: {selected}");

        return selected;
    }

    private static async Task SaveEncounter(Encounter encounter, DateTime startDate)
    {
        var encounterJson = JsonSerializer.Serialize(encounter, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        var baseFolder = Directory.GetCurrentDirectory();
        var batchFolderName = Path.Combine(baseFolder, "..", "..", "..", $"Generator", $"output", $"Batch_{startDate:yyyyMMdd_HHmmss}");

        if (!Directory.Exists(batchFolderName))
        {
            Directory.CreateDirectory(batchFolderName);
            Logger.Instance.Verbose($"Created batch folder");
        }

        var fileName = $"{encounter.Id}.json";
        var filePath = Path.Combine(batchFolderName, fileName);

        await File.WriteAllTextAsync(filePath, encounterJson);
        Logger.Instance.Information($"Encounter {encounter.Id} written to folder\n");
    }
}
