namespace TrainDataGen.Entities;

public class Area
{
    public string Type { get; set; }
    public int Size { get; set; }

    public Area(string type, int size)
    {
        Type = type;
        Size = size;
    }
}