using System.Text.Json.Serialization;

namespace TrainingDataGenerator.Entities;

public class ExpThreshold
{
    [JsonPropertyName("level")]
    public int Level { get; set; }
    [JsonPropertyName("cakewalk")]
    public int Cakewalk { get; set; }
    [JsonPropertyName("easy")]
    public int Easy { get; set; }
    [JsonPropertyName("medium")]
    public int Medium { get; set; }
    [JsonPropertyName("hard")]
    public int Hard { get; set; }
    [JsonPropertyName("deadly")]
    public int Deadly { get; set; }
    [JsonPropertyName("impossible")]
    public int Impossible { get; set; }
}
