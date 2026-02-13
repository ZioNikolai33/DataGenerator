using System.Text.Json.Serialization;

namespace TrainingDataGenerator.Entities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Results
{
    Victory,
    Defeat,
    Undecided
}
