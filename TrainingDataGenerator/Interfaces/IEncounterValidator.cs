using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Validators;

namespace TrainingDataGenerator.Interfaces;

public interface IEncounterValidator
{
    ValidationResult ValidateEncounter(Encounter encounter);
    Task ValidateDatasetAsync(IEnumerable<Encounter> encounters, DateTime startDate);
}
