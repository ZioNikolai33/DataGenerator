namespace TrainDataGen.Entities;

public class Choice
{
    public int Number { get; set; }
    public List<string> Choices { get; set; }

    public Choice(int number, List<string> choices)
    {
        Number = number;
        Choices = choices;
    }

    public List<string> GetRandomChoice(IEnumerable<string>? items = null)
    {
        var filteredChoices = items != null
            ? Choices.Where(choice => items == null || !items.Contains(choice)).ToList()
            : Choices;

        if (filteredChoices.Count <= Number)
            return filteredChoices;

        return GetRandomSample(filteredChoices, Number);
    }

    public List<string> GetRandomChoiceWithoutCheck()
    {
        if (Choices.Count <= Number)
            return Choices;

        return GetRandomSample(Choices, Number);
    }

    public override string ToString()
    {
        return $"{Number} - {string.Join(", ", Choices)}";
    }

    private static List<string> GetRandomSample(List<string> source, int count)
    {
        var rng = new Random();
        return source.OrderBy(_ => rng.Next()).Take(count).ToList();
    }
}