using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;

namespace TrainingDataGenerator.Interfaces;

public interface ICombatCalculator
{
    void CalculatePowers<T>(List<T> targets, CRRatios difficulty) where T : ICombatCalculator;
    void SetBaseStats();
    void SetOffensivePower<T>(List<T> targets, CRRatios difficulty) where T : ICombatCalculator;
    void SetHealingPower();
    int CalculateSpeedValue();
    int CalculateStatsValue();
    int CalculateSkillsValue();
    double CalculateSpellUsagePercentage(Spell spell, CRRatios difficulty);
}
