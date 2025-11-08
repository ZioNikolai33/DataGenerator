namespace TrainDataGen.Entities;

public class Skill
{
    public byte Modifier { get; set; }
    public bool IsProficient { get; set; }
    public bool IsExpert { get; set; }

    public Skill(byte modifier, bool isProficient = false, bool isExpert = false)
    {
        Modifier = modifier;
        IsProficient = isProficient;
        IsExpert = isExpert;
    }

    public void SetProficiency(bool isProficient, byte proficiencyBonus)
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

    public void SetExpertise(bool isExpert, byte proficiencyBonus)
    {
        if (IsExpert && !isExpert)
            Modifier -= proficiencyBonus;
        else if (!IsExpert && isExpert)
        {
            if (!IsProficient)
            {
                Modifier += proficiencyBonus;
                IsProficient = true;
            }

            Modifier += proficiencyBonus;
        }

        IsExpert = isExpert;
    }
}