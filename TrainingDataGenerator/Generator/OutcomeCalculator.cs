using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Generator;

public static class OutcomeCalculator
{
    public static Encounter CalculateOutcome(Encounter encounter)
    {
        Logger.Instance.Verbose($"Calculating outcome for encounter {encounter.Id}");

        var random = Random.Shared;
        var randomFactorParty = random.Next(-10, 11); // -10% to +10%
        var randomFactorMonsters = random.Next(-10, 11); // -10% to +10%

        Logger.Instance.Information($"Random factor for Party: {randomFactorParty}% - Random factor for Monsters: {randomFactorMonsters}%");

        var baseStatsParty = (int)encounter.PartyMembers.Average(m => m.CalculateBaseStats());
        Logger.Instance.Information($"Total Base Stats for Party: {baseStatsParty}");
        var baseStatsMonsters = (int)encounter.Monsters.Average(m => m.CalculateBaseStats());
        Logger.Instance.Information($"Total Base Stats for Monsters: {baseStatsMonsters}");

        var offensivePowerParty = encounter.PartyMembers.Sum(m => m.CalculateOffensivePower(encounter.Monsters, encounter.Difficulty));
        Logger.Instance.Verbose($"Total Offensive Power for Party: {offensivePowerParty}");
        var offensivePowerMonsters = encounter.Monsters.Sum(m => m.CalculateOffensivePower(encounter.PartyMembers, encounter.Difficulty));
        Logger.Instance.Verbose($"Total Offensive Power for Monsters: {offensivePowerMonsters}");

        var healingPowerParty = encounter.PartyMembers.Sum(m => m.CalculateHealingPower());
        Logger.Instance.Verbose($"Total Healing Power for Party: {healingPowerParty}");
        var healingPowerMonsters = encounter.Monsters.Sum(m => m.CalculateHealingPower());
        Logger.Instance.Verbose($"Total Healing Power for Monsters: {healingPowerMonsters}");

        var totalPartyCombatPower = offensivePowerParty + healingPowerParty;
        var totalMonstersCombatPower = offensivePowerMonsters + healingPowerMonsters;

        CombatCalculator.ApplyBaseStatsIncrement(baseStatsParty, baseStatsMonsters, ref totalPartyCombatPower, ref totalMonstersCombatPower);

        totalPartyCombatPower = (int)(totalPartyCombatPower * (1 + (randomFactorParty / 100.0)));
        Logger.Instance.Information($"Total Power for Party after random factor: {totalPartyCombatPower}");
        totalMonstersCombatPower = (int)(totalMonstersCombatPower * (1 + (randomFactorMonsters / 100.0)));
        Logger.Instance.Information($"Total Power for Monsters after random factor: {totalMonstersCombatPower}");

        if (totalPartyCombatPower <= 0)
            totalPartyCombatPower = 1;

        if (totalMonstersCombatPower <= 0)
            totalMonstersCombatPower = 1;

        encounter.Outcome = CombatCalculator.CalculateCombatOutcome(encounter.PartyMembers, encounter.Monsters, totalPartyCombatPower, totalMonstersCombatPower, baseStatsParty, baseStatsMonsters);

        return encounter;
    }
}
