using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Validators;

public class DatasetStatistics
{
    public int TotalEncounters { get; private set; }
    public Dictionary<string, int> OutcomeDistribution { get; } = new();
    public Dictionary<CRRatios, int> DifficultyDistribution { get; } = new();
    public Dictionary<int, int> PartyLevelDistribution { get; } = new();

    public void Update(Encounter encounter)
    {
        TotalEncounters++;

        var outcome = encounter.Outcome?.ToString() ?? "Unknown";
        OutcomeDistribution[outcome] = OutcomeDistribution.GetValueOrDefault(outcome) + 1;

        DifficultyDistribution[encounter.Difficulty] =
            DifficultyDistribution.GetValueOrDefault(encounter.Difficulty) + 1;

        foreach (var member in encounter.PartyMembers)
        {
            PartyLevelDistribution[member.Level] =
                PartyLevelDistribution.GetValueOrDefault(member.Level) + 1;
        }
    }
}