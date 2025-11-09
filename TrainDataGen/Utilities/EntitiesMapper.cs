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
            "skill-animal_handling" => Skills.AnimalHandling,
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
            "skill-sleight_of_hand" => Skills.SleightOfHand,
            "skill-stealth" => Skills.Stealth,
            "skill-survival" => Skills.Survival,
            _ => throw new ArgumentException($"Unknown skill name: {skillName}")
        };
    }

    public static Race Map(RaceMapper race)
    {
        return new Race
        {
            Index = race.Index,
            Name = race.Name,
            Speed = race.Speed,
            Size = race.Size,
            AbilityBonuses = race.AbilityBonuses,
            Proficiencies = (List<Skills>) race.StartingProficiences.Select(item => FromString(item.Index)),
        };
    }
}
