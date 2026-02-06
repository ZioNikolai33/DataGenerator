using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities;

public class Skill : BaseEntity
{
    public sbyte Modifier { get; set; }
    public bool IsProficient { get; set; }
    public bool IsExpert { get; set; }

    public Skill(BaseEntity skill, sbyte modifier, byte proficiencyBonus = 2, bool isProficient = false, bool isExpert = false) : base(skill.Index, skill.Name)
    {
        Modifier = modifier;
        IsProficient = isProficient;
        IsExpert = isExpert;

        if (IsProficient)
            Modifier += (sbyte)proficiencyBonus;
        else if (IsExpert)
            Modifier += (sbyte)(proficiencyBonus * 2);
    }

    public void SetProficiency(bool isProficient, byte proficiencyBonus)
    {
        if (IsProficient && !isProficient)
        {
            Modifier -= (sbyte)proficiencyBonus;

            if (IsExpert)
            {
                Modifier -= (sbyte)proficiencyBonus;
                IsExpert = false;
            }
        }
        else if (!IsProficient && isProficient)
            Modifier += (sbyte)proficiencyBonus;

        IsProficient = isProficient;
    }

    public void SetExpertise(bool isExpert, byte proficiencyBonus)
    {
        if (!IsProficient)
            return;

        if (IsExpert && !isExpert)
            Modifier -= (sbyte)proficiencyBonus;
        else if (!IsExpert && isExpert)
            Modifier += (sbyte)proficiencyBonus;

        IsExpert = isExpert;
    }
}