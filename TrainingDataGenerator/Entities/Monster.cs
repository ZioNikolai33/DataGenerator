using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;
using static TrainingDataGenerator.Entities.Mappers.MonsterMapper;

namespace TrainingDataGenerator.Entities;

public class Monster : BaseEntity, ICombatCalculator
{
    public string Desc { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Subtype { get; set; } = string.Empty;
    public string Alignment { get; set; } = string.Empty;
    public List<ArmorClass> AC { get; set; } = new List<ArmorClass>();
    public int HitPoints { get; set; }
    public string HitDice { get; set; } = string.Empty;
    public string HitPointsRoll { get; set; } = string.Empty;
    public SpeedType Speed { get; set; } = new SpeedType();
    public Attribute Strength { get; set; } = new Attribute();
    public Attribute Dexterity { get; set; } = new Attribute();
    public Attribute Constitution { get; set; } = new Attribute();
    public Attribute Intelligence { get; set; } = new Attribute();
    public Attribute Wisdom { get; set; } = new Attribute();
    public Attribute Charisma { get; set; } = new Attribute();
    public List<string> Proficiencies { get; set; } = new List<string>();
    public List<Skill> Skills { get; set; } = new List<Skill>();
    public List<string> DamageVulnerabilities { get; set; } = new List<string>();
    public List<string> DamageResistances { get; set; } = new List<string>();
    public List<string> DamageImmunities { get; set; } = new List<string>();
    public List<BaseEntity> ConditionImmunities { get; set; } = new List<BaseEntity>();
    public SensesType Senses { get; set; } = new SensesType();
    public string Languages { get; set; } = string.Empty;
    public double ChallengeRating { get; set; }
    public byte ProficiencyBonus { get; set; }
    public int Xp { get; set; }
    public List<SpecialAbility> SpecialAbilities { get; set; } = new List<SpecialAbility>();
    public List<NormalAction> Actions { get; set; } = new List<NormalAction>();
    public List<LegendaryAction> LegendaryActions { get; set; } = new List<LegendaryAction>();
    public List<Reaction> Reactions { get; set; } = new List<Reaction>();

    public class ArmorClass
    {
        public string Desc { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Value { get; set; }
        public List<BaseEntity> Armor { get; set; } = new List<BaseEntity>();
        public BaseEntity Spell { get; set; } = new BaseEntity();
        public BaseEntity Condition { get; set; } = new BaseEntity();

        public ArmorClass(MonsterMapper.ArmorClass armorClass)
        {
            Desc = armorClass.Desc;
            Type = armorClass.Type;
            Value = armorClass.Value;
            Armor = armorClass.Armor ?? new List<BaseEntity>();
            Spell = armorClass.Spell;
            Condition = armorClass.Condition;
        }
    }

    public class SpeedType
    {
        public string? Walk { get; set; }
        public string? Swim { get; set; }
        public string? Fly { get; set; }
        public string? Burrow { get; set; }
        public string? Climb { get; set; }
        public bool? Hover { get; set; }
    }

    public class SensesType
    {
        public string? Darkvision { get; set; }
        public string? Tremorsense { get; set; }
        public string? Blindsight { get; set; }
        public string? Truesight { get; set; }
        public byte? PassivePerception { get; set; }
    }

    public class SpecialAbility
    {
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public Spellcasting? Spellcast { get; set; }
        public Usage? Usage { get; set; }
        public Dc? Dc { get; set; }
        public List<Damage>? Damage { get; set; }

        public class Spellcasting
        {
            public int Level { get; set; }
            public BaseEntity Ability { get; set; } = new BaseEntity();
            public int Dc { get; set; }
            public int Modifier { get; set; }
            public List<string> ComponentsRequired { get; set; } = new List<string>();
            public string School { get; set; } = string.Empty;
            public Slots SpellSlots { get; set; } = new Slots();
            public List<Spell> Spells { get; set; } = new List<Spell>();
        }

        public SpecialAbility(MonsterMapper.SpecialAbility ability)
        {
            Name = ability.Name;
            Desc = ability.Desc;
            if (ability.Spellcasting != null)
            {
                Spellcast = new Spellcasting
                {
                    Level = ability.Spellcasting.Level ?? 0,
                    Ability = ability.Spellcasting.Ability ?? new BaseEntity(),
                    Dc = ability.Spellcasting.Dc ?? 0,
                    Modifier = ability.Spellcasting.Modifier ?? 0,
                    ComponentsRequired = ability.Spellcasting.ComponentsRequired ?? new List<string>(),
                    School = ability.Spellcasting.School ?? string.Empty,
                    SpellSlots = (ability.Spellcasting.Slots != null) ? new Slots(ability.Spellcasting.Slots) : new Slots(),
                    Spells = ability.Spellcasting.Spells?.Select(item => new Spell(EntitiesFinder.GetEntityByIndex(Lists.spells, new BaseEntity(item.Url.Substring(item.Url.LastIndexOf('/') + 1), item.Name)))).ToList() ?? new List<Spell>()
                };
            }
            Usage = (ability.Usage != null) ? new Usage(ability.Usage.Type, ability.Usage.Times, ability.Usage.RestTypes, ability.Usage.Dice, ability.Usage.MinValue) : null;
            Dc = new Dc(ability.Dc);
            Damage = (ability.Damage != null) ? ability.Damage.Select(item => new Damage(item)).ToList() : null;
        }
    }

    public class NormalAction
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public List<MultiAction> Actions { get; set; }
        public byte? AttackBonus { get; set; }
        public List<Damage> Damage { get; set; }
        public Dc Dc { get; set; }
        public Usage? Usage { get; set; }

        public class MultiAction
        {
            public string ActionName { get; set; } = string.Empty;
            public int Count { get; set; }
            public string Type { get; set; } = string.Empty;
        }

        public NormalAction(MonsterMapper.NormalAction action)
        {
            Name = action.Name;
            Desc = action.Desc;
            Actions = action.Actions?.Select(item => new MultiAction
            {
                ActionName = item.ActionName,
                Count = (item.Count != null) ? (item.Count is string ? -1 : (int)item.Count) : 0,
                Type = item.Type
            }).ToList() ?? new List<MultiAction>();
            AttackBonus = action.AttackBonus != null ? (byte?)action.AttackBonus : null;
            Damage = action.Damage?.Select(item => new Damage(item)).ToList() ?? new List<Damage>();
            Dc = new Dc(action.Dc);
            Usage = (action.Usage != null) ? new Usage(action.Usage.Type, action.Usage.Times, action.Usage.RestTypes, action.Usage.Dice, action.Usage.MinValue) : null;

            if (action.ActionOptions != null && action.ActionOptions.From.Options.Count > 0)
            {
                var random = new Random();
                var choose = action.ActionOptions.Choose;

                var selectedOptions = action.ActionOptions.From.Options.OrderBy(_ => random.Next()).Take(choose).ToList();

                foreach (var option in selectedOptions)
                {
                    if (option.OptionType.Equals("multiple", StringComparison.OrdinalIgnoreCase))
                    {
                        Actions.AddRange(option.Items.Select(item => new MultiAction
                        {
                            ActionName = item.ActionName,
                            Count = (item.Count != null) ? (int)item.Count : 0,
                            Type = item.Type
                        }).ToList());
                    }
                    else
                    {
                        Actions.Add(new MultiAction
                        {
                            ActionName = option.ActionName,
                            Count = (option.Count != null) ? (int)option.Count : 0,
                            Type = option.Type
                        });
                    }
                }
            }
        }
    }

    public class LegendaryAction
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public int? AttackBonus { get; set; }
        public Dc? Dc { get; set; }
        public List<Damage>? Damage { get; set; }

        public LegendaryAction(MonsterMapper.LegendaryAction action)
        {
            Name = action.Name;
            Desc = action.Desc;
            AttackBonus = action.AttackBonus;
            Dc = new Dc(action.Dc);
            Damage = (action.Damage != null) ? action.Damage.Select(item => new Damage(item)).ToList() : null;
        }
    }

    public class Reaction
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public int? AttackBonus { get; set; }
        public Dc Dc { get; set; }
        public List<Damage> Damage { get; set; } = new List<Damage>();

        public Reaction(MonsterMapper.Reaction reaction)
        {
            Name = reaction.Name;
            Desc = reaction.Desc;
            AttackBonus = reaction.AttackBonus;
            Dc = new Dc(reaction.Dc);
            Damage = (reaction.Damage != null) ? reaction.Damage.Select(item => new Damage(item)).ToList() : new List<Damage>();
        }
    }

    public class Dc
    {
        public BaseEntity DcType { get; set; } = new BaseEntity();
        public int DcValue { get; set; }
        public string SuccessType { get; set; } = string.Empty;

        public Dc(MonsterMapper.Dc? dc)
        {
            if (dc != null)
            {
                DcType = dc.DcType;
                DcValue = dc.DcValue;
                SuccessType = dc.SuccessType;
            }
        }
    }

    public class Damage
    {
        public string Type { get; set; } = string.Empty;
        public string DamageType { get; set; } = string.Empty;
        public string DamageDice { get; set; } = string.Empty;
        public Dc? Dc { get; set; }
        public byte? Choose { get; set; }
        public DamageOptionSet From { get; set; } = new DamageOptionSet(new MonsterMapper.DamageOptionSet());

        public Damage(MonsterMapper.Damage? damage)
        {
            if (damage != null)
            {
                Type = damage.Type;
                DamageType = damage.DamageType?.Index ?? string.Empty;
                DamageDice = damage.DamageDice;
                Dc = new Dc(damage.Dc);
                Choose = damage.Choose != null ? (byte?)damage.Choose : null;
                From = damage.From != null ? new DamageOptionSet(damage.From) : new DamageOptionSet(new MonsterMapper.DamageOptionSet());
            }
        }
    }

    public class DamageOptionSet
    {
        public string Option_Set_Type { get; set; }
        public List<DamageOption> Options { get; set; }

        public DamageOptionSet(MonsterMapper.DamageOptionSet damage)
        {
            Option_Set_Type = damage.Option_Set_Type;
            Options = damage.Options.Select(item => new DamageOption
            {
                Option_Type = item.Option_Type,
                Notes = item.Notes,
                Damage_Type = item.Damage_Type.Index,
                Damage_Dice = item.Damage_Dice
            }).ToList();
        }
    }

    public class DamageOption
    {
        public string Desc { get; set; } = string.Empty;
        public string Option_Type { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Damage_Type { get; set; } = string.Empty;
        public string Damage_Dice { get; set; } = string.Empty;
    }

    public Monster(MonsterMapper monster) : base(monster.Index, monster.Name)
    {
        Desc = monster.Desc;
        Size = monster.Size;
        Type = monster.Type;
        Subtype = monster.Subtype;
        Alignment = monster.Alignment;
        AC = monster.AC.Select(item => new ArmorClass(item)).ToList();
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
        DamageVulnerabilities = monster.DamageVulnerabilities;
        DamageResistances = monster.DamageResistances;
        DamageImmunities = monster.DamageImmunities;
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
        ProficiencyBonus = (byte)monster.ProficiencyBonus;
        Xp = monster.Xp;
        SpecialAbilities = (monster.SpecialAbilities != null) ? monster.SpecialAbilities.Select(item => new SpecialAbility(item)).ToList() : new List<SpecialAbility>();
        Actions = (monster.Actions != null) ? monster.Actions.Select(item => new NormalAction(item)).ToList() : new List<NormalAction>();
        LegendaryActions = (monster.LegendaryActions != null) ? monster.LegendaryActions.Select(item => new LegendaryAction(item)).ToList() : new List<LegendaryAction>();
        Reactions = (monster.Reactions != null) ? monster.Reactions.Select(item => new Reaction(item)).ToList() : new List<Reaction>();
    }

    private void CreateSkills()
    {
        Skills = new List<Skill>
        {
             new Skill(new BaseEntity("skill-acrobatics", "Acrobatics"), Dexterity.Modifier),
             new Skill(new BaseEntity("skill-animal-handling", "Animal Handling"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-arcana", "Arcana"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-athletics", "Athletics"), Strength.Modifier),
             new Skill(new BaseEntity("skill-deception", "Deception"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-history", "History"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-insight", "Insight"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-intimidation", "Intimidation"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-investigation", "Investigation"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-medicine", "Medicine"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-nature", "Nature"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-perception", "Perception"), Wisdom.Modifier),
             new Skill(new BaseEntity("skill-performance", "Performance"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-persuasion", "Persuasion"), Charisma.Modifier),
             new Skill(new BaseEntity("skill-religion", "Religion"), Intelligence.Modifier),
             new Skill(new BaseEntity("skill-sleight-of-hand", "Sleight of Hand"), Dexterity.Modifier),
             new Skill(new BaseEntity("skill-stealth", "Stealth"), Dexterity.Modifier),
             new Skill(new BaseEntity("skill-survival", "Survival"), Wisdom.Modifier)
        };
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

        Logger.Instance.Information($"Total Base Stats for {Name}: {totalBaseStats}");

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

        offensivePower += CalculateAttackPower(party.Cast<Member>().ToList(), difficulty);
        offensivePower += CalculateSpellsPower(party.Cast<Member>().ToList(), difficulty);

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

        Logger.Instance.Information($"Total Healing Power for {Name}: {healingPower}");
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

    private int CalculateAttackPower(List<Member> party, CRRatios difficulty)
    {
        var offensivePower = 0.0;

        if (Actions.Count == 0)
            return 0;

        offensivePower += CalculateSimpleAttacks(party, difficulty);
        offensivePower += CalculateDcAttacks(party, difficulty);
        offensivePower += CalculateMultiAttacks(party, difficulty);

        Logger.Instance.Information($"Offensive Power for {Name}: {(int)offensivePower / Actions.Count}");

        return (int)(offensivePower / Actions.Count);
    }

    private int CalculateSpellsPower(List<Member> party, CRRatios difficulty)
    {
        var offensivePower = 0.0;
        var spellcast = SpecialAbilities.FirstOrDefault(sa => sa.Spellcast?.Dc != 0 && sa.Spellcast?.Level != 0)?.Spellcast ?? null;
        var spells = SpecialAbilities.Where(sa => sa.Spellcast?.Dc != 0 && sa.Spellcast?.Level != 0).SelectMany(sa => sa.Spellcast?.Spells ?? new List<Spell>()).ToList();

        if (spells.Count(item => item.IsDamageSpell()) == 0)
            return 0;

        if (spellcast != null)
            foreach (var spell in spells)
                offensivePower += CalculateSpellPower(spellcast, spell, party, difficulty);
        
        Logger.Instance.Information($"Spells Power for {Name}: {(int)offensivePower / spells.Count(item => item.IsDamageSpell())}");

        return (int)(offensivePower / spells.Count(item => item.IsDamageSpell()));
    }

    private double CalculateSimpleAttacks(List<Member> party, CRRatios difficulty)
    {
        var offensivePower = 0.0;
        var partyAvgAc = (int)party.Average(m => m.ArmorClass);
        var actions = Actions.Where(a => a.AttackBonus.HasValue && a.Damage.Count > 0).ToList();

        foreach (var action in actions)
            offensivePower += CalculateSimpleAttack(party, partyAvgAc, action, difficulty);

        return offensivePower;
    }

    private double CalculateDcAttacks(List<Member> party, CRRatios difficulty)
    {
        var offensivePower = 0.0;
        var actions = Actions.Where(a => a.Dc.DcValue != 0 && string.IsNullOrEmpty(a.Dc.DcType.Index)).ToList();

        foreach (var action in actions)
            offensivePower += CalculateDcAttack(party, action, difficulty);

        return offensivePower;
    }

    private double CalculateSimpleAttack(List<Member> party, int partyAvgAc, NormalAction action, CRRatios difficulty)
    {
        var chooseableDamages = new List<DamageOption>();
        var attackBonus = action.AttackBonus.HasValue ? action.AttackBonus.Value : 0;

        if (action.Damage.Any(item => item.From != null && item.From.Options.Count > 0))
            foreach (var damage in action.Damage.Where(item => item.From != null))
                chooseableDamages.AddRange(damage.From.Options.OrderBy(_ => new Random().Next()).Take(damage.Choose ?? 0).ToList());

        var averageDamage = action.Damage.Any(item => !string.IsNullOrEmpty(item.DamageDice)) ? action.Damage.Where(item => !string.IsNullOrEmpty(item.DamageDice)).Sum(d => DataManipulation.GetDiceValue(d.DamageDice, this)) : 0;
        averageDamage += (chooseableDamages.Count > 0) ? chooseableDamages.Sum(item => DataManipulation.GetDiceValue(item.Damage_Dice, this)) : 0;

        var damageTypes = action.Damage.Select(d => d.DamageType).Distinct().ToList();
        var hitPercentage = DataManipulation.CalculateRollPercentage(partyAvgAc, attackBonus);
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

    private double CalculateDcAttack(List<Member> party, NormalAction action, CRRatios difficulty)
    {
        var chooseableDamages = new List<DamageOption>();

        if (action.Damage.Any(item => item.From != null && item.From.Options.Count > 0))
            foreach (var damage in action.Damage.Where(item => item.From != null))
                chooseableDamages.AddRange(damage.From.Options.OrderBy(_ => new Random().Next()).Take(damage.Choose ?? 0).ToList());

        var partyAvgPercentage = party.Average(m => m.GetSavePercentage(action.Dc.DcType.Index, action.Dc.DcValue));
        var averageDamage = action.Damage.Any(item => !string.IsNullOrEmpty(item.DamageDice)) ? action.Damage.Where(item => !string.IsNullOrEmpty(item.DamageDice)).Sum(d => DataManipulation.GetDiceValue(d.DamageDice, this)) : 0;
        averageDamage += (chooseableDamages.Count > 0) ? chooseableDamages.Sum(item => DataManipulation.GetDiceValue(item.Damage_Dice, this)) : 0;
        var damageTypes = action.Damage.Select(d => d.DamageType).Distinct().ToList();
        var usagePercentage = CalculateUsagePercentage(action, difficulty);
        var totalPower = (1.0 - partyAvgPercentage) * averageDamage * usagePercentage;

        if (action.Dc.SuccessType.Equals("half", StringComparison.OrdinalIgnoreCase))
            totalPower += partyAvgPercentage * (averageDamage / 2) * usagePercentage;

        CombatCalculator.ApplyDefenses(party,
            p => p.Resistances,
            p => p.Immunities,
            p => p.Vulnerabilities,
            damageTypes,
            ref totalPower);

        return totalPower;
    }

    private double CalculateSpellPower(SpecialAbility.Spellcasting spellcast, Spell spell, List<Member> party, CRRatios difficulty)
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
                usagePercentage = DataManipulation.CalculateRollPercentage(action.Usage.MinValue ?? 0, 0, (short)DataManipulation.GetDiceValue(action.Usage.Dice ?? "0"));
            else if (action.Usage.Type.Equals("recharge after rest", StringComparison.OrdinalIgnoreCase) || action.Usage.Type.Equals("per day", StringComparison.OrdinalIgnoreCase))
                usagePercentage = (action.Usage.Times ?? 1) / (int)difficulty;

        return usagePercentage;
    }

    private double CalculateMultiAttacks(List<Member> party, CRRatios difficulty) 
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
                    var spellsChosen = SpecialAbilities.First(sa => sa.Name.Equals(multiattackAction.ActionName, StringComparison.OrdinalIgnoreCase)).Spellcast?.Spells.OrderBy(_ => new Random().Next()).Take(multiattackAction.Count).ToList() ?? new List<Spell>();
                    var spellcast = SpecialAbilities.First(sa => sa.Name.Equals(multiattackAction.ActionName, StringComparison.OrdinalIgnoreCase)).Spellcast;

                    if (spellcast != null)
                        foreach (var spell in spellsChosen)
                            offensivePower += CalculateSpellPower(spellcast, spell, party);
                }
            }
            else
            {
                var action = Actions.First(a => a.Name.Equals(multiattackAction.ActionName, StringComparison.OrdinalIgnoreCase));
                var actionCount = multiattackAction.Count;

                if (action == null || actionCount == 0) continue;

                if (action.Damage.Count > 0)
                    offensivePower += CalculateSimpleAttack(party, partyAvgAc, action, difficulty) * actionCount;
                else if (action.Dc.DcValue != 0 && !string.IsNullOrEmpty(action.Dc.DcType.Index))
                    offensivePower += CalculateDcAttack(party, action, difficulty) * actionCount;
            }
        }

        return offensivePower;
    }

    public override string ToString()
    {
        var str = $"Monster: {Name} (CR: {ChallengeRating})\n";
        str += $" Type: {Size} {Type}, Alignment: {Alignment}\n";
        str += $" HP: {HitPoints} ({HitDice}) | AC: {string.Join(", ", AC.Select(ac => ac.Value.ToString()))}\n";
        str += $" STR: {Strength.Value} ({Strength.Modifier}), DEX: {Dexterity.Value} ({Dexterity.Modifier}), CON: {Constitution.Value} ({Constitution.Modifier}), INT: {Intelligence.Value} ({Intelligence.Modifier}), WIS: {Wisdom.Value} ({Wisdom.Modifier}), CHA: {Charisma.Value} ({Charisma.Modifier})\n";
        str += $" Skills: {string.Join(", ", Skills.Where(s => s.IsProficient).Select(s => $"{s.Name} (+{s.Modifier})"))}\n";
        str += $" Languages: {Languages}\n";

        return str;
    }
}
