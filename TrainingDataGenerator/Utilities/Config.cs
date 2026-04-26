using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrainingDataGenerator.Utilities;

public class Config
{
    [JsonPropertyName("Database")]
    public Database Db { get; set; } = new Database();
    [JsonPropertyName("NumberOfCycles")]
    public int NumberOfCycles { get; set; } = 10;
    [JsonPropertyName("Logging")]
    public LogLevel Logging { get; set; } = new LogLevel();
    [JsonPropertyName("RandomSeed")]
    public int RandomSeed { get; set; } = 0;
    [JsonPropertyName("LogsFolder")]
    public string LogsFolder { get; set; } = "..\\..\\..\\Generator\\logs";
    [JsonPropertyName("OutputFolder")]
    public string OutputFolder { get; set; } = "..\\..\\..\\Generator\\output";

    public class Database
    {
        [JsonPropertyName("connectionString")]
        public string ConnectionString { get; set; } = string.Empty;
        [JsonPropertyName("databaseName")]
        public string DatabaseName { get; set; } = string.Empty;
    }

    public class LogLevel
    {
        [JsonPropertyName("Default")]
        public string Default { get; set; } = "Information";
        [JsonPropertyName("Microsoft.AspNetCore")]
        public string Microsoft { get; set; } = "Warning";
    }

    public static Config LoadConfig()
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