using TrainingDataGenerator.Analysis.Entities;

namespace TrainingDataGenerator.Interfaces;

public interface IDatasetAnalyzer
{
    Task<AnalysisReport> AnalyzeDatasetAsync(string datasetPath);
}