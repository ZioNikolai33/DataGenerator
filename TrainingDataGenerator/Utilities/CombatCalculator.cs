using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Utilities;

public static class CombatCalculator
{
    public static double CalculateRollPercentage(int toReach, int bonus, short diceNumber = 20)
    {
        var hitPercentage = 0.0;

        for (var roll = 1; roll <= diceNumber; roll++)
        {
            if (diceNumber == 20 && roll == 1)
                continue;

            if (roll == diceNumber)
            {
                hitPercentage += (double)1 / diceNumber;
                continue;
            }

            if (roll + bonus >= toReach)
                hitPercentage += (double)1 / diceNumber;
        }

        return (double)hitPercentage;
    }

    public static void ApplyDefenses<T>(List<T> targets,
        Func<T, List<string>> getResistances,
        Func<T, List<string>> getImmunities,
        Func<T, List<string>> getVulnerabilities,
        string damageType,
        ref double baseDamage)
    {
        var totalPower = 0.0;

        if (targets == null || targets.Count == 0)
            return;

        foreach (var target in targets)
        {
            var power = baseDamage;
            var resistances = getResistances(target);
            var immunities = getImmunities(target);
            var vulnerabilities = getVulnerabilities(target);

            if (resistances.Contains(damageType))
                power *= 0.5;
            if (immunities.Contains(damageType))
                power = 0;
            if (vulnerabilities.Contains(damageType))
                power *= 2;

            totalPower += power;
        }

        baseDamage = totalPower / targets.Count;
    }

    public static void ApplyDefenses<T>(List<T> targets,
        Func<T, List<string>> getResistances,
        Func<T, List<string>> getImmunities,
        Func<T, List<string>> getVulnerabilities,
        List<string> damageTypes,
        ref double baseDamage)
    {
        var totalPower = 0.0;

        if (targets == null || targets.Count == 0)
            return;

        foreach (var target in targets)
        {
            var power = baseDamage;
            var resistances = getResistances(target);
            var immunities = getImmunities(target);
            var vulnerabilities = getVulnerabilities(target);

            foreach (var damageType in damageTypes.Distinct().ToList())
            {
                if (resistances.Contains(damageType))
                    power *= 0.5;
                if (immunities.Contains(damageType))
                    power = 0;
                if (vulnerabilities.Contains(damageType))
                    power *= 2;
            }

            totalPower += power;
        }

        baseDamage = totalPower / targets.Count;
    }

    public static void ApplyBaseStatsIncrement(int baseStatsParty, int baseStatsMonsters, ref int totalPartyCombatPower, ref int totalMonstersCombatPower)
    {
        var baseStatsDifference = Math.Max(baseStatsParty, baseStatsMonsters) - Math.Min(baseStatsParty, baseStatsMonsters);

        if (baseStatsParty != baseStatsMonsters && baseStatsDifference == baseStatsParty)
        {
            totalPartyCombatPower = (int)(totalPartyCombatPower * (1.0 + (baseStatsMonsters / baseStatsParty)));
            totalMonstersCombatPower = (int)(totalMonstersCombatPower * (1.0 - (baseStatsMonsters / baseStatsParty)));
        }
        else if (baseStatsParty != baseStatsMonsters && baseStatsDifference == baseStatsMonsters)
        {
            totalMonstersCombatPower = (int)(totalMonstersCombatPower * (1.0 + (baseStatsParty / baseStatsMonsters)));
            totalPartyCombatPower = (int)(totalPartyCombatPower * (1.0 - (baseStatsParty / baseStatsMonsters)));
        }
    }

    public static Result CalculateCombatOutcome(List<PartyMember> party, List<Monster> monsters, int totalPartyCombatPower, int totalMonstersCombatPower, int baseStatsParty, int baseStatsMonsters, ILogger logger)
    {
        var result = new Result();
        var totalPartyHp = party.Sum(p => p.HitPoints);
        var totalMonstersHp = monsters.Sum(m => m.HitPoints);
        var numberOfTurnsToDefeatMonsters = (int)Math.Ceiling((double)totalMonstersHp / totalPartyCombatPower);
        var numberOfTurnsToDefeatParty = (int)Math.Ceiling((double)totalPartyHp / totalMonstersCombatPower);

        if (numberOfTurnsToDefeatMonsters < numberOfTurnsToDefeatParty)
        {
            result.Outcome = Results.Victory;
            result.Details = $"Party wins in {numberOfTurnsToDefeatMonsters} turns.";
            result.TotalRounds = (short)numberOfTurnsToDefeatMonsters;
            logger.Information($"Party wins in {numberOfTurnsToDefeatMonsters} turns.");
        }
        else if (numberOfTurnsToDefeatMonsters > numberOfTurnsToDefeatParty)
        {
            result.Outcome = Results.Defeat;
            result.Details = $"Monsters win in {numberOfTurnsToDefeatParty} turns.";
            result.TotalRounds = (short)numberOfTurnsToDefeatParty;
            logger.Information($"Monsters win in {numberOfTurnsToDefeatParty} turns.");
        }
        else if (numberOfTurnsToDefeatMonsters == numberOfTurnsToDefeatParty)
        {
            if (baseStatsParty >= baseStatsMonsters)
            {
                result.Outcome = Results.Victory;
                result.Details = $"Party wins in a tie-breaker after {numberOfTurnsToDefeatMonsters} turns.";
                result.TotalRounds = (short)numberOfTurnsToDefeatMonsters;
                logger.Information($"Party wins in a tie-breaker after {numberOfTurnsToDefeatMonsters} turns.");
            }
            else
            {
                result.Outcome = Results.Defeat;
                result.Details = $"Monsters win in a tie-breaker after {numberOfTurnsToDefeatParty} turns.";
                result.TotalRounds = (short)numberOfTurnsToDefeatParty;
                logger.Information($"Monsters win in a tie-breaker after {numberOfTurnsToDefeatParty} turns.");
            }
        }

        return result;
    }
}
