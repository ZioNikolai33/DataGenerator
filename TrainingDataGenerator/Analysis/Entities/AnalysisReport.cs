namespace TrainingDataGenerator.Analysis.Entities;

public class AnalysisReport
{
    public int TotalEncounters { get; set; }
    public List<AnalysisData> Analyses { get; set; } = [];
}
