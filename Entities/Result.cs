using TrainingDataGenerator.Entities.Enums;

namespace TrainingDataGenerator.Entities;

public class Result
{
    public Results Outcome { get; set; }
    public string Details { get; set; } = string.Empty;

    public Result(Results outcome, string details)
    {
        Outcome = outcome;
        Details = details;
    }
}
