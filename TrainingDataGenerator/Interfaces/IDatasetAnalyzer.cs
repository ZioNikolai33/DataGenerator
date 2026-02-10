using TrainingDataGenerator.Analysis;

namespace TrainingDataGenerator.Interfaces;

public interface IDatasetAnalyzer
{
    Task<AnalysisReport> AnalyzeDatasetAsync(string datasetPath);
}