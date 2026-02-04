namespace TrainingDataGenerator.Interfaces;

public interface IRandomProvider
{
    int Next();
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
    T SelectRandom<T>(IEnumerable<T> items);

    List<T> SelectRandom<T>(IEnumerable<T> items, int count);
    List<T> Shuffle<T>(IEnumerable<T> items);
}