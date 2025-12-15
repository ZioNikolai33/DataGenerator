namespace TrainDataGen.Entities;

public class Difficulty
{
    public int Easy { get; set; }
    public int Medium { get; set; }
    public int Hard { get; set; }
    public int Deadly { get; set; }

    public Difficulty(int easy = 0, int medium = 0, int hard = 0, int deadly = 0)
    {
        Easy = easy;
        Medium = medium;
        Hard = hard;
        Deadly = deadly;
    }

    public override string ToString()
    {
        return $"easy={Easy}, medium={Medium}, hard={Hard}, deadly={Deadly}";
    }
}