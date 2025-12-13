using System.Text.Json.Serialization;

namespace TrainDataGen.Entities;

public class ExpThreshold
{
    [JsonPropertyName("level")]
    public int Level { get; set; }
    [JsonPropertyName("easy")]
    public int Easy { get; set; }
    [JsonPropertyName("medium")]
    public int Medium { get; set; }
    [JsonPropertyName("hard")]
    public int Hard { get; set; }
    [JsonPropertyName("deadly")]
    public int Deadly { get; set; }
}
