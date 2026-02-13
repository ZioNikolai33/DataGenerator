using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Validators.Entities;

public class DatasetStatistics
{
    public int TotalEncounters { get; private set; }
    public int ValidEncounters { get; private set; }
    public int InvalidEncounters => TotalEncounters - ValidEncounters;
    public Dictionary<string, int> OutcomeDistribution { get; } = new();
    public Dictionary<CRRatios, int> DifficultyDistribution { get; } = new();
    public Dictionary<string, int> PartyClassDistribution { get; } = new();
    public Dictionary<string, int> PartyRaceDistribution { get; } = new();
    public Dictionary<int, int> PartyLevelDistribution { get; } = new();
    public Dictionary<int, int> PartySizeDistribution { get; } = new();
    public Dictionary<double, int> MonsterCRDistribution { get; } = new();
    public Dictionary<int, int> MonsterCountDistribution { get; } = new();
    public Dictionary<int, int> CombatDurationDistribution { get; } = new();

    public void Update(Encounter encounter, bool isValid)
    {
        TotalEncounters++;

        if (isValid)
            ValidEncounters++;

        // Update outcome distribution
        var outcome = encounter.Outcome?.Outcome.ToString() ?? "Unknown";
        OutcomeDistribution[outcome] = OutcomeDistribution.GetValueOrDefault(outcome) + 1;

        // Update outcome by duration
        CombatDurationDistribution[encounter.Outcome?.TotalRounds ?? 0] = CombatDurationDistribution.GetValueOrDefault(encounter.Outcome?.TotalRounds ?? 0) + 1;

        // Update difficulty distribution
        DifficultyDistribution[encounter.Difficulty] = DifficultyDistribution.GetValueOrDefault(encounter.Difficulty) + 1;

        // Update Sizes distribution
        PartySizeDistribution[encounter.PartyMembers.Count] = PartySizeDistribution.GetValueOrDefault(encounter.PartyMembers.Count) + 1;
        MonsterCountDistribution[encounter.Monsters.Count] = MonsterCountDistribution.GetValueOrDefault(encounter.Monsters.Count) + 1;

        foreach (var member in encounter.PartyMembers)
        {
            PartyLevelDistribution[member.Level] = PartyLevelDistribution.GetValueOrDefault(member.Level) + 1;
            PartyClassDistribution[member.Class] = PartyClassDistribution.GetValueOrDefault(member.Class) + 1;
            PartyRaceDistribution[member.Race] = PartyRaceDistribution.GetValueOrDefault(member.Race) + 1;
        }

        foreach (var monster in encounter.Monsters)
            MonsterCRDistribution[monster.ChallengeRating] = MonsterCRDistribution.GetValueOrDefault(monster.ChallengeRating) + 1;
    }
}