using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Entities;

public class Spell: BaseEntity
{
    public string Range { get; set; }
    public byte Level { get; set; }
    public string Duration { get; set; }
    public bool Concentration { get; set; }
    public string CastingTime { get; set; }
    public List<ClassDictionary>? HealAtSlotLevel { get; set; }
    public string School { get; set; }
    public List<string> Classes { get; set; }
    public List<string>? Subclasses { get; set; }
    public Area? AreaEffect { get; set; }
    public DifficultyClass? Dc { get; set; }
    public SpellDamage? Damage { get; set; }
    public string? AttackType { get; set; }
    public string? Uses { get; set; }

    public Spell(SpellMapper spell, string? uses = "") : base(spell.Index, spell.Name)
    {
        Index = spell.Index;
        Name = spell.Name;
        Range = spell.Range;
        Level = spell.Level;
        Duration = spell.Duration;
        Concentration = spell.Concentration;
        CastingTime = spell.CastingTime;
        HealAtSlotLevel = spell.HealAtSlotLevel != null ? spell.HealAtSlotLevel.Select(item => new ClassDictionary(short.Parse(item.Key), item.Value)).ToList() : null;
        School = spell.School.Index;
        Classes = spell.Classes.Select(c => c.Index).ToList();
        Subclasses = spell.Subclasses?.Select(sc => sc.Index).ToList();
        AreaEffect = spell.AreaOfEffect;
        Dc = spell.Dc;
        Damage = spell.Damage != null ? new SpellDamage(spell.Damage) : null;
        AttackType = spell.AttackType;
        Uses = uses;
    }

    public int GetSpellPower(short level, Slots spellSlots, Member member)
    {
        var spellPower = 0;

        if (IsDamageSpell())
        {
            if (Damage?.DamageSlots != null)
            {
                var damageDice = Damage.DamageSlots.OrderBy(x => x.Key).First().Value;

                spellPower = DataManipulation.GetDiceValue(damageDice, member);
                spellPower += (spellSlots.GetSlotsLevelAvailable() - Damage.DamageSlots.OrderBy(x => x.Key).First().Key) * 2;
            }
            else if (Damage?.DamageAtCharacterLevel != null)
            {
                var damageDice = Damage.DamageAtCharacterLevel.OrderBy(x => x.Key).FirstOrDefault(x => x.Key <= level)?.Value ?? "0d0";
                var damageParts = damageDice.Split('d');

                spellPower = (int.Parse(damageParts[0]) * (int.Parse(damageParts[1]))) / 2;
            }
        }

        return spellPower;
    }

    public int GetSpellPercentage(Member partyMember, List<Monster> monsters)
    {
        var spellPercentage = 0;
        var attackBonus = 0;
        var averageMonsterAc = (int)monsters.Average(item => item.AC.Average(x => x.Value));
        var averageMonsterSaveBonus = 0;

        if (IsDamageSpell())
        {
            if (RequiresAttackRoll())
            {
                switch (partyMember.SpellcastingAbility)
                {
                    case "strength":
                        attackBonus = partyMember.Strength.Modifier + partyMember.ProficiencyBonus;
                        break;
                    case "dexterity":
                        attackBonus = partyMember.Dexterity.Modifier + partyMember.ProficiencyBonus;
                        break;
                    case "constitution":
                        attackBonus = partyMember.Constitution.Modifier + partyMember.ProficiencyBonus;
                        break;
                    case "intelligence":
                        attackBonus = partyMember.Intelligence.Modifier + partyMember.ProficiencyBonus;
                        break;
                    case "wisdom":
                        attackBonus = partyMember.Wisdom.Modifier + partyMember.ProficiencyBonus;
                        break;
                    case "charisma":
                        attackBonus = partyMember.Charisma.Modifier + partyMember.ProficiencyBonus;
                        break;
                }

                spellPercentage = DataManipulation.CalculateRollPercentage(averageMonsterAc, attackBonus);
            }
            else if (RequiresSavingThrow() && Dc != null)
            {
                var saveDc = 8 + partyMember.ProficiencyBonus;
                switch (partyMember.SpellcastingAbility)
                {
                    case "strength":
                        saveDc += partyMember.Strength.Modifier;
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Strength.Modifier);
                        break;
                    case "dexterity":
                        saveDc += partyMember.Dexterity.Modifier;
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Dexterity.Modifier);
                        break;
                    case "constitution":
                        saveDc += partyMember.Constitution.Modifier;
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Constitution.Modifier);
                        break;
                    case "intelligence":
                        saveDc += partyMember.Intelligence.Modifier;
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Intelligence.Modifier);
                        break;
                    case "wisdom":
                        saveDc += partyMember.Wisdom.Modifier;
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Wisdom.Modifier);
                        break;
                    case "charisma":
                        saveDc += partyMember.Charisma.Modifier;
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Charisma.Modifier);
                        break;
                }

                spellPercentage = DataManipulation.CalculateRollPercentage(saveDc, averageMonsterSaveBonus);
            }
        }

        return spellPercentage;
    }

    public int GetHealingPower(Slots spellSlots, Member member)
    {
        var healingPower = 0;

        if (IsHealingSpell() && HealAtSlotLevel != null)
        {
            var healDice = HealAtSlotLevel.OrderBy(x => x.Key).First().Value;

            healingPower = DataManipulation.GetDiceValue(healDice, member);
            healingPower += (spellSlots.GetSlotsLevelAvailable() - HealAtSlotLevel.OrderBy(x => x.Key).First().Key) * 2;
        }

        return healingPower;
    }

    public bool IsDamageSpell() => Damage != null;
    public bool IsHealingSpell() => HealAtSlotLevel != null;
    public bool RequiresAttackRoll() => AttackType != null;
    public bool RequiresSavingThrow() => Dc != null;
}

public class SpellDamage
{
    public string? DamageType { get; set; }
    public List<ClassDictionary>? DamageSlots { get; set; }
    public List<ClassDictionary>? DamageAtCharacterLevel { get; set; }

    public SpellDamage(SpellMapper.DamageInfo damage)
    {
        DamageType = damage.damage_type != null ? damage.damage_type.Index : null;
        DamageSlots = damage.damage_at_slot_level != null ? damage.damage_at_slot_level.Keys.Select(item => new ClassDictionary(short.Parse(item), damage.damage_at_slot_level[item])).ToList() : null;
        DamageAtCharacterLevel = damage.damage_at_character_level != null ? damage.damage_at_character_level.Keys.Select(item => new ClassDictionary(short.Parse(item), damage.damage_at_character_level[item])).ToList() : null;
    }
}