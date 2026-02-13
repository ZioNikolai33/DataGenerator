using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Validators.Entities;

namespace TrainingDataGenerator.Interfaces;

public interface IEncounterValidator
{
    ValidationResult ValidateEncounter(Encounter encounter);
    Task ValidateDatasetAsync(IEnumerable<Encounter> encounters, string startDate);
}
