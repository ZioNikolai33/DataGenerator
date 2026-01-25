using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Utilities;

public static class CombatCalculator
{
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

    public static Result CalculateCombatOutcome(List<Member> party, List<Monster> monsters, int totalPartyCombatPower, int totalMonstersCombatPower)
    {
        var result = new Result();

        return result;
    }
}
