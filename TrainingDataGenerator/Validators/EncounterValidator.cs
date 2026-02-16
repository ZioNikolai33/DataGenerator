using ClosedXML.Excel;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Validators.Entities;

namespace TrainingDataGenerator.Validators;

public class EncounterValidator : IEncounterValidator
{
    private readonly ILogger _logger;
    private readonly IExporterService _exporterService;

    public EncounterValidator(ILogger logger, IExporterService exporterService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _exporterService = exporterService ?? throw new ArgumentNullException(nameof(exporterService));
    }

    public ValidationResult ValidateEncounter(Encounter encounter)
    {
        var errors = new List<string>();

        ValidateGeneralInfo(encounter, errors);
        ValidateParty(encounter.PartyMembers, errors);
        ValidateMonsters(encounter.Monsters, errors);
        ValidateOutcome(encounter.Outcome, errors);

        return new ValidationResult(errors);
    }

    private void ValidateGeneralInfo(Encounter encounter, List<string> errors)
    {
        if (encounter == null)
        {
            errors.Add("Encounter cannot be null");
            return;
        }

        if (string.IsNullOrWhiteSpace(encounter.Id))
            errors.Add("Encounter ID cannot be empty");

        if (!System.Text.RegularExpressions.Regex.IsMatch(encounter.Id, @"^E\d{8}-[A-Z]$"))
            errors.Add("Encounter ID must follow the pattern: 'E' followed by 8 digits, a '-', and a single character difficulty");
    }

    private void ValidateParty(IEnumerable<PartyMember> partyMembers, List<string> errors)
    {
        if (!partyMembers.Any())
            errors.Add("Party cannot be empty");

        if (partyMembers.Count() > 7)
            errors.Add("Party cannot have more than 7 members");

        if (partyMembers.Any(p => string.IsNullOrWhiteSpace(p.Class)))
            errors.Add("Party member class cannot be empty");

        if (partyMembers.Any(p => string.IsNullOrWhiteSpace(p.Race)))
            errors.Add("Party member race cannot be empty");

        if (partyMembers.Any(p => p.Strength == null || p.Dexterity == null || p.Constitution == null || p.Intelligence == null || p.Wisdom == null || p.Charisma == null))
            errors.Add("Party member attributes cannot be null");

        if (partyMembers.Any(p => p.Strength.Value < 1 || p.Strength.Value > 24))
            errors.Add("Party member strength must be between 1 and 24");

        if (partyMembers.Any(p => p.Dexterity.Value < 1 || p.Dexterity.Value > 20))
            errors.Add("Party member dexterity must be between 1 and 20");

        if (partyMembers.Any(p => p.Constitution.Value < 1 || p.Constitution.Value > 24))
            errors.Add("Party member constitution must be between 1 and 24");

        if (partyMembers.Any(p => p.Intelligence.Value < 1 || p.Intelligence.Value > 20))
            errors.Add("Party member intelligence must be between 1 and 20");

        if (partyMembers.Any(p => p.Wisdom.Value < 1 || p.Wisdom.Value > 20))
            errors.Add("Party member wisdom must be between 1 and 20");

        if (partyMembers.Any(p => p.Charisma.Value < 1 || p.Charisma.Value > 20))
            errors.Add("Party member charisma must be between 1 and 20");

        if (partyMembers.Any(p => p.HitPoints < 5))
            errors.Add("Party member hit points must be at least 5");

        if (partyMembers.Any(p => p.ArmorClass < 9))
            errors.Add("Party member armor class must be at least 9");

        if (partyMembers.Any(p => p.ProficiencyBonus < 2))
            errors.Add("Party member proficiency bonus must be at least 2");

        if (partyMembers.Any(p => p.Skills == null || p.Skills.Count != 18))
            errors.Add("Party member must have 18 skills");

        if (partyMembers.Any(p => p.Level < 1 || p.Level > 20))
            errors.Add("Invalid party member level");

        if (partyMembers.Any(p => p.HitDie < 6 || p.HitDie > 12))
            errors.Add("Party member hit die must be between d6 and d12");

        if (partyMembers.Any(p => p.Speed < 25))
            errors.Add("Party member speed must be at least 25");

        if (partyMembers.Any(p => p.Initiative < -1))
            errors.Add("Party member initiative must be at least -1");
    }

    private void ValidateMonsters(IEnumerable<Monster> monsters, List<string> errors)
    {
        if (!monsters.Any())
            errors.Add("Monsters cannot be empty");

        if (monsters.Count() > 15)
            errors.Add("Encounter cannot have more than 15 monsters");

        if (monsters.Any(m => string.IsNullOrWhiteSpace(m.Name)))
            errors.Add("Monster name cannot be empty");
    }

    private void ValidateOutcome(Result outcome, List<string> errors)
    {
        if (outcome == null)
        {
            errors.Add("Outcome cannot be null");
            return;
        }            

        if (outcome.Outcome.Equals(Results.Undecided))
            errors.Add("Outcome cannot be unknown");
    }

    public async Task ValidateDatasetAsync(IEnumerable<Encounter> encounters, string startDate)
    {
        var errors = new List<string>();
        var stats = new DatasetStatistics();

        foreach (var encounter in encounters)
        {
            var result = ValidateEncounter(encounter);

            if (!result.IsValid)
                errors.AddRange(result.Errors);

            stats.Update(encounter, result.IsValid);
        }

        // Check for balance
        stats.CalculatePercentage();

        if (stats.OutcomeDistribution.Any(kvp => kvp.Count < stats.TotalEncounters * 0.1))
            errors.Add($"Imbalanced outcome distribution: {string.Join(", ", stats.OutcomeDistribution)}");

        var validationResult = new ValidationResult(errors, stats);
        SaveValidationStatsAsync(validationResult.Statistics ?? new DatasetStatistics(), startDate);
        await SaveValidationErrorsAsync(validationResult.Errors, startDate);
    }

    private async Task SaveValidationErrorsAsync(List<string> errors, string startDate)
    {
        var baseFolder = Directory.GetCurrentDirectory();
        var fileName = "validation_errors.json";
        var batchFolderName = Path.Combine(baseFolder, "..", "..", "..", "Generator", "output", $"Batch_{startDate}", "validation");

        if (!Directory.Exists(batchFolderName))
        {
            Directory.CreateDirectory(batchFolderName);
            _logger.Verbose("Created batch folder");
        }

        var filePath = Path.Combine(batchFolderName, fileName);

        await _exporterService.ExportToJsonAsync(errors, filePath);
    }

    private void SaveValidationStatsAsync(DatasetStatistics validationResult, string startDate)
    {
        var baseFolder = Directory.GetCurrentDirectory();
        var fileName = "validation.xlsx";
        var batchFolderName = Path.Combine(baseFolder, "..", "..", "..", "Generator", "output", $"Batch_{startDate}", "validation");
        var workbook = new XLWorkbook();

        CreateExcel(workbook, validationResult);

        if (!Directory.Exists(batchFolderName))
        {
            Directory.CreateDirectory(batchFolderName);
            _logger.Verbose("Created batch folder");
        }

        var filePath = Path.Combine(batchFolderName, fileName);

        _exporterService.ExportToExcelAsync(workbook, filePath);
    }

    #region Excel Creation

    private void CreateExcel(XLWorkbook workbook, DatasetStatistics datasetStatistics)
    {
        var statsSheet = workbook.Worksheets.Add("Statistics");

        CreateSummaryLine(statsSheet, 1, "Total Encounters", datasetStatistics.TotalEncounters); // Summary Statistics: Total Encounters
        CreateSummaryLine(statsSheet, 2, "Valid Encounters", datasetStatistics.ValidEncounters); // Summary Statistics: Valid Encounters
        CreateSummaryLine(statsSheet, 3, "Invalid Encounters", datasetStatistics.InvalidEncounters); // Summary Statistics: Invalid Encounters
        CreateSummaryLine(statsSheet, 4, "Encounters Won (Party)", datasetStatistics.OutcomeDistribution.Where(o => o.Value.Equals(Results.Victory.ToString())).Select(o => o.Count).FirstOrDefault()); // Summary Statistics: Encounter Won (Party)
        CreateSummaryLine(statsSheet, 5, "Encounters Lost (Party)", datasetStatistics.OutcomeDistribution.Where(o => o.Value.Equals(Results.Defeat.ToString())).Select(o => o.Count).FirstOrDefault());// Summary Statistics: Encounter Lost (Party)

        // Difficulty Distribution Table
        var startDifficultyCol = 4;
        CreateCountTable(statsSheet, 1, startDifficultyCol, datasetStatistics.DifficultyDistribution, "Difficulty", "Count");

        // Party Level Distribution Table
        var startLevelCol = startDifficultyCol + 4;
        CreateCountTable(statsSheet, 1, startLevelCol, datasetStatistics.PartyLevelDistribution, "Party Level", "Count");

        // Party Class Distribution Table
        var startClassCol = startLevelCol + 4;
        CreateCountTable(statsSheet, 1, startClassCol, datasetStatistics.PartyClassDistribution, "Party Class", "Count");

        // Party Race Distribution Table
        var startRaceCol = startClassCol + 4;
        CreateCountTable(statsSheet, 1, startRaceCol, datasetStatistics.PartyRaceDistribution, "Party Race", "Count");

        // Party Size Distribution Table
        var startSizeCol = startRaceCol + 4;
        CreateCountTable(statsSheet, 1, startSizeCol, datasetStatistics.PartySizeDistribution, "Party Size", "Count");

        // Monster CR Distribution Table
        var startCRCol = startSizeCol + 4;
        CreateCountTable(statsSheet, 1, startCRCol, datasetStatistics.MonsterCRDistribution, "Monster CR", "Count");

        // Monster Count Distribution Table
        var startMonsterCountCol = startCRCol + 4;
        CreateCountTable(statsSheet, 1, startMonsterCountCol, datasetStatistics.MonsterCountDistribution, "Monster Count", "Count");

        // Combat Duration Distribution Table
        var startDurationCol = startMonsterCountCol + 4;
        CreateCountTable(statsSheet, 1, startDurationCol, datasetStatistics.CombatDurationDistribution, "Combat Duration", "Count");

        // Auto-fit columns
        statsSheet.Columns().AdjustToContents();
    }

    private void CreateSummaryLine(IXLWorksheet sheet, int row, string label, int value)
    {
        sheet.Cell(row, 1).Value = label;
        sheet.Cell(row, 2).Value = value;
        sheet.Range(row, 1, row, 2).Style.Font.Bold = true;
    }

    private void CreateCountTable(IXLWorksheet sheet, int startRow, int startCol, IEnumerable<Distribution<string>> data, string header1, string header2)
    {
        sheet.Cell(startRow, startCol).Value = header1;
        sheet.Cell(startRow, startCol + 1).Value = header2;
        sheet.Cell(startRow, startCol + 2).Value = "Percentage";
        sheet.Range(startRow, startCol, startRow, startCol + 2).Style.Font.Bold = true;

        var row = startRow + 1;

        foreach (var kvp in data.OrderBy(x => x.Value))
        {
            sheet.Cell(row, startCol).Value = kvp.Value;
            sheet.Cell(row, startCol + 1).Value = kvp.Count;
            sheet.Cell(row, startCol + 2).Value = kvp.Percentage;
            sheet.Cell(row, startCol + 2).Style.NumberFormat.Format = "0.0%";

            row++;
        }
    }

    #endregion
}