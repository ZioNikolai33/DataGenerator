using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Generator;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator;

internal static class Program
{
    private static void Main()
    {
        try
        {            
            var startTime = DateTime.Now;
            Logger.Instance.Information($"Starting data generation at {startTime}");

            Logger.Instance.Information("Connecting to database...");
            var database = new Database();

            if (database.GetInstance() == null)
                throw new InvalidOperationException("Database connection failed");

            Logger.Instance.Information("Connection to database established");

            DataGenerator.Generate(database, startTime);
            Logger.Instance.Information($"Data generation completed at {DateTime.Now}");
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