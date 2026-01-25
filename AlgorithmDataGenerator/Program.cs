using AlgorithmDataGenerator.Generator;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Utilities;

namespace AlgorithmDataGenerator;

internal static class Program
{
    private static void Main()
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

            DataGenerator.Generate(database, startTime);
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