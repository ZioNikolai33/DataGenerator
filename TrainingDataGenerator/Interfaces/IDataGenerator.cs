using TrainingDataGenerator.DataBase;

namespace TrainingDataGenerator.Interfaces;

public interface IDataGenerator
{
    Task GenerateAsync(Database database, DateTime startDate);
}