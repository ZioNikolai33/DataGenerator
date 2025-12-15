using System.Text.Json.Serialization;

namespace TrainingDataGenerator.Entities;

public class MonsterMultiplier
{
    [JsonPropertyName("number")]
    public int Number { get; set; }
    [JsonPropertyName("multiplier")]
    public double Multiplier { get; set; }
}