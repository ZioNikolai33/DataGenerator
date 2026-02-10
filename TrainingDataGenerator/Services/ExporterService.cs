using ClosedXML.Excel;
using System.Text.Json;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Validators;

namespace TrainingDataGenerator.Services;

public class ExporterService : IExporterService
{
    public async Task ExportToJsonAsync<T>(T obj, string filePath)
    {
        var json = JsonSerializer.Serialize(obj,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

        await File.WriteAllTextAsync(filePath, json);
    }

    public void ExportToExcelAsync(DatasetStatistics datasetStatistics, string filePath)
    {
        using var workbook = new XLWorkbook();
        var statsSheet = workbook.Worksheets.Add("Statistics");

        // Summary Statistics
        statsSheet.Cell(1, 1).Value = "Total Encounters";
        statsSheet.Cell(1, 2).Value = datasetStatistics.TotalEncounters;
        statsSheet.Range(1, 1, 1, 2).Style.Font.Bold = true;

        // Outcome Distribution Table
        var outcomeStartRow = 3;
        statsSheet.Cell(outcomeStartRow, 1).Value = "Outcome";
        statsSheet.Cell(outcomeStartRow, 2).Value = "Count";
        statsSheet.Range(outcomeStartRow, 1, outcomeStartRow, 2).Style.Font.Bold = true;

        var outcomeRow = outcomeStartRow + 1;
        foreach (var kvp in datasetStatistics.OutcomeDistribution.OrderByDescending(x => x.Value))
        {
            statsSheet.Cell(outcomeRow, 1).Value = kvp.Key;
            statsSheet.Cell(outcomeRow, 2).Value = kvp.Value;
            outcomeRow++;
        }

        // Difficulty Distribution Table
        var difficultyStartRow = outcomeRow + 2;
        statsSheet.Cell(difficultyStartRow, 1).Value = "Difficulty";
        statsSheet.Cell(difficultyStartRow, 2).Value = "Count";
        statsSheet.Range(difficultyStartRow, 1, difficultyStartRow, 2).Style.Font.Bold = true;

        var difficultyRow = difficultyStartRow + 1;
        foreach (var kvp in datasetStatistics.DifficultyDistribution.OrderBy(x => x.Key))
        {
            statsSheet.Cell(difficultyRow, 1).Value = kvp.Key.ToString();
            statsSheet.Cell(difficultyRow, 2).Value = kvp.Value;
            difficultyRow++;
        }

        // Party Level Distribution Table
        var levelStartRow = difficultyRow + 2;
        statsSheet.Cell(levelStartRow, 1).Value = "Party Level";
        statsSheet.Cell(levelStartRow, 2).Value = "Count";
        statsSheet.Range(levelStartRow, 1, levelStartRow, 2).Style.Font.Bold = true;

        var levelRow = levelStartRow + 1;
        foreach (var kvp in datasetStatistics.PartyLevelDistribution.OrderBy(x => x.Key))
        {
            statsSheet.Cell(levelRow, 1).Value = kvp.Key;
            statsSheet.Cell(levelRow, 2).Value = kvp.Value;
            levelRow++;
        }

        // Auto-fit columns
        statsSheet.Columns().AdjustToContents();

        workbook.SaveAs(filePath);
    }
}
