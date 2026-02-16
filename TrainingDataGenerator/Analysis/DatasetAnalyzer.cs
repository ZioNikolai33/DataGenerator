using System.Text.Json;
using TrainingDataGenerator.Analysis.Entities;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Analysis;

public class DatasetAnalyzer : IDatasetAnalyzer
{
    private readonly ILogger _logger;

    public DatasetAnalyzer(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AnalysisReport> AnalyzeDatasetAsync(string datasetPath)
    {
        var encounters = await LoadEncountersAsync(datasetPath);
        var report = new AnalysisReport();

        // Analyze distributions
        report.TotalEncounters = encounters.Count;
        report.OutcomeBalance = CalculateOutcomeBalance(encounters);
        report.DifficultyBalance = CalculateDifficultyBalance(encounters);
        report.ClassDistribution = CalculateClassDistribution(encounters);
        report.RaceDistribution = CalculateRaceDistribution(encounters);
        report.LevelDistribution = CalculateLevelDistribution(encounters);
        report.PartySizeDistribution = CalculatePartySizeDistribution(encounters);
        report.MonsterCountDistribution = CalculateMonsterCountDistribution(encounters);
        report.AverageTurnsToVictory = CalculateAverageTurnsToVictory(encounters);
        report.AveragePartyLevel = CalculateAveragePartyLevel(encounters);

        // Identify issues
        report.Issues = IdentifyDatasetIssues(report);

        LogReport(report);

        return report;
    }

    private async Task<List<Encounter>> LoadEncountersAsync(string datasetPath)
    {
        var encounters = new List<Encounter>();
        
        if (!Directory.Exists(datasetPath))
        {
            _logger.Warning($"Dataset path does not exist: {datasetPath}");
            return encounters;
        }

        var files = Directory.GetFiles(datasetPath, "*.json", SearchOption.AllDirectories);
        _logger.Information($"Found {files.Length} JSON files to analyze");

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var encounter = JsonSerializer.Deserialize<Encounter>(json);
                if (encounter != null)
                    encounters.Add(encounter);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Failed to deserialize {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        _logger.Information($"Successfully loaded {encounters.Count} encounters");
        return encounters;
    }

    private Dictionary<string, double> CalculateOutcomeBalance(List<Encounter> encounters)
    {
        if (encounters.Count == 0)
            return new Dictionary<string, double>();

        return encounters
            .GroupBy(e => e.Outcome?.ToString() ?? "Unknown")
            .ToDictionary(g => g.Key, g => (double)g.Count() / encounters.Count);
    }

    private Dictionary<CRRatios, double> CalculateDifficultyBalance(List<Encounter> encounters)
    {
        if (encounters.Count == 0)
            return new Dictionary<CRRatios, double>();

        return encounters
            .GroupBy(e => e.Difficulty)
            .ToDictionary(g => g.Key, g => (double)g.Count() / encounters.Count);
    }

    private Dictionary<string, double> CalculateClassDistribution(List<Encounter> encounters)
    {
        var totalCharacters = encounters.Sum(e => e.PartyMembers.Count);
        
        if (totalCharacters == 0)
            return new Dictionary<string, double>();

        return encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(p => p.Class ?? "Unknown")
            .ToDictionary(g => g.Key, g => (double)g.Count() / totalCharacters);
    }

    private Dictionary<string, double> CalculateRaceDistribution(List<Encounter> encounters)
    {
        var totalCharacters = encounters.Sum(e => e.PartyMembers.Count);
        
        if (totalCharacters == 0)
            return new Dictionary<string, double>();

        return encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(p => p.Race ?? "Unknown")
            .ToDictionary(g => g.Key, g => (double)g.Count() / totalCharacters);
    }

    private Dictionary<byte, int> CalculateLevelDistribution(List<Encounter> encounters)
    {
        return encounters
            .SelectMany(e => e.PartyMembers)
            .GroupBy(p => p.Level)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private Dictionary<int, int> CalculatePartySizeDistribution(List<Encounter> encounters)
    {
        return encounters
            .GroupBy(e => e.PartyMembers.Count)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private Dictionary<int, int> CalculateMonsterCountDistribution(List<Encounter> encounters)
    {
        return encounters
            .GroupBy(e => e.Monsters.Count)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private Dictionary<string, double> CalculateAverageTurnsToVictory(List<Encounter> encounters)
    {
        var victoriesWithTurns = encounters
            .Where(e => e.Outcome.Equals(Results.Victory) && e.Outcome.TotalRounds > 0)
            .ToList();

        if (victoriesWithTurns.Count == 0)
            return new Dictionary<string, double>();

        return new Dictionary<string, double>
        {
            ["OverallAverage"] = victoriesWithTurns.Average(e => e.Outcome?.TotalRounds ?? 0),
            ["Cakewalk"] = CalculateAverageRoundsByDifficulty(victoriesWithTurns, CRRatios.Cakewalk),
            ["Easy"] = CalculateAverageRoundsByDifficulty(victoriesWithTurns, CRRatios.Easy),
            ["Normal"] = CalculateAverageRoundsByDifficulty(victoriesWithTurns, CRRatios.Normal),
            ["Hard"] = CalculateAverageRoundsByDifficulty(victoriesWithTurns, CRRatios.Hard),
            ["Deadly"] = CalculateAverageRoundsByDifficulty(victoriesWithTurns, CRRatios.Deadly),
            ["Impossible"] = CalculateAverageRoundsByDifficulty(victoriesWithTurns, CRRatios.Impossible)
        };
    }

    private double CalculateAverageRoundsByDifficulty(List<Encounter> encounters, CRRatios difficulty)
    {
        var filtered = encounters.Where(e => e.Difficulty == difficulty).ToList();
        return filtered.Count > 0 ? filtered.Average(e => e.Outcome?.TotalRounds ?? 0) : 0;
    }

    private double CalculateAveragePartyLevel(List<Encounter> encounters)
    {
        var allPartyMembers = encounters.SelectMany(e => e.PartyMembers).ToList();
        return allPartyMembers.Count > 0 ? allPartyMembers.Average(p => p.Level) : 0;
    }

    private List<string> IdentifyDatasetIssues(AnalysisReport report)
    {
        var issues = new List<string>();

        // Check for severe outcome imbalance (>70% single outcome)
        if (report.OutcomeBalance.Any(kvp => kvp.Value > 0.7))
        {
            var dominant = report.OutcomeBalance.First(kvp => kvp.Value > 0.7);
            issues.Add($"Severe outcome imbalance detected: {dominant.Key} represents {dominant.Value:P1} of encounters");
        }

        // Check for missing outcomes (<1%)
        var missingOutcomes = report.OutcomeBalance
            .Where(kvp => kvp.Value < 0.01)
            .Select(kvp => kvp.Key);
        
        if (missingOutcomes.Any())
            issues.Add($"Severely underrepresented outcomes: {string.Join(", ", missingOutcomes)}");

        // Check for underrepresented classes (<1%)
        var underrepresentedClasses = report.ClassDistribution
            .Where(kvp => kvp.Value < 0.01)
            .Select(kvp => kvp.Key);
        
        if (underrepresentedClasses.Any())
            issues.Add($"Underrepresented classes: {string.Join(", ", underrepresentedClasses)}");

        // Check for underrepresented races (<1%)
        var underrepresentedRaces = report.RaceDistribution
            .Where(kvp => kvp.Value < 0.01)
            .Select(kvp => kvp.Key);
        
        if (underrepresentedRaces.Any())
            issues.Add($"Underrepresented races: {string.Join(", ", underrepresentedRaces)}");

        // Check for difficulty balance (should be relatively even)
        var difficultyImbalance = report.DifficultyBalance
            .Where(kvp => kvp.Value < 0.15 || kvp.Value > 0.35)
            .Select(kvp => $"{kvp.Key}:{kvp.Value:P1}");
        
        if (difficultyImbalance.Any())
            issues.Add($"Difficulty distribution imbalance: {string.Join(", ", difficultyImbalance)}");

        // Check for insufficient data
        if (report.TotalEncounters < 1000)
            issues.Add($"Small dataset size ({report.TotalEncounters} encounters). Consider generating at least 10,000 for ML training");

        // Check for party level concentration
        var dominantLevel = report.LevelDistribution
            .Where(kvp => (double)kvp.Value / report.TotalEncounters > 0.3)
            .Select(kvp => kvp.Key);
        
        if (dominantLevel.Any())
            issues.Add($"Level distribution concentrated around: {string.Join(", ", dominantLevel)}");

        return issues;
    }

    private void LogReport(AnalysisReport report)
    {
        _logger.Information("\n=== Dataset Analysis Report ===");
        _logger.Information($"Total Encounters: {report.TotalEncounters}");
        
        _logger.Information("\n--- Outcome Balance ---");
        foreach (var outcome in report.OutcomeBalance.OrderByDescending(kvp => kvp.Value))
        {
            _logger.Information($"{outcome.Key}: {outcome.Value:P2} ({(int)(outcome.Value * report.TotalEncounters)} encounters)");
        }

        _logger.Information("\n--- Difficulty Balance ---");
        foreach (var difficulty in report.DifficultyBalance.OrderBy(kvp => kvp.Key))
        {
            _logger.Information($"{difficulty.Key}: {difficulty.Value:P2} ({(int)(difficulty.Value * report.TotalEncounters)} encounters)");
        }

        _logger.Information("\n--- Top 10 Classes ---");
        foreach (var cls in report.ClassDistribution.OrderByDescending(kvp => kvp.Value).Take(10))
        {
            _logger.Information($"{cls.Key}: {cls.Value:P2}");
        }

        _logger.Information("\n--- Top 10 Races ---");
        foreach (var race in report.RaceDistribution.OrderByDescending(kvp => kvp.Value).Take(10))
        {
            _logger.Information($"{race.Key}: {race.Value:P2}");
        }

        _logger.Information("\n--- Level Distribution ---");
        foreach (var level in report.LevelDistribution.OrderBy(kvp => kvp.Key))
        {
            _logger.Information($"Level {level.Key}: {level.Value} characters");
        }

        _logger.Information("\n--- Party Size Distribution ---");
        foreach (var size in report.PartySizeDistribution.OrderBy(kvp => kvp.Key))
        {
            _logger.Information($"{size.Key} members: {size.Value} encounters");
        }

        _logger.Information("\n--- Monster Count Distribution ---");
        foreach (var count in report.MonsterCountDistribution.OrderBy(kvp => kvp.Key))
        {
            _logger.Information($"{count.Key} monsters: {count.Value} encounters");
        }

        _logger.Information("\n--- Average Combat Rounds (Party Victories) ---");
        foreach (var avg in report.AverageTurnsToVictory.OrderBy(kvp => kvp.Key))
        {
            _logger.Information($"{avg.Key}: {avg.Value:F2} rounds");
        }

        _logger.Information($"\nAverage Party Level: {report.AveragePartyLevel:F2}");

        if (report.Issues.Any())
        {
            _logger.Warning("\n--- Issues Detected ---");
            foreach (var issue in report.Issues)
            {
                _logger.Warning($"{issue}");
            }
        }
        else
        {
            _logger.Information("\n No major issues detected");
        }

        _logger.Information("\n=== End of Report ===\n");
    }
}