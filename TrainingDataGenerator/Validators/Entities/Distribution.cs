namespace TrainingDataGenerator.Validators.Entities;

public class Distribution<T> where T : class
{
    public T Value { get; set; } = default!;
    public int Count { get; set; }
    public string Percentage { get; set; } = "0%";

    public Distribution(T value)
    {
        Value = value;
    }
}
