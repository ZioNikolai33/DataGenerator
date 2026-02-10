using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Interfaces;

public interface IDataGenerator
{
    Task GenerateAsync(Database database, List<Encounter> encountersDataset, DateTime startDate);
}