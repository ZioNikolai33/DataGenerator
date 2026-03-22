using System.Data;

namespace TrainingDataGenerator.Analysis.Entities;

public class AnalysisReport
{
    public int TotalEncounters { get; set; }
    public List<Analysis> Analyses { get; set; } = [];
    public DataTable PartyHealth { get; set; } = new();
    public DataTable PartyAttributes { get; set; } = new();
    public DataTable PartyStats { get; set; } = new();
    public DataTable MonsterStats { get; set; } = new();
    public DataTable Results { get; set; } = new();
}
