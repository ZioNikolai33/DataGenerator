using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Extensions;

public static class RandomProviderExtensions
{
    public static int RollDice(this IRandomProvider random, int sides)
    {
        if (random == null)
            throw new ArgumentNullException(nameof(random));

        if (sides < 1)
            throw new ArgumentOutOfRangeException(nameof(sides), "Dice must have at least 1 side");

        return random.Next(1, sides + 1);
    }

    public static int RollDice(this IRandomProvider random, int count, int sides)
    {
        if (random == null)
            throw new ArgumentNullException(nameof(random));

        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count), "Must roll at least 1 die");

        var total = 0;
        for (var i = 0; i < count; i++)
        {
            total += random.RollDice(sides);
        }

        return total;
    }

    public static bool NextBool(this IRandomProvider random, double probability = 0.5)
    {
        if (random == null)
            throw new ArgumentNullException(nameof(random));

        if (probability < 0.0 || probability > 1.0)
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0.0 and 1.0");

        return random.Next(0, 100) < (probability * 100);
    }
}