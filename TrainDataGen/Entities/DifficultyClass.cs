namespace TrainDataGen.Entities;

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