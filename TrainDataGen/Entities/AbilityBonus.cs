using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities;

public class AbilityBonus
{
    public BaseEntity ability { get; set; }
    public int Bonus { get; set; }

    public AbilityBonus(BaseEntity ability, int bonus)
    {
        ability.Index = ability.Index;
        ability.Name = ability.Name;
        Bonus = bonus;
    }
}