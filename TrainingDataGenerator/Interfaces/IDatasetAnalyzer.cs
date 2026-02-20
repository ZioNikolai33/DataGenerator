using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Interfaces;

public interface IDatasetAnalyzer
{
    void AnalyzeDatasetAsync(IEnumerable<Encounter> encounters, string startDate);
}