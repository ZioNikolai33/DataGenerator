using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities;

public class Attribute: BaseEntity
{
    public byte Value { get; set; }
    public byte Modifier { get; set; }
    public byte Save { get; set; }

    public Attribute(byte value, string index, string name) : base(index, name)
    {
        Value = value;
        Modifier = (byte)((value - 10) / 2);
        Save = (byte)((value - 10) / 2);
    }
}