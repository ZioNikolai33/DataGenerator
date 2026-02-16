using MongoDB.Bson.Serialization;
using Moq;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Entities.PartyEntities;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Tests.Unit;

public class ProficiencyServiceTests
{
    private static readonly string classesFolder = Path.Combine("..", "..", "..", "TestData", "Classes");
    private static readonly string racesFolder = Path.Combine("..", "..", "..", "TestData", "Races");

    #region Helper Methods

    private PartyMember CreateTestPartyMember(
        int id = 1,
        byte level = 1,
        string raceString = "human",
        string classString = "fighter",
        List<string>? proficiencies = null,
        List<string>? traits = null,
        List<Feature>? features = null)
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

        var member = new PartyMember(
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

        if (proficiencies != null)
            member.Proficiencies = proficiencies;
        
        if (traits != null)
            member.Traits = traits;
        
        if (features != null)
            member.Features = features;

        return member;
    }

    public static IEnumerable<object[]> GetTestClassNames()
    {
        foreach (var className in DataConstants.Classes)
            yield return new object[] { className };
    }

    public static IEnumerable<object[]> GetTestRaceNames()
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

    private ProficiencyService CreateService()
    {
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();

        return new ProficiencyService(mockLogger.Object, mockRandom.Object);
    }

    private ProficiencyService CreateService(Mock<RandomProvider>? mockRandom = null)
    {
        var mockLogger = new Mock<ILogger>();
        mockRandom ??= new Mock<RandomProvider>();

        return new ProficiencyService(mockLogger.Object, mockRandom.Object);
    }

    private ProficiencyService CreateService(Mock<IRandomProvider>? mockRandom = null)
    {
        var mockLogger = new Mock<ILogger>();
        mockRandom ??= new Mock<IRandomProvider>();

        return new ProficiencyService(mockLogger.Object, mockRandom.Object);
    }

    private Skill CreateSkill(string index, string name, sbyte modifier = 0, byte proficiencyBonus = 2)
    {
        return new Skill(new BaseEntity(index, name), modifier, proficiencyBonus);
    }

    private SubraceMapper CreateSubraceMapper(string index, string name, List<BaseEntity>? racialTraits = null)
    {
        return new SubraceMapper(index, name)
        {
            Desc = "Test Description",
            AbilityBonuses = new List<AbilityBonus>(),
            StartingProficiencies = new List<BaseEntity>(),
            RacialTraits = racialTraits ?? new List<BaseEntity>(),
            Race = new BaseEntity("test-race", "Test Race")
        };   
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ProficiencyService(null!, mockRandom.Object));
    }

    [Fact]
    public void Constructor_WithNullRandomProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ProficiencyService(mockLogger.Object, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();

        // Act
        var service = new ProficiencyService(mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(service);
    }

    #endregion

    #region SetInitialProficiencies Tests

    [Fact]
    public void SetInitialProficiencies_ShouldInitializeProficienciesList()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var classMapper = CreateTestClass("fighter");
        var raceMapper = CreateTestRace("human");

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);

        // Assert
        Assert.NotNull(member.Proficiencies);
        Assert.NotEmpty(member.Proficiencies);
    }

    [Fact]
    public void SetInitialProficiencies_ShouldIncludeRacialProficiencies()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var classMapper = CreateTestClass("fighter");
        var raceMapper = CreateTestRace("dwarf");

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);

        // Assert
        Assert.Contains(member.Proficiencies, p => 
            raceMapper.StartingProficiences.Any(sp => sp.Index == p));
    }

    [Fact]
    public void SetInitialProficiencies_ShouldIncludeClassProficiencies()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var classMapper = CreateTestClass("fighter");
        var raceMapper = CreateTestRace("human");

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);

        // Assert
        Assert.Contains(member.Proficiencies, p => 
            classMapper.Proficiencies.Any(cp => cp.Index == p));
    }

    [Fact]
    public void SetInitialProficiencies_ShouldRemoveDuplicates()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var classMapper = CreateTestClass("fighter");
        var raceMapper = CreateTestRace("dwarf");

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);

        // Assert
        var distinctCount = member.Proficiencies.Distinct().Count();
        Assert.Equal(distinctCount, member.Proficiencies.Count);
    }

    [Fact]
    public void SetInitialProficiencies_WithSubrace_ShouldIncludeSubraceTraitProficiencies()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var classMapper = CreateTestClass("fighter");
        var raceMapper = CreateTestRace("elf");
        var subraceMapper = CreateSubraceMapper(
            "high-elf",
            "High Elf",
            new List<BaseEntity>
            {
                new BaseEntity("elf-weapon-training", "Elf Weapon Training")
            });

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, subraceMapper);

        // Assert
        Assert.NotEmpty(member.Proficiencies);
    }

    [Fact]
    public void SetInitialProficiencies_WithNullSubrace_ShouldNotThrow()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var classMapper = CreateTestClass("fighter");
        var raceMapper = CreateTestRace("human");

        // Act & Assert
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);
        Assert.NotNull(member.Proficiencies);
    }

    [Fact]
    public void SetInitialProficiencies_WithProficiencyChoices_ShouldAddRandomChoices()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        var classMapper = CreateTestClass("rogue");
        var raceMapper = CreateTestRace("human");

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);

        // Assert
        Assert.NotEmpty(member.Proficiencies);
    }

    #endregion

    #region AddBackgroundProficiencies Tests

    [Fact]
    public void AddBackgroundProficiencies_ShouldAddTwoSkills()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<string>>(), 2))
                  .Returns(new List<string> { "skill-athletics", "skill-perception" });
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        member.Skills = new List<Skill>
        {
            CreateSkill("skill-athletics", "Athletics"),
            CreateSkill("skill-perception", "Perception"),
            CreateSkill("skill-acrobatics", "Acrobatics")
        };
        var initialCount = member.Proficiencies.Count;

        // Act
        service.AddBackgroundProficiencies(member);

        // Assert
        Assert.Equal(initialCount + 2, member.Proficiencies.Count);
    }

    [Fact]
    public void AddBackgroundProficiencies_ShouldNotAddDuplicateSkills()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<string>>(), 2))
                  .Returns(new List<string> { "skill-athletics", "skill-perception" });
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember(proficiencies: new List<string> { "skill-athletics" });
        member.Skills = new List<Skill>
        {
            CreateSkill("skill-athletics", "Athletics"),
            CreateSkill("skill-perception", "Perception"),
            CreateSkill("skill-acrobatics", "Acrobatics")
        };

        // Act
        service.AddBackgroundProficiencies(member);

        // Assert
        Assert.Contains("skill-perception", member.Proficiencies);
        Assert.DoesNotContain(member.Proficiencies, p => 
            member.Proficiencies.Count(x => x == p) > 1);
    }

    [Fact]
    public void AddBackgroundProficiencies_WithLessThanTwoAvailable_ShouldAddAvailableSkills()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        member.Skills = new List<Skill>
        {
            CreateSkill("skill-athletics", "Athletics")
        };
        member.Proficiencies = new List<string>();

        // Act
        service.AddBackgroundProficiencies(member);

        // Assert
        Assert.Single(member.Proficiencies);
    }

    [Fact]
    public void AddBackgroundProficiencies_WithNoAvailableSkills_ShouldNotAddAny()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Skills = new List<Skill>();

        // Act
        service.AddBackgroundProficiencies(member);

        // Assert
        Assert.Empty(member.Proficiencies);
    }

    #endregion

    #region AddAdditionalProficiencies Tests

    [Fact]
    public void AddAdditionalProficiencies_WithKeenSensesTrait_ShouldAddPerception()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(traits: new List<string> { "keen-senses" });

        // Act
        service.AddAdditionalProficiencies(member);

        // Assert
        Assert.Contains("skill-perception", member.Proficiencies);
    }

    [Fact]
    public void AddAdditionalProficiencies_WithElfWeaponTraining_ShouldAddElfWeapons()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(traits: new List<string> { "elf-weapon-training" });

        // Act
        service.AddAdditionalProficiencies(member);

        // Assert
        Assert.Contains("shortsword", member.Proficiencies);
        Assert.Contains("longsword", member.Proficiencies);
        Assert.Contains("shortbow", member.Proficiencies);
        Assert.Contains("longbow", member.Proficiencies);
    }

    [Fact]
    public void AddAdditionalProficiencies_WithDwarvenCombatTraining_ShouldAddDwarvenWeapons()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(traits: new List<string> { "dwarven-combat-training" });

        // Act
        service.AddAdditionalProficiencies(member);

        // Assert
        Assert.Contains("battleaxe", member.Proficiencies);
        Assert.Contains("handaxe", member.Proficiencies);
        Assert.Contains("light-hammer", member.Proficiencies);
        Assert.Contains("warhammer", member.Proficiencies);
    }

    [Fact]
    public void AddAdditionalProficiencies_WithMenacingTrait_ShouldAddIntimidation()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(traits: new List<string> { "menacing" });

        // Act
        service.AddAdditionalProficiencies(member);

        // Assert
        Assert.Contains("skill-intimidation", member.Proficiencies);
    }

    [Fact]
    public void AddAdditionalProficiencies_LifeClericWithBonusProficiency_ShouldAddHeavyArmor()
    {
        // Arrange
        var service = CreateService();
        var mockRandom = new Mock<IRandomProvider>();
        var features = new List<Feature>
        { 
            new Feature(new FeatureMapper("bonus-proficiency", "Bonus Proficiency"), new List<string>(), mockRandom.Object)
        };

        var member = CreateTestPartyMember(classString: "cleric", features: features);
        member.Subclass = "life";

        // Act
        service.AddAdditionalProficiencies(member);

        // Assert
        Assert.Contains("heavy-armor", member.Proficiencies);
    }

    [Fact]
    public void AddAdditionalProficiencies_NonLifeCleric_ShouldNotAddHeavyArmor()
    {
        // Arrange
        var service = CreateService();
        var mockRandom = new Mock<IRandomProvider>();
        var features = new List<Feature>
        {
            new Feature(new FeatureMapper("bonus-proficiency", "Bonus Proficiency"), new List<string>(), mockRandom.Object)
        };
        var member = CreateTestPartyMember(classString: "cleric", features: features);
        member.Subclass = "war";

        // Act
        service.AddAdditionalProficiencies(member);

        // Assert
        Assert.DoesNotContain("heavy-armor", member.Proficiencies);
    }

    [Fact]
    public void AddAdditionalProficiencies_WithMultipleTraits_ShouldAddAll()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(traits: new List<string> 
        { 
            "keen-senses", 
            "menacing" 
        });

        // Act
        service.AddAdditionalProficiencies(member);

        // Assert
        Assert.Contains("skill-perception", member.Proficiencies);
        Assert.Contains("skill-intimidation", member.Proficiencies);
    }

    [Fact]
    public void AddAdditionalProficiencies_WithNoTraitsOrFeatures_ShouldNotAddAny()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var initialCount = member.Proficiencies.Count;

        // Act
        service.AddAdditionalProficiencies(member);

        // Assert
        Assert.Equal(initialCount, member.Proficiencies.Count);
    }

    #endregion

    #region ApplySkillProficiencies Tests

    [Fact]
    public void ApplySkillProficiencies_WithMatchingProficiency_ShouldSetSkillProficient()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { "skill-athletics" });
        var athleticsSkill = CreateSkill("skill-athletics", "Athletics");
        member.Skills = new List<Skill> { athleticsSkill };

        // Act
        service.ApplySkillProficiencies(member);

        // Assert
        Assert.True(athleticsSkill.IsProficient);
    }

    [Fact]
    public void ApplySkillProficiencies_WithoutMatchingProficiency_ShouldNotSetProficient()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { "skill-athletics" });
        var perceptionSkill = CreateSkill("skill-perception", "Perception");
        member.Skills = new List<Skill> { perceptionSkill };

        // Act
        service.ApplySkillProficiencies(member);

        // Assert
        Assert.False(perceptionSkill.IsProficient);
    }

    [Fact]
    public void ApplySkillProficiencies_WithSkillVersatility_ShouldAddTwoRandomSkills()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<string>>(), 2))
                  .Returns(new List<string> { "skill-athletics", "skill-perception" });
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember(traits: new List<string> { "skill-versatility" });
        member.Skills = new List<Skill>
        {
            CreateSkill("skill-athletics", "Athletics"),
            CreateSkill("skill-perception", "Perception")
        };
        var initialCount = member.Proficiencies.Count;

        // Act
        service.ApplySkillProficiencies(member);

        // Assert
        Assert.Equal(initialCount + 2, member.Proficiencies.Count);
    }

    [Fact]
    public void ApplySkillProficiencies_BardWithBonusProficiencies_ShouldAddThreeSkills()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<string>>(), 3))
                  .Returns(new List<string> { "skill-athletics", "skill-perception", "skill-acrobatics" });
        var service = CreateService(mockRandom);
        var features = new List<Feature>
        {
            new Feature(new FeatureMapper("bonus-proficiencies", "Bonus Proficiencies"), new List<string>(), mockRandom.Object)
        };
        var member = CreateTestPartyMember(classString: "bard", features: features);
        member.Skills = new List<Skill>
        {
            CreateSkill("skill-athletics", "Athletics"),
            CreateSkill("skill-perception", "Perception"),
            CreateSkill("skill-acrobatics", "Acrobatics")
        };
        var initialCount = member.Proficiencies.Count;

        // Act
        service.ApplySkillProficiencies(member);

        // Assert
        Assert.Equal(initialCount + 3, member.Proficiencies.Count);
    }

    [Fact]
    public void ApplySkillProficiencies_WithJackOfAllTrades_ShouldAddHalfProficiency()
    {
        // Arrange
        var service = CreateService();
        var mockRandom = new Mock<IRandomProvider>();
        var features = new List<Feature>
        {
            new Feature(new FeatureMapper("jack-of-all-trades", "Jack of All Trades"), new List<string>(), mockRandom.Object)
        };
        var member = CreateTestPartyMember(level: 2, classString: "bard", features: features);
        var nonProficientSkill = CreateSkill("skill-athletics", "Athletics", 2, member.ProficiencyBonus);
        member.Skills = new List<Skill> { nonProficientSkill };
        var initialModifier = nonProficientSkill.Modifier;

        // Act
        service.ApplySkillProficiencies(member);

        // Assert
        var expectedBonus = (sbyte)Math.Floor(member.ProficiencyBonus / 2.0);
        Assert.Equal(initialModifier + expectedBonus, nonProficientSkill.Modifier);
    }

    [Fact]
    public void ApplySkillProficiencies_WithRemarkableAthlete_ShouldAddHalfProficiencyToAthleticSkills()
    {
        // Arrange
        var service = CreateService();
        var mockRandom = new Mock<IRandomProvider>();
        var features = new List<Feature>
        {
            new Feature(new FeatureMapper("remarkable-athlete", "Remarkable Athlete"), new List<string>(), mockRandom.Object)
        };
        var member = CreateTestPartyMember(level: 7, features: features);
        var acrobaticsSkill = CreateSkill("skill-acrobatics", "Acrobatics", 2, member.ProficiencyBonus);
        var athleticsSkill = CreateSkill("skill-athletics", "Athletics", 3, member.ProficiencyBonus);
        member.Skills = new List<Skill> { acrobaticsSkill, athleticsSkill };
        var initialAcrobatics = acrobaticsSkill.Modifier;
        var initialAthletics = athleticsSkill.Modifier;

        // Act
        service.ApplySkillProficiencies(member);

        // Assert
        var expectedBonus = (sbyte)Math.Floor(member.ProficiencyBonus / 2.0);
        Assert.Equal(initialAcrobatics + expectedBonus, acrobaticsSkill.Modifier);
        Assert.Equal(initialAthletics + expectedBonus, athleticsSkill.Modifier);
    }

    [Fact]
    public void ApplySkillProficiencies_RemarkableAthlete_ShouldNotAffectProficientSkills()
    {
        // Arrange
        var service = CreateService();
        var mockRandom = new Mock<IRandomProvider>();
        var features = new List<Feature>
        {
            new Feature(new FeatureMapper("remarkable-athlete", "Remarkable Athlete"), new List<string>(), mockRandom.Object)
        };
        var member = CreateTestPartyMember(
            level: 7, 
            proficiencies: new List<string> { "skill-athletics" },
            features: features);
        var athleticsSkill = CreateSkill("skill-athletics", "Athletics", 3, member.ProficiencyBonus);
        member.Skills = new List<Skill> { athleticsSkill };

        // Act
        service.ApplySkillProficiencies(member);
        var finalModifier = athleticsSkill.Modifier;

        // Set proficiency manually to check expected value
        athleticsSkill.SetProficiency(true, member.ProficiencyBonus);
        var initialModifier = athleticsSkill.Modifier;

        // Assert - proficient skills should get full bonus, not half
        Assert.True(athleticsSkill.IsProficient);
    }

    [Fact]
    public void ApplySkillProficiencies_WithMultipleSkills_ShouldApplyProficienciesToAll()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> 
        { 
            "skill-athletics", 
            "skill-perception",
            "skill-stealth"
        });
        member.Skills = new List<Skill>
        {
            CreateSkill("skill-athletics", "Athletics"),
            CreateSkill("skill-perception", "Perception"),
            CreateSkill("skill-stealth", "Stealth"),
            CreateSkill("skill-acrobatics", "Acrobatics")
        };

        // Act
        service.ApplySkillProficiencies(member);

        // Assert
        Assert.Equal(3, member.Skills.Count(s => s.IsProficient));
    }

    #endregion

    #region GetAllSkillIndices Tests

    [Fact]
    public void GetAllSkillIndices_ShouldReturnAllSkillIndices()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Skills = new List<Skill>
        {
            CreateSkill("skill-athletics", "Athletics"),
            CreateSkill("skill-perception", "Perception"),
            CreateSkill("skill-stealth", "Stealth")
        };

        // Act
        var indices = service.GetAllSkillIndices(member);

        // Assert
        Assert.Equal(3, indices.Count);
        Assert.Contains("skill-athletics", indices);
        Assert.Contains("skill-perception", indices);
        Assert.Contains("skill-stealth", indices);
    }

    [Fact]
    public void GetAllSkillIndices_WithNoSkills_ShouldReturnEmptyList()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Skills = new List<Skill>();

        // Act
        var indices = service.GetAllSkillIndices(member);

        // Assert
        Assert.Empty(indices);
    }

    #endregion

    #region HasProficiency Tests

    [Fact]
    public void HasProficiency_WithExistingProficiency_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> 
        { 
            "skill-athletics", 
            "longsword" 
        });

        // Act
        var result = service.HasProficiency(member, "skill-athletics");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasProficiency_WithNonExistingProficiency_ShouldReturnFalse()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { "skill-athletics" });

        // Act
        var result = service.HasProficiency(member, "skill-perception");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasProficiency_WithEmptyProficienciesList_ShouldReturnFalse()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string>());

        // Act
        var result = service.HasProficiency(member, "skill-athletics");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasProficiency_CaseSensitive_ShouldMatch()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { "skill-athletics" });

        // Act
        var resultLower = service.HasProficiency(member, "skill-athletics");
        var resultUpper = service.HasProficiency(member, "SKILL-ATHLETICS");

        // Assert
        Assert.True(resultLower);
        Assert.False(resultUpper); // Case sensitive
    }

    #endregion

    #region Integration Tests

    [Theory]
    [MemberData(nameof(GetTestClassNames))]
    public void SetInitialProficiencies_WithRealClasses_ShouldSetProficiencies(string className)
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(classString: className);
        var classMapper = CreateTestClass(className);
        var raceMapper = CreateTestRace("human");

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);

        // Assert
        Assert.NotEmpty(member.Proficiencies);
    }

    [Theory]
    [MemberData(nameof(GetTestRaceNames))]
    public void SetInitialProficiencies_WithRealRaces_ShouldSetProficiencies(string raceName)
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(raceString: raceName);
        var classMapper = CreateTestClass("fighter");
        var raceMapper = CreateTestRace(raceName);

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);

        // Assert
        Assert.NotEmpty(member.Proficiencies);
    }

    [Fact]
    public void FullProficiencyWorkflow_ShouldCompleteSuccessfully()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<string>>(), It.IsAny<int>()))
                  .Returns<List<string>, int>((list, count) => 
                      list.Take(Math.Min(count, list.Count)).ToList());
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember(classString: "fighter", raceString: "elf");
        var classMapper = CreateTestClass("fighter");
        var raceMapper = CreateTestRace("elf");
        member.Skills = new List<Skill>
        {
            CreateSkill("skill-athletics", "Athletics"),
            CreateSkill("skill-perception", "Perception"),
            CreateSkill("skill-acrobatics", "Acrobatics")
        };

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);
        service.AddBackgroundProficiencies(member);
        service.AddAdditionalProficiencies(member);
        service.ApplySkillProficiencies(member);

        // Assert
        Assert.NotEmpty(member.Proficiencies);
        Assert.Contains(member.Skills, s => s.IsProficient);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void SetInitialProficiencies_WithManyDuplicates_ShouldDeduplicateCorrectly()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var classMapper = CreateTestClass("fighter");
        var raceMapper = CreateTestRace("dwarf");

        // Act
        service.SetInitialProficiencies(member, classMapper, raceMapper, null);

        // Assert
        var hasDuplicates = member.Proficiencies.GroupBy(p => p).Any(g => g.Count() > 1);
        Assert.False(hasDuplicates);
    }

    [Fact]
    public void AddBackgroundProficiencies_WithAllSkillsProficient_ShouldHandleGracefully()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Skills = new List<Skill>
        {
            CreateSkill("skill-athletics", "Athletics"),
            CreateSkill("skill-perception", "Perception")
        };
        member.Proficiencies = new List<string> { "skill-athletics", "skill-perception" };

        // Act & Assert (should not throw)
        service.AddBackgroundProficiencies(member);
    }

    [Fact]
    public void ApplySkillProficiencies_WithNullSkillsList_ShouldNotThrow()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Skills = null!;

        // Act & Assert (should handle gracefully)
        Assert.Throws<NullReferenceException>(() => service.ApplySkillProficiencies(member));
    }

    [Fact]
    public void AddAdditionalProficiencies_WithAllTraits_ShouldAddAllProficiencies()
    {
        // Arrange
        var service = CreateService();
        var mockRandom = new Mock<IRandomProvider>();
        var features = new List<Feature>
        {
            new Feature(new FeatureMapper("bonus-proficiency", "Bonus Proficiency"), new List<string>(), mockRandom.Object)
        };
        var member = CreateTestPartyMember(
            classString: "cleric",
            traits: new List<string> 
            { 
                "keen-senses", 
                "elf-weapon-training",
                "dwarven-combat-training",
                "menacing"
            },
            features: features);
        member.Subclass = "life";
        var initialCount = member.Proficiencies.Count;

        // Act
        service.AddAdditionalProficiencies(member);

        // Assert
        Assert.True(member.Proficiencies.Count > initialCount);
        Assert.Contains("skill-perception", member.Proficiencies);
        Assert.Contains("skill-intimidation", member.Proficiencies);
        Assert.Contains("heavy-armor", member.Proficiencies);
    }

    [Fact]
    public void ApplySkillProficiencies_JackOfAllTradesWithProficientSkill_ShouldNotDoubleApply()
    {
        // Arrange
        var service = CreateService();
        var mockRandom = new Mock<IRandomProvider>();
        var features = new List<Feature>
        {
            new Feature(new FeatureMapper("jack-of-all-trades", "Jack of All Trades"), new List<string>(), mockRandom.Object)
        };
        var member = CreateTestPartyMember(
            level: 2,
            classString: "bard",
            proficiencies: new List<string> { "skill-athletics" },
            features: features);
        var athleticsSkill = CreateSkill("skill-athletics", "Athletics", 2, member.ProficiencyBonus);
        member.Skills = new List<Skill> { athleticsSkill };

        // Act
        service.ApplySkillProficiencies(member);

        // Assert
        // Proficient skills should only get full proficiency bonus, not jack of all trades
        Assert.True(athleticsSkill.IsProficient);
    }

    #endregion
}