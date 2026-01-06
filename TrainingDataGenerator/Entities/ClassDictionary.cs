namespace TrainingDataGenerator.Entities;

public class ClassDictionary
{
    public short Key { get; set; }
    public string Value { get; set; } = string.Empty;

    public ClassDictionary(short key, string value)
    {
        Key = key;
        Value = value;
    }
}
