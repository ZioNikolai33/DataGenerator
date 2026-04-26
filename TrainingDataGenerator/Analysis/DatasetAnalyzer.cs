using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Reflection;
using TrainingDataGenerator.Analysis.Entities;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Analysis;

public class DatasetAnalyzer : IDatasetAnalyzer
{
    private readonly IExporterService _exporterService;
    private readonly ILogger _logger;
    private readonly Config _config;

    public DatasetAnalyzer(IExporterService exporterService, ILogger logger)
    {
        _exporterService = exporterService ?? throw new ArgumentNullException(nameof(exporterService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = Config.LoadConfig();
    }

    public void AnalyzeDatasetAsync(IEnumerable<Encounter> encounters, string startDate)
    {
        var report = new AnalysisReport();
        CreateReportData(report, encounters);
        ExportToExcel(report, startDate);
    }

    private void ExportToExcel(AnalysisReport report, string startDate)
    {
        var fileName = "analysis.xlsx";
        var batchFolderName = Path.Combine(_config.OutputFolder, $"Batch_{startDate}", "analyses");
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
        var analysesParty = new List<AnalysisData>();
        var analysesMonsters = new List<AnalysisData>();
        report.TotalEncounters = encounters.Count();

        foreach (var encounter in encounters)
            foreach (var member in encounter.PartyMembers)
                analysesParty.Add(new AnalysisData(encounter.Difficulty, encounter.Id, member, encounter.Outcome, encounter.PartyMembers.Count()));

        foreach (var encounter in encounters)
            foreach (var monster in encounter.Monsters)
                analysesMonsters.Add(new AnalysisData(encounter.Difficulty, encounter.Id, monster, encounter.Outcome, encounter.PartyMembers.Count()));

        report.PartyHealth = ConvertToDataTable(analysesParty.Select(p => new { p.Class, p.Race, p.HitPoints, p.Constitution, p.Level }).ToList());
        report.PartyAttributes = ConvertToDataTable(analysesParty.Select(p => new { p.Class, p.Strength, p.Dexterity, p.Constitution, p.Intelligence, p.Wisdom, p.Charisma }).ToList());
        report.PartyStats = ConvertToDataTable(analysesParty.Select(p => new { p.Class, p.Level, p.BaseStatsPower, p.OffensivePower, p.HealingPower }).ToList());
        report.MonsterStats = ConvertToDataTable(analysesMonsters.Select(p => new { p.Id, p.Name, p.Difficulty, p.ChallengeRating, p.BaseStatsPower, p.OffensivePower, p.HealingPower, p.Result }).ToList());
        report.Results = ConvertToDataTable(analysesMonsters.Select(e => new { e.Id, e.Difficulty, e.Result, e.Exp, e.PartyMemberCount })
            .GroupBy(e => e.Id)
            .Select(g => new { g.Key, g.First().Difficulty, Result = g.First().Result, Exp = g.Sum(x => x.Exp), PartyMemberCount = g.First().PartyMemberCount })
            .ToList());

        var analysisData = CreateDataTables("Difficulty - Results", "Difficulty", "Wins", "Losses", encounters);
        var resultsDiffGroups = encounters
            .Select(e => new { e.Difficulty, e.Outcome.Outcome })
            .GroupBy(e => e.Difficulty)
            .Select(g => new { Difficulty = g.Key, Wins = g.Count(x => x.Outcome == Results.Victory), Losses = g.Count(x => x.Outcome == Results.Defeat) })
            .OrderBy(x => x.Difficulty);
        foreach (var group in resultsDiffGroups)
            analysisData.Data.Rows.Add(new object[] { group.Difficulty, group.Wins, group.Losses });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Class - Melee - Ranged", "Class", "Melee", "Ranged", encounters);
        var equipmentGroups = encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(c => c.Class)
            .Select(g => new { Class = g.Key, AverageMelee = g.Average(c => c.MeleeWeapons.Count()), AverageRanged = g.Average(c => c.RangedWeapons.Count()) })
            .OrderBy(x => x.Class);
        foreach (var group in equipmentGroups)
            analysisData.Data.Rows.Add(new object[] { group.Class, Math.Round(group.AverageMelee, 0), Math.Round(group.AverageRanged, 0) });
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

        analysisData = CreateDataTables("Monsters - CR - BaseStats", "Monster", "CR", "BaseStats", encounters);
        var monsterBaseGroups = encounters
            .SelectMany(e => e.Monsters)
            .GroupBy(m => m.Name)
            .Select(g => new { Monster = g.Key, CR = g.First().ChallengeRating, BaseStats = g.Average(m => m.BaseStats) })
            .OrderBy(x => x.Monster);
        foreach (var group in monsterBaseGroups)
            analysisData.Data.Rows.Add(new object[] { group.Monster, group.CR, group.BaseStats });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Monsters - CR - OffensivePower", "Monster", "CR", "OffensivePower", encounters);
        var monsterOffensiveGroups = encounters
            .SelectMany(e => e.Monsters)
            .GroupBy(m => m.Name)
            .Select(g => new { Monster = g.Key, CR = g.First().ChallengeRating, OffensivePower = (int)g.Average(m => m.OffensivePower) })
            .OrderBy(x => x.Monster);
        foreach (var group in monsterOffensiveGroups)
            analysisData.Data.Rows.Add(new object[] { group.Monster, group.CR, group.OffensivePower });
        report.Analyses.Add(analysisData);

        analysisData = CreateDataTables("Monsters - CR - HealingPower", "Monster", "CR", "HealingPower", encounters);
        var monsterHealingGroups = encounters
            .SelectMany(e => e.Monsters)
            .GroupBy(m => m.Name)
            .Select(g => new { Monster = g.Key, CR = g.First().ChallengeRating, HealingPower = (int)g.Average(m => m.HealingPower) })
            .OrderBy(x => x.Monster);
        foreach (var group in monsterHealingGroups)
            analysisData.Data.Rows.Add(new object[] { group.Monster, group.CR, group.HealingPower });
        report.Analyses.Add(analysisData);
    }

    private Entities.Analysis CreateDataTables(string sheetName, string firstColumn, string secondColumn, IEnumerable<Encounter> encounters)
    {
        var analysisData = new Entities.Analysis(sheetName, new DataTable());

        analysisData.Data.Columns.Add(firstColumn, typeof(string));
        analysisData.Data.Columns.Add(secondColumn, typeof(double));

        return analysisData;
    }

    private Entities.Analysis CreateDataTables(string sheetName, string firstColumn, string secondColumn, string thirdColumn, IEnumerable<Encounter> encounters)
    {
        var analysisData = new Entities.Analysis(sheetName, new DataTable());

        analysisData.Data.Columns.Add(firstColumn, typeof(string));
        analysisData.Data.Columns.Add(secondColumn, typeof(double));
        analysisData.Data.Columns.Add(thirdColumn, typeof(double));

        return analysisData;
    }

    private DataTable ConvertToDataTable<T>(IEnumerable<T> data)
    {
        var dataTable = new DataTable();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var propertyType = property.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            dataTable.Columns.Add(property.Name, underlyingType);
        }

        foreach (var item in data)
        {
            var row = dataTable.NewRow();
            foreach (var property in properties)
            {
                var value = property.GetValue(item);
                row[property.Name] = value ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    private void CreateExcel(XLWorkbook workbook, AnalysisReport report)
    {
        var worksheet = workbook.Worksheets.Add("Results");
        worksheet.Cell(1, 1).InsertTable(report.Results);

        worksheet = workbook.Worksheets.Add("Party Health");
        worksheet.Cell(1, 1).InsertTable(report.PartyHealth);

        worksheet = workbook.Worksheets.Add("Party Attributes");
        worksheet.Cell(1, 1).InsertTable(report.PartyAttributes);

        worksheet = workbook.Worksheets.Add("Party Stats");
        worksheet.Cell(1, 1).InsertTable(report.PartyStats);

        worksheet = workbook.Worksheets.Add("Monster Stats");
        worksheet.Cell(1, 1).InsertTable(report.MonsterStats);

        foreach (var analysis in report.Analyses)
        {
            worksheet = workbook.Worksheets.Add(analysis.SheetName);
            worksheet.Cell(1, 1).InsertTable(analysis.Data);
        }
    }
}