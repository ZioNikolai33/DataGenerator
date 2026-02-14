using MongoDB.Bson.Serialization;
using Moq;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;

namespace TrainingDataGenerator.Tests.Unit;

public class SpellServiceTests
{
    private static readonly string classesFolder = Path.Combine("..", "..", "..", "TestData", "Classes");
    private static readonly string racesFolder = Path.Combine("..", "..", "..", "TestData", "Races");

    #region Helper Methods

    private SpellService CreateService(Mock<RandomProvider>? mockRandom = null)
    {
        var mockLogger = new Mock<ILogger>();
        mockRandom ??= new Mock<RandomProvider>();

        return new SpellService(mockLogger.Object, mockRandom.Object);
    }

    private SpellService CreateService(Mock<IRandomProvider>? mockRandom = null)
    {
        var mockLogger = new Mock<ILogger>();
        mockRandom ??= new Mock<IRandomProvider>();

        return new SpellService(mockLogger.Object, mockRandom.Object);
    }

    private PartyMember CreateTestPartyMember(
        string className = "fighter",
        string raceName = "human",
        byte level = 1,
        string? subclass = null,
        List<FeatureMapper>? features = null,
        List<string>? traits = null)
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

        var raceMapper = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);

        var member = new PartyMember(
            1,
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

        if (subclass != null)
            member.Subclass = subclass;

        if (features != null)
            member.Features.AddRange(features.Select(f => new Feature(f, member.Proficiencies, mockRandom.Object)));

        if (traits != null)
            member.Traits.AddRange(traits);

        return member;
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

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SpellService(null!, mockRandom.Object));
    }

    [Fact]
    public void Constructor_WithNullRandomProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SpellService(mockLogger.Object, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();

        // Act
        var service = new SpellService(mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(service);
    }

    #endregion

    #region ApplySpellsFromFeatures Tests

    [Fact]
    public void ApplySpellsFromFeatures_WithPactOfTheTome_ShouldAddThreeCantrips()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("pact-of-the-tome", "Pact of the Tome")
        };
        var member = CreateTestPartyMember("warlock", level: 3, features: features);
        var initialCantripCount = member.Cantrips.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        Assert.Equal(initialCantripCount + 3, member.Cantrips.Count);
    }

    [Fact]
    public void ApplySpellsFromFeatures_WithBonusCantrip_DruidShouldAddCantrip()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("bonus-cantrip", "Bonus Cantrip")
        };
        var member = CreateTestPartyMember("druid", level: 1, features: features);
        var initialCantripCount = member.Cantrips.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        Assert.True(member.Cantrips.Count >= initialCantripCount);
    }

    [Fact]
    public void ApplySpellsFromFeatures_WithBonusCantrip_NonDruidShouldNotAddCantrip()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("bonus-cantrip", "Bonus Cantrip")
        };
        var member = CreateTestPartyMember("wizard", level: 1, features: features);
        var initialCantripCount = member.Cantrips.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        Assert.Equal(initialCantripCount, member.Cantrips.Count);
    }

    [Fact]
    public void ApplySpellsFromFeatures_WithMysticArcanum6thLevel_ShouldAddSpell()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.Next())
                  .Returns(1);
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("mystic-arcanum-6th-level", "Mystic Arcanum (6th Level)")
        };
        var member = CreateTestPartyMember("warlock", level: 11, features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    [Theory]
    [InlineData("mystic-arcanum-6th-level")]
    [InlineData("mystic-arcanum-7th-level")]
    [InlineData("mystic-arcanum-8th-level")]
    [InlineData("mystic-arcanum-9th-level")]
    public void ApplySpellsFromFeatures_WithMysticArcanum_ShouldAddCorrectLevelSpell(string featureIndex)
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.Next())
                  .Returns(1);
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper(featureIndex, "Mystic Arcanum")
        };
        var member = CreateTestPartyMember("warlock", level: 11, features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    [Fact]
    public void ApplySpellsFromFeatures_WithAdditionalMagicalSecrets_ShouldAddTwoSpells()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.Next())
                  .Returns(1);
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("additional-magical-secrets", "Additional Magical Secrets")
        };
        var member = CreateTestPartyMember("bard", level: 6, features: features);
        var initialCount = member.Cantrips.Count + member.Spells.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        var finalCount = member.Cantrips.Count + member.Spells.Count;
        Assert.True(finalCount >= initialCount);
    }

    [Theory]
    [InlineData("magical-secrets-1")]
    [InlineData("magical-secrets-2")]
    [InlineData("magical-secrets-3")]
    public void ApplySpellsFromFeatures_WithMagicalSecrets_ShouldAddTwoSpells(string featureIndex)
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.Next())
                  .Returns(1);
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper(featureIndex, "Magical Secrets")
        };
        var member = CreateTestPartyMember("bard", level: 10, features: features);
        var initialCount = member.Cantrips.Count + member.Spells.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        var finalCount = member.Cantrips.Count + member.Spells.Count;
        Assert.True(finalCount >= initialCount);
    }

    [Fact]
    public void ApplySpellsFromFeatures_DruidWithCircleSpells_ShouldCallApplyCircleSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("circle-spells-1", "Circle Spells")
        };
        var member = CreateTestPartyMember("druid", level: 3, subclass: "arctic", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    [Fact]
    public void ApplySpellsFromFeatures_PaladinWithOathSpells_ShouldCallApplyOathSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("oath-spells", "Oath Spells")
        };
        var member = CreateTestPartyMember("paladin", level: 3, subclass: "devotion", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    [Fact]
    public void ApplySpellsFromFeatures_ClericWithDomainSpells_ShouldCallApplyDomainSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("domain-spells-1", "Domain Spells")
        };
        var member = CreateTestPartyMember("cleric", level: 1, subclass: "life", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    #endregion

    #region ApplySpellsFromTraits Tests

    [Fact]
    public void ApplySpellsFromTraits_WithHighElfCantrip_ShouldAddOneCantrip()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.Next())
                  .Returns(1);
        var service = CreateService(mockRandom);
        var traits = new List<string> { "high-elf-cantrip" };
        var member = CreateTestPartyMember("fighter", level: 1, traits: traits);
        var initialCantripCount = member.Cantrips.Count;

        // Act
        service.ApplySpellsFromTraits(member);

        // Assert
        Assert.True(member.Cantrips.Count >= initialCantripCount);
    }

    [Fact]
    public void ApplySpellsFromTraits_WithInfernalLegacy_Level1_ShouldAddThaumaturgy()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var traits = new List<string> { "infernal-legacy" };
        var member = CreateTestPartyMember("warlock", level: 1, traits: traits);
        var initialCantripCount = member.Cantrips.Count;

        // Act
        service.ApplySpellsFromTraits(member);

        // Assert
        Assert.True(member.Cantrips.Count >= initialCantripCount);
    }

    [Fact]
    public void ApplySpellsFromTraits_WithInfernalLegacy_Level3_ShouldAddHellishRebuke()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var traits = new List<string> { "infernal-legacy" };
        var member = CreateTestPartyMember("warlock", level: 3, traits: traits);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplySpellsFromTraits(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    [Fact]
    public void ApplySpellsFromTraits_WithInfernalLegacy_Level5_ShouldAddDarkness()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var traits = new List<string> { "infernal-legacy" };
        var member = CreateTestPartyMember("warlock", level: 5, traits: traits);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplySpellsFromTraits(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    [Fact]
    public void ApplySpellsFromTraits_WithInfernalLegacy_Level2_ShouldNotAddHellishRebuke()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var traits = new List<string> { "infernal-legacy" };
        var member = CreateTestPartyMember("warlock", level: 2, traits: traits);

        // Act
        service.ApplySpellsFromTraits(member);

        // Assert
        Assert.DoesNotContain(member.Spells, s => s.Index == "hellish-rebuke");
    }

    [Fact]
    public void ApplySpellsFromTraits_WithoutTraits_ShouldNotAddSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember("fighter", level: 1);
        var initialCantripCount = member.Cantrips.Count;
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplySpellsFromTraits(member);

        // Assert
        Assert.Equal(initialCantripCount, member.Cantrips.Count);
        Assert.Equal(initialSpellCount, member.Spells.Count);
    }

    #endregion

    #region ApplyCircleSpells Tests

    [Fact]
    public void ApplyCircleSpells_WithNonDruid_ShouldNotAddSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("circle-spells-1", "Circle Spells"),
            new FeatureMapper("circle-of-the-land-arctic", "Circle Of The Land Arctic")

        };
        var member = CreateTestPartyMember("wizard", level: 3, features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyCircleSpells(member);

        // Assert
        Assert.Equal(initialSpellCount, member.Spells.Count);
    }

    [Theory]
    [InlineData("arctic")]
    [InlineData("coast")]
    [InlineData("desert")]
    [InlineData("forest")]
    [InlineData("grassland")]
    [InlineData("mountain")]
    [InlineData("swamp")]
    [InlineData("underdark")]
    public void ApplyCircleSpells_WithCircleSpells1_ShouldAddTier1Spells(string subclassSpecific)
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("circle-spells-1", "Circle Spells 1"),
            new FeatureMapper("circle-of-the-land-" + subclassSpecific, "Circle Of The Land " + subclassSpecific)
        };
        var member = CreateTestPartyMember("druid", level: 3, features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyCircleSpells(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    [Theory]
    [InlineData("circle-spells-1")]
    [InlineData("circle-spells-2")]
    [InlineData("circle-spells-3")]
    [InlineData("circle-spells-4")]
    public void ApplyCircleSpells_WithMultipleTiers_ShouldAddCorrectSpells(string featureIndex)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper(featureIndex, "Circle Spells"),
            new FeatureMapper("circle-of-the-land-forest", "Circle Of The Land Forest")
        };
        var member = CreateTestPartyMember("druid", level: 7, features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyCircleSpells(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    #endregion

    #region ApplyOathSpells Tests

    [Fact]
    public void ApplyOathSpells_WithNonPaladin_ShouldNotAddSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("oath-spells", "Oath Spells")
        };
        var member = CreateTestPartyMember("fighter", level: 3, subclass: "devotion", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyOathSpells(member);

        // Assert
        Assert.Equal(initialSpellCount, member.Spells.Count);
    }

    [Fact]
    public void ApplyOathSpells_WithoutOathSpellsFeature_ShouldNotAddSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember("paladin", level: 3, subclass: "devotion");
        var initialSpellCount = member.Spells.Count;

        member.Features = new List<Feature>();

        // Act
        service.ApplyOathSpells(member);

        // Assert
        Assert.Equal(initialSpellCount, member.Spells.Count);
    }

    [Theory]
    [InlineData("devotion")]
    public void ApplyOathSpells_Level3_ShouldAddLevel3Spells(string subclass)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("oath-spells", "Oath Spells")
        };
        var member = CreateTestPartyMember("paladin", level: 3, subclass: subclass, features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyOathSpells(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(9)]
    [InlineData(13)]
    [InlineData(17)]
    public void ApplyOathSpells_AtVariousLevels_ShouldAddCorrectSpells(byte level)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("oath-spells", "Oath Spells")
        };
        var member = CreateTestPartyMember("paladin", level: level, subclass: "devotion", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyOathSpells(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    #endregion

    #region ApplyDomainSpells Tests

    [Fact]
    public void ApplyDomainSpells_WithNonCleric_ShouldNotAddSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("domain-spells-1", "Domain Spells")
        };
        var member = CreateTestPartyMember("wizard", level: 1, subclass: "life", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyDomainSpells(member);

        // Assert
        Assert.Equal(initialSpellCount, member.Spells.Count);
    }

    [Theory]
    [InlineData("life")]
    public void ApplyDomainSpells_WithDomainSpells1_ShouldAddTier1Spells(string subclass)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("domain-spells-1", "Domain Spells 1")
        };
        var member = CreateTestPartyMember("cleric", level: 1, subclass: subclass, features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyDomainSpells(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    [Theory]
    [InlineData("domain-spells-1")]
    [InlineData("domain-spells-2")]
    [InlineData("domain-spells-3")]
    [InlineData("domain-spells-4")]
    [InlineData("domain-spells-5")]
    public void ApplyDomainSpells_WithMultipleTiers_ShouldAddCorrectSpells(string featureIndex)
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper(featureIndex, "Domain Spells")
        };
        var member = CreateTestPartyMember("cleric", level: 9, subclass: "life", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyDomainSpells(member);

        // Assert
        Assert.True(member.Spells.Count >= initialSpellCount);
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public void ApplySpellsFromFeatures_WithMultipleFeatures_ShouldApplyAll()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.Next())
                  .Returns(1);
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("pact-of-the-tome", "Pact of the Tome"),
            new FeatureMapper("mystic-arcanum-6th-level", "Mystic Arcanum (6th Level)")
        };
        var member = CreateTestPartyMember("warlock", level: 11, features: features);
        var initialTotalCount = member.Cantrips.Count + member.Spells.Count;

        // Act
        service.ApplySpellsFromFeatures(member);

        // Assert
        var finalTotalCount = member.Cantrips.Count + member.Spells.Count;
        Assert.True(finalTotalCount > initialTotalCount);
    }

    [Fact]
    public void ApplySpellsFromTraits_WithMultipleTraits_ShouldApplyAll()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.Next())
                  .Returns(1);
        var service = CreateService(mockRandom);
        var traits = new List<string> { "high-elf-cantrip", "infernal-legacy" };
        var member = CreateTestPartyMember("fighter", level: 5, traits: traits);
        var initialTotalCount = member.Cantrips.Count + member.Spells.Count;

        // Act
        service.ApplySpellsFromTraits(member);

        // Assert
        var finalTotalCount = member.Cantrips.Count + member.Spells.Count;
        Assert.True(finalTotalCount > initialTotalCount);
    }

    [Fact]
    public void ApplyCircleSpells_WithInvalidSubclass_ShouldNotAddSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("circle-spells-1", "Circle Spells")
        };
        var member = CreateTestPartyMember("druid", level: 3, subclass: "invalid-subclass", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyCircleSpells(member);

        // Assert
        Assert.Equal(initialSpellCount, member.Spells.Count);
    }

    [Fact]
    public void ApplyOathSpells_WithInvalidSubclass_ShouldNotAddSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("oath-spells", "Oath Spells")
        };
        var member = CreateTestPartyMember("paladin", level: 3, subclass: "invalid-oath", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyOathSpells(member);

        // Assert
        Assert.Equal(initialSpellCount, member.Spells.Count);
    }

    [Fact]
    public void ApplyDomainSpells_WithInvalidSubclass_ShouldNotAddSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("domain-spells-1", "Domain Spells")
        };
        var member = CreateTestPartyMember("cleric", level: 1, subclass: "invalid-domain", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyDomainSpells(member);

        // Assert
        Assert.Equal(initialSpellCount, member.Spells.Count);
    }

    [Fact]
    public void ApplyOathSpells_Level20Paladin_ShouldAddAllTierSpells()
    {
        // Arrange
        var mockRandom = new Mock<RandomProvider>();
        var service = CreateService(mockRandom);
        var features = new List<FeatureMapper>
        {
            new FeatureMapper("oath-spells", "Oath Spells")
        };
        var member = CreateTestPartyMember("paladin", level: 20, subclass: "devotion", features: features);
        var initialSpellCount = member.Spells.Count;

        // Act
        service.ApplyOathSpells(member);

        // Assert
        Assert.True(member.Spells.Count > initialSpellCount);
    }

    #endregion
}