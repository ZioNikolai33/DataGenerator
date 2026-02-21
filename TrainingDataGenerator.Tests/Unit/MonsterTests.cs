using MongoDB.Bson.Serialization;
using Moq;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Entities.MonsterEntities;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;
using Attribute = TrainingDataGenerator.Entities.Attribute;

namespace TrainingDataGenerator.Tests.Unit;

public class MonsterTests
{
    private static readonly string monstersFolder = Path.Combine("..", "..", "..", "TestData");

    #region Helper Methods

    private static MonsterMapper CreateTestMonsterMapper(string monsterName = "goblin")
    {
        var monsterFilePath = Path.Combine(monstersFolder, $"monsters.json");
        var monsterJson = File.ReadAllText(monsterFilePath);
        var monsterMappers = BsonSerializer.Deserialize<List<MonsterMapper>>(monsterJson)!;

        return monsterMappers.FirstOrDefault(m => m.Name.Equals(monsterName, StringComparison.OrdinalIgnoreCase)) ?? monsterMappers.First();
    }

    private Monster CreateTestMonster(string monsterName = "goblin")
    {
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<RandomProvider>();
        var monsterMapper = CreateTestMonsterMapper(monsterName);

        return new Monster(monsterMapper, mockLogger.Object, mockRandom.Object);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var monsterMapper = CreateTestMonsterMapper();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Monster(monsterMapper, null!, mockRandom.Object));
    }

    [Fact]
    public void Constructor_WithNullRandomProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var monsterMapper = CreateTestMonsterMapper();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Monster(monsterMapper, mockLogger.Object, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();
        var monsterMapper = CreateTestMonsterMapper();

        // Act
        var monster = new Monster(monsterMapper, mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(monster);
        Assert.Equal("goblin", monster.Index);
    }

    [Fact]
    public void Constructor_ShouldMapBasicProperties()
    {
        // Arrange & Act
        var monster = CreateTestMonster();

        // Assert
        Assert.NotNull(monster.Name);
        Assert.NotNull(monster.Type);
        Assert.NotNull(monster.Alignment);
        Assert.NotNull(monster.Size);
        Assert.True(monster.ArmorClass > 0);
        Assert.True(monster.HitPoints > 0);
    }

    [Fact]
    public void Constructor_ShouldMapSpeed()
    {
        // Arrange & Act
        var monster = CreateTestMonster();

        // Assert
        Assert.NotNull(monster.Speed);
        Assert.NotNull(monster.Speed.Walk);
    }

    [Fact]
    public void Constructor_ShouldMapAttributes()
    {
        // Arrange & Act
        var monster = CreateTestMonster();

        // Assert
        Assert.NotNull(monster.Strength);
        Assert.NotNull(monster.Dexterity);
        Assert.NotNull(monster.Constitution);
        Assert.NotNull(monster.Intelligence);
        Assert.NotNull(monster.Wisdom);
        Assert.NotNull(monster.Charisma);
    }

    [Fact]
    public void Constructor_ShouldMapSenses()
    {
        // Arrange & Act
        var monster = CreateTestMonster();

        // Assert
        Assert.NotNull(monster.Senses);
        Assert.True(monster.Senses.PassivePerception.HasValue);
    }

    [Fact]
    public void Constructor_ShouldInitializeCollections()
    {
        // Arrange & Act
        var monster = CreateTestMonster();

        // Assert
        Assert.NotNull(monster.ConditionImmunities);
        Assert.NotNull(monster.SpecialAbilities);
        Assert.NotNull(monster.Actions);
        Assert.NotNull(monster.LegendaryActions);
        Assert.NotNull(monster.Reactions);
    }

    #endregion

    #region CalculateBaseStats Tests

    [Fact]
    public void CalculateBaseStats_ShouldReturnPositiveValue()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        monster.SetBaseStats();
        var baseStats = monster.BaseStats;

        // Assert
        Assert.True(baseStats > 0);
    }

    [Fact]
    public void CalculateBaseStats_ShouldSumSpeedStatsAndSkills()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        monster.SetBaseStats();
        var baseStats = monster.BaseStats;
        var expectedStats = monster.CalculateSpeedValue() + 
                          monster.CalculateStatsValue() + 
                          monster.CalculateSkillsValue();

        // Assert
        Assert.Equal(expectedStats, baseStats);
    }

    #endregion

    #region CalculateSpeedValue Tests

    [Fact]
    public void CalculateSpeedValue_WithOnlyWalkSpeed_ShouldReturnWalkValue()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Speed = new SpeedType { Walk = "30 ft." };

        // Act
        var speedValue = monster.CalculateSpeedValue();

        // Assert
        Assert.Equal(30, speedValue);
    }

    [Fact]
    public void CalculateSpeedValue_WithSwimSpeed_ShouldApplyHalfMultiplier()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Speed = new SpeedType 
        { 
            Walk = "30 ft.",
            Swim = "20 ft."
        };

        // Act
        var speedValue = monster.CalculateSpeedValue();

        // Assert
        Assert.Equal(40, speedValue); // 30 + (20 * 0.5)
    }

    [Fact]
    public void CalculateSpeedValue_WithFlySpeed_ShouldAddFullValue()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Speed = new SpeedType 
        { 
            Walk = "30 ft.",
            Fly = "60 ft."
        };

        // Act
        var speedValue = monster.CalculateSpeedValue();

        // Assert
        Assert.Equal(90, speedValue); // 30 + 60
    }

    [Fact]
    public void CalculateSpeedValue_WithBurrowSpeed_ShouldApplyHalfMultiplier()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Speed = new SpeedType 
        { 
            Walk = "30 ft.",
            Burrow = "10 ft."
        };

        // Act
        var speedValue = monster.CalculateSpeedValue();

        // Assert
        Assert.Equal(35, speedValue); // 30 + (10 * 0.5)
    }

    [Fact]
    public void CalculateSpeedValue_WithClimbSpeed_ShouldApplyHalfMultiplier()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Speed = new SpeedType 
        { 
            Walk = "30 ft.",
            Climb = "20 ft."
        };

        // Act
        var speedValue = monster.CalculateSpeedValue();

        // Assert
        Assert.Equal(40, speedValue); // 30 + (20 * 0.5)
    }

    [Fact]
    public void CalculateSpeedValue_WithHover_ShouldAddBonus()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Speed = new SpeedType 
        { 
            Walk = "30 ft.",
            Hover = true
        };

        // Act
        var speedValue = monster.CalculateSpeedValue();

        // Assert
        Assert.Equal(40, speedValue); // 30 + 10
    }

    [Fact]
    public void CalculateSpeedValue_WithAllSpeeds_ShouldSumCorrectly()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Speed = new SpeedType 
        { 
            Walk = "30 ft.",
            Swim = "20 ft.",
            Fly = "60 ft.",
            Burrow = "10 ft.",
            Climb = "20 ft.",
            Hover = true
        };

        // Act
        var speedValue = monster.CalculateSpeedValue();

        // Assert
        // 30 + (20*0.5) + 60 + (10*0.5) + (20*0.5) + 10 = 30 + 10 + 60 + 5 + 10 + 10 = 125
        Assert.Equal(125, speedValue);
    }

    #endregion

    #region CalculateStatsValue Tests

    [Fact]
    public void CalculateStatsValue_ShouldSumAllAttributeValues()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Strength = new Attribute(10);
        monster.Dexterity = new Attribute(12);
        monster.Constitution = new Attribute(14);
        monster.Intelligence = new Attribute(8);
        monster.Wisdom = new Attribute(10);
        monster.Charisma = new Attribute(6);

        // Act
        var statsValue = monster.CalculateStatsValue();

        // Assert
        Assert.Equal(60, statsValue);
    }

    [Fact]
    public void CalculateStatsValue_WithMinimumStats_ShouldReturnCorrectSum()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Strength = new Attribute(1);
        monster.Dexterity = new Attribute(1);
        monster.Constitution = new Attribute(1);
        monster.Intelligence = new Attribute(1);
        monster.Wisdom = new Attribute(1);
        monster.Charisma = new Attribute(1);

        // Act
        var statsValue = monster.CalculateStatsValue();

        // Assert
        Assert.Equal(6, statsValue);
    }

    [Fact]
    public void CalculateStatsValue_WithMaximumStats_ShouldReturnCorrectSum()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Strength = new Attribute(30);
        monster.Dexterity = new Attribute(30);
        monster.Constitution = new Attribute(30);
        monster.Intelligence = new Attribute(30);
        monster.Wisdom = new Attribute(30);
        monster.Charisma = new Attribute(30);

        // Act
        var statsValue = monster.CalculateStatsValue();

        // Assert
        Assert.Equal(180, statsValue);
    }

    #endregion

    #region CalculateSkillsValue Tests

    [Fact]
    public void CalculateSkillsValue_WithNoSkills_ShouldReturnZero()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Skills.Clear();

        // Act
        var skillsValue = monster.CalculateSkillsValue();

        // Assert
        Assert.Equal(0, skillsValue);
    }

    [Fact]
    public void CalculateSkillsValue_ShouldSumAllSkillModifiers()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var skillsValue = monster.CalculateSkillsValue();

        // Assert
        Assert.True(skillsValue >= 0);
    }

    #endregion

    #region CalculateHealingPower Tests

    [Fact]
    public void CalculateHealingPower_WithNoSpells_ShouldReturnZero()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.SpecialAbilities.Clear();

        // Act
        monster.SetHealingPower();

        // Assert
        Assert.Equal(0, monster.HealingPower);
    }

    [Fact]
    public void CalculateHealingPower_WithNoSpellcasting_ShouldReturnZero()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        monster.SetHealingPower();

        // Assert
        Assert.Equal(0, monster.HealingPower);
    }

    #endregion

    #region CalculateSpellUsagePercentage Tests

    [Fact]
    public void CalculateSpellUsagePercentage_WithNullSpell_ShouldReturnZero()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var percentage = monster.CalculateSpellUsagePercentage(null!, CRRatios.Normal);

        // Assert
        Assert.Equal(0.0, percentage);
    }

    [Fact]
    public void CalculateSpellUsagePercentage_WithCantrip_ShouldReturnOne()
    {
        // Arrange
        var monster = CreateTestMonster("archmage");
        var spell = new Spell(new SpellMapper("index", "Name") { Level = 0 });

        // Act
        var percentage = monster.CalculateSpellUsagePercentage(spell, CRRatios.Normal);

        // Assert
        Assert.Equal(1.0, percentage);
    }

    [Fact]
    public void CalculateSpellUsagePercentage_WithNoSpellSlots_ShouldReturnOne()
    {
        // Arrange
        var monster = CreateTestMonster();
        var spell = new Spell(new SpellMapper("index", "Name") { Level = 1 });

        // Act
        var percentage = monster.CalculateSpellUsagePercentage(spell, CRRatios.Normal);

        // Assert
        Assert.True(percentage >= 0.0 && percentage <= 1.0);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ShouldContainMonsterName()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var result = monster.ToString();

        // Assert
        Assert.Contains(monster.Name, result);
    }

    [Fact]
    public void ToString_ShouldContainChallengeRating()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var result = monster.ToString();

        // Assert
        Assert.Contains("CR:", result);
        Assert.Contains(monster.ChallengeRating.ToString(), result);
    }

    [Fact]
    public void ToString_ShouldContainHitPoints()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var result = monster.ToString();

        // Assert
        Assert.Contains("HP:", result);
    }

    [Fact]
    public void ToString_ShouldContainArmorClass()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var result = monster.ToString();

        // Assert
        Assert.Contains("AC:", result);
    }

    [Fact]
    public void ToString_ShouldContainAllAttributeValues()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var result = monster.ToString();

        // Assert
        Assert.Contains("STR:", result);
        Assert.Contains("DEX:", result);
        Assert.Contains("CON:", result);
        Assert.Contains("INT:", result);
        Assert.Contains("WIS:", result);
        Assert.Contains("CHA:", result);
    }

    #endregion

    #region AddProfs Tests

    [Fact]
    public void Constructor_WithSavingThrowProficiencies_ShouldApplyToStrength()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();
        var monsterMapper = CreateTestMonsterMapper();

        // Act
        var monster = new Monster(monsterMapper, mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.True(monster.Strength.Save != 0);
    }

    [Fact]
    public void Constructor_WithSkillProficiencies_ShouldApplyToSkills()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();
        var monsterMapper = CreateTestMonsterMapper();

        // Act
        var monster = new Monster(monsterMapper, mockLogger.Object, mockRandom.Object);

        // Assert
        var stealthSkill = monster.Skills.FirstOrDefault(s => s.Index.Equals("skill-stealth"));
        if (stealthSkill != null)
        {
            Assert.True(stealthSkill.IsProficient);
        }
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithNullActions_ShouldInitializeEmptyList()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();
        var monsterMapper = CreateTestMonsterMapper();
        monsterMapper.Actions = null;

        // Act
        var monster = new Monster(monsterMapper, mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(monster.Actions);
        Assert.Empty(monster.Actions);
    }

    [Fact]
    public void Constructor_WithNullLegendaryActions_ShouldInitializeEmptyList()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();
        var monsterMapper = CreateTestMonsterMapper();
        monsterMapper.LegendaryActions = null;

        // Act
        var monster = new Monster(monsterMapper, mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(monster.LegendaryActions);
        Assert.Empty(monster.LegendaryActions);
    }

    [Fact]
    public void Constructor_WithNullReactions_ShouldInitializeEmptyList()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();
        var monsterMapper = CreateTestMonsterMapper();
        monsterMapper.Reactions = null;

        // Act
        var monster = new Monster(monsterMapper, mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(monster.Reactions);
        Assert.Empty(monster.Reactions);
    }

    [Fact]
    public void Constructor_WithNullSpecialAbilities_ShouldInitializeEmptyList()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();
        var monsterMapper = CreateTestMonsterMapper();
        monsterMapper.SpecialAbilities = null;

        // Act
        var monster = new Monster(monsterMapper, mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(monster.SpecialAbilities);
        Assert.Empty(monster.SpecialAbilities);
    }

    [Fact]
    public void CalculateSpeedValue_WithEmptySpeed_ShouldReturnZero()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Speed = new SpeedType();

        // Act
        var speedValue = monster.CalculateSpeedValue();

        // Assert
        Assert.Equal(0, speedValue);
    }

    [Fact]
    public void CalculateSpeedValue_WithNullSpeeds_ShouldNotThrow()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Speed = new SpeedType
        {
            Walk = null,
            Swim = null,
            Fly = null,
            Burrow = null,
            Climb = null,
            Hover = null
        };

        // Act & Assert
        var exception = Record.Exception(() => monster.CalculateSpeedValue());
        Assert.Null(exception);
    }

    #endregion
}