namespace TrainingDataGenerator.Entities;

public class Usage
{
    public string Type { get; set; }
    public byte? Times { get; set; }
    public string? Dice { get; set; }
    public byte? MinValue { get; set; }
    public List<string>? RestTypes { get; set; }

    public Usage(string type, byte? times, List<string>? restTypes, string? dice, byte? minValue)
    {
        Type = type;
        Times = times;
        RestTypes = restTypes ?? new List<string>();
        Dice = dice ?? String.Empty;
        MinValue = minValue ?? 0;
    }
}
