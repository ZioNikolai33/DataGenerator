using MongoDB.Bson.Serialization;
using Moq;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Tests.Unit;

public class AttributeServiceTests
{
    private static readonly string classesFolder = Path.Combine("..", "..", "..", "TestData", "Classes");
    private static readonly string racesFolder = Path.Combine("..", "..", "..", "TestData", "Races");

    #region Helper Methods

    private PartyMember CreateTestPartyMember(
        int id = 1,
        byte level = 1,
        string raceString = "human",
        string classString = "fighter")
        {
            var mockLogger = new Mock<ILogger>();
            var mockRandom = new Mock<RandomProvider>();
            var mockAttributeService = new Mock<IAttributeService>();
            var mockEquipmentService = new Mock<IEquipmentService>();
            var mockSpellService = new Mock<ISpellService>();
            var mockFeatureService = new Mock<IFeatureService>();
            var mockProficiencyService = new Mock<IProficiencyService>();
            var mockTraitService = new Mock<ITraitService>();
            var mockResistanceService = new Mock<IResistanceService>();

        var raceMapper = CreateTestRace(raceString);
        var classMapper = CreateTestClass(classString);

        return new PartyMember(
                id,
                level,
                raceMapper,
                classMapper,
                mockLogger.Object,
                mockRandom.Object,
                mockAttributeService.Object,
                mockEquipmentService.Object,
                mockSpellService.Object,
                mockFeatureService.Object,
                mockProficiencyService.Object,
                mockTraitService.Object,
                mockResistanceService.Object
            );
        }

    public static IEnumerable<object[]> GetTestClassNames()
    {
        foreach (var className in DataConstants.Classes)
            yield return new object[] { className };
    }

    public static IEnumerable<object[]> GetValueToModifiers()
    {
        foreach (var kvp in DataConstants.ValueToModifiers)
            yield return new object[] { kvp.Key, kvp.Value };
    }

    public static IEnumerable<object[]> GetAllClassesAndRaces()
    {
        foreach (var race in DataConstants.Races)
            foreach (var className in DataConstants.Classes)
                yield return new object[] { race, className };
    }

    private static ClassMapper CreateTestClass(string className)
    {
        var classFilePath = Path.Combine(classesFolder, $"{className}.json");
        var classJson = File.ReadAllText(classFilePath);

        return BsonSerializer.Deserialize<ClassMapper>(classJson)!;
    }

    private static RaceMapper CreateTestRace(string raceName)
    {
        var raceFilePath = Path.Combine(racesFolder, $"{raceName}.json");
        var raceJson = File.ReadAllText(raceFilePath);

        return BsonSerializer.Deserialize<RaceMapper>(raceJson)!;
    }

    private AttributeService CreateService(Mock<RandomProvider>? mockRandom = null)
    {
        var mockLogger = new Mock<ILogger>();
        mockRandom ??= new Mock<RandomProvider>();

        return new AttributeService(mockLogger.Object, mockRandom.Object);
    }

    private AttributeService CreateService(Mock<IRandomProvider>? mockRandom = null)
    {
        var mockLogger = new Mock<ILogger>();
        mockRandom ??= new Mock<IRandomProvider>();

        return new AttributeService(mockLogger.Object, mockRandom.Object);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AttributeService(null!, mockRandom.Object));
    }

    [Fact]
    public void Constructor_WithNullRandomProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AttributeService(mockLogger.Object, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<RandomProvider>();

        // Act
        var service = new AttributeService(mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(service);
    }

    #endregion

    #region GenerateStandardArray Tests

    [Fact]
    public void GenerateStandardArray_ShouldReturnSixAttributes()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);

        // Act
        var attributes = service.GenerateStandardArray();

        // Assert
        Assert.Equal(6, attributes.Count);
    }

    [Fact]
    public void GenerateStandardArray_ShouldContainStandardArrayValues()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var expectedValues = new List<byte> { 15, 14, 13, 12, 10, 8 };

        // Act
        var attributes = service.GenerateStandardArray();
        var sortedAttributes = attributes.OrderByDescending(x => x).ToList();

        // Assert
        Assert.Equal(expectedValues, sortedAttributes);
    }

    [Fact]
    public void GenerateStandardArray_ShouldShuffleValues()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var shuffledArray = new List<byte> { 10, 15, 8, 14, 13, 12 };
        mockRandom.Setup(r => r.Shuffle(It.IsAny<IEnumerable<byte>>()))
                  .Returns(shuffledArray);
        var service = CreateService(mockRandom);

        // Act
        var attributes = service.GenerateStandardArray();

        // Assert
        Assert.Equal(shuffledArray, attributes);
        mockRandom.Verify(r => r.Shuffle(It.IsAny<IEnumerable<byte>>()), Times.Once);
    }

    #endregion

    #region ApplyRacialBonuses Tests

    [Fact]
    public void ApplyRacialBonuses_WithStrengthBonus_ShouldIncreaseStrength()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 15, 14, 13, 12, 10, 8 };
        var bonuses = new List<AbilityBonus>
        {
            new AbilityBonus(new BaseEntity("str", "Strength"), 2)
        };

        // Act
        service.ApplyRacialBonuses(attributes, bonuses);

        // Assert
        Assert.Equal(17, attributes[0]); // STR at index 0
    }

    [Fact]
    public void ApplyRacialBonuses_WithMultipleBonuses_ShouldApplyAll()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 15, 14, 13, 12, 10, 8 };
        var bonuses = new List<AbilityBonus>
        {
            new AbilityBonus(new BaseEntity("str", "Strength"), 2),
            new AbilityBonus(new BaseEntity("con", "Constitution"), 1)
        };

        // Act
        service.ApplyRacialBonuses(attributes, bonuses);

        // Assert
        Assert.Equal(17, attributes[0]); // STR +2
        Assert.Equal(14, attributes[2]); // CON +1
    }

    [Theory]
    [InlineData("str", 0)]
    [InlineData("dex", 1)]
    [InlineData("con", 2)]
    [InlineData("int", 3)]
    [InlineData("wis", 4)]
    [InlineData("cha", 5)]
    public void ApplyRacialBonuses_WithSpecificAbility_ShouldApplyToCorrectIndex(string abilityIndex, int expectedIndex)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 10, 10, 10, 10, 10, 10 };
        var bonuses = new List<AbilityBonus>
        {
            new AbilityBonus(new BaseEntity(abilityIndex, ""), 2)
        };

        // Act
        service.ApplyRacialBonuses(attributes, bonuses);

        // Assert
        Assert.Equal(12, attributes[expectedIndex]);
    }

    [Fact]
    public void ApplyRacialBonuses_WithInvalidAbilityIndex_ShouldNotModifyAttributes()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 10, 10, 10, 10, 10, 10 };
        var originalAttributes = new List<byte>(attributes);
        var bonuses = new List<AbilityBonus>
        {
            new AbilityBonus(new BaseEntity("invalid", ""), 2)
        };

        // Act
        service.ApplyRacialBonuses(attributes, bonuses);

        // Assert
        Assert.Equal(originalAttributes, attributes);
    }

    [Fact]
    public void ApplyRacialBonuses_WithEmptyBonusList_ShouldNotModifyAttributes()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 15, 14, 13, 12, 10, 8 };
        var originalAttributes = new List<byte>(attributes);
        var bonuses = new List<AbilityBonus>();

        // Act
        service.ApplyRacialBonuses(attributes, bonuses);

        // Assert
        Assert.Equal(originalAttributes, attributes);
    }

    #endregion

    #region ApplyAbilityScoreImprovements Tests

    [Fact]
    public void ApplyAbilityScoreImprovements_WithOneImprovement_ShouldIncreaseTwoPoints()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 15, 14, 13, 12, 10, 8 };
        var initialSum = attributes.Sum(x => x);

        // Act
        service.ApplyAbilityScoreImprovements(attributes, 1);

        // Assert
        var finalSum = attributes.Sum(x => x);
        Assert.Equal(initialSum + 2, finalSum);
    }

    [Fact]
    public void ApplyAbilityScoreImprovements_WithMultipleImprovements_ShouldApplyCorrectly()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 15, 14, 13, 12, 10, 8 };
        var initialSum = attributes.Sum(x => (int)x);
        byte improvements = 3;

        // Act
        service.ApplyAbilityScoreImprovements(attributes, improvements);

        // Assert
        var finalSum = attributes.Sum(x => (int)x);
        Assert.Equal(initialSum + (improvements * 2), finalSum);
    }

    [Fact]
    public void ApplyAbilityScoreImprovements_ShouldNotExceedMaxAttributeValue()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 20, 20, 20, 20, 20, 20 };

        // Act
        service.ApplyAbilityScoreImprovements(attributes, 5);

        // Assert
        Assert.All(attributes, attr => Assert.True(attr <= 20));
    }

    [Fact]
    public void ApplyAbilityScoreImprovements_WithZeroImprovements_ShouldNotModifyAttributes()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 15, 14, 13, 12, 10, 8 };
        var originalAttributes = new List<byte>(attributes);

        // Act
        service.ApplyAbilityScoreImprovements(attributes, 0);

        // Assert
        Assert.Equal(originalAttributes, attributes);
    }

    [Fact]
    public void ApplyAbilityScoreImprovements_ShouldSkipMaxedAttributes()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var callCount = 0;
        mockRandom.Setup(r => r.Next(0, 6))
                  .Returns(() =>
                  {
                      callCount++;
                      return callCount <= 3 ? 0 : 1; // First 3 calls return index 0 (maxed), then switch to index 1
                  });
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 20, 14, 13, 12, 10, 8 }; // First attribute at max

        // Act
        service.ApplyAbilityScoreImprovements(attributes, 1);

        // Assert
        Assert.Equal(20, attributes[0]); // Should remain at max
        Assert.Equal(16, attributes[1]); // Should increase by 2
    }

    #endregion

    #region ApplyClassSpecificBonuses Tests

    [Fact]
    public void ApplyClassSpecificBonuses_BarbarianWithPrimalChampion_ShouldIncreaseSturengthAndConstitution()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 16, 14, 16, 10, 12, 8 };
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("primal-champion", "Primal Champion")
        };

        // Act
        service.ApplyClassSpecificBonuses(attributes, "barbarian", features);

        // Assert
        Assert.Equal(20, attributes[0]); // STR +4
        Assert.Equal(20, attributes[2]); // CON +4
    }

    [Fact]
    public void ApplyClassSpecificBonuses_BarbarianWithoutPrimalChampion_ShouldNotModify()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 16, 14, 16, 10, 12, 8 };
        var originalAttributes = new List<byte>(attributes);
        var features = new List<FeatureMapper>();

        // Act
        service.ApplyClassSpecificBonuses(attributes, "barbarian", features);

        // Assert
        Assert.Equal(originalAttributes, attributes);
    }

    [Fact]
    public void ApplyClassSpecificBonuses_NonBarbarianClass_ShouldNotModify()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 16, 14, 16, 10, 12, 8 };
        var originalAttributes = new List<byte>(attributes);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("primal-champion", "Primal Champion")
        };

        // Act
        service.ApplyClassSpecificBonuses(attributes, "fighter", features);

        // Assert
        Assert.Equal(originalAttributes, attributes);
    }

    [Theory]
    [InlineData("Barbarian")]
    [InlineData("BARBARIAN")]
    [InlineData("BaRbArIaN")]
    public void ApplyClassSpecificBonuses_BarbarianCaseInsensitive_ShouldApply(string className)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 16, 14, 16, 10, 12, 8 };
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("primal-champion", "Primal Champion")
        };

        // Act
        service.ApplyClassSpecificBonuses(attributes, className, features);

        // Assert
        Assert.Equal(20, attributes[0]); // STR +4
        Assert.Equal(20, attributes[2]); // CON +4
    }

    #endregion

    #region AddSavingThrowProficiencies Tests

    [Theory]
    [MemberData(nameof(GetTestClassNames))]
    public void AddSavingThrowProficiencies_ShouldSetCorrectAttribute(string classes)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var classMapper = CreateTestClass(classes);
        var savingThrowIndexes = classMapper.SavingThrows.Select(s => s.Index);
        var member = CreateTestPartyMember(1, 1, "human", classes);

        // Act
        service.AddSavingThrowProficiencies(member, classMapper);

        // Assert
        foreach (var savingThrowIndex in savingThrowIndexes)
            switch (savingThrowIndex.ToLower())
            {
                case "str":
                    Assert.Equal(member.Strength.Modifier + member.ProficiencyBonus, member.Strength.Save);
                    break;
                case "dex":
                    Assert.Equal(member.Dexterity.Modifier + member.ProficiencyBonus, member.Dexterity.Save);
                    break;
                case "con":
                    Assert.Equal(member.Constitution.Modifier + member.ProficiencyBonus, member.Constitution.Save);
                    break;
                case "int":
                    Assert.Equal(member.Intelligence.Modifier + member.ProficiencyBonus, member.Intelligence.Save);
                    break;
                case "wis":
                    Assert.Equal(member.Wisdom.Modifier + member.ProficiencyBonus, member.Wisdom.Save);
                    break;
                case "cha":
                    Assert.Equal(member.Charisma.Modifier + member.ProficiencyBonus, member.Charisma.Save);
                    break;
            }
    }

    [Fact]
    public void AddSavingThrowProficiencies_WithMultipleSaves_ShouldApplyAll()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var race = CreateTestRace("human");
        var classMapper = CreateTestClass("fighter");
        var member = CreateTestPartyMember(1, 1);

        // Act
        service.AddSavingThrowProficiencies(member, classMapper);

        // Assert
        Assert.Equal(member.Strength.Modifier + member.ProficiencyBonus, member.Strength.Save);
        Assert.Equal(member.Constitution.Modifier + member.ProficiencyBonus, member.Constitution.Save);
    }

    #endregion

    #region GetAttributeModifier Tests

    [Theory]
    [MemberData(nameof(GetValueToModifiers))]
    public void GetAttributeModifier_ShouldCalculateCorrectly(byte attributeValue, int expectedModifier)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);

        // Act
        var modifier = service.GetAttributeModifier(attributeValue);

        // Assert
        Assert.Equal(expectedModifier, modifier);
    }

    #endregion

    #region SetAttributes Integration Tests

    [Theory]
    [MemberData(nameof(GetAllClassesAndRaces))]
    public void SetAttributes_ShouldSetAllAttributesOnMember(string raceName, string className)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = CreateTestPartyMember(1, 1, raceName, className);

        // Act
        service.SetAttributes(member, classMapper, race.AbilityBonuses, new List<FeatureMapper>());

        // Assert
        Assert.NotNull(member.Strength);
        Assert.NotNull(member.Dexterity);
        Assert.NotNull(member.Constitution);
        Assert.NotNull(member.Intelligence);
        Assert.NotNull(member.Wisdom);
        Assert.NotNull(member.Charisma);
        
        Assert.InRange(member.Strength.Value, 8, 30);
        Assert.InRange(member.Dexterity.Value, 8, 30);
        Assert.InRange(member.Constitution.Value, 8, 30);
        Assert.InRange(member.Intelligence.Value, 8, 30);
        Assert.InRange(member.Wisdom.Value, 8, 30);
        Assert.InRange(member.Charisma.Value, 8, 30);
    }

    [Fact]
    public void SetAttributes_WithAbilityImprovements_ShouldApplyThem()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var race = CreateTestRace("human");
        var classMapper = CreateTestClass("fighter");
        var member = CreateTestPartyMember(1, 4);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("ability-score-improvement-1", string.Empty)
        };

        // Act
        service.SetAttributes(member, classMapper, race.AbilityBonuses, features);

        // Assert
        var totalAttributes = member.Strength.Value + member.Dexterity.Value + member.Constitution.Value +
                             member.Intelligence.Value + member.Wisdom.Value + member.Charisma.Value;
        
        // Standard array sum (72) + human racial bonuses (6) + ASI (2) = 80
        Assert.Equal(80, totalAttributes);
    }

    [Fact]
    public void SetAttributes_BarbarianLevel20WithPrimalChampion_ShouldApplyBonus()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var race = CreateTestRace("human");
        var classMapper = CreateTestClass("barbarian");
        var member = CreateTestPartyMember(1, 20, "human", "barbarian");
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("primal-champion", string.Empty)
        };

        // Act
        service.SetAttributes(member, classMapper, race.AbilityBonuses, features);

        // Assert
        // Should have applied +4 to STR and CON
        var totalAttributes = member.Strength.Value + member.Dexterity.Value + member.Constitution.Value +
                             member.Intelligence.Value + member.Wisdom.Value + member.Charisma.Value;
        
        Assert.True(totalAttributes > 72); // Base standard array + racial + primal champion
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void ApplyAbilityScoreImprovements_WithAllAttributesAtMax_ShouldNotInfiniteLoop()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 20, 20, 20, 20, 20, 20 };

        // Act & Assert (should complete without hanging)
        service.ApplyAbilityScoreImprovements(attributes, 1);
        
        // Verify no changes
        Assert.All(attributes, attr => Assert.Equal(20, attr));
    }

    [Fact]
    public void ApplyRacialBonuses_WithLargeBonuses_ShouldApplyCorrectly()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var attributes = new List<byte> { 15, 14, 13, 12, 10, 8 };
        var bonuses = new List<AbilityBonus>
        {
            new AbilityBonus(new BaseEntity("str", string.Empty), 10)
        };

        // Act
        service.ApplyRacialBonuses(attributes, bonuses);

        // Assert
        Assert.Equal(25, attributes[0]); // Can exceed 20 with racial bonuses
    }

    #endregion
}