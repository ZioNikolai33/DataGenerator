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
        if (stats.OutcomeDistribution.Any(kvp => kvp.Value < stats.TotalEncounters * 0.1))
            errors.Add($"Imbalanced outcome distribution: {string.Join(", ", stats.OutcomeDistribution)}");

        var validationResult = new ValidationResult(errors, stats);
        SaveValidationStatsAsync(validationResult.Statistics ?? new DatasetStatistics(), startDate);
        await SaveValidationErrorsAsync(validationResult.Errors, startDate);
    }

    private void SaveValidationStatsAsync(DatasetStatistics validationResult, string startDate)
    {
        var baseFolder = Directory.GetCurrentDirectory();
        var fileName = "validation.xlsx";
        var batchFolderName = Path.Combine(baseFolder, "..", "..", "..", "Generator", "output", $"Batch_{startDate}");

        if (!Directory.Exists(batchFolderName))
        {
            Directory.CreateDirectory(batchFolderName);
            _logger.Verbose("Created batch folder");
        }

        var filePath = Path.Combine(batchFolderName, fileName);

        _exporterService.ExportToExcelAsync(validationResult, filePath);
    }

    private async Task SaveValidationErrorsAsync(List<string> errors, string startDate)
    {
        var baseFolder = Directory.GetCurrentDirectory();
        var fileName = "validation_errors.json";
        var batchFolderName = Path.Combine(baseFolder, "..", "..", "..", "Generator", "output", $"Batch_{startDate}");

        if (!Directory.Exists(batchFolderName))
        {
            Directory.CreateDirectory(batchFolderName);
            _logger.Verbose("Created batch folder");
        }

        var filePath = Path.Combine(batchFolderName, fileName);

        await _exporterService.ExportToJsonAsync(errors, filePath);
    }
}