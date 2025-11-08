namespace TrainDataGen.Entities;

public class AbilityBonus
{
    public string Name { get; set; }
    public int Bonus { get; set; }

    public AbilityBonus(string name, int bonus)
    {
        Name = name;
        Bonus = bonus;
    }
}