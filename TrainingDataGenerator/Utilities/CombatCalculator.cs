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
}
