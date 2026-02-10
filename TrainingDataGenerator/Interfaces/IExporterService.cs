using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Validators;

namespace TrainingDataGenerator.Interfaces;

public interface IExporterService
{
    Task ExportToJsonAsync<T>(T obj, string filePath);
    void ExportToExcelAsync(DatasetStatistics datasetStatistics, string filePath);
}
