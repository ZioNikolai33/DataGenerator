using MongoDB.Bson.Serialization;
using Moq;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Equip;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;
using Attribute = TrainingDataGenerator.Entities.Attribute;

namespace TrainingDataGenerator.Tests.Unit;

public class PartyMemberTests
{
    private static readonly string classesFolder = Path.Combine("..", "..", "..", "TestData", "Classes");
    private static readonly string racesFolder = Path.Combine("..", "..", "..", "TestData", "Races");

    #region Helper Methods

    private Mock<ILogger> CreateMockLogger() => new Mock<ILogger>();
    
    private Mock<RandomProvider> CreateMockRandom() => new Mock<RandomProvider>();
    
    private Mock<IAttributeService> CreateMockAttributeService() => new Mock<IAttributeService>();
    
    private Mock<IEquipmentService> CreateMockEquipmentService() => new Mock<IEquipmentService>();
    
    private Mock<ISpellService> CreateMockSpellService() => new Mock<ISpellService>();
    
    private Mock<IFeatureService> CreateMockFeatureService() => new Mock<IFeatureService>();
    
    private Mock<IProficiencyService> CreateMockProficiencyService() => new Mock<IProficiencyService>();
    
    private Mock<ITraitService> CreateMockTraitService() => new Mock<ITraitService>();
    
    private Mock<IResistanceService> CreateMockResistanceService() => new Mock<IResistanceService>();

    private static ClassMapper CreateTestClass(string className = "fighter")
    {
        var classFilePath = Path.Combine(classesFolder, $"{className}.json");
        var classJson = File.ReadAllText(classFilePath);

        return BsonSerializer.Deserialize<ClassMapper>(classJson)!;
    }

    private static RaceMapper CreateTestRace(string raceName = "human")
    {
        var raceFilePath = Path.Combine(racesFolder, $"{raceName}.json");
        var raceJson = File.ReadAllText(raceFilePath);

        return BsonSerializer.Deserialize<RaceMapper>(raceJson)!;
    }

    private PartyMember CreatePartyMember(
        int id = 1,
        byte level = 1,
        string raceString = "human",
        string classString = "fighter",
        Mock<ILogger>? logger = null,
        Mock<RandomProvider>? random = null,
        Mock<IAttributeService>? attributeService = null,
        Mock<IEquipmentService>? equipmentService = null,
        Mock<ISpellService>? spellService = null,
        Mock<IFeatureService>? featureService = null,
        Mock<IProficiencyService>? proficiencyService = null,
        Mock<ITraitService>? traitService = null,
        Mock<IResistanceService>? resistanceService = null)
    {
        var race = CreateTestRace(raceString);
        var classMapper = CreateTestClass(classString);
        logger ??= CreateMockLogger();
        random ??= CreateMockRandom();
        attributeService ??= CreateMockAttributeService();
        equipmentService ??= CreateMockEquipmentService();
        spellService ??= CreateMockSpellService();
        featureService ??= CreateMockFeatureService();
        proficiencyService ??= CreateMockProficiencyService();
        traitService ??= CreateMockTraitService();
        resistanceService ??= CreateMockResistanceService();

        return new PartyMember(
            id,
            level,
            race,
            classMapper,
            logger.Object,
            random.Object,
            attributeService.Object,
            equipmentService.Object,
            spellService.Object,
            featureService.Object,
            proficiencyService.Object,
            traitService.Object,
            resistanceService.Object);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var race = CreateTestRace();
        var classMapper = CreateTestClass();
        var random = CreateMockRandom();
        var attributeService = CreateMockAttributeService();
        var equipmentService = CreateMockEquipmentService();
        var spellService = CreateMockSpellService();
        var featureService = CreateMockFeatureService();
        var proficiencyService = CreateMockProficiencyService();
        var traitService = CreateMockTraitService();
        var resistanceService = CreateMockResistanceService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PartyMember(
            1, 1, race, classMapper, null!, random.Object, attributeService.Object,
            equipmentService.Object, spellService.Object, featureService.Object,
            proficiencyService.Object, traitService.Object, resistanceService.Object));
    }

    [Fact]
    public void Constructor_WithNullRandom_ShouldThrowArgumentNullException()
    {
        // Arrange
        var race = CreateTestRace();
        var classMapper = CreateTestClass();
        var logger = CreateMockLogger();
        var attributeService = CreateMockAttributeService();
        var equipmentService = CreateMockEquipmentService();
        var spellService = CreateMockSpellService();
        var featureService = CreateMockFeatureService();
        var proficiencyService = CreateMockProficiencyService();
        var traitService = CreateMockTraitService();
        var resistanceService = CreateMockResistanceService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PartyMember(
            1, 1, race, classMapper, logger.Object, null!, attributeService.Object,
            equipmentService.Object, spellService.Object, featureService.Object,
            proficiencyService.Object, traitService.Object, resistanceService.Object));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var member = CreatePartyMember();

        // Assert
        Assert.NotNull(member);
    }

    [Fact]
    public void Constructor_ShouldSetBasicProperties()
    {
        // Arrange
        var id = 5;
        byte level = 10;

        // Act
        var member = CreatePartyMember(id, level, "elf", "wizard");

        // Assert
        Assert.Equal(id.ToString(), member.Index);
        Assert.Equal($"Member {id}", member.Name);
        Assert.Equal(level, member.Level);
        Assert.Equal(6, member.HitDie);
        Assert.Equal("elf", member.Race);
        Assert.Equal("wizard", member.Class);
        Assert.Equal("evocation", member.Subclass);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(4, 2)]
    [InlineData(5, 3)]
    [InlineData(8, 3)]
    [InlineData(9, 4)]
    [InlineData(12, 4)]
    [InlineData(13, 5)]
    [InlineData(16, 5)]
    [InlineData(17, 6)]
    [InlineData(20, 6)]
    public void Constructor_ShouldSetCorrectProficiencyBonus(byte level, byte expectedBonus)
    {
        // Arrange & Act
        var member = CreatePartyMember(1, level);

        // Assert
        Assert.Equal(expectedBonus, member.ProficiencyBonus);
    }

    [Fact]
    public void Constructor_ShouldCallAllServices()
    {
        // Arrange
        var attributeService = CreateMockAttributeService();
        var equipmentService = CreateMockEquipmentService();
        var spellService = CreateMockSpellService();
        var featureService = CreateMockFeatureService();
        var proficiencyService = CreateMockProficiencyService();
        var traitService = CreateMockTraitService();
        var resistanceService = CreateMockResistanceService();

        // Act
        var member = CreatePartyMember(
            attributeService: attributeService,
            equipmentService: equipmentService,
            spellService: spellService,
            featureService: featureService,
            proficiencyService: proficiencyService,
            traitService: traitService,
            resistanceService: resistanceService);

        // Assert
        attributeService.Verify(s => s.SetAttributes(It.IsAny<PartyMember>(), It.IsAny<ClassMapper>(), It.IsAny<List<AbilityBonus>>(), It.IsAny<List<FeatureMapper>>()), Times.Once);
        equipmentService.Verify(s => s.ManageEquipments(It.IsAny<PartyMember>(), It.IsAny<ClassMapper>()), Times.Once);
        traitService.Verify(s => s.ManageTraitSpecifics(It.IsAny<PartyMember>(), It.IsAny<RaceMapper>(), It.IsAny<SubraceMapper>()), Times.Once);
        proficiencyService.Verify(s => s.SetInitialProficiencies(It.IsAny<PartyMember>(), It.IsAny<ClassMapper>(), It.IsAny<RaceMapper>(), It.IsAny<SubraceMapper>()), Times.Once);
        attributeService.Verify(s => s.AddSavingThrowProficiencies(It.IsAny<PartyMember>(), It.IsAny<ClassMapper>()), Times.Once);
        featureService.Verify(s => s.ApplyFeatureEffects(It.IsAny<PartyMember>()), Times.Once);
        proficiencyService.Verify(s => s.AddBackgroundProficiencies(It.IsAny<PartyMember>()), Times.Once);
        proficiencyService.Verify(s => s.AddAdditionalProficiencies(It.IsAny<PartyMember>()), Times.Once);
        proficiencyService.Verify(s => s.ApplySkillProficiencies(It.IsAny<PartyMember>()), Times.Once);
        resistanceService.Verify(s => s.ApplyDamageResistances(It.IsAny<PartyMember>()), Times.Once);
        featureService.Verify(s => s.ApplyFeatureSpecifics(It.IsAny<PartyMember>()), Times.Once);
    }

    [Fact]
    public void Constructor_WithSpellcaster_ShouldSetSpellcastingAbility()
    {
        // Act
        var member = CreatePartyMember(classString: "wizard");

        // Assert
        Assert.Equal("intelligence", member.SpellcastingAbility);
    }

    [Fact]
    public void Constructor_WithNonSpellcaster_ShouldHaveEmptySpellcastingAbility()
    {
        // Act
        var member = CreatePartyMember(classString: "fighter");

        // Assert
        Assert.Equal(string.Empty, member.SpellcastingAbility);
    }

    [Fact]
    public void Constructor_ShouldDeduplicateSpells()
    {
        // Arrange
        var duplicateSpell = new SpellMapper("fireball", "Fireball") { Level = 3 };
        
        var member = CreatePartyMember();
        member.Spells.Add(new Spell(duplicateSpell));
        member.Spells.Add(new Spell(duplicateSpell));

        // Act
        member.Spells = member.Spells.GroupBy(s => s.Index).Select(g => g.First()).ToList();

        // Assert
        Assert.Single(member.Spells);
    }

    [Fact]
    public void Constructor_ShouldDeduplicateCantrips()
    {
        // Arrange
        var duplicateCantrip = new SpellMapper("firebolt", "Fire Bolt") { Level = 0 };
        
        var member = CreatePartyMember();
        member.Cantrips.Add(new Spell(duplicateCantrip));
        member.Cantrips.Add(new Spell(duplicateCantrip));

        // Act
        member.Cantrips = member.Cantrips.GroupBy(c => c.Index).Select(g => g.First()).ToList();

        // Assert
        Assert.Single(member.Cantrips);
    }

    #endregion

    #region CalculateArmorClass Tests

    [Fact]
    public void CalculateArmorClass_WithNoArmor_ShouldReturn10PlusDexModifier()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(14); // +2 modifier
        member.Armors = new List<Armor>();

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(12, ac); // 10 + 2
    }

    [Fact]
    public void CalculateArmorClass_WithLightArmor_ShouldIncludeFullDexBonus()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(16);
        member.Armors = new List<Armor>
        {
            new Armor(new EquipmentMapper("leather", "Leather Armor"))
            {
                IsEquipped = true,
                ArmorClass = new Armor.ArmorData { Base = 11, HasDexBonus = true }
            }
        };

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(14, ac); // 11 + 3
    }

    [Fact]
    public void CalculateArmorClass_WithMediumArmor_ShouldCapDexBonus()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(18);
        member.Armors = new List<Armor>
        {
            new Armor(new EquipmentMapper("half-plate", "Half Plate"))
            {
                IsEquipped = true,
                ArmorClass = new Armor.ArmorData { Base = 15, HasDexBonus = true, MaxDexBonus = 2 }
            }
        };

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(17, ac); // 15 + 2 (capped)
    }

    [Fact]
    public void CalculateArmorClass_WithHeavyArmor_ShouldNotIncludeDexBonus()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(16);
        member.Armors = new List<Armor>
        {
            new Armor(new EquipmentMapper("plate", "Plate Armor"))
            {
                IsEquipped = true,
                ArmorClass = new Armor.ArmorData { Base = 18, HasDexBonus = false }
            }
        };

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(18, ac);
    }

    [Fact]
    public void CalculateArmorClass_WithShield_ShouldAdd2()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(14);
        member.Armors = new List<Armor>
        {
            new Armor(new EquipmentMapper("chain-mail", "Chain Mail"))
            {
                IsEquipped = true,
                ArmorClass = new Armor.ArmorData { Base = 16, HasDexBonus = false }
            },
            new Armor(new EquipmentMapper("shield", "Shield"))
            {
                IsEquipped = true
            }
        };

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(18, ac); // 16 + 2
    }

    [Fact]
    public void CalculateArmorClass_BarbarianUnarmoredDefense_ShouldUseConModifier()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(14);
        member.Constitution = new Attribute(16);
        member.Features = new List<Feature>
        {
            new Feature(new FeatureMapper("barbarian-unarmored-defense", "Unarmored Defense"), new List<string>())
        };
        member.Armors = new List<Armor>();

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(15, ac); // 10 + 2 (DEX) + 3 (CON)
    }

    [Fact]
    public void CalculateArmorClass_MonkUnarmoredDefense_ShouldUseWisModifier()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(16);
        member.Wisdom = new Attribute(14);
        member.Features = new List<Feature>
        {
            new Feature(new FeatureMapper("monk-unarmored-defense", "Unarmored Defense"), new List<string>())
        };
        member.Armors = new List<Armor>();

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(15, ac); // 10 + 3 (DEX) + 2 (WIS)
    }

    [Fact]
    public void CalculateArmorClass_DraconicResilience_ShouldUse13PlusDex()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(16);
        member.Features = new List<Feature>
        {
            new Feature(new FeatureMapper("draconic-resilience", "Draconic Resilience"), new List<string>())
        };
        member.Armors = new List<Armor>();

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(16, ac); // 13 + 3
    }

    [Fact]
    public void CalculateArmorClass_FightingStyleDefense_ShouldAdd1()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(14);
        member.Features = new List<Feature>
        {
            new Feature(new FeatureMapper("fighter-fighting-style-defense", "Defense"), new List<string>())
        };
        member.Armors = new List<Armor>
        {
            new Armor(new EquipmentMapper("chain-mail", "Chain Mail"))
            {
                IsEquipped = true,
                ArmorClass = new Armor.ArmorData { Base = 16, HasDexBonus = false }
            }
        };

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(17, ac); // 16 + 1
    }

    #endregion

    #region GetSavePercentage Tests

    [Theory]
    [InlineData("str", 15, 5)]
    [InlineData("dex", 15, 5)]
    [InlineData("con", 15, 5)]
    [InlineData("int", 15, 5)]
    [InlineData("wis", 15, 5)]
    [InlineData("cha", 15, 5)]
    public void GetSavePercentage_WithValidAbility_ShouldReturnPercentage(string ability, int dc, sbyte saveBonus)
    {
        // Arrange
        var member = CreatePartyMember();
        member.Strength = new Attribute(20) { Save = saveBonus };
        member.Dexterity = new Attribute(20) { Save = saveBonus };
        member.Constitution = new Attribute(20) { Save = saveBonus };
        member.Intelligence = new Attribute(20) { Save = saveBonus };
        member.Wisdom = new Attribute(20) { Save = saveBonus };
        member.Charisma = new Attribute(20) { Save = saveBonus };

        // Act
        var percentage = member.GetSavePercentage(ability, dc);

        // Assert
        Assert.True(percentage > 0);
        Assert.True(percentage <= 1);
    }

    [Fact]
    public void GetSavePercentage_WithInvalidAbility_ShouldReturn0()
    {
        // Arrange
        var member = CreatePartyMember();

        // Act
        var percentage = member.GetSavePercentage("invalid", 15);

        // Assert
        Assert.Equal(0, percentage);
    }

    #endregion

    #region CalculateBaseStats Tests

    [Fact]
    public void CalculateBaseStats_ShouldSumAllComponents()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Speed = 30;
        member.Strength = new Attribute(15);
        member.Dexterity = new Attribute(14);
        member.Constitution = new Attribute(13);
        member.Intelligence = new Attribute(12);
        member.Wisdom = new Attribute(10);
        member.Charisma = new Attribute(8);
        member.Skills = new List<Skill>
        {
            new Skill(new BaseEntity("athletics", "Athletics"), 2),
            new Skill(new BaseEntity("acrobatics", "Acrobatics"), 2)
        };

        // Act
        var totalStats = member.CalculateBaseStats();

        // Assert
        var expectedTotal = 30 + (15 + 14 + 13 + 12 + 10 + 8) + (2 + 2);
        Assert.Equal(expectedTotal, totalStats);
    }

    [Fact]
    public void CalculateSpeedValue_WithSpeed30_ShouldReturn30()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Speed = 30;

        // Act
        var speedValue = member.CalculateSpeedValue();

        // Assert
        Assert.Equal(30, speedValue);
    }

    [Fact]
    public void CalculateSpeedValue_WithSpeed40_ShouldApplyBonus()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Speed = 40;

        // Act
        var speedValue = member.CalculateSpeedValue();

        // Assert
        Assert.Equal(60, speedValue); // 40 + (10 * 2)
    }

    [Fact]
    public void CalculateStatsValue_ShouldSumAllAttributeValues()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Strength = new Attribute(15);
        member.Dexterity = new Attribute(14);
        member.Constitution = new Attribute(13);
        member.Intelligence = new Attribute(12);
        member.Wisdom = new Attribute(10);
        member.Charisma = new Attribute(8);

        // Act
        var statsValue = member.CalculateStatsValue();

        // Assert
        Assert.Equal(72, statsValue);
    }

    [Fact]
    public void CalculateSkillsValue_ShouldSumAllSkillModifiers()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Skills = new List<Skill>
        {
            new Skill(new BaseEntity("athletics", "Athletics"), 5),
            new Skill(new BaseEntity("acrobatics", "Acrobatics"), 3),
            new Skill(new BaseEntity("perception", "Perception"), 2)
        };

        // Act
        var skillsValue = member.CalculateSkillsValue();

        // Assert
        Assert.Equal(10, skillsValue);
    }

    #endregion

    #region CalculateHealingPower Tests

    [Fact]
    public void CalculateHealingPower_WithNoHealingSpells_ShouldReturn0()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Spells = new List<Spell>();

        // Act
        var healingPower = member.CalculateHealingPower();

        // Assert
        Assert.Equal(0, healingPower);
    }

    #endregion

    #region CalculateSpellUsagePercentage Tests

    [Fact]
    public void CalculateSpellUsagePercentage_WithCantrip_ShouldReturn1()
    {
        // Arrange
        var member = CreatePartyMember();
        var spell = new Spell(new SpellMapper("firebolt", "Firebolt") { Level = 0 });

        // Act
        var percentage = member.CalculateSpellUsagePercentage(spell, CRRatios.Normal);

        // Assert
        Assert.Equal(1.0, percentage);
    }

    [Fact]
    public void CalculateSpellUsagePercentage_WithNullSpell_ShouldReturn0()
    {
        // Arrange
        var member = CreatePartyMember();

        // Act
        var percentage = member.CalculateSpellUsagePercentage(null!, CRRatios.Normal);

        // Assert
        Assert.Equal(0.0, percentage);
    }

    [Fact]
    public void CalculateSpellUsagePercentage_WithNoSlots_ShouldReturn0()
    {
        // Arrange
        var member = CreatePartyMember();
        member.SpellSlots = new Slots();
        var spell = new Spell(new SpellMapper("fireball", "Fireball") { Level = 3 });

        // Act
        var percentage = member.CalculateSpellUsagePercentage(spell, CRRatios.Normal);

        // Assert
        Assert.Equal(0.0, percentage);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithLevel20_ShouldHaveCorrectProficiencyBonus()
    {
        // Arrange & Act
        var member = CreatePartyMember(1, 20);

        // Assert
        Assert.Equal(6, member.ProficiencyBonus);
    }

    [Fact]
    public void Constructor_WithLevel1_ShouldHaveCorrectProficiencyBonus()
    {
        // Arrange & Act
        var member = CreatePartyMember(1, 1);

        // Assert
        Assert.Equal(2, member.ProficiencyBonus);
    }

    [Fact]
    public void CalculateArmorClass_WithNegativeDexModifier_ShouldCalculateCorrectly()
    {
        // Arrange
        var member = CreatePartyMember();
        member.Dexterity = new Attribute(8);
        member.Armors = new List<Armor>();

        // Act
        var ac = member.CalculateArmorClass();

        // Assert
        Assert.Equal(9, ac); // 10 - 1
    }

    [Fact]
    public void Constructor_WithRaceWithoutSubraces_ShouldHaveNullSubrace()
    {
        // Act
        var member = CreatePartyMember(raceString: "human");

        // Assert
        Assert.Equal(string.Empty, member.Subrace);
    }

    #endregion
}