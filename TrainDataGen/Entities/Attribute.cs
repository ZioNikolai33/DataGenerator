namespace TrainDataGen.Entities;

public class Attribute
{
    public byte Value { get; set; }
    public byte Modifier { get; set; }
    public byte Save { get; set; }

    public Attribute(int value)
    {
        Value = value;
        Modifier = (value - 10) / 2;
        Save = (value - 10) / 2;
    }
}