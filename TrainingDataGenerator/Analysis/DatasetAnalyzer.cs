using ClosedXML.Excel;
using System.Data;
using TrainingDataGenerator.Analysis.Entities;
using TrainingDataGenerator.Analysis.Enums;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Analysis;

public class DatasetAnalyzer : IDatasetAnalyzer
{
    private readonly IExporterService _exporterService;
    private readonly ILogger _logger;

    public DatasetAnalyzer(IExporterService exporterService, ILogger logger)
    {
        _exporterService = exporterService ?? throw new ArgumentNullException(nameof(exporterService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void AnalyzeDatasetAsync(IEnumerable<Encounter> encounters, string startDate)
    {
        var report = new AnalysisReport();
        CreateReportData(report, encounters);
        ExportToExcel(report, startDate);
    }

    private void ExportToExcel(AnalysisReport report, string startDate)
    {
        var baseFolder = Directory.GetCurrentDirectory();
        var fileName = "analysis.xlsx";
        var batchFolderName = Path.Combine(baseFolder, "..", "..", "..", "Generator", "output", $"Batch_{startDate}", "analyses");
        var workbook = new XLWorkbook();

        CreateExcel(workbook, report);

        if (!Directory.Exists(batchFolderName))
        {
            Directory.CreateDirectory(batchFolderName);
            _logger.Verbose("Created batch folder");
        }

        var filePath = Path.Combine(batchFolderName, fileName);

        _exporterService.ExportToExcelAsync(workbook, filePath);
    }

    private void CreateReportData(AnalysisReport report, IEnumerable<Encounter> encounters)
    {
        report.TotalEncounters = encounters.Count();

        var analysisData = CreateDataTables("Class - HP", "Class", "Average HP", encounters);
        var classGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Class)
            .Select(g => new { Class = g.Key, AverageHP = g.Average(c => c.HitPoints) })
            .OrderBy(x => x.Class);
        foreach (var group in classGroups)
            analysisData.Data.Rows.Add(new object[] { group.Class, Math.Round(group.AverageHP, 0) });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Race - HP", "Race", "Average HP", encounters);
        var raceGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Race)
            .Select(g => new { Race = g.Key, AverageHP = g.Average(c => c.HitPoints) })
            .OrderBy(x => x.Race);
        foreach (var group in raceGroups)
            analysisData.Data.Rows.Add(new object[] { group.Race, Math.Round(group.AverageHP, 0) });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Level - Constitution - HP", "Level", " Average Constitution", "Average HP", encounters);
        var levelGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Level)
            .Select(g => new { Level = g.Key, AverageHP = g.Average(c => c.HitPoints), AverageConstitution = g.Average(c => c.Constitution.Value) })
            .OrderBy(x => x.Level);
        foreach (var group in levelGroups)
            analysisData.Data.Rows.Add(new object[] { group.Level, Math.Round(group.AverageConstitution, 0), Math.Round(group.AverageHP, 0) });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Class - CA", "Class", "CA", encounters);
        var caGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Class)
            .Select(g => new { Class = g.Key, AverageCA = g.Average(c => c.ArmorClass) })
            .OrderBy(x => x.Class);
        foreach (var group in caGroups)
            analysisData.Data.Rows.Add(new object[] { group.Class, Math.Round(group.AverageCA, 0) });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Class - Skills Proficiency", "Class", "Skills", encounters);
        var skillsGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Class)
            .Select(g => new { Class = g.Key, AverageSkills = g.Average(c => c.Skills.Count(s => s.IsProficient)) })
            .OrderBy(x => x.Class);
        foreach (var group in skillsGroups)
            analysisData.Data.Rows.Add(new object[] { group.Class, Math.Round(group.AverageSkills, 0) });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Class - Spells", "Class", "Spells", encounters);
        var spellsGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Class)
            .Select(g => new { Class = g.Key, AverageSpells = g.Average(c => c.Spells.Count()) })
            .OrderBy(x => x.Class);
        foreach (var group in spellsGroups)
            analysisData.Data.Rows.Add(new object[] { group.Class, Math.Round(group.AverageSpells, 0) });
        report.Analyses.Add(analysisData);             

        analysisData = CreateDataTables("Class - BaseStats", "Class", "BaseStats", encounters);
        var baseStatsGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Class)
            .Select(g => new { Class = g.Key, AverageBaseStats = g.Average(c => c.BaseStats) })
            .OrderBy(x => x.Class);
        foreach (var group in baseStatsGroups)
            analysisData.Data.Rows.Add(new object[] { group.Class, Math.Round(group.AverageBaseStats, 0) });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Class - OffensivePower", "Class", "OffensivePower", encounters);
        var offensiveStatsGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Class)
            .Select(g => new { Class = g.Key, AverageOffensivePower = g.Average(c => c.OffensivePower) })
            .OrderBy(x => x.Class);
        foreach (var group in offensiveStatsGroups)
            analysisData.Data.Rows.Add(new object[] { group.Class, Math.Round(group.AverageOffensivePower, 0) });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Class - HealingPower", "Class", "HealingPower", encounters);
        var healingGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Class)
            .Select(g => new { Class = g.Key, AverageHealing = g.Average(c => c.HealingPower) })
            .OrderBy(x => x.Class);
        foreach (var group in healingGroups)
            analysisData.Data.Rows.Add(new object[] { group.Class, Math.Round(group.AverageHealing, 0) });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Level - Power Balance", "Level", "Avg Offensive", "Avg Healing", encounters);
        var levelPowerBalance = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Level)
            .Select(g => new { Level = g.Key, AvgOffensive = g.Average(c => c.OffensivePower), AvgHealing = g.Average(c => c.HealingPower) })
            .OrderBy(x => x.Level);
        foreach (var group in levelPowerBalance)
            analysisData.Data.Rows.Add(new object[] { group.Level, Math.Round(group.AvgOffensive, 0), Math.Round(group.AvgHealing, 0) });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Party Size - Results", "Party Size", "Wins", "Losses", encounters);
        var partySizeGroups = encounters
            .Select(e => new { PartySize = e.PartyMembers.Count(), e.Outcome.Outcome })
            .GroupBy(x => x.PartySize)
            .Select(g => new { PartySize = g.Key, Wins = g.Count(x => x.Outcome == Results.Victory), Losses = g.Count(x => x.Outcome == Results.Defeat) })
            .OrderBy(x => x.PartySize);
        foreach (var group in partySizeGroups)
            analysisData.Data.Rows.Add(new object[] { group.PartySize, group.Wins, group.Losses });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Class - Results", "Class", "Wins", "Losses", encounters);
        var resultGroups = encounters
            .SelectMany(e => e.PartyMembers.Select(pm => new { pm.Class, e.Outcome.Outcome }))
            .GroupBy(x => x.Class)
            .Select(g => new { Class = g.Key, Wins = g.Count(x => x.Outcome == Results.Victory), Losses = g.Count(x => x.Outcome == Results.Defeat) })
            .OrderBy(x => x.Class);
        foreach (var group in resultGroups)
            analysisData.Data.Rows.Add(new object[] { group.Class, group.Wins, group.Losses });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Level - Results", "Level", "Wins", "Losses", encounters);
        var levelResultsGroups = encounters
            .SelectMany(e => e.PartyMembers.Select(pm => new { pm.Level, e.Outcome.Outcome }))
            .GroupBy(c => c.Level)
            .Select(g => new { Level = g.Key, Wins = g.Count(x => x.Outcome == Results.Victory), Losses = g.Count(x => x.Outcome == Results.Defeat) })
            .OrderBy(x => x.Level);
        foreach (var group in levelResultsGroups)
            analysisData.Data.Rows.Add(new object[] { group.Level, group.Wins, group.Losses });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Difficulty - Results", "Difficulty", "Wins", "Losses", encounters);
        var resultsDiffGroups = encounters
            .Select(e => new { e.Difficulty, e.Outcome.Outcome })
            .GroupBy(e => e.Difficulty)
            .Select(g => new { Difficulty = g.Key, Wins = g.Count(x => x.Outcome == Results.Victory), Losses = g.Count(x => x.Outcome == Results.Defeat) })
            .OrderBy(x => x.Difficulty);
        foreach (var group in resultsDiffGroups)
            analysisData.Data.Rows.Add(new object[] { group.Difficulty, group.Wins, group.Losses });
        report.Analyses.Add(analysisData);
    }

    private AnalysisData CreateDataTables(string sheetName, string firstColumn, string secondColumn, IEnumerable<Encounter> encounters)
    {
        var analysisData = new AnalysisData(sheetName, new DataTable());

        analysisData.Data.Columns.Add(firstColumn, typeof(string));
        analysisData.Data.Columns.Add(secondColumn, typeof(double));

        return analysisData;
    }

    private AnalysisData CreateDataTables(string sheetName, string firstColumn, string secondColumn, string thirdColumn, IEnumerable<Encounter> encounters)
    {
        var analysisData = new AnalysisData(sheetName, new DataTable());

        analysisData.Data.Columns.Add(firstColumn, typeof(string));
        analysisData.Data.Columns.Add(secondColumn, typeof(double));
        analysisData.Data.Columns.Add(thirdColumn, typeof(double));

        return analysisData;
    }

    private void CreateExcel(XLWorkbook workbook, AnalysisReport report)
    {
        foreach (var analysis in report.Analyses)
        {
            var worksheet = workbook.Worksheets.Add(analysis.SheetName);
            worksheet.Cell(1, 1).InsertTable(analysis.Data);
        }
    }
}