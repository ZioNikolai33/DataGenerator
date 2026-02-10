namespace TrainingDataGenerator.Validators;

public class ValidationResult
{
    public bool IsValid => !Errors.Any();
    public List<string> Errors { get; }
    public DatasetStatistics? Statistics { get; }

    public ValidationResult(List<string> errors, DatasetStatistics? statistics = null)
    {
        Errors = errors;
        Statistics = statistics;
    }

    public override string ToString()
    {
        return $"Validation Result: {(IsValid ? "Valid" : "Invalid")}\n" +
               $"Errors: {string.Join("\n", Errors)}\n" +
               $"Statistics: {Statistics}";
    }
}