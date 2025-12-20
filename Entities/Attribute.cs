using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities;

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

        if (Value % 2 == 0)
        {
            Modifier++;
            Save++;
        }        
    }

    public void SetProficiency(bool isProficient, sbyte proficiencyBonus)
    {
        if (isProficient)
            Save += proficiencyBonus;
        else
            Save -= proficiencyBonus;
    }
}