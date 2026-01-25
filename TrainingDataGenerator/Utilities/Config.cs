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
}