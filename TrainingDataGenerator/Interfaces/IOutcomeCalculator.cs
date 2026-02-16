using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Interfaces;

public interface IOutcomeCalculator
{
    Encounter CalculateOutcome(Encounter encounter);
}