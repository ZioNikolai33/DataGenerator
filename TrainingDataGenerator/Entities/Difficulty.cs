namespace TrainingDataGenerator.Entities;

public class Difficulty
{
    public int Cakewalk { get; set; }
    public int Easy { get; set; }
    public int Medium { get; set; }
    public int Hard { get; set; }
    public int Deadly { get; set; }
    public int Impossible { get; set; }

    public Difficulty(int cakewalk = 0, int easy = 0, int medium = 0, int hard = 0, int deadly = 0, int impossible = 0)
    {
        Cakewalk = cakewalk;
        Easy = easy;
        Medium = medium;
        Hard = hard;
        Deadly = deadly;
        Impossible = impossible;
    }

    public override string ToString()
    {
        return $"cakewalk={Cakewalk}, easy={Easy}, medium={Medium}, hard={Hard}, deadly={Deadly}, impossible={Impossible}";
    }
}