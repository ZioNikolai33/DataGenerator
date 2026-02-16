using ClosedXML.Excel;

namespace TrainingDataGenerator.Interfaces;

public interface IExporterService
{
    Task ExportToJsonAsync<T>(T obj, string filePath);
    void ExportToExcelAsync(XLWorkbook workbook, string filePath);
}
