namespace AlgorithmDataGenerator.Entities;

public class Monster
{
    public string Name { get; set; } = string.Empty;
    public double ChallengeRating { get; set; }
    public int Xp { get; set; }
    public int BaseStats { get; set; }

    public Monster(string name, double challengeRating, int xp, int baseStats)
    {
        Name = name;
        ChallengeRating = challengeRating;
        Xp = xp;
        BaseStats = baseStats;
    }

    public Monster(TrainingDataGenerator.Entities.Monster monster, int baseStats)
    {
        Name = monster.Name;
        ChallengeRating = monster.ChallengeRating;
        Xp = monster.Xp;
        BaseStats = baseStats;
    }

    public override string ToString() =>
        $"{Name} - Challenge Rating: {ChallengeRating}, XP: {Xp}, BaseStats: {BaseStats}";
}
