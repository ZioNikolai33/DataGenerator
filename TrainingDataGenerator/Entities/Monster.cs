using TrainingDataGenerator.Abstracts;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Entities.MonsterEntities;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;
using static TrainingDataGenerator.Entities.MonsterEntities.Damage;

namespace TrainingDataGenerator.Entities;

public class Monster : Creature, ICombatCalculator
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;

    public string Desc { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Subtype { get; set; } = string.Empty;
    public string Alignment { get; set; } = string.Empty;
    public string HitDice { get; set; } = string.Empty;
    public string HitPointsRoll { get; set; } = string.Empty;
    public SpeedType Speed { get; set; } = new SpeedType();
    public List<BaseEntity> ConditionImmunities { get; set; } = new List<BaseEntity>();
    public SensesType Senses { get; set; } = new SensesType();
    public string Languages { get; set; } = string.Empty;
    public double ChallengeRating { get; set; }
    public int Xp { get; set; }
    public List<SpecialAbility> SpecialAbilities { get; set; } = new List<SpecialAbility>();
    public List<NormalAction> Actions { get; set; } = new List<NormalAction>();
    public List<LegendaryAction> LegendaryActions { get; set; } = new List<LegendaryAction>();
    public List<Reaction> Reactions { get; set; } = new List<Reaction>();

    public Monster(MonsterMapper monster, ILogger logger, IRandomProvider random)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));

        Index = monster.Index;
        Name = monster.Name;
        Desc = monster.Desc;
        Size = monster.Size;
        Type = monster.Type;
        Subtype = monster.Subtype;
        Alignment = monster.Alignment;
        ArmorClass = (byte)monster.AC.Average(a => a.Value);
        HitPoints = monster.HitPoints;
        HitDice = monster.HitDice;
        HitPointsRoll = monster.HitPointsRoll;
        Speed = new SpeedType
        {
            Walk = monster.Speed.Contains("walk") ? monster.Speed["walk"].AsString : null,
            Swim = monster.Speed.Contains("swim") ? monster.Speed["swim"].AsString : null,
            Fly = monster.Speed.Contains("fly") ? monster.Speed["fly"].AsString : null,
            Burrow = monster.Speed.Contains("burrow") ? monster.Speed["burrow"].AsString : null,
            Climb = monster.Speed.Contains("climb") ? monster.Speed["climb"].AsString : null,
            Hover = monster.Speed.Contains("hover") ? monster.Speed["hover"].AsBoolean : null
        };
        Strength = new Attribute((byte)monster.Strength);
        Dexterity = new Attribute((byte)monster.Dexterity);
        Constitution = new Attribute((byte)monster.Constitution);
        Intelligence = new Attribute((byte)monster.Intelligence);
        Wisdom = new Attribute((byte)monster.Wisdom);
        Charisma = new Attribute((byte)monster.Charisma);
        
        CreateSkills();
        AddProfs(monster);

        Proficiencies = monster.Proficiencies.Select(item => item.Proficiency.Index).ToList();
        Vulnerabilities = monster.DamageVulnerabilities;
        Resistances = monster.DamageResistances;
        Immunities = monster.DamageImmunities;
        ConditionImmunities = monster.ConditionImmunities;
        Senses = new SensesType
        {
            Darkvision = monster.Senses.Contains("darkvision") ? monster.Senses["darkvision"].AsString : null,
            Tremorsense = monster.Senses.Contains("tremorsense") ? monster.Senses["tremorsense"].AsString : null,
            Blindsight = monster.Senses.Contains("blindsight") ? monster.Senses["blindsight"].AsString : null,
            Truesight = monster.Senses.Contains("truesight") ? monster.Senses["truesight"].AsString : null,
            PassivePerception = monster.Senses.Contains("passive_perception") ? (byte?)monster.Senses["passive_perception"].AsInt32 : null
        };
        Languages = monster.Languages;
        ChallengeRating = monster.ChallengeRating;
        ProficiencyBonus = (sbyte)monster.ProficiencyBonus;
        Xp = monster.Xp;
        SpecialAbilities = (monster.SpecialAbilities != null) ? monster.SpecialAbilities.Select(item => new SpecialAbility(item)).ToList() : new List<SpecialAbility>();
        Actions = (monster.Actions != null) ? monster.Actions.Select(item => new NormalAction(item)).ToList() : new List<NormalAction>();
        LegendaryActions = (monster.LegendaryActions != null) ? monster.LegendaryActions.Select(item => new LegendaryAction(item)).ToList() : new List<LegendaryAction>();
        Reactions = (monster.Reactions != null) ? monster.Reactions.Select(item => new Reaction(item)).ToList() : new List<Reaction>();
    }

    private void AddProfs(MonsterMapper monster)
    {
        foreach (var proficiency in monster.Proficiencies)
        {
            var profName = proficiency.Proficiency.Index;
            var value = (sbyte)proficiency.Value;

            if (profName.StartsWith("saving-throw"))
            {
                var attrName = profName.Replace("saving-throw: ", "").Trim();

                switch (attrName)
                {
                    case "str":
                        Strength.Save += (sbyte)proficiency.Value;
                        break;
                    case "dex":
                        Dexterity.Save += (sbyte)proficiency.Value;
                        break;
                    case "con":
                        Constitution.Save += (sbyte)proficiency.Value;
                        break;
                    case "int":
                        Intelligence.Save += (sbyte)proficiency.Value;
                        break;
                    case "wis":
                        Wisdom.Save += (sbyte)proficiency.Value;
                        break;
                    case "cha":
                        Charisma.Save += (sbyte)proficiency.Value;
                        break;
                }
            }
            else if (profName.StartsWith("skill"))
            {
                var skillName = profName.Replace("skill: ", "").Trim();
                var skill = Skills.FirstOrDefault(s => s.Index.Equals(skillName));

                if (skill != null)
                    skill.SetProficiency(true, (sbyte)(proficiency.Value - skill.Modifier));
            }
        }
    }

    public int CalculateBaseStats()
    {
        var totalBaseStats = 0;

        totalBaseStats += CalculateSpeedValue();
        totalBaseStats += CalculateStatsValue();
        totalBaseStats += CalculateSkillsValue();

        _logger.Verbose($"Total Base Stats for {Name}: {totalBaseStats}");

        return totalBaseStats;
    }

    public int CalculateSpeedValue()
    {
        int speedValue = 0;

        if (!string.IsNullOrEmpty(Speed.Walk))
            speedValue += int.Parse(Speed.Walk.Split(' ')[0]);
        
        if (!string.IsNullOrEmpty(Speed.Swim))
            speedValue += (int)(int.Parse(Speed.Swim.Split(' ')[0]) * 0.5);

        if (!string.IsNullOrEmpty(Speed.Fly))
            speedValue += int.Parse(Speed.Fly.Split(' ')[0]);

        if (!string.IsNullOrEmpty(Speed.Burrow))
            speedValue += (int)(int.Parse(Speed.Burrow.Split(' ')[0]) * 0.5);

        if (!string.IsNullOrEmpty(Speed.Climb))
            speedValue += (int)(int.Parse(Speed.Climb.Split(' ')[0]) * 0.5);

        if (Speed.Hover.HasValue && Speed.Hover.Value)
            speedValue += 10;

        return speedValue;
    }

    public int CalculateStatsValue() => Strength.Value + Dexterity.Value + Constitution.Value + Intelligence.Value + Wisdom.Value + Charisma.Value;

    public int CalculateSkillsValue() => Skills.Sum(item => item.Modifier);

    public int CalculateOffensivePower<T>(List<T> party, CRRatios difficulty) where T : ICombatCalculator
    {
        var offensivePower = 0;

        offensivePower += CalculateAttackPower(party.Cast<PartyMember>().ToList(), difficulty);
        offensivePower += CalculateSpellsPower(party.Cast<PartyMember>().ToList(), difficulty);

        return offensivePower;
    }

    public int CalculateHealingPower()
    {
        var healingPower = 0;
        var spellcast = SpecialAbilities.FirstOrDefault(sa => sa.Spellcast?.Dc != 0 && sa.Spellcast?.Level != 0)?.Spellcast ?? null;
        var spells = SpecialAbilities.Where(sa => sa.Spellcast?.Dc != 0 && sa.Spellcast?.Level != 0).SelectMany(sa => sa.Spellcast?.Spells ?? new List<Spell>()).ToList();

        foreach (var spell in spells)
            if (spell.IsDamageSpell() && spellcast != null)
                if (spell.IsHealingSpell())
                    healingPower += spell.GetHealingPower(spellcast.SpellSlots, this);

        _logger.Verbose($"Total Healing Power for {Name}: {healingPower}");
        return healingPower;
    }

    public double CalculateSpellUsagePercentage(Spell spell, CRRatios difficulty)
    {
        var spellSlots = SpecialAbilities.FirstOrDefault(sa => sa.Spellcast?.Dc != 0 && sa.Spellcast?.Level != 0)?.Spellcast?.SpellSlots ?? null;
        var spells = SpecialAbilities.Where(sa => sa.Spellcast?.Dc != 0 && sa.Spellcast?.Level != 0).SelectMany(sa => sa.Spellcast?.Spells ?? new List<Spell>()).ToList();

        if (spell == null || spellSlots == null)
            return 0.0;

        if (spells.Count > 0 && spellSlots == null)
            return 1.0;

        if (spell.Level == 0)
            return 1.0;

        if (spellSlots.GetSlotsLevelAvailable() == 0 || (spellSlots.GetSlotsLevelAvailable() < spell.Level && spell.Uses == ""))
            return 0.0;

        if (spell.Uses != "")
            return 1 / (int)difficulty;

        var totalAvailableSlots = spellSlots.GetTotalSlots(spell.Level);
        var numCompetingSpells = spells.Count(s => s.Level > 0 && s.Level <= spell.Level && s.Uses == "");

        for (var level = 0; level < DataConstants.MaxSpellLevel; level++)
            if (level < spell.Level && spellSlots.GetSlotsLevelAvailable() >= level)
                numCompetingSpells += spells.Count(s => s.Level > 0 && s.Level <= level && s.Uses == "");

        if (numCompetingSpells == 0)
            return 0.0;

        var avgSlotsPerSpell = (double)totalAvailableSlots / Math.Max(1, spells.Count(s => s.Level > 0 && s.Uses == ""));
        var usagePercentage = Math.Min(1.0, avgSlotsPerSpell / (int)difficulty);

        return usagePercentage;
    }

    private int CalculateAttackPower(List<PartyMember> party, CRRatios difficulty)
    {
        var offensivePower = 0.0;

        if (Actions.Count == 0)
            return 0;

        offensivePower += CalculateSimpleAttacks(party, difficulty);
        offensivePower += CalculateDcAttacks(party, difficulty);
        offensivePower += CalculateMultiAttacks(party, difficulty);

        _logger.Verbose($"Offensive Power for {Name}: {(int)offensivePower / Actions.Count}");

        return (int)(offensivePower / Actions.Count);
    }

    private int CalculateSpellsPower(List<PartyMember> party, CRRatios difficulty)
    {
        var offensivePower = 0.0;
        var spellcast = SpecialAbilities.FirstOrDefault(sa => sa.Spellcast?.Dc != 0 && sa.Spellcast?.Level != 0)?.Spellcast ?? null;
        var spells = SpecialAbilities.Where(sa => sa.Spellcast?.Dc != 0 && sa.Spellcast?.Level != 0).SelectMany(sa => sa.Spellcast?.Spells ?? new List<Spell>()).ToList();

        if (spells.Count(item => item.IsDamageSpell()) == 0)
            return 0;

        if (spellcast != null)
            foreach (var spell in spells)
                offensivePower += CalculateSpellPower(spellcast, spell, party, difficulty);
        
        _logger.Verbose($"Spells Power for {Name}: {(int)offensivePower / spells.Count(item => item.IsDamageSpell())}");

        return (int)(offensivePower / spells.Count(item => item.IsDamageSpell()));
    }

    private double CalculateSimpleAttacks(List<PartyMember> party, CRRatios difficulty)
    {
        var offensivePower = 0.0;
        var partyAvgAc = (int)party.Average(m => m.ArmorClass);
        var actions = Actions.Where(a => a.AttackBonus.HasValue && a.Damage.Count > 0).ToList();

        foreach (var action in actions)
            offensivePower += CalculateSimpleAttack(party, partyAvgAc, action, difficulty);

        return offensivePower;
    }

    private double CalculateDcAttacks(List<PartyMember> party, CRRatios difficulty)
    {
        var offensivePower = 0.0;
        var actions = Actions.Where(a => a.Dc?.DcValue != 0 && string.IsNullOrEmpty(a.Dc?.DcType)).ToList();

        foreach (var action in actions)
            offensivePower += CalculateDcAttack(party, action, difficulty);

        return offensivePower;
    }

    private double CalculateSimpleAttack(List<PartyMember> party, int partyAvgAc, NormalAction action, CRRatios difficulty)
    {
        var chooseableDamages = new List<DamageOption>();
        var attackBonus = action.AttackBonus.HasValue ? action.AttackBonus.Value : 0;

        if (action.Damage.Any(item => item.From != null && item.From.Options.Count > 0))
            foreach (var damage in action.Damage.Where(item => item.From != null))
                chooseableDamages.AddRange(damage.From.Options.OrderBy(_ => Random.Shared.Next()).Take(damage.Choose ?? 0).ToList());

        var averageDamage = action.Damage.Any(item => !string.IsNullOrEmpty(item.DamageDice)) ? action.Damage.Where(item => !string.IsNullOrEmpty(item.DamageDice)).Sum(d => UtilityMethods.GetDiceValue(d.DamageDice, this)) : 0;
        averageDamage += (chooseableDamages.Count > 0) ? chooseableDamages.Sum(item => UtilityMethods.GetDiceValue(item.Damage_Dice, this)) : 0;

        var damageTypes = action.Damage.Select(d => d.DamageType).Distinct().ToList();
        var hitPercentage = CombatCalculator.CalculateRollPercentage(partyAvgAc, attackBonus);
        var usagePercentage = CalculateUsagePercentage(action, difficulty);
        var totalPower = hitPercentage * averageDamage * usagePercentage;

        CombatCalculator.ApplyDefenses(party,
            p => p.Resistances,
            p => p.Immunities,
            p => p.Vulnerabilities,
            damageTypes,
            ref totalPower);

        return totalPower;
    }

    private double CalculateDcAttack(List<PartyMember> party, NormalAction action, CRRatios difficulty)
    {
        var chooseableDamages = new List<DamageOption>();

        if (action.Damage.Any(item => item.From != null && item.From.Options.Count > 0))
            foreach (var damage in action.Damage.Where(item => item.From != null))
                chooseableDamages.AddRange(damage.From.Options.OrderBy(_ => Random.Shared.Next()).Take(damage.Choose ?? 0).ToList());

        var partyAvgPercentage = party.Average(m => m.GetSavePercentage(action.Dc?.DcType ?? string.Empty, action.Dc?.DcValue ?? 0));
        var averageDamage = action.Damage.Any(item => !string.IsNullOrEmpty(item.DamageDice)) ? action.Damage.Where(item => !string.IsNullOrEmpty(item.DamageDice)).Sum(d => UtilityMethods.GetDiceValue(d.DamageDice, this)) : 0;
        averageDamage += (chooseableDamages.Count > 0) ? chooseableDamages.Sum(item => UtilityMethods.GetDiceValue(item.Damage_Dice, this)) : 0;
        var damageTypes = action.Damage.Select(d => d.DamageType).Distinct().ToList();
        var usagePercentage = CalculateUsagePercentage(action, difficulty);
        var totalPower = (1.0 - partyAvgPercentage) * averageDamage * usagePercentage;

        if (action.Dc?.DcSuccess.Equals("half", StringComparison.OrdinalIgnoreCase) == true)
            totalPower += partyAvgPercentage * (averageDamage / 2) * usagePercentage;

        CombatCalculator.ApplyDefenses(party,
            p => p.Resistances,
            p => p.Immunities,
            p => p.Vulnerabilities,
            damageTypes,
            ref totalPower);

        return totalPower;
    }

    private double CalculateSpellPower(SpecialAbility.Spellcasting spellcast, Spell spell, List<PartyMember> party, CRRatios difficulty)
    {
        if (spell.IsDamageSpell() && spellcast != null)
        {
            var spellPower = spell.GetSpellPower(spellcast, this);
            var usagePercentage = CalculateSpellUsagePercentage(spell, difficulty);
            var hitPercentage = spell.GetSpellPercentage(spellcast, this, party);
            var totalPower = hitPercentage * spellPower * usagePercentage;

            if (spell.Dc != null && spell.Dc.DcType != null)
                if (spell.Dc.DcSuccess.Equals("half", StringComparison.OrdinalIgnoreCase))
                    totalPower += hitPercentage * (spellPower / 2) * usagePercentage;

            CombatCalculator.ApplyDefenses(party,
                p => p.Resistances,
                p => p.Immunities,
                p => p.Vulnerabilities,
                spell.Damage?.DamageType ?? string.Empty,
                ref totalPower);

            return totalPower;
        }

        return 0;
    }

    private double CalculateUsagePercentage(NormalAction action, CRRatios difficulty)
    {
        double usagePercentage = 1.0;

        if (action.Usage != null)
            if (action.Usage.Type.Equals("recharge on roll", StringComparison.OrdinalIgnoreCase))
                usagePercentage = CombatCalculator.CalculateRollPercentage(action.Usage.MinValue ?? 0, 0, (short)UtilityMethods.GetDiceValue(action.Usage.Dice ?? "0"));
            else if (action.Usage.Type.Equals("recharge after rest", StringComparison.OrdinalIgnoreCase) || action.Usage.Type.Equals("per day", StringComparison.OrdinalIgnoreCase))
                usagePercentage = (action.Usage.Times ?? 1) / (int)difficulty;

        return usagePercentage;
    }

    private double CalculateMultiAttacks(List<PartyMember> party, CRRatios difficulty) 
    {
        var offensivePower = 0.0;
        var partyAvgAc = (int)party.Average(m => m.ArmorClass);
        var multiattackActions = Actions.Where(a => a.Actions.Count > 0).SelectMany(a => a.Actions).ToList();

        foreach (var multiattackAction in multiattackActions)
        {
            if (multiattackAction.Type.Equals("magic", StringComparison.OrdinalIgnoreCase)) // Only Glabrezu has it
            {
                if (SpecialAbilities.Select(sa => sa.Name).Contains(multiattackAction.ActionName))
                {
                    var spellsChosen = SpecialAbilities.First(sa => sa.Name.Equals(multiattackAction.ActionName, StringComparison.OrdinalIgnoreCase)).Spellcast?.Spells.OrderBy(_ => Random.Shared.Next()).Take(multiattackAction.Count).ToList() ?? new List<Spell>();
                    var spellcast = SpecialAbilities.First(sa => sa.Name.Equals(multiattackAction.ActionName, StringComparison.OrdinalIgnoreCase)).Spellcast;

                    if (spellcast != null)
                        foreach (var spell in spellsChosen)
                            offensivePower += CalculateSpellPower(spellcast, spell, party, difficulty);
                }
            }
            else
            {
                var action = Actions.First(a => a.Name.Equals(multiattackAction.ActionName, StringComparison.OrdinalIgnoreCase));
                var actionCount = multiattackAction.Count;

                if (action == null || actionCount == 0) continue;

                if (action.Damage.Count > 0)
                    offensivePower += CalculateSimpleAttack(party, partyAvgAc, action, difficulty) * actionCount;
                else if (action.Dc?.DcValue != 0 && !string.IsNullOrEmpty(action.Dc?.DcType))
                    offensivePower += CalculateDcAttack(party, action, difficulty) * actionCount;
            }
        }

        return offensivePower;
    }

    public override string ToString()
    {
        var str = $"Monster: {Name} (CR: {ChallengeRating})\n";
        str += $" Type: {Size} {Type}, Alignment: {Alignment}\n";
        str += $" HP: {HitPoints} ({HitDice}) | AC: {string.Join(", ", ArmorClass)}\n";
        str += $" STR: {Strength.Value} ({Strength.Modifier}), DEX: {Dexterity.Value} ({Dexterity.Modifier}), CON: {Constitution.Value} ({Constitution.Modifier}), INT: {Intelligence.Value} ({Intelligence.Modifier}), WIS: {Wisdom.Value} ({Wisdom.Modifier}), CHA: {Charisma.Value} ({Charisma.Modifier})\n";
        str += $" Skills: {string.Join(", ", Skills.Where(s => s.IsProficient).Select(s => $"{s.Name} (+{s.Modifier})"))}\n";
        str += $" Languages: {Languages}\n";

        return str;
    }
}
