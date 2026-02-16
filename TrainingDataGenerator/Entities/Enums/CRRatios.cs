using System.Text.Json.Serialization;

namespace TrainingDataGenerator.Entities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CRRatios
{
    Cakewalk = 1,
    Easy = 3,
    Normal = 5,
    Hard = 6,
    Deadly = 8,
    Impossible = 10
}