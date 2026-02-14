using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrainingDataGenerator.Analysis;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Exporters;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;
using TrainingDataGenerator.Utilities;
using TrainingDataGenerator.Validators;

namespace TrainingDataGenerator;

internal static class Program
{
    private static readonly DateTime StartTime = DateTime.Now;

    private static async Task Main()
    {
        // Build the DI container
        var host = CreateHostBuilder().Build();

        try
        {
            var startTimeString = StartTime.ToString("yyyyMMdd_HHmmss");
            var encounterDataset = new List<Encounter>();
            var logger = host.Services.GetRequiredService<ILogger>();

            var randomProvider = host.Services.GetRequiredService<IRandomProvider>();
            if (randomProvider is RandomProvider rp)
                logger.Information($"Using RandomSeed: {rp.GetSeed()}");

            logger.Verbose($"Starting data generation at {StartTime}");
            logger.Verbose("Initializing services...");

            // Initialize database
            logger.Verbose("Connecting to database...");
            var database = host.Services.GetRequiredService<Database>();

            if (database.GetInstance() == null)
                throw new InvalidOperationException("Database connection failed");

            logger.Verbose("Connection to database established");

            // Generate encounters
            var dataGenerator = host.Services.GetRequiredService<IDataGenerator>();
            await dataGenerator.GenerateAsync(database, encounterDataset, startTimeString);

            logger.Verbose($"Data generation completed at {DateTime.Now}");
            logger.Verbose($"Total encounters generated: {encounterDataset.Count}");
            logger.Verbose("Analyzing dataset...");

            // Validate encounters
            var validator = host.Services.GetRequiredService<IEncounterValidator>();
            await validator.ValidateDatasetAsync(encounterDataset, startTimeString);

            logger.Verbose("Dataset validation completed");
            logger.Information($"Total execution time: {DateTime.Now - StartTime}");
        }
        catch (InvalidOperationException ex)
        {
            var logger = host.Services.GetRequiredService<ILogger>();
            logger.Error(ex.Message);
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            var logger = host.Services.GetRequiredService<ILogger>();
            logger.Fatal(ex.Message);
            Environment.Exit(1);
        }
        finally
        {
            // Ensure logs are flushed
            if (host.Services.GetService<ILogger>() is Logger logger)
            {
                logger.Dispose();
            }
        }
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Core Infrastructure
                services.AddSingleton<ILogger>(Logger.Instance);
                services.AddSingleton<IRandomProvider>(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var seed = config.GetValue("RandomSeed", 0);
                    return new RandomProvider(seed);
                });
                services.AddSingleton<Database>();

                // Domain Services (Scoped for potential parallel processing)
                services.AddScoped<IAttributeService, AttributeService>();
                services.AddScoped<IEquipmentService, EquipmentService>();
                services.AddScoped<ISpellService, SpellService>();
                services.AddScoped<IFeatureService, FeatureService>();
                services.AddScoped<IProficiencyService, ProficiencyService>();
                services.AddScoped<ITraitService, TraitService>();
                services.AddScoped<IResistanceService, ResistanceService>();

                // Generators
                services.AddScoped<IDataGenerator, DataGeneratorService>();
                services.AddScoped<IPartyGenerator, PartyGeneratorService>();
                services.AddScoped<IMonsterGenerator, MonsterGeneratorService>();

                // Validators
                services.AddScoped<IEncounterValidator, EncounterValidator>();

                // Analyzers
                services.AddScoped<IDatasetAnalyzer, DatasetAnalyzer>();

                // Exporters
                services.AddScoped<IExporterService, ExporterService>();
            });
}