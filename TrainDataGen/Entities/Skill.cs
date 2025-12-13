using TrainDataGen.Entities.Enums;
using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities;

public class Skill : BaseEntity
{
    public sbyte Modifier { get; set; }
    public bool IsProficient { get; set; }
    public bool IsExpert { get; set; }

    public Skill(BaseEntity skill, sbyte modifier, bool isProficient = false, bool isExpert = false) : base(skill.Index, skill.Name)
    {
        Modifier = modifier;
        IsProficient = isProficient;
        IsExpert = isExpert;
    }

    public void SetProficiency(bool isProficient, sbyte proficiencyBonus)
    {
        if (IsProficient && !isProficient)
        {
            Modifier -= proficiencyBonus;

            if (IsExpert)
            {
                Modifier -= proficiencyBonus;
                IsExpert = false;
            }
        }
        else if (!IsProficient && isProficient)
            Modifier += proficiencyBonus;

        IsProficient = isProficient;
    }

    public void SetExpertise(bool isExpert, sbyte proficiencyBonus)
    {
        if (!IsProficient)
            return;

        if (IsExpert && !isExpert)
            Modifier -= proficiencyBonus;
        else if (!IsExpert && isExpert)
            Modifier += proficiencyBonus;

        IsExpert = isExpert;
    }
}