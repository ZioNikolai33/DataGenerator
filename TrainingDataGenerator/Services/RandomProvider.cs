using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Services;

public class RandomProvider : IRandomProvider
{
    private readonly Random _random;

    public RandomProvider()
    {
        _random = Random.Shared;
    }

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

        return items.OrderBy(_ => _random.Next(int.MaxValue))
                   .Take(count)
                   .ToList();
    }

    public List<T> Shuffle<T>(IEnumerable<T> items)
    {
        if (_random == null)
            throw new ArgumentNullException(nameof(_random));

        if (items == null)
            throw new ArgumentNullException(nameof(items));

        return items.OrderBy(_ => _random.Next(int.MaxValue)).ToList();
    }
}