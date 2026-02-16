using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.MonsterEntities;

public class DifficultyClass
{
    public string DcType { get; set; }
    public string DcSuccess { get; set; }

    public DifficultyClass(string dcType, string dcSuccess)
    {
        DcType = dcType;
        DcSuccess = dcSuccess;
    }
}
