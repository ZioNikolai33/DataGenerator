using ClosedXML.Excel;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Validators.Entities;

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

    public void ExportToExcelAsync(DatasetStatistics datasetStatistics, string filePath)
    {
        using var workbook = new XLWorkbook();
        var statsSheet = workbook.Worksheets.Add("Statistics");
                
        CreateSummaryLine(statsSheet, 1, "Total Encounters", datasetStatistics.TotalEncounters); // Summary Statistics: Total Encounters
        CreateSummaryLine(statsSheet, 2, "Valid Encounters", datasetStatistics.ValidEncounters); // Summary Statistics: Valid Encounters
        CreateSummaryLine(statsSheet, 3, "Invalid Encounters", datasetStatistics.InvalidEncounters); // Summary Statistics: Invalid Encounters
        CreateSummaryLine(statsSheet, 4, "Encounters Won (Party)", datasetStatistics.OutcomeDistribution.Where(o => o.Key.Equals(Results.Victory.ToString())).Select(o => o.Value).FirstOrDefault()); // Summary Statistics: Encounter Won (Party)
        CreateSummaryLine(statsSheet, 5, "Encounters Lost (Party)", datasetStatistics.OutcomeDistribution.Where(o => o.Key.Equals(Results.Defeat.ToString())).Select(o => o.Value).FirstOrDefault());// Summary Statistics: Encounter Lost (Party)

        // Difficulty Distribution Table
        var startDifficultyCol = 4;
        CreateCountTable(statsSheet, 1, startDifficultyCol, datasetStatistics.DifficultyDistribution, "Difficulty", "Count");

        // Party Level Distribution Table
        var startLevelCol = startDifficultyCol + 3;
        CreateCountTable(statsSheet, 1, startLevelCol, datasetStatistics.PartyLevelDistribution, "Party Level", "Count");

        // Party Class Distribution Table
        var startClassCol = startLevelCol + 3;
        CreateCountTable(statsSheet, 1, startClassCol, datasetStatistics.PartyClassDistribution, "Party Class", "Count");

        // Party Race Distribution Table
        var startRaceCol = startClassCol + 3;
        CreateCountTable(statsSheet, 1, startRaceCol, datasetStatistics.PartyRaceDistribution, "Party Race", "Count");

        // Party Size Distribution Table
        var startSizeCol = startRaceCol + 3;
        CreateCountTable(statsSheet, 1, startSizeCol, datasetStatistics.PartySizeDistribution, "Party Size", "Count");

        // Monster CR Distribution Table
        var startCRCol = startSizeCol + 3;
        CreateCountTable(statsSheet, 1, startCRCol, datasetStatistics.MonsterCRDistribution, "Monster CR", "Count");

        // Monster Count Distribution Table
        var startMonsterCountCol = startCRCol + 3;
        CreateCountTable(statsSheet, 1, startMonsterCountCol, datasetStatistics.MonsterCountDistribution, "Monster Count", "Count");

        // Combat Duration Distribution Table
        var startDurationCol = startMonsterCountCol + 3;
        CreateCountTable(statsSheet, 1, startDurationCol, datasetStatistics.CombatDurationDistribution, "Combat Duration (Rounds)", "Count");

        // Auto-fit columns
        statsSheet.Columns().AdjustToContents();

        workbook.SaveAs(filePath);
    }

    private void CreateSummaryLine(IXLWorksheet sheet, int row, string label, int value)
    {
        sheet.Cell(row, 1).Value = label;
        sheet.Cell(row, 2).Value = value;
        sheet.Range(row, 1, row, 2).Style.Font.Bold = true;
    }

    private void CreateCountTable(IXLWorksheet sheet, int startRow, int startCol, IEnumerable<KeyValuePair<string, int>> data, string header1, string header2)
    {
        sheet.Cell(startRow, startCol).Value = header1;
        sheet.Cell(startRow, startCol + 1).Value = header2;
        sheet.Range(startRow, startCol, startRow, startCol + 1).Style.Font.Bold = true;

        var row = startRow + 1;

        foreach (var kvp in data.OrderBy(x => x.Key))
        {
            sheet.Cell(row, startCol).Value = kvp.Key;
            sheet.Cell(row, startCol + 1).Value = kvp.Value;
            row++;
        }
    }

    private void CreateCountTable(IXLWorksheet sheet, int startRow, int startCol, IEnumerable<KeyValuePair<CRRatios, int>> data, string header1, string header2)
    {
        sheet.Cell(startRow, startCol).Value = header1;
        sheet.Cell(startRow, startCol + 1).Value = header2;
        sheet.Range(startRow, startCol, startRow, startCol + 1).Style.Font.Bold = true;

        var row = startRow + 1;

        foreach (var kvp in data.OrderBy(x => x.Key))
        {
            sheet.Cell(row, startCol).Value = kvp.Key.ToString();
            sheet.Cell(row, startCol + 1).Value = kvp.Value;
            row++;
        }
    }

    private void CreateCountTable(IXLWorksheet sheet, int startRow, int startCol, IEnumerable<KeyValuePair<int, int>> data, string header1, string header2)
    {
        sheet.Cell(startRow, startCol).Value = header1;
        sheet.Cell(startRow, startCol + 1).Value = header2;
        sheet.Range(startRow, startCol, startRow, startCol + 1).Style.Font.Bold = true;

        var row = startRow + 1;

        foreach (var kvp in data.OrderBy(x => x.Key))
        {
            sheet.Cell(row, startCol).Value = kvp.Key;
            sheet.Cell(row, startCol + 1).Value = kvp.Value;
            row++;
        }
    }

    private void CreateCountTable(IXLWorksheet sheet, int startRow, int startCol, IEnumerable<KeyValuePair<double, int>> data, string header1, string header2)
    {
        sheet.Cell(startRow, startCol).Value = header1;
        sheet.Cell(startRow, startCol + 1).Value = header2;
        sheet.Range(startRow, startCol, startRow, startCol + 1).Style.Font.Bold = true;

        var row = startRow + 1;

        foreach (var kvp in data.OrderBy(x => x.Key))
        {
            sheet.Cell(row, startCol).Value = kvp.Key;
            sheet.Cell(row, startCol + 1).Value = kvp.Value;
            row++;
        }
    }
}
