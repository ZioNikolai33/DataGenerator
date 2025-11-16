using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities;

public class Attribute
{
    public byte Value { get; set; }
    public byte Modifier { get; set; }
    public byte Save { get; set; }

    public Attribute(byte value)
    {
        Value = value;
        Modifier = (byte)((value - 10) / 2);
        Save = (byte)((value - 10) / 2);
    }

    public void AddValue(byte value)
    {
        Value += value;
        Modifier = (byte)((Value - 10) / 2);
        Save = (byte)((Value - 10) / 2);
    }

    public void SetProficiency(bool isProficient, byte proficiencyBonus)
    {
        if (isProficient)
            Save += proficiencyBonus;
        else
            Save -= proficiencyBonus;
    }
}