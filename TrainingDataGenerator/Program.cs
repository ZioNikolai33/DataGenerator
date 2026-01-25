using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Generator;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator;

internal static class Program
{
    private static async Task Main()
    {
        try
        {            
            var startTime = DateTime.Now;
            Logger.Instance.Verbose($"Starting data generation at {startTime}");

            Logger.Instance.Verbose("Connecting to database...");
            var database = new Database();

            if (database.GetInstance() == null)
                throw new InvalidOperationException("Database connection failed");

            Logger.Instance.Verbose("Connection to database established");

            await DataGenerator.GenerateAsync(database, startTime);
            Logger.Instance.Verbose($"Data generation completed at {DateTime.Now}");
        }
        catch (InvalidOperationException ex)
        {
            Logger.Instance.Error(ex.Message);
        }
        catch (Exception ex)
        {
            Logger.Instance.Error(ex, "An error occurred during data generation.");
        }        
    }
}