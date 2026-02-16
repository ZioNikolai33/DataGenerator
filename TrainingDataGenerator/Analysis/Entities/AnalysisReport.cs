using System.Text.Json;
using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Analysis.Entities;

public class AnalysisReport
{
    public int TotalEncounters { get; set; }
    public Dictionary<string, double> OutcomeBalance { get; set; } = new();
    public Dictionary<CRRatios, double> DifficultyBalance { get; set; } = new();
    public Dictionary<string, double> ClassDistribution { get; set; } = new();
    public Dictionary<string, double> RaceDistribution { get; set; } = new();
    public Dictionary<byte, int> LevelDistribution { get; set; } = new();
    public Dictionary<int, int> PartySizeDistribution { get; set; } = new();
    public Dictionary<int, int> MonsterCountDistribution { get; set; } = new();
    public Dictionary<string, double> AverageTurnsToVictory { get; set; } = new();
    public double AveragePartyLevel { get; set; }
    public List<string> Issues { get; set; } = new();

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}
