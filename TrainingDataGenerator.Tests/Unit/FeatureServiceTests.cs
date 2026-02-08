using MongoDB.Bson.Serialization;
using Moq;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Equip;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;
using TrainingDataGenerator.Utilities;
using static TrainingDataGenerator.Entities.Mappers.FeatureMapper;

namespace TrainingDataGenerator.Tests.Unit;

public class FeatureServiceTests
{
    private static readonly string classesFolder = Path.Combine("..", "..", "..", "TestData", "Classes");
    private static readonly string racesFolder = Path.Combine("..", "..", "..", "TestData", "Races");

    #region Helper Methods

    private FeatureService CreateService(Mock<ILogger>? mockLogger = null)
    {
        mockLogger ??= new Mock<ILogger>();
        return new FeatureService(mockLogger.Object);
    }

    private PartyMember CreateTestPartyMember(string className = "fighter", byte level = 1)
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

        var raceMapper = CreateTestRace("human");
        var classMapper = CreateTestClass(className);

        return new PartyMember(
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

    private FeatureSpecific CreateExpertiseOptions(List<string> options)
    {
        var optionsList = options.Select(o => new Option
        {
            Item = new OptionReference
            {
                Item = new BaseEntity(o, o)
            }
        }).ToList();

        return new FeatureSpecific
        {
            ExpertiseOptions = new ExpertiseOptions
            {
                From = new OptionSet
                {
                    Options = optionsList
                }
            }
        };
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FeatureService(null!));
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldCreateInstance()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act
        var service = new FeatureService(mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    #endregion

    #region ApplyFeatureEffects Tests

    [Fact]
    public void ApplyFeatureEffects_WithFastMovement_ShouldIncreaseSpeed()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var initialSpeed = member.Speed;
        member.Features = new List<Feature>() { new Feature(new FeatureMapper("fast-movement", "Fast Movement"), new List<string>()) };

        // Act
        service.ApplyFeatureEffects(member);

        // Assert
        Assert.Equal(initialSpeed + 10, member.Speed);
    }

    [Fact]
    public void ApplyFeatureEffects_WithoutFastMovement_ShouldNotChangeSpeed()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var initialSpeed = member.Speed;

        // Act
        service.ApplyFeatureEffects(member);

        // Assert
        Assert.Equal(initialSpeed, member.Speed);
    }

    [Fact]
    public void ApplyFeatureEffects_MonkWithUnarmoredMovement_ShouldIncreaseSpeed()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(className: "monk");
        var initialSpeed = member.Speed;
        member.Features.Add(new Feature(new FeatureMapper("unarmored-movement-1", "Unarmored Movement"), new List<string>()));

        // Act
        service.ApplyFeatureEffects(member);

        // Assert
        Assert.Equal(initialSpeed + 10, member.Speed);
    }

    [Fact]
    public void ApplyFeatureEffects_MonkWithUnarmoredMovementAndArmorEquipped_ShouldNotIncreaseSpeed()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(className: "monk");
        var initialSpeed = member.Speed;
        member.Features.Add(new Feature(new FeatureMapper("unarmored-movement-1", "Unarmored Movement"), new List<string>()));
        
        var armor = new Armor(new EquipmentMapper("leather-armor", "Leather Armor"));
        armor.IsEquipped = true;
        member.Armors.Add(armor);

        // Act
        service.ApplyFeatureEffects(member);

        // Assert
        Assert.Equal(initialSpeed, member.Speed);
    }

    [Fact]
    public void ApplyFeatureEffects_MonkWithDiamondSoul_ShouldAddProficiencyToMentalSaves()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(className: "monk", level: 14);
        var profBonus = member.ProficiencyBonus;
        
        var initialConSave = member.Constitution.Save;
        var initialIntSave = member.Intelligence.Save;
        var initialWisSave = member.Wisdom.Save;
        var initialChaSave = member.Charisma.Save;
        
        member.Features.Add(new Feature(new FeatureMapper("diamond-soul", "Diamond Soul"), new List<string>()));

        // Act
        service.ApplyFeatureEffects(member);

        // Assert
        Assert.Equal(initialConSave + profBonus, member.Constitution.Save);
        Assert.Equal(initialIntSave + profBonus, member.Intelligence.Save);
        Assert.Equal(initialWisSave + profBonus, member.Wisdom.Save);
        Assert.Equal(initialChaSave + profBonus, member.Charisma.Save);
    }

    [Fact]
    public void ApplyFeatureEffects_NonMonkWithMonkFeatures_ShouldNotApplyMonkSpecificEffects()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(className: "fighter");
        var initialSpeed = member.Speed;
        member.Features.Add(new Feature(new FeatureMapper("unarmored-movement-1", "Unarmored Movement"), new List<string>()));

        // Act
        service.ApplyFeatureEffects(member);

        // Assert
        Assert.Equal(initialSpeed, member.Speed);
    }

    [Fact]
    public void ApplyFeatureEffects_WithMultipleSpeedFeatures_ShouldStackSpeed()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(className: "monk");
        var initialSpeed = member.Speed;
        member.Features.Add(new Feature(new FeatureMapper("fast-movement", "Fast Movement"), new List<string>()));
        member.Features.Add(new Feature(new FeatureMapper("unarmored-movement-1", "Unarmored Movement"), new List<string>()));

        // Act
        service.ApplyFeatureEffects(member);

        // Assert
        Assert.Equal(initialSpeed + 20, member.Speed);
    }

    #endregion

    #region ApplyFeatureSpecifics Tests

    [Fact]
    public void ApplyFeatureSpecifics_WithExpertise_ShouldApplyToSkills()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var profBonus = member.ProficiencyBonus;
        
        var skill = new Skill(new BaseEntity("athletics", "Athletics"), 2);
        skill.SetProficiency(true, profBonus);
        member.Skills.Add(skill);

        var featureMapper = new FeatureMapper("expertise-1", "Expertise");
        featureMapper.FeatureSpec = CreateExpertiseOptions(new List<string> { "athletics" });

        var feature = new Feature(featureMapper, new List<string>());
        feature.FeatureType = FeatureSpecificTypes.Expertise;
        feature.FeatureSpec = new List<string> { "athletics" };
        member.Features = new List<Feature>() { feature };

        // Act
        service.ApplyFeatureSpecifics(member);

        // Assert
        var athleticsSkill = member.Skills.First(s => s.Index == "athletics");
        Assert.Equal(2 + (profBonus * 2), athleticsSkill.Modifier);
    }

    [Fact]
    public void ApplyFeatureSpecifics_WithFavoredEnemy_ShouldAddToFeatureSpecifics()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        
        var featureMapper = new FeatureMapper("favored-enemy", "Favored Enemy");        
        var feature = new Feature(featureMapper, new List<string>());
        feature.FeatureType = FeatureSpecificTypes.Enemy;

        feature.FeatureSpec = new List<string> { "dragons", "undead" };
        member.Features.Add(feature);

        // Act
        service.ApplyFeatureSpecifics(member);

        // Assert
        Assert.Contains("dragons", member.FeatureSpecifics);
        Assert.Contains("undead", member.FeatureSpecifics);
    }

    [Fact]
    public void ApplyFeatureSpecifics_WithFavoredTerrain_ShouldAddToFeatureSpecifics()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        
        var featureMapper = new FeatureMapper("favored-terrain", "Favored Terrain");        
        var feature = new Feature(featureMapper, new List<string>());

        feature.FeatureSpec = new List<string> { "forest", "mountain" };
        feature.FeatureType = FeatureSpecificTypes.Terrain;
        member.Features.Add(feature);

        // Act
        service.ApplyFeatureSpecifics(member);

        // Assert
        Assert.Contains("forest", member.FeatureSpecifics);
        Assert.Contains("mountain", member.FeatureSpecifics);
    }

    [Fact]
    public void ApplyFeatureSpecifics_WithSubfeature_ShouldAddNewFeatures()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var service = CreateService(mockLogger);
        var member = CreateTestPartyMember();
        member.Features = new List<Feature>();

        var initialFeatureCount = member.Features.Count;
        
        var subfeatureMapper = new FeatureMapper("dragon-ancestor-black---acid-damage", "dragon-ancestor-black---acid-damage");        
        var featureMapper = new FeatureMapper("dragon-ancestor", "");
        var feature = new Feature(featureMapper, new List<string>());
        feature.FeatureType = FeatureSpecificTypes.Subfeature;
        feature.FeatureSpec = new List<string> { "dragon-ancestor-black---acid-damage" };

        member.Features = new List<Feature>() { feature };

        // Act
        service.ApplyFeatureSpecifics(member);

        // Assert
        Assert.True(member.Features.Count > initialFeatureCount);
        Assert.Contains(member.Features, f => f.Index == "dragon-ancestor-black---acid-damage");
    }

    [Fact]
    public void ApplyFeatureSpecifics_WithNullFeatureType_ShouldSkipFeature()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Features = new List<Feature>();
        var initialFeatureCount = member.Features.Count;        
        var featureMapper = new FeatureMapper("some-feature", "Some Feature");
        var feature = new Feature(featureMapper, new List<string>());

        member.Features.Add(feature);

        // Act
        service.ApplyFeatureSpecifics(member);

        // Assert
        Assert.Equal(initialFeatureCount + 1, member.Features.Count);
    }

    [Fact]
    public void ApplyFeatureSpecifics_WithEmptyFeatureSpec_ShouldNotCrash()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        
        var featureMapper = new FeatureMapper("expertise-1", "Expertise");        
        var feature = new Feature(featureMapper, new List<string>());
        feature.FeatureType = FeatureSpecificTypes.Expertise;

        feature.FeatureSpec = null;
        member.Features.Add(feature);

        // Act & Assert
        var exception = Record.Exception(() => service.ApplyFeatureSpecifics(member));
        Assert.Null(exception);
    }

    [Fact]
    public void ApplyFeatureSpecifics_WithMultipleExpertiseFeatures_ShouldApplyAll()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var profBonus = member.ProficiencyBonus;
        
        var skill1 = new Skill(new BaseEntity("athletics", "Athletics"), 2);
        skill1.SetProficiency(true, profBonus);
        skill1.SetExpertise(true, profBonus);
        member.Skills.Add(skill1);
        
        var skill2 = new Skill(new BaseEntity("acrobatics", "Acrobatics"), 2);
        skill2.SetProficiency(true, profBonus);
        skill2.SetExpertise(true, profBonus);
        member.Skills.Add(skill2);

        var featureMapper = new FeatureMapper("expertise-1", "Expertise");
        
        var feature = new Feature(featureMapper, new List<string>());
        feature.FeatureSpec = new List<string> { "athletics", "acrobatics" };
        feature.FeatureType = FeatureSpecificTypes.Expertise;
        member.Features.Add(feature);

        // Act
        service.ApplyFeatureSpecifics(member);

        // Assert
        Assert.Equal(2 + (profBonus * 2), member.Skills.First(s => s.Index == "athletics").Modifier);
        Assert.Equal(2 + (profBonus * 2), member.Skills.First(s => s.Index == "acrobatics").Modifier);
    }

    #endregion

    #region CheckFeaturePrerequisites Tests

    [Fact]
    public void CheckFeaturePrerequisites_WithMetFeaturePrerequisite_ShouldKeepFeature()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        
        member.Features.Add(new Feature(new FeatureMapper("prerequisite-feature", "Prerequisite Feature"), new List<string>()));
        member.Features.Add(new Feature(new FeatureMapper("dependent-feature", "Dependent Feature"), new List<string>()));

        var allFeatures = new List<FeatureMapper>
        {
            new FeatureMapper("dependent-feature", "Dependent Feature")
            {
                Prerequisites = new List<FeaturePrerequisite>
                {
                    new FeaturePrerequisite
                    {
                        Type = "feature",
                        Feature = "prerequisite-feature"
                    }
                }
            }
        };

        // Act
        service.CheckFeaturePrerequisites(member, allFeatures);

        // Assert
        Assert.Contains(member.Features, f => f.Index == "dependent-feature");
    }

    [Fact]
    public void CheckFeaturePrerequisites_WithUnmetFeaturePrerequisite_ShouldRemoveFeature()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var service = CreateService(mockLogger);
        var member = CreateTestPartyMember();
        
        member.Features.Add(new Feature(new FeatureMapper("dependent-feature", "Dependent Feature"), new List<string>()));

        var allFeatures = new List<FeatureMapper>
        {
            new FeatureMapper("dependent-feature", "Dependent Feature")
            {
                Prerequisites = new List<FeaturePrerequisite>
                {
                    new FeaturePrerequisite
                    {
                        Type = "feature",
                        Feature = "missing-feature"
                    }
                }
            }
        };

        // Act
        service.CheckFeaturePrerequisites(member, allFeatures);

        // Assert
        Assert.DoesNotContain(member.Features, f => f.Index == "dependent-feature");
        mockLogger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void CheckFeaturePrerequisites_WithMetSpellPrerequisite_ShouldKeepFeature()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Features = new List<Feature>();

        member.Spells.Add(new Spell(new SpellMapper("fireball", "Fireball")));
        member.Features.Add(new Feature(new FeatureMapper("dependent-feature", "Dependent Feature"), new List<string>()));

        var allFeatures = new List<FeatureMapper>
        {
            new FeatureMapper("dependent-feature", "Dependent Feature")
            {
                Prerequisites = new List<FeaturePrerequisite>
                {
                    new FeaturePrerequisite
                    {
                        Type = "spell",
                        Spell = "fireball"
                    }
                }
            }
        };

        // Act
        service.CheckFeaturePrerequisites(member, allFeatures);

        // Assert
        Assert.Contains(member.Features, f => f.Index == "dependent-feature");
    }

    [Fact]
    public void CheckFeaturePrerequisites_WithMetCantripPrerequisite_ShouldKeepFeature()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Features = new List<Feature>();

        member.Cantrips.Add(new Spell(new SpellMapper("eldritch-blast", "Eldritch Blast")));
        member.Features.Add(new Feature(new FeatureMapper("dependent-feature", "Dependent Feature"), new List<string>()));

        var allFeatures = new List<FeatureMapper>
        {
            new FeatureMapper("dependent-feature", "Dependent Feature")
            {
                Prerequisites = new List<FeaturePrerequisite>
                {
                    new FeaturePrerequisite
                    {
                        Type = "spell",
                        Spell = "eldritch-blast"
                    }
                }
            }
        };

        // Act
        service.CheckFeaturePrerequisites(member, allFeatures);

        // Assert
        Assert.Contains(member.Features, f => f.Index == "dependent-feature");
    }

    [Fact]
    public void CheckFeaturePrerequisites_WithUnmetSpellPrerequisite_ShouldRemoveFeature()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        
        member.Features.Add(new Feature(new FeatureMapper("dependent-feature", "Dependent Feature"), new List<string>()));

        var allFeatures = new List<FeatureMapper>
        {
            new FeatureMapper("dependent-feature", "Dependent Feature")
            {
                Prerequisites = new List<FeaturePrerequisite>
                {
                    new FeaturePrerequisite
                    {
                        Type = "spell",
                        Spell = "missing-spell"
                    }
                }
            }
        };

        // Act
        service.CheckFeaturePrerequisites(member, allFeatures);

        // Assert
        Assert.DoesNotContain(member.Features, f => f.Index == "dependent-feature");
    }

    [Fact]
    public void CheckFeaturePrerequisites_WithNoPrerequisites_ShouldKeepAllFeatures()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Features = new List<Feature>();

        member.Features.Add(new Feature(new FeatureMapper("feature-1", "Feature 1"), new List<string>()));
        member.Features.Add(new Feature(new FeatureMapper("feature-2", "Feature 2"), new List<string>()));

        var allFeatures = new List<FeatureMapper>
        {
            new FeatureMapper("feature-1", "Feature 1"),
            new FeatureMapper("feature-2", "Feature 2")
        };

        // Act
        service.CheckFeaturePrerequisites(member, allFeatures);

        // Assert
        Assert.Equal(2, member.Features.Count);
    }

    [Fact]
    public void CheckFeaturePrerequisites_WithMultiplePrerequisites_ShouldCheckAll()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Features = new List<Feature>();

        member.Features.Add(new Feature(new FeatureMapper("prereq-1", "Prerequisite 1"), new List<string>()));
        member.Spells.Add(new Spell(new SpellMapper("prereq-spell", "Prerequisite Spell")));
        member.Features.Add(new Feature(new FeatureMapper("dependent-feature", "Dependent Feature"), new List<string>()));

        var allFeatures = new List<FeatureMapper>
        {
            new FeatureMapper("dependent-feature", "Dependent Feature")
            {
                Prerequisites = new List<FeaturePrerequisite>
                {
                    new FeaturePrerequisite
                    {
                        Type = "feature",
                        Feature = "prereq-1"
                    },
                    new FeaturePrerequisite
                    {
                        Type = "spell",
                        Spell = "prereq-spell"
                    }
                }
            }
        };

        // Act
        service.CheckFeaturePrerequisites(member, allFeatures);

        // Assert
        Assert.Contains(member.Features, f => f.Index == "dependent-feature");
    }

    [Fact]
    public void CheckFeaturePrerequisites_WithEmptyFeatureList_ShouldNotCrash()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var allFeatures = new List<FeatureMapper>();

        // Act & Assert
        var exception = Record.Exception(() => service.CheckFeaturePrerequisites(member, allFeatures));
        Assert.Null(exception);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void ApplyFeatureEffects_CompleteMonkScenario_ShouldApplyAllEffects()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(className: "monk", level: 14);
        var initialSpeed = member.Speed;
        var profBonus = member.ProficiencyBonus;
        var initialConSave = member.Constitution.Save;
        
        member.Features.Add(new Feature(new FeatureMapper("fast-movement", "Fast Movement"), new List<string>()));
        member.Features.Add(new Feature(new FeatureMapper("unarmored-movement-1", "Unarmored Movement"), new List<string>()));
        member.Features.Add(new Feature(new FeatureMapper("diamond-soul", "Diamond Soul"), new List<string>()));

        // Act
        service.ApplyFeatureEffects(member);

        // Assert
        Assert.Equal(initialSpeed + 20, member.Speed);
        Assert.Equal(initialConSave + profBonus, member.Constitution.Save);
    }

    [Fact]
    public void ApplyFeatureSpecifics_WithMultipleFeatureTypes_ShouldApplyAll()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var profBonus = member.ProficiencyBonus;
        
        var skill = new Skill(new BaseEntity("stealth", "Stealth"), 2);
        skill.SetProficiency(true, profBonus);
        skill.SetExpertise(true, profBonus);
        member.Skills.Add(skill);

        var expertiseMapper = new FeatureMapper("expertise-1", "Expertise");
        var expertiseFeature = new Feature(expertiseMapper, new List<string>());
        expertiseFeature.FeatureType = FeatureSpecificTypes.Expertise;
        expertiseFeature.FeatureSpec = new List<string> { "stealth" };
        member.Features.Add(expertiseFeature);

        var enemyMapper = new FeatureMapper("favored-enemy", "Favored Enemy");
        var enemyFeature = new Feature(enemyMapper, new List<string>());
        enemyFeature.FeatureType = FeatureSpecificTypes.Enemy;
        enemyFeature.FeatureSpec = new List<string> { "aberrations" };
        member.Features.Add(enemyFeature);

        // Act
        service.ApplyFeatureSpecifics(member);

        // Assert
        Assert.Equal(2 + (profBonus * 2), member.Skills.First(s => s.Index == "stealth").Modifier);
        Assert.Contains("aberrations", member.FeatureSpecifics);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ApplyFeatureEffects_WithEmptyFeatureList_ShouldNotCrash()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Features.Clear();

        // Act & Assert
        var exception = Record.Exception(() => service.ApplyFeatureEffects(member));
        Assert.Null(exception);
    }

    [Fact]
    public void ApplyFeatureSpecifics_WithNonExistentSkill_ShouldNotCrash()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        
        var featureMapper = new FeatureMapper("expertise-1", "Expertise");
        var feature = new Feature(featureMapper, new List<string>());
        feature.FeatureType = FeatureSpecificTypes.Expertise;
        feature.FeatureSpec = new List<string> { "non-existent-skill" };
        member.Features.Add(feature);

        // Act & Assert
        var exception = Record.Exception(() => service.ApplyFeatureSpecifics(member));
        Assert.Null(exception);
    }

    [Fact]
    public void ApplyFeatureSpecifics_WithNonExistentSubfeature_ShouldNotCrash()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        
        var featureMapper = new FeatureMapper("subfeature-parent", "Subfeature Parent");
        var feature = new Feature(featureMapper, new List<string>());
        feature.FeatureType = FeatureSpecificTypes.Subfeature;
        feature.FeatureSpec = new List<string> { "non-existent-subfeature" };
        member.Features.Add(feature);

        // Act & Assert
        var exception = Record.Exception(() => service.ApplyFeatureSpecifics(member));
        Assert.Null(exception);
    }

    #endregion
}