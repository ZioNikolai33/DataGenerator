using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Extensions;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Services;

public class ProficiencyService : IProficiencyService
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;

    public ProficiencyService(
        ILogger logger,
        IRandomProvider random)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public void SetInitialProficiencies(
        PartyMember member, 
        ClassMapper classMapper, 
        RaceMapper raceMapper, 
        SubraceMapper? subraceMapper)
    {
        _logger.Verbose($"Setting initial proficiencies for {member.Name}");

        member.Proficiencies = new List<string>();

        // Get racial traits
        var raceTraits = GetRaceTraits(raceMapper, subraceMapper);

        // Add starting proficiencies from race
        member.Proficiencies.AddRange(raceMapper.StartingProficiences.Select(p => p.Index));
        _logger.Verbose($"Added {raceMapper.StartingProficiences.Count} racial proficiencies");

        // Add starting proficiencies from class
        member.Proficiencies.AddRange(classMapper.Proficiencies.Select(p => p.Index));
        _logger.Verbose($"Added {classMapper.Proficiencies.Count} class proficiencies");

        // Add proficiencies from racial traits
        if (raceTraits.Count > 0)
        {
            foreach (var trait in raceTraits)
            {
                if (trait.Proficiencies.Count > 0)
                {
                    member.Proficiencies.AddRange(trait.Proficiencies.Select(p => p.Index));
                }
            }
        }

        // Add random proficiencies from race
        var raceProficiencies = raceMapper.GetRandomProficiency(member.Proficiencies);
        member.Proficiencies.AddRange(raceProficiencies);
        if (raceProficiencies.Count > 0)
            _logger.Verbose($"Added {raceProficiencies.Count} random racial proficiencies");

        // Add random proficiencies from class choices
        if (classMapper.ProficiencyChoices.Count > 0)
        {
            foreach (var choice in classMapper.ProficiencyChoices)
            {
                var chosen = choice.GetRandomChoice(member.Proficiencies);
                member.Proficiencies.AddRange(chosen);
                if (chosen.Count > 0)
                    _logger.Verbose($"Added {chosen.Count} proficiencies from class choices");
            }
        }

        // Add random proficiencies from racial trait choices
        if (raceTraits.Count > 0)
        {
            foreach (var trait in raceTraits)
            {
                if (trait.ProficiencyChoice != null)
                {
                    var chosen = trait.ProficiencyChoice.GetRandomChoice(member.Proficiencies);
                    member.Proficiencies.AddRange(chosen);
                    if (chosen.Count > 0)
                        _logger.Verbose($"Added {chosen.Count} proficiencies from trait choices");
                }
            }
        }

        // Remove duplicates
        var originalCount = member.Proficiencies.Count;
        member.Proficiencies = member.Proficiencies.Distinct().ToList();
        
        if (originalCount != member.Proficiencies.Count)
            _logger.Verbose($"Removed {originalCount - member.Proficiencies.Count} duplicate proficiencies");

        _logger.Verbose($"Total proficiencies: {member.Proficiencies.Count}");
    }

    public void AddBackgroundProficiencies(PartyMember member)
    {
        _logger.Verbose($"Adding background proficiencies for {member.Name}");

        // Custom Background: 2 Skills, 2 Tool Proficiencies or Languages (tools/languages not managed)
        var allSkills = GetAllSkillIndices(member);
        var availableSkills = allSkills
            .Where(skill => !member.Proficiencies.Contains(skill))
            .ToList();

        if (availableSkills.Count < 2)
        {
            _logger.Warning($"Only {availableSkills.Count} skills available for background proficiencies");
            member.Proficiencies.AddRange(availableSkills);
            return;
        }

        var selectedSkills = _random.SelectRandom(availableSkills, 2);
        member.Proficiencies.AddRange(selectedSkills);
        
        _logger.Verbose($"Added 2 background skill proficiencies: {string.Join(", ", selectedSkills)}");
    }

    public void AddAdditionalProficiencies(PartyMember member)
    {
        _logger.Verbose($"Adding additional proficiencies from traits and features for {member.Name}");

        var addedCount = 0;

        // Trait-based proficiencies
        if (member.Traits.Count > 0)
        {
            if (member.Traits.Contains("keen-senses"))
            {
                member.Proficiencies.Add("skill-perception");
                addedCount++;
                _logger.Verbose("Added Perception from Keen Senses trait");
            }

            if (member.Traits.Contains("elf-weapon-training"))
            {
                var elfWeapons = new List<string> { "shortsword", "longsword", "shortbow", "longbow" };
                member.Proficiencies.AddRange(elfWeapons);
                addedCount += elfWeapons.Count;
                _logger.Verbose($"Added Elf Weapon Training: {string.Join(", ", elfWeapons)}");
            }

            if (member.Traits.Contains("dwarven-combat-training"))
            {
                var dwarvenWeapons = new List<string> { "battleaxe", "handaxe", "light-hammer", "warhammer" };
                member.Proficiencies.AddRange(dwarvenWeapons);
                addedCount += dwarvenWeapons.Count;
                _logger.Verbose($"Added Dwarven Combat Training: {string.Join(", ", dwarvenWeapons)}");
            }

            if (member.Traits.Contains("menacing"))
            {
                member.Proficiencies.Add("skill-intimidation");
                addedCount++;
                _logger.Verbose("Added Intimidation from Menacing trait");
            }
        }

        // Feature-based proficiencies
        if (member.Features.Count > 0)
        {
            // Life Cleric Bonus Proficiency
            if (member.Class == "cleric" && 
                member.Subclass == "life" && 
                member.Features.Any(f => f.Index == "bonus-proficiency"))
            {
                member.Proficiencies.Add("heavy-armor");
                addedCount++;
                _logger.Verbose("Added Heavy Armor proficiency from Life Domain");
            }
        }

        _logger.Verbose($"Added {addedCount} additional proficiencies");
    }

    public void ApplySkillProficiencies(PartyMember member)
    {
        _logger.Verbose($"Applying skill proficiencies for {member.Name}");

        var featureIndices = member.Features.Select(f => f.Index).ToList();

        // Skill Versatility (Half-Elf) - 2 random skills
        if (member.Traits.Contains("skill-versatility"))
        {
            var additionalSkills = _random.SelectRandom(GetAllSkillIndices(member), 2);
            member.Proficiencies.AddRange(additionalSkills);
            _logger.Verbose($"Added 2 skills from Skill Versatility: {string.Join(", ", additionalSkills)}");
        }

        // Bonus Proficiencies (Bard) - 3 random skills
        if (member.Class == "bard" && featureIndices.Contains("bonus-proficiencies"))
        {
            var additionalSkills = _random.SelectRandom(GetAllSkillIndices(member), 3);
            member.Proficiencies.AddRange(additionalSkills);
            _logger.Verbose($"Added 3 skills from Bard Bonus Proficiencies: {string.Join(", ", additionalSkills)}");
        }

        // Apply proficiency bonuses to skills
        var proficientSkillsCount = 0;
        foreach (var skill in member.Skills)
        {
            if (member.Proficiencies.Contains(skill.Index))
            {
                skill.SetProficiency(true, member.ProficiencyBonus);
                proficientSkillsCount++;
            }
        }
        _logger.Verbose($"Applied proficiency bonus to {proficientSkillsCount} skills");

        // Jack of All Trades (Bard) - Half proficiency to non-proficient skills
        if (featureIndices.Contains("jack-of-all-trades"))
        {
            var halfProfBonus = (sbyte)Math.Floor(member.ProficiencyBonus / 2.0);
            var affectedSkills = member.Skills.Where(s => !s.IsProficient).ToList();
            
            foreach (var skill in affectedSkills)
            {
                skill.Modifier += halfProfBonus;
            }
            
            _logger.Verbose($"Applied Jack of All Trades (+{halfProfBonus}) to {affectedSkills.Count} non-proficient skills");
        }

        // Remarkable Athlete (Champion Fighter) - Half proficiency to STR/DEX skills
        if (featureIndices.Contains("remarkable-athlete"))
        {
            var athleticSkills = new List<string> 
            { 
                "skill-acrobatics", 
                "skill-athletics", 
                "skill-sleight-of-hand", 
                "skill-stealth" 
            };
            
            var halfProfBonus = (sbyte)Math.Floor(member.ProficiencyBonus / 2.0);
            var affectedSkills = member.Skills
                .Where(s => !s.IsProficient && athleticSkills.Contains(s.Index))
                .ToList();
            
            foreach (var skill in affectedSkills)
            {
                skill.Modifier += halfProfBonus;
            }
            
            _logger.Verbose($"Applied Remarkable Athlete (+{halfProfBonus}) to {affectedSkills.Count} athletic skills");
        }
    }

    public List<string> GetAllSkillIndices(PartyMember member)
    {
        return member.Skills.Select(s => s.Index).ToList();
    }

    public bool HasProficiency(PartyMember member, string proficiencyIndex)
    {
        return member.Proficiencies.Contains(proficiencyIndex);
    }

    #region Private Helper Methods

    private List<TraitMapper> GetRaceTraits(RaceMapper raceMapper, SubraceMapper? subraceMapper)
    {
        if (raceMapper.Traits.Count == 0)
            return new List<TraitMapper>();

        var allTraitEntities = raceMapper.Traits.ToList();

        if (subraceMapper != null)
        {
            allTraitEntities.AddRange(subraceMapper.RacialTraits);
        }

        var raceTraits = allTraitEntities
            .Select(traitEntity => EntitiesFinder.GetEntityByIndex(
                Lists.traits,
                new BaseEntity(raceMapper.Index, raceMapper.Name),
                new BaseEntity(subraceMapper?.Index ?? string.Empty, subraceMapper?.Name ?? string.Empty),
                traitEntity))
            .Where(trait => trait != null && trait.Parent == null)
            .ToList();

        return raceTraits;
    }

    #endregion
}