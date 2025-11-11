using TrainDataGen.Entities;
using TrainDataGen.Entities.Enums;
using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Utilities;

public static class EntitiesMapper
{
    public static Skills FromString(string skillName)
    {
        return skillName.ToLower() switch
        {
            "skill-acrobatics" => Skills.Acrobatics,
            "skill-animal-handling" => Skills.AnimalHandling,
            "skill-arcana" => Skills.Arcana,
            "skill-athletics" => Skills.Athletics,
            "skill-deception" => Skills.Deception,
            "skill-history" => Skills.History,
            "skill-insight" => Skills.Insight,
            "skill-intimidation" => Skills.Intimidation,
            "skill-investigation" => Skills.Investigation,
            "skill-medicine" => Skills.Medicine,
            "skill-nature" => Skills.Nature,
            "skill-perception" => Skills.Perception,
            "skill-performance" => Skills.Performance,
            "skill-persuasion" => Skills.Persuasion,
            "skill-religion" => Skills.Religion,
            "skill-sleight-of-hand" => Skills.SleightOfHand,
            "skill-stealth" => Skills.Stealth,
            "skill-survival" => Skills.Survival,
            _ => throw new ArgumentException($"Unknown skill name: {skillName}")
        };
    }

    public static List<Skills> FromStringMultiple(IEnumerable<string> skillNames)
    {
        var skills = new List<Skills>();

        foreach (var skillName in skillNames)
            skills.Add(FromString(skillName));

        return skills;
    }

    public static string ToStringEnum(Skills skill)
    {
        switch (skill) {
            case Skills.Acrobatics:
                return "skill-acrobatics";
            case Skills.AnimalHandling:
                return "skill-animal-handling";
            case Skills.Arcana:
                return "skill-arcana";
            case Skills.Athletics:
                return "skill-athletics";
            case Skills.Deception:
                return "skill-deception";
            case Skills.History:
                return "skill-history";
            case Skills.Insight:
                return "skill-insight";
            case Skills.Intimidation:
                return "skill-intimidation";
            case Skills.Investigation:
                return "skill-investigation";
            case Skills.Medicine:
                return "skill-medicine";
            case Skills.Nature:
                return "skill-nature";
            case Skills.Perception:
                return "skill-perception";
            case Skills.Performance:
                return "skill-performance";
            case Skills.Persuasion:
                return "skill-persuasion";
            case Skills.Religion:
                return "skill-religion";
            case Skills.SleightOfHand:
                return "skill-sleight-of-hand";
            case Skills.Stealth:
                return "skill-stealth";
            case Skills.Survival:
                return "skill-survival";
            default:
                throw new ArgumentException($"Unknown skill enum: {skill}");
        }
    }
}
