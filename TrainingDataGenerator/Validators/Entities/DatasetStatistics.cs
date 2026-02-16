using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Validators.Entities;

public class DatasetStatistics
{
    public int TotalEncounters { get; private set; }
    public int ValidEncounters { get; private set; }
    public int InvalidEncounters => TotalEncounters - ValidEncounters;
    public List<Distribution<string>> OutcomeDistribution { get; } = new List<Distribution<string>>();
    public List<Distribution<string>> DifficultyDistribution { get; } = new List<Distribution<string>>();
    public List<Distribution<string>> PartyClassDistribution { get; } = new List<Distribution<string>>();
    public List<Distribution<string>> PartyRaceDistribution { get; } = new List<Distribution<string>>();
    public List<Distribution<string>> PartyLevelDistribution { get; } = new List<Distribution<string>>();
    public List<Distribution<string>> PartySizeDistribution { get; } = new List<Distribution<string>>();
    public List<Distribution<string>> MonsterCRDistribution { get; } = new List<Distribution<string>>();
    public List<Distribution<string>> MonsterCountDistribution { get; } = new List<Distribution<string>>();
    public List<Distribution<string>> CombatDurationDistribution { get; } = new List<Distribution<string>>();

    public void Update(Encounter encounter, bool isValid)
    {
        TotalEncounters++;

        if (isValid)
            ValidEncounters++;

        // Update outcome distribution
        var outcome = encounter.Outcome?.Outcome.ToString() ?? "Unknown";
        UpdateOrAddDistribution(OutcomeDistribution, outcome);

        // Update outcome by duration
        var duration = encounter.Outcome?.TotalRounds ?? 0;
        UpdateOrAddDistribution(CombatDurationDistribution, duration.ToString());

        // Update difficulty distribution
        UpdateOrAddDistribution(DifficultyDistribution, encounter.Difficulty.ToString());

        // Update sizes distribution
        UpdateOrAddDistribution(PartySizeDistribution, encounter.PartyMembers.Count.ToString());
        UpdateOrAddDistribution(MonsterCountDistribution, encounter.Monsters.Count.ToString());

        // Update party member distributions
        foreach (var member in encounter.PartyMembers)
        {
            UpdateOrAddDistribution(PartyLevelDistribution, member.Level.ToString());
            UpdateOrAddDistribution(PartyClassDistribution, member.Class);
            UpdateOrAddDistribution(PartyRaceDistribution, member.Race);
        }

        // Update monster CR distribution
        foreach (var monster in encounter.Monsters)
        {
            UpdateOrAddDistribution(MonsterCRDistribution, monster.ChallengeRating.ToString());
        }
    }

    public void CalculatePercentage()
    {
        foreach (var distribution in new[] { OutcomeDistribution, DifficultyDistribution, PartyClassDistribution, PartyRaceDistribution, PartyLevelDistribution, PartySizeDistribution, MonsterCRDistribution, MonsterCountDistribution, CombatDurationDistribution })
            foreach (var item in distribution)
                item.Percentage = TotalEncounters > 0 ? $"{(double)item.Count / TotalEncounters * 100:0.0}%" : "0%";
    }

    private void UpdateOrAddDistribution(List<Distribution<string>> distribution, string value)
    {
        var item = distribution.FirstOrDefault(d => d.Value == value);

        if (item != null)
            item.Count++;
        else
            distribution.Add(new Distribution<string>(value) { Count = 1 });
    }
}