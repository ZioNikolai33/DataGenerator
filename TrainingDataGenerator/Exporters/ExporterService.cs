using ClosedXML.Excel;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Exporters;

public class ExporterService : IExporterService
{
    public async Task ExportToJsonAsync<T>(T obj, string filePath)
    {
        var json = JsonSerializer.Serialize(obj,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

        await File.WriteAllTextAsync(filePath, json);
    }

    public void ExportToExcelAsync(XLWorkbook workbook, string filePath) =>
        workbook.SaveAs(filePath);
}
