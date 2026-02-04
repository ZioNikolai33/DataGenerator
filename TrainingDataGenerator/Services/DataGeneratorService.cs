using System.Text.Json;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Generator;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Services;

public class DataGeneratorService : IDataGenerator
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;
    private readonly IPartyGenerator _partyGenerator;
    private readonly IMonsterGenerator _monsterGenerator;
    private readonly Config _config;

    public DataGeneratorService(
        ILogger logger,
        IRandomProvider random,
        IPartyGenerator partyGenerator,
        IMonsterGenerator monsterGenerator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));
        _partyGenerator = partyGenerator ?? throw new ArgumentNullException(nameof(partyGenerator));
        _monsterGenerator = monsterGenerator ?? throw new ArgumentNullException(nameof(monsterGenerator));
        _config = LoadConfig();
    }

    public async Task GenerateAsync(Database database, DateTime startDate)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        try
        {
            _logger.Information($"Starting generation of {_config.NumberOfCycles} encounters\n");

            for (var i = 1; i <= _config.NumberOfCycles; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                _logger.Information($"Creating Encounter {i:D8}...");

                var difficulty = GetRandomDifficulty();
                var party = _partyGenerator.GenerateRandomParty(database);
                var monstersList = DataManipulation.GetMonstersDifficultiesList(database);
                var monsters = _monsterGenerator.GenerateRandomMonsters(
                    difficulty,
                    party.Select(p => p.Level).ToList(), 
                    monstersList);

                var encounter = new Encounter(i, difficulty, party, monsters);
                var encounterWithOutcome = OutcomeCalculator.CalculateOutcome(encounter, _logger);

                await SaveEncounterAsync(encounterWithOutcome, startDate);
            }

            _logger.Information("Data generation completed successfully.");
        }
        catch (OperationCanceledException)
        {
            _logger.Warning("Data generation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw;
        }
    }

    private CRRatios GetRandomDifficulty()
    {
        var values = Enum.GetValues<CRRatios>();
        var selected = _random.SelectRandom(values.ToList());

        _logger.Information($"Random difficulty selected: {selected}");
        return selected;
    }

    private async Task SaveEncounterAsync(Encounter encounter, DateTime startDate)
    {
        var encounterJson = JsonSerializer.Serialize(
            encounter, 
            new JsonSerializerOptions 
            { 
                WriteIndented = true, 
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
            });

        var baseFolder = Directory.GetCurrentDirectory();
        var batchFolderName = Path.Combine(
            baseFolder, 
            "..", "..", "..", 
            "Generator", 
            "output", 
            $"Batch_{startDate:yyyyMMdd_HHmmss}");

        if (!Directory.Exists(batchFolderName))
        {
            Directory.CreateDirectory(batchFolderName);
            _logger.Verbose("Created batch folder");
        }

        var fileName = $"{encounter.Id}.json";
        var filePath = Path.Combine(batchFolderName, fileName);

        await File.WriteAllTextAsync(filePath, encounterJson);
        _logger.Information($"Encounter {encounter.Id} saved to {fileName}\n");
    }

    private static Config LoadConfig()
    {
        try
        {
            var configText = File.ReadAllText("appsettings.json");
            return JsonSerializer.Deserialize<Config>(configText) ?? new Config();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            return new Config();
        }
    }
}