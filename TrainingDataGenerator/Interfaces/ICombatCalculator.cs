using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Interfaces;

public interface ICombatCalculator
{
    int CalculateBaseStats();
    int CalculateSpeedValue();
    int CalculateStatsValue();
    int CalculateSkillsValue();
    int CalculateOffensivePower<T>(List<T> targets, CRRatios difficulty) where T : ICombatCalculator;
    int CalculateHealingPower();
    double CalculateSpellUsagePercentage(Spell spell, CRRatios difficulty);
}
