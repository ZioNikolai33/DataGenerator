using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Generator;

public static class OutcomeCalculator
{
    public static Encounter CalculateOutcome(Encounter encounter, ILogger logger)
    {
        logger.Verbose($"Calculating outcome for encounter {encounter.Id}");

        var random = Random.Shared;
        var randomFactorParty = random.Next(-10, 11); // -10% to +10%
        var randomFactorMonsters = random.Next(-10, 11); // -10% to +10%

        logger.Information($"Random factor for Party: {randomFactorParty}% - Random factor for Monsters: {randomFactorMonsters}%");

        var baseStatsParty = (int)encounter.PartyMembers.Average(m => m.CalculateBaseStats());
        logger.Information($"Total Base Stats for Party: {baseStatsParty}");
        var baseStatsMonsters = (int)encounter.Monsters.Average(m => m.CalculateBaseStats());
        logger.Information($"Total Base Stats for Monsters: {baseStatsMonsters}");

        var offensivePowerParty = encounter.PartyMembers.Sum(m => m.CalculateOffensivePower(encounter.Monsters, encounter.Difficulty));
        logger.Information($"Total Offensive Power for Party: {offensivePowerParty}");
        var offensivePowerMonsters = encounter.Monsters.Sum(m => m.CalculateOffensivePower(encounter.PartyMembers, encounter.Difficulty));
        logger.Information($"Total Offensive Power for Monsters: {offensivePowerMonsters}");

        var healingPowerParty = encounter.PartyMembers.Sum(m => m.CalculateHealingPower());
        logger.Information($"Total Healing Power for Party: {healingPowerParty}");
        var healingPowerMonsters = encounter.Monsters.Sum(m => m.CalculateHealingPower());
        logger.Information($"Total Healing Power for Monsters: {healingPowerMonsters}");

        var totalPartyCombatPower = offensivePowerParty + healingPowerParty;
        var totalMonstersCombatPower = offensivePowerMonsters + healingPowerMonsters;

        CombatCalculator.ApplyBaseStatsIncrement(baseStatsParty, baseStatsMonsters, ref totalPartyCombatPower, ref totalMonstersCombatPower);

        totalPartyCombatPower = (int)(totalPartyCombatPower * (1 + (randomFactorParty / 100.0)));
        logger.Information($"Total Power for Party after random factor: {totalPartyCombatPower}");
        totalMonstersCombatPower = (int)(totalMonstersCombatPower * (1 + (randomFactorMonsters / 100.0)));
        logger.Information($"Total Power for Monsters after random factor: {totalMonstersCombatPower}");

        if (totalPartyCombatPower <= 0)
            totalPartyCombatPower = 1;

        if (totalMonstersCombatPower <= 0)
            totalMonstersCombatPower = 1;

        encounter.Outcome = CombatCalculator.CalculateCombatOutcome(encounter.PartyMembers, encounter.Monsters, totalPartyCombatPower, totalMonstersCombatPower, baseStatsParty, baseStatsMonsters, logger);

        return encounter;
    }
}
