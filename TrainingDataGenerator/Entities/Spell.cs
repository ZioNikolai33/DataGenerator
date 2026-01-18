using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Utilities;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using static TrainingDataGenerator.Entities.Mappers.MonsterMapper;

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

    public int GetSpellPower(Member member)
    {
        var spellPower = 0.0;

        if (IsDamageSpell() && Uses == "")
            if (Damage?.DamageSlots != null && Damage.DamageSlots.Count > 0)
                spellPower = Damage.DamageSlots.Where(item => member.SpellSlots.GetSlotsLevelAvailable() >= item.Key).Average(x => DataManipulation.GetDiceValue(x.Value, member));
            else if (Damage?.DamageAtCharacterLevel != null && Damage.DamageAtCharacterLevel.Count > 0)
                spellPower = Damage.DamageAtCharacterLevel.Where(item => member.Level >= item.Key).Average(x => DataManipulation.GetDiceValue(x.Value, member));
        else if (IsDamageSpell() && Uses != "")
            if (Damage?.DamageSlots != null && Damage.DamageSlots.Count > 0)
                spellPower = Damage?.DamageSlots.Average(item => DataManipulation.GetDiceValue(item.Value, member)) ?? 0;

        return (int)spellPower;
    }

    public int GetSpellPower(Monster.SpecialAbility.Spellcasting spellcast, Monster monster)
    {
        var spellPower = 0.0;

        if (IsDamageSpell())
            if (Damage?.DamageSlots != null)
                spellPower = Damage.DamageSlots.Where(item => spellcast.SpellSlots.GetSlotsLevelAvailable() >= item.Key).Average(x => DataManipulation.GetDiceValue(x.Value, monster));
            else if (Damage?.DamageAtCharacterLevel != null)
                spellPower = Damage.DamageAtCharacterLevel.Where(item => spellcast.Level >= item.Key).Average(x => DataManipulation.GetDiceValue(x.Value, monster));

        return (int)spellPower;
    }

    public double GetSpellPercentage(Member partyMember, List<Monster> monsters)
    {
        var spellPercentage = 1.0;
        var attackBonus = 0;
        var averageMonsterAc = (int)monsters.Average(item => item.AC.Average(x => x.Value));
        var averageMonsterSaveBonus = 0;
        var spellAbilityModifier = DataManipulation.GetSpellcastingModifier(partyMember);

        if (IsDamageSpell())
        {
            if (RequiresAttackRoll())
            {
                attackBonus = spellAbilityModifier + partyMember.ProficiencyBonus;
                spellPercentage = DataManipulation.CalculateRollPercentage(averageMonsterAc, attackBonus);
            }
            else if (RequiresSavingThrow() && Dc != null)
            {
                var saveDc = 8 + partyMember.ProficiencyBonus + spellAbilityModifier;

                switch (DataManipulation.ConvertAbilityIndex(Dc.DcType.Index))
                {
                    case "strength":
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Strength.Modifier);
                        break;
                    case "dexterity":
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Dexterity.Modifier);
                        break;
                    case "constitution":
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Constitution.Modifier);
                        break;
                    case "intelligence":
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Intelligence.Modifier);
                        break;
                    case "wisdom":
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Wisdom.Modifier);
                        break;
                    case "charisma":
                        averageMonsterSaveBonus = (int)monsters.Average(item => item.Charisma.Modifier);
                        break;
                }

                spellPercentage = (int)(1.0 - DataManipulation.CalculateRollPercentage(saveDc, averageMonsterSaveBonus));
            }
        }

        return spellPercentage;
    }

    public double GetSpellPercentage(Monster.SpecialAbility.Spellcasting spellcast, Monster monster, List<Member> party)
    {
        var spellPercentage = 1.0;
        var averagePartyAc = (int)party.Average(item => item.ArmorClass);
        var averagePartySaveBonus = 0;
        var spellAbilityModifier = DataManipulation.GetSpellcastingModifier(monster);

        if (IsDamageSpell())
        {
            if (RequiresAttackRoll())
                spellPercentage = DataManipulation.CalculateRollPercentage(averagePartyAc, spellcast.Modifier);
            else if (RequiresSavingThrow() && Dc != null)
            {
                switch (DataManipulation.ConvertAbilityIndex(Dc.DcType.Index))
                {
                    case "strength":
                        averagePartySaveBonus = (int)party.Average(item => item.Strength.Modifier);
                        break;
                    case "dexterity":
                        averagePartySaveBonus = (int)party.Average(item => item.Dexterity.Modifier);
                        break;
                    case "constitution":
                        averagePartySaveBonus = (int)party.Average(item => item.Constitution.Modifier);
                        break;
                    case "intelligence":
                        averagePartySaveBonus = (int)party.Average(item => item.Intelligence.Modifier);
                        break;
                    case "wisdom":
                        averagePartySaveBonus = (int)party.Average(item => item.Wisdom.Modifier);
                        break;
                    case "charisma":
                        averagePartySaveBonus = (int)party.Average(item => item.Charisma.Modifier);
                        break;
                }

                spellPercentage = (int)(1.0 - DataManipulation.CalculateRollPercentage(spellcast.Dc, averagePartySaveBonus));
            }
        }

        return spellPercentage;
    }

    public int GetHealingPower(Slots spellSlots, Member member)
    {
        var healingPower = 0;

        if (IsHealingSpell() && HealAtSlotLevel != null)
            healingPower = (int)HealAtSlotLevel.Where(item => spellSlots.GetSlotsLevelAvailable() >= item.Key).Average(x => DataManipulation.GetDiceValue(x.Value, member));

        return healingPower;
    }
    public int GetHealingPower(Slots spellSlots, Monster monster)
    {
        var healingPower = 0;

        if (IsHealingSpell() && HealAtSlotLevel != null)
            healingPower = (int)HealAtSlotLevel.Where(item => spellSlots.GetSlotsLevelAvailable() >= item.Key).Average(x => DataManipulation.GetDiceValue(x.Value, monster));

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