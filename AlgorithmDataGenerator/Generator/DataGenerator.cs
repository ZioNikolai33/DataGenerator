using AlgorithmDataGenerator.Entities;
using System.IO;
using System.Text.Json;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Utilities;

namespace AlgorithmDataGenerator.Generator;

public static class DataGenerator
{
    public static void Generate(Database db, DateTime startDate)
    {
        var random = new Random();
        var characters = new List<Member>();

        try
        {
            Logger.Instance.Information("Retrieving App Settings...");
            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("appsettings.json"));

            if (config == null)
                throw new InvalidOperationException("Configuration file is missing or invalid.");

            Logger.Instance.Information("App Settings successfully retrieved");

            Logger.Instance.Information($"Start generating {config.NumberOfCycles} party members\n");
            for (var i = 1; i <= config.NumberOfCycles; i++)
            {
                var level = (byte)random.Next(1, 21);
                characters.Add(new Member(i, level, Lists.races.OrderBy(_ => random.Next()).First(), Lists.classes.OrderBy(_ => random.Next()).First()));
            }
            Logger.Instance.Information("Data generation completed.");

            CalculateBaseStats(characters);
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

    private static void CalculateBaseStats(List<Member> members)
    {
        Logger.Instance.Information($"Calculating Base Stats for {members.Count} characters");

        var random = new Random();
        var characters = new List<Character>();

        characters.AddRange(members.Select(item => new Character(item, item.GetTotalBaseStats())));
        characters = characters.OrderBy(c => c.BaseStats).ToList();

        Logger.Instance.Information($"{string.Join("\n", characters)}");
        Logger.Instance.Information("Base Stats calculation completed.\n");

        SaveCharacters(characters, DateTime.Now);
    }

    private static void SaveCharacters(List<Character> characters, DateTime startDate)
    {
        var charactersJson = JsonSerializer.Serialize(characters, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        var baseFolder = Directory.GetCurrentDirectory();
        var batchFolderName = Path.Combine(baseFolder, "..", "..", "..", $"Generator", $"output");

        if (!Directory.Exists(batchFolderName))
        {
            Directory.CreateDirectory(batchFolderName);
            Logger.Instance.Information($"Created output folder");
        }

        var fileName = $"characters_{startDate:yyyyMMdd_HHmmss}.json";
        var filePath = Path.Combine(batchFolderName, fileName);

        File.WriteAllText(filePath, charactersJson);
        Logger.Instance.Information($"Characters written to folder\n");
    }
}
