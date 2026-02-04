using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator;

internal static class Program
{
    private static async Task Main()
    {
        // Build the DI container
        var host = CreateHostBuilder().Build();

        try
        {
            var startTime = DateTime.Now;
            var logger = host.Services.GetRequiredService<ILogger>();

            logger.Verbose($"Starting data generation at {startTime}");
            logger.Verbose("Initializing services...");

            // Initialize database
            logger.Verbose("Connecting to database...");
            var database = host.Services.GetRequiredService<Database>();

            if (database.GetInstance() == null)
                throw new InvalidOperationException("Database connection failed");

            logger.Verbose("Connection to database established");

            // Generate encounters
            var dataGenerator = host.Services.GetRequiredService<IDataGenerator>();
            await dataGenerator.GenerateAsync(database, startTime);

            logger.Verbose($"Data generation completed at {DateTime.Now}");
            logger.Information($"Total execution time: {DateTime.Now - startTime}");
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
                services.AddSingleton<IRandomProvider, RandomProvider>();
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
            });
}