using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Services;

public class AnalyzerRunner
{
    private readonly IDatasetAnalyzer _analyzer;
    private readonly ILogger _logger;

    public AnalyzerRunner(IDatasetAnalyzer analyzer, ILogger logger)
    {
        _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task RunAnalysisAsync(string datasetPath, string? outputPath = null)
    {
        try
        {
            _logger.Information("Starting dataset analysis...");
            
            var report = await _analyzer.AnalyzeDatasetAsync(datasetPath);

            if (!string.IsNullOrEmpty(outputPath))
            {
                var reportJson = report.ToJson();
                await File.WriteAllTextAsync(outputPath, reportJson);
                _logger.Information($"Analysis report saved to: {outputPath}");
            }

            _logger.Information("Dataset analysis completed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error during dataset analysis: {ex}");
            throw;
        }
    }
}