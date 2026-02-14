using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using MongoDB.Bson.Serialization;
using Moq;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;
using TrainingDataGenerator.Utilities;
using static TrainingDataGenerator.Entities.Mappers.TraitMapper;

namespace TrainingDataGenerator.Tests.Unit;

public class TraitServiceTests
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

    public static IEnumerable<object[]> GetTestRacesNames()
    {
        foreach (var raceName in DataConstants.Races)
            yield return new object[] { raceName };
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

    private TraitService CreateService()
    {
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();

        return new TraitService(mockLogger.Object, mockRandom.Object);
    }

    private TraitMapper CreateTraitMapper(string index, string name, TraitMapper? parent = null, TraitSpecific? traitSpec = null)
    {
        return new TraitMapper(index, name)
        {
            Parent = parent,
            TraitSpec = traitSpec,
        };
    }

    private SubraceMapper CreateSubraceMapper(string index, string name, List<BaseEntity>? racialTraits = null)
    {
        return new SubraceMapper(index, name)
        {
            Desc = "Test Description",
            AbilityBonuses = new List<AbilityBonus>(),
            StartingProficiencies = new List<BaseEntity>(),
            RacialTraits = racialTraits ?? new List<BaseEntity>()
        };
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TraitService(null!, new Mock<IRandomProvider>().Object));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();

        // Act
        var service = new TraitService(mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(service);
    }

    #endregion

    #region ManageTraitSpecifics Tests

    [Fact]
    public void ManageTraitSpecifics_WithNoTraits_ShouldReturnEarly()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var raceMapper = new RaceMapper("test-race", "Test Race")
        {
            Speed = 30,
            Size = Size.Medium,
            Traits = new List<BaseEntity>()
        };

        // Act
        service.ManageTraitSpecifics(member, raceMapper, null);

        // Assert
        Assert.Empty(member.Traits);
    }

    [Fact]
    public void ManageTraitSpecifics_WithRaceTraits_ShouldSetMemberTraits()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: "elf");
        var raceMapper = CreateTestRace("elf");

        // Act
        service.ManageTraitSpecifics(member, raceMapper, null);

        // Assert
        Assert.NotEmpty(member.Traits);
    }

    [Fact]
    public void ManageTraitSpecifics_WithSubrace_ShouldIncludeSubraceTraits()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: "elf");
        var raceMapper = CreateTestRace("elf");
        var subraceMapper = CreateSubraceMapper(
            "high-elf",
            "High Elf",
            new List<BaseEntity>
            {
                new BaseEntity("elf-weapon-training", "Elf Weapon Training")
            });

        // Act
        service.ManageTraitSpecifics(member, raceMapper, subraceMapper);

        // Assert
        Assert.NotEmpty(member.Traits);
    }

    [Fact]
    public void ManageTraitSpecifics_WithNullSubrace_ShouldHandleGracefully()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: "human");
        var raceMapper = CreateTestRace("human");

        // Act & Assert (should not throw)
        service.ManageTraitSpecifics(member, raceMapper, null);
    }

    #endregion

    #region GetRaceTraits Tests

    [Fact]
    public void GetRaceTraits_WithRaceOnly_ShouldReturnRaceTraits()
    {
        // Arrange
        var service = CreateService();
        var raceMapper = CreateTestRace("elf");

        // Act
        var traits = service.GetRaceTraits(raceMapper, null);

        // Assert
        Assert.NotNull(traits);
        Assert.NotEmpty(traits);
    }

    [Fact]
    public void GetRaceTraits_WithSubrace_ShouldCombineTraits()
    {
        // Arrange
        var service = CreateService();
        var raceMapper = CreateTestRace("elf");
        var subraceMapper = CreateSubraceMapper(
            "high-elf",
            "High Elf",
            new List<BaseEntity>
            {
                new BaseEntity("high-elf-cantrip", "High Elf Cantrip")
            });

        // Act
        var traits = service.GetRaceTraits(raceMapper, subraceMapper);

        // Assert
        Assert.NotNull(traits);
        Assert.NotEmpty(traits);
    }

    [Fact]
    public void GetRaceTraits_ShouldFilterOutSubtraits()
    {
        // Arrange
        var service = CreateService();
        var raceMapper = CreateTestRace("human");

        // Act
        var traits = service.GetRaceTraits(raceMapper, null);

        // Assert
        Assert.All(traits, trait => Assert.Null(trait.Parent));
    }

    [Fact]
    public void GetRaceTraits_WithNullSubrace_ShouldOnlyReturnRaceTraits()
    {
        // Arrange
        var service = CreateService();
        var raceMapper = CreateTestRace("dwarf");

        // Act
        var traits = service.GetRaceTraits(raceMapper, null);

        // Assert
        Assert.NotNull(traits);
        Assert.All(traits, trait => Assert.Null(trait.Parent));
    }

    [Fact]
    public void GetRaceTraits_WithEmptyTraits_ShouldReturnEmptyList()
    {
        // Arrange
        var service = CreateService();
        var raceMapper = new RaceMapper("test-race", "Test Race")
        {
            Speed = 30,
            Size = Size.Medium,
            Traits = new List<BaseEntity>()
        };

        // Act
        var traits = service.GetRaceTraits(raceMapper, null);

        // Assert
        Assert.Empty(traits);
    }

    #endregion

    #region SelectSubtraits Tests

    [Fact]
    public void SelectSubtraits_WithNoSubtraitOptions_ShouldReturnEmpty()
    {
        // Arrange
        var service = CreateService();
        var traits = new List<TraitMapper>
        {
            CreateTraitMapper("trait-1", "Trait 1")
        };

        // Act
        var subtraits = service.SelectSubtraits(traits);

        // Assert
        Assert.Empty(subtraits);
    }

    [Fact]
    public void SelectSubtraits_WithNullTraitSpec_ShouldReturnEmpty()
    {
        // Arrange
        var service = CreateService();
        var traits = new List<TraitMapper>
        {
            CreateTraitMapper("trait-1", "Trait 1", null, null)
        };

        // Act
        var subtraits = service.SelectSubtraits(traits);

        // Assert
        Assert.Empty(subtraits);
    }

    [Fact]
    public void SelectSubtraits_WithEmptyTraitsList_ShouldReturnEmpty()
    {
        // Arrange
        var service = CreateService();
        var traits = new List<TraitMapper>();

        // Act
        var subtraits = service.SelectSubtraits(traits);

        // Assert
        Assert.Empty(subtraits);
    }

    [Fact]
    public void SelectSubtraits_WithMultipleTraits_ShouldProcessAll()
    {
        // Arrange
        var service = CreateService();
        var traits = new List<TraitMapper>
        {
            CreateTraitMapper("trait-1", "Trait 1"),
            CreateTraitMapper("trait-2", "Trait 2")
        };

        // Act
        var subtraits = service.SelectSubtraits(traits);

        // Assert
        Assert.NotNull(subtraits);
    }

    #endregion

    #region HasTrait Tests

    [Fact]
    public void HasTrait_WithExistingTrait_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Traits = new List<string> { "darkvision", "fey-ancestry" };

        // Act
        var result = service.HasTrait(member, "darkvision");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasTrait_WithNonExistingTrait_ShouldReturnFalse()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Traits = new List<string> { "darkvision", "fey-ancestry" };

        // Act
        var result = service.HasTrait(member, "stonecunning");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasTrait_WithEmptyTraitsList_ShouldReturnFalse()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Traits = new List<string>();

        // Act
        var result = service.HasTrait(member, "darkvision");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasTrait_WithNullTraitIndex_ShouldReturnFalse()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Traits = new List<string> { "darkvision" };

        // Act
        var result = service.HasTrait(member, null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasTrait_CaseSensitive_ShouldMatch()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Traits = new List<string> { "darkvision" };

        // Act
        var resultLower = service.HasTrait(member, "darkvision");
        var resultUpper = service.HasTrait(member, "Darkvision");

        // Assert
        Assert.True(resultLower);
        Assert.False(resultUpper); // Case sensitive
    }

    [Fact]
    public void HasTrait_WithMultipleTraits_ShouldCheckCorrectly()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Traits = new List<string> 
        { 
            "darkvision", 
            "fey-ancestry", 
            "trance",
            "keen-senses"
        };

        // Act
        var has1 = service.HasTrait(member, "darkvision");
        var has2 = service.HasTrait(member, "trance");
        var has3 = service.HasTrait(member, "stonecunning");

        // Assert
        Assert.True(has1);
        Assert.True(has2);
        Assert.False(has3);
    }

    #endregion

    #region Integration Tests

    [Theory]
    [MemberData(nameof(GetTestRacesNames))]
    public void ManageTraitSpecifics_WithRealRaces_ShouldSetTraits(string raceName)
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: raceName);
        var raceMapper = CreateTestRace(raceName);

        // Act
        service.ManageTraitSpecifics(member, raceMapper, null);

        // Assert
        if (raceMapper.Traits.Count > 0)
            Assert.NotEmpty(member.Traits);
    }

    [Fact]
    public void GetRaceTraits_WithRealElfRace_ShouldReturnValidTraits()
    {
        // Arrange
        var service = CreateService();
        var raceMapper = CreateTestRace("elf");

        // Act
        var traits = service.GetRaceTraits(raceMapper, null);

        // Assert
        Assert.NotNull(traits);
        Assert.NotEmpty(traits);
        Assert.All(traits, trait =>
        {
            Assert.NotNull(trait.Index);
            Assert.NotNull(trait.Name);
        });
    }

    [Fact]
    public void FullTraitWorkflow_ShouldCompleteSuccessfully()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: "elf");
        var raceMapper = CreateTestRace("elf");

        // Act
        service.ManageTraitSpecifics(member, raceMapper, null);
        var raceTraits = service.GetRaceTraits(raceMapper, null);
        var subtraits = service.SelectSubtraits(raceTraits);

        // Assert
        Assert.NotEmpty(member.Traits);
        Assert.NotEmpty(raceTraits);
        Assert.NotNull(subtraits);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void HasTrait_WithDuplicateTraits_ShouldStillReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Traits = new List<string> 
        { 
            "darkvision", 
            "darkvision" // Duplicate
        };

        // Act
        var result = service.HasTrait(member, "darkvision");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ManageTraitSpecifics_WithBothRaceAndSubraceTraits_ShouldCombine()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: "elf");
        var raceMapper = CreateTestRace("elf");
        var subraceMapper = CreateSubraceMapper(
            "high-elf",
            "High Elf",
            new List<BaseEntity>
            {
                new BaseEntity("high-elf-cantrip", "High Elf Cantrip"),
                new BaseEntity("extra-language", "Extra Language")
            });

        // Act
        service.ManageTraitSpecifics(member, raceMapper, subraceMapper);

        // Assert
        Assert.NotEmpty(member.Traits);
        // Should have traits from both race and subrace
    }

    [Fact]
    public void GetRaceTraits_WithMixedParentAndSubtraits_ShouldOnlyReturnParents()
    {
        // Arrange
        var service = CreateService();
        var raceMapper = CreateTestRace("dwarf");

        // Act
        var traits = service.GetRaceTraits(raceMapper, null);

        // Assert
        Assert.All(traits, trait => Assert.Null(trait.Parent));
    }

    #endregion

    #region Specific Race Tests

    [Fact]
    public void ManageTraitSpecifics_HumanRace_ShouldSetTraits()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: "human");
        var raceMapper = CreateTestRace("human");

        // Act
        service.ManageTraitSpecifics(member, raceMapper, null);

        // Assert
        Assert.Empty(member.Traits);
    }

    [Fact]
    public void ManageTraitSpecifics_DwarfRace_ShouldSetTraits()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: "dwarf");
        var raceMapper = CreateTestRace("dwarf");

        // Act
        service.ManageTraitSpecifics(member, raceMapper, null);

        // Assert
        Assert.NotEmpty(member.Traits);
        Assert.Contains(member.Traits, t => t.Contains("darkvision") || t.Contains("dwarven"));
    }

    [Fact]
    public void HasTrait_DwarfWithStonecunning_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: "dwarf");
        var raceMapper = CreateTestRace("dwarf");
        service.ManageTraitSpecifics(member, raceMapper, null);

        // Act
        var hasStonecunning = service.HasTrait(member, "stonecunning");

        // Assert
        Assert.True(hasStonecunning);
    }

    [Fact]
    public void HasTrait_ElfWithDarkvision_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: "elf");
        var raceMapper = CreateTestRace("elf");
        service.ManageTraitSpecifics(member, raceMapper, null);

        // Act
        var hasDarkvision = service.HasTrait(member, "darkvision");

        // Assert
        Assert.True(hasDarkvision);
    }

    #endregion
}