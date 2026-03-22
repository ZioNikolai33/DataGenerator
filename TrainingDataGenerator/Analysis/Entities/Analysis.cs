using System.Data;

namespace TrainingDataGenerator.Analysis.Entities;

public class Analysis
{
    public string SheetName { get; set; } = string.Empty;
    public DataTable Data { get; set; } = new DataTable();

    public Analysis(string sheetName, DataTable data)
    {
        SheetName = sheetName;
        Data = data;
    }
}
