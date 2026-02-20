using System.Data;

namespace TrainingDataGenerator.Analysis.Entities;

public class AnalysisData
{
    public string SheetName { get; set; } = string.Empty;
    public DataTable Data { get; set; } = new DataTable();

    public AnalysisData(string sheetName, DataTable data)
    {
        SheetName = sheetName;
        Data = data;
    }
}
