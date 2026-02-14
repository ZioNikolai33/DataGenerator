using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Services;

public class RandomProvider : IRandomProvider
{
    private readonly Random _random;
    private readonly int _seed;

    public RandomProvider(int seed = 0)
    {
        if (seed == 0)
            seed = Environment.TickCount;

        _seed = seed;
        _random = new Random(seed);
    }

    public int GetSeed() => _seed;

    public RandomProvider(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public int Next()
    {
        return _random.Next();
    }

    public int Next(int maxValue)
    {
        return _random.Next(maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }

    public T SelectRandom<T>(IEnumerable<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var itemsList = items as IList<T> ?? items.ToList();

        if (itemsList.Count == 0)
            throw new ArgumentException("Cannot select from an empty collection", nameof(items));

        var randomIndex = _random.Next(itemsList.Count);
        return itemsList[randomIndex];
    }

    public List<T> SelectRandom<T>(IEnumerable<T> items, int count)
    {
        if (_random == null)
            throw new ArgumentNullException(nameof(_random));

        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var array = items.ToArray();

        if (count > array.Length)
            return array.ToList(); // Return all items if count exceeds available items

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative");

        // Use partial Fisher-Yates: only shuffle the first 'count' elements
        for (int i = 0; i < count; i++)
        {
            int j = _random.Next(i, array.Length);
            (array[i], array[j]) = (array[j], array[i]);
        }

        // Return the first 'count' elements
        return array.Take(count).ToList();
    }

    public List<T> Shuffle<T>(IEnumerable<T> items)
    {
        if (_random == null)
            throw new ArgumentNullException(nameof(_random));

        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var array = items.ToArray();
        var n = array.Length;

        // Fisher-Yates shuffle algorithm
        for (int i = n - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);

            (array[i], array[j]) = (array[j], array[i]);
        }

        return array.ToList();
    }
}