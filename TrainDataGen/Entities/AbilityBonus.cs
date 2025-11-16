using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities;

public class AbilityBonus
{
    public BaseEntity Ability { get; set; }
    public int Bonus { get; set; }

    public AbilityBonus(BaseEntity ability, int bonus)
    {
        Ability.Index = ability.Index;
        Ability.Name = ability.Name;
        Bonus = bonus;
    }
}