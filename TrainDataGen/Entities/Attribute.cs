using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities;

public class Attribute
{
    public byte Value { get; set; }
    public sbyte Modifier { get; set; }
    public sbyte Save { get; set; }

    public Attribute(byte value)
    {
        Value = value;
        Modifier = (sbyte)((value - 10) / 2);
        Save = (sbyte)((value - 10) / 2);
    }

    public void AddValue(byte value)
    {
        Value += value;
        Modifier = (sbyte)((Value - 10) / 2);
        Save = (sbyte)((Value - 10) / 2);
    }

    public void SetProficiency(bool isProficient, sbyte proficiencyBonus)
    {
        if (isProficient)
            Save += proficiencyBonus;
        else
            Save -= proficiencyBonus;
    }
}