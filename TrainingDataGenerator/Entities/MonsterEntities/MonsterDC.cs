namespace TrainingDataGenerator.Entities.MonsterEntities;

public class MonsterDC : DifficultyClass
{
    public int DcValue { get; set; }

    public MonsterDC(string dcType, string dcSuccess, int dcValue) : base(dcType, dcSuccess)
    {
        DcValue = dcValue;
    }
}
