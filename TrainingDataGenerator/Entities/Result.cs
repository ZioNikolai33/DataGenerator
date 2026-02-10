using TrainingDataGenerator.Entities.Enums;

namespace TrainingDataGenerator.Entities;

public class Result
{
    public Results Outcome { get; set; }
    public short TotalRounds { get; set; }
    public string Details { get; set; } = string.Empty;

    public Result(Results outcome, short totalRounds, string details)
    {
        Outcome = outcome;
        TotalRounds = totalRounds;
        Details = details;
    }

    public Result()
    {
        Outcome = Results.Undecided;
        TotalRounds = 0;
        Details = string.Empty;
    }
}
