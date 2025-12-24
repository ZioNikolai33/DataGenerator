namespace TrainingDataGenerator.Entities;

public sealed class Encounter
{
    public string Id { get; set; }
    public List<Member> PartyMembers { get; set; }
    public List<Monster> Monsters { get; set; }
    public Result Outcome { get; set; } = new Result();

    public Encounter(int index, CRRatios difficulty, List<Member> partyMembers, List<Monster> monsters)
    {
        var difficultyLetter = difficulty switch
        {
            CRRatios.Cakewalk => "C",
            CRRatios.Easy => "E",
            CRRatios.Normal => "N",
            CRRatios.Hard => "H",
            CRRatios.Deadly => "D",
            CRRatios.Impossible => "I",
            _ => throw new NotImplementedException()
        };

        var indexPadded = index.ToString().PadLeft(8, '0');

        Id = $"E{indexPadded}-{difficultyLetter.ToString().ToUpper()}"; // Encounter ID format: E00000001-N
        PartyMembers = partyMembers;
        Monsters = monsters;
    }
}
