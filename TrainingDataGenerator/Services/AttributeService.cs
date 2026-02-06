using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Extensions;
using TrainingDataGenerator.Interfaces;
using Attribute = TrainingDataGenerator.Entities.Attribute;

namespace TrainingDataGenerator.Services;

public class AttributeService : IAttributeService
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;

    // Standard Array values for D&D 5e character creation
    private static readonly byte[] StandardArrayValues = { 15, 14, 13, 12, 10, 8 };
    private const byte MaxAttributeValue = 20;
    private const byte AbilityScoreImprovementPoints = 2;

    public AttributeService(
        ILogger logger,
        IRandomProvider random)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public void SetAttributes(
        PartyMember member, 
        ClassMapper classMapper, 
        List<AbilityBonus> raceAbilityBonuses, 
        List<FeatureMapper> features)
    {
        _logger.Verbose($"Setting attributes for {member.Name}");

        // Generate base attributes using standard array
        var attributes = GenerateStandardArray();
        _logger.Verbose($"Generated base attributes: [{string.Join(", ", attributes)}]");

        // Apply racial bonuses
        if (raceAbilityBonuses.Count > 0)
        {
            ApplyRacialBonuses(attributes, raceAbilityBonuses);
            _logger.Verbose($"Applied racial bonuses: [{string.Join(", ", attributes)}]");
        }

        // Apply ability score improvements from leveling
        var abilityImprovements = (byte)features.Count(f => f.Index.Contains("ability-score-improvement"));

        if (abilityImprovements > 0)
        {
            ApplyAbilityScoreImprovements(attributes, abilityImprovements);
            _logger.Verbose($"Applied {abilityImprovements} ability score improvements: [{string.Join(", ", attributes)}]");
        }

        // Apply class-specific bonuses (e.g., Barbarian Primal Champion)
        ApplyClassSpecificBonuses(attributes, member.Class, features);

        // Set the attributes on the party member
        member.Strength = new Attribute(attributes[0]);
        member.Dexterity = new Attribute(attributes[1]);
        member.Constitution = new Attribute(attributes[2]);
        member.Intelligence = new Attribute(attributes[3]);
        member.Wisdom = new Attribute(attributes[4]);
        member.Charisma = new Attribute(attributes[5]);

        _logger.Verbose($"Final attributes - STR: {attributes[0]}, DEX: {attributes[1]}, " +
                       $"CON: {attributes[2]}, INT: {attributes[3]}, WIS: {attributes[4]}, CHA: {attributes[5]}");
    }

    public List<byte> GenerateStandardArray()
    {
        var attributes = StandardArrayValues.ToList();
        
        // Shuffle the array randomly to assign to different abilities
        return _random.Shuffle(attributes).ToList();
    }

    public void ApplyRacialBonuses(List<byte> attributes, List<AbilityBonus> raceAbilityBonuses)
    {
        foreach (var bonus in raceAbilityBonuses)
        {
            var abilityIndex = GetAbilityIndex(bonus.Ability.Index);

            if (abilityIndex >= 0 && abilityIndex < attributes.Count)
            {
                attributes[abilityIndex] += (byte)bonus.Bonus;
                _logger.Verbose($"Applied +{bonus.Bonus} to {bonus.Ability.Index.ToUpper()}");
            }
        }
    }

    public void ApplyAbilityScoreImprovements(List<byte> attributes, byte numberOfImprovements)
    {
        for (byte improvement = 0; improvement < numberOfImprovements; improvement++)
        {
            var pointsRemaining = AbilityScoreImprovementPoints;

            while (pointsRemaining > 0)
            {
                if (attributes.All(a => a >= MaxAttributeValue))
                    return;

                var attributeIndex = _random.Next(0, 6);

                // Don't increase if already at max
                if (attributes[attributeIndex] >= MaxAttributeValue)
                    continue;

                attributes[attributeIndex]++;
                pointsRemaining--;

                _logger.Verbose($"ASI {improvement + 1}: Increased attribute {attributeIndex} to {attributes[attributeIndex]}");
            }
        }
    }

    public void ApplyClassSpecificBonuses(List<byte> attributes, string classIndex, List<FeatureMapper> features)
    {
        // Barbarian: Primal Champion (+4 STR, +4 CON)
        if (classIndex.Equals("barbarian", StringComparison.OrdinalIgnoreCase))
        {
            if (features.Any(f => f.Index == "primal-champion"))
            {
                attributes[0] += 4; // Strength
                attributes[2] += 4; // Constitution
                
                _logger.Verbose("Applied Primal Champion bonus: +4 STR, +4 CON");
            }
        }
    }

    public void AddSavingThrowProficiencies(PartyMember member, ClassMapper classMapper)
    {
        _logger.Verbose($"Adding saving throw proficiencies for {member.Name}");

        foreach (var savingThrow in classMapper.SavingThrows)
        {
            switch (savingThrow.Index.ToLower())
            {
                case "str":
                    member.Strength?.SetProficiency(true, (sbyte)member.ProficiencyBonus);
                    _logger.Verbose($"Added STR saving throw proficiency (+{member.ProficiencyBonus})");
                    break;
                case "dex":
                    member.Dexterity?.SetProficiency(true, (sbyte)member.ProficiencyBonus);
                    _logger.Verbose($"Added DEX saving throw proficiency (+{member.ProficiencyBonus})");
                    break;
                case "con":
                    member.Constitution?.SetProficiency(true, (sbyte)member.ProficiencyBonus);
                    _logger.Verbose($"Added CON saving throw proficiency (+{member.ProficiencyBonus})");
                    break;
                case "int":
                    member.Intelligence?.SetProficiency(true, (sbyte)member.ProficiencyBonus);
                    _logger.Verbose($"Added INT saving throw proficiency (+{member.ProficiencyBonus})");
                    break;
                case "wis":
                    member.Wisdom?.SetProficiency(true, (sbyte)member.ProficiencyBonus);
                    _logger.Verbose($"Added WIS saving throw proficiency (+{member.ProficiencyBonus})");
                    break;
                case "cha":
                    member.Charisma?.SetProficiency(true, (sbyte)member.ProficiencyBonus);
                    _logger.Verbose($"Added CHA saving throw proficiency (+{member.ProficiencyBonus})");
                    break;
            }
        }
    }

    public int GetAttributeModifier(byte attributeValue)
    {
        return (int)Math.Floor((attributeValue - 10) / 2.0);
    }

    #region Private Helper Methods

    private int GetAbilityIndex(string abilityIndex)
    {
        return abilityIndex.ToLower() switch
        {
            "str" => 0,
            "dex" => 1,
            "con" => 2,
            "int" => 3,
            "wis" => 4,
            "cha" => 5,
            _ => -1
        };
    }

    #endregion
}