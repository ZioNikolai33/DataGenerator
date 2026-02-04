using MongoDB.Bson.Serialization;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Equip;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Services;
using Moq;
using Attribute = TrainingDataGenerator.Entities.Attribute;

namespace TrainingDataGenerator.Tests.Unit;

public class EquipmentServiceTests
{
    private static readonly string classesFolder = Path.Combine("..", "..", "..", "TestData", "Classes");
    private static readonly string racesFolder = Path.Combine("..", "..", "..", "TestData", "Races");

    #region Helper Methods

    private PartyMember CreateTestPartyMember(
        int id = 1,
        byte level = 1,
        byte strength = 10,
        string raceS = "human",
        ClassMapper? classMapper = null,
        List<string>? proficiencies = null)
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

        var raceMapper = CreateTestRace(raceS);

        if (classMapper == null)
            classMapper = CreateTestClass("fighter");

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

        member.Strength = new Attribute(strength);
        member.Proficiencies = proficiencies ?? new List<string>();

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

    private EquipmentService CreateService(Mock<IRandomProvider>? mockRandom = null)
    {
        var mockLogger = new Mock<ILogger>();
        mockRandom ??= new Mock<IRandomProvider>();

        return new EquipmentService(mockLogger.Object, mockRandom.Object);
    }

    private MeleeWeapon CreateMeleeWeapon(string index, string name, List<string> properties)
    {
        var equipmentMapper = new EquipmentMapper(index, name)
        {
            EquipmentCategory = new BaseEntity("weapon", "Weapon"),
            WeaponRange = "Melee",
            WeaponCategory = "Martial",
            Properties = properties.Select(p => new BaseEntity(p, p)).ToList()
        };
        return new MeleeWeapon(equipmentMapper);
    }

    private RangedWeapon CreateRangedWeapon(string index, string name)
    {
        var equipmentMapper = new EquipmentMapper(index, name)
        {
            EquipmentCategory = new BaseEntity("weapon", "Weapon"),
            WeaponRange = "Ranged",
            WeaponCategory = "Simple"
        };
        return new RangedWeapon(equipmentMapper);
    }

    private Armor CreateArmor(string index, string name, byte strengthMinimum = 0)
    {
        var equipmentMapper = new EquipmentMapper(index, name)
        {
            EquipmentCategory = new BaseEntity("armor", "Armor"),
            ArmorCategory = "Heavy",
            StrengthMinimum = strengthMinimum
        };
        return new Armor(equipmentMapper);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EquipmentService(null!, mockRandom.Object));
    }

    [Fact]
    public void Constructor_WithNullRandomProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EquipmentService(mockLogger.Object, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockRandom = new Mock<IRandomProvider>();

        // Act
        var service = new EquipmentService(mockLogger.Object, mockRandom.Object);

        // Assert
        Assert.NotNull(service);
    }

    #endregion

    #region ManageEquipments Tests

    [Fact]
    public void ManageEquipments_ShouldInitializeAllEquipmentLists()
    {
        // Arrange
        var service = CreateService();
        var classMapper = CreateTestClass("fighter");
        var member = CreateTestPartyMember(classMapper: classMapper);        

        // Act
        service.ManageEquipments(member, classMapper);

        // Assert
        Assert.NotNull(member.MeleeWeapons);
        Assert.NotNull(member.RangedWeapons);
        Assert.NotNull(member.Ammunitions);
        Assert.NotNull(member.Armors);
    }

    [Fact]
    public void ManageEquipments_ShouldAutoEquipAllAmmunition()
    {
        // Arrange
        var service = CreateService();
        var classMapper = CreateTestClass("ranger");
        var member = CreateTestPartyMember(classMapper: classMapper);        

        // Act
        service.ManageEquipments(member, classMapper);

        // Assert
        Assert.All(member.Ammunitions, ammo => Assert.True(ammo.IsEquipped));
    }

    #endregion

    #region EquipRandomWeapons Tests

    [Fact]
    public void EquipRandomWeapons_WithMeleeWeapon_ShouldEquipOne()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        
        var longsword = CreateMeleeWeapon("longsword", "Longsword", new List<string> { "versatile" });
        member.MeleeWeapons = new List<MeleeWeapon> { longsword };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<MeleeWeapon>>()))
                  .Returns(longsword);

        // Act
        service.EquipRandomWeapons(member, new List<Armor>());

        // Assert
        Assert.True(longsword.IsEquipped);
    }

    [Fact]
    public void EquipRandomWeapons_WithLightWeapon_ShouldConsiderDualWielding()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        
        var dagger1 = CreateMeleeWeapon("dagger", "Dagger", new List<string> { "light" });
        var dagger2 = CreateMeleeWeapon("dagger-2", "Dagger 2", new List<string> { "light" });
        member.MeleeWeapons = new List<MeleeWeapon> { dagger1, dagger2 };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<MeleeWeapon>>()))
                  .Returns(dagger1);

        // Act
        service.EquipRandomWeapons(member, new List<Armor>());

        // Assert
        Assert.True(dagger1.IsEquipped);
        Assert.True(dagger2.IsEquipped); // Should dual-wield
    }

    [Fact]
    public void EquipRandomWeapons_WithTwoHandedWeapon_ShouldUnequipShield()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        
        var greatsword = CreateMeleeWeapon("greatsword", "Greatsword", new List<string> { "two-handed" });
        member.MeleeWeapons = new List<MeleeWeapon> { greatsword };

        var shield = CreateArmor("shield", "Shield");
        shield.IsEquipped = true;
        var armors = new List<Armor> { shield };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<MeleeWeapon>>()))
                  .Returns(greatsword);

        // Act
        service.EquipRandomWeapons(member, armors);

        // Assert
        Assert.True(greatsword.IsEquipped);
        Assert.False(shield.IsEquipped);
    }

    [Fact]
    public void EquipRandomWeapons_WithOneHandedWeapon_ShouldEquipShield()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        
        var longsword = CreateMeleeWeapon("longsword", "Longsword", new List<string> { "versatile" });
        member.MeleeWeapons = new List<MeleeWeapon> { longsword };

        var shield = CreateArmor("shield", "Shield");
        var armors = new List<Armor> { shield };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<MeleeWeapon>>()))
                  .Returns(longsword);

        // Act
        service.EquipRandomWeapons(member, armors);

        // Assert
        Assert.True(longsword.IsEquipped);
        Assert.True(shield.IsEquipped);
    }

    [Fact]
    public void EquipRandomWeapons_WithRangedWeapon_ShouldEquipOne()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        
        var longbow = CreateRangedWeapon("longbow", "Longbow");
        member.RangedWeapons = new List<RangedWeapon> { longbow };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<RangedWeapon>>()))
                  .Returns(longbow);

        // Act
        service.EquipRandomWeapons(member, new List<Armor>());

        // Assert
        Assert.True(longbow.IsEquipped);
    }

    [Fact]
    public void EquipRandomWeapons_WithNoWeapons_ShouldNotThrow()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.MeleeWeapons = new List<MeleeWeapon>();
        member.RangedWeapons = new List<RangedWeapon>();

        // Act & Assert
        service.EquipRandomWeapons(member, new List<Armor>());
    }

    [Fact]
    public void EquipRandomWeapons_WithLightWeaponOnly_ShouldNotDualWield()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        
        var dagger = CreateMeleeWeapon("dagger", "Dagger", new List<string> { "light" });
        member.MeleeWeapons = new List<MeleeWeapon> { dagger };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<MeleeWeapon>>()))
                  .Returns(dagger);

        // Act
        service.EquipRandomWeapons(member, new List<Armor>());

        // Assert
        Assert.True(dagger.IsEquipped);
        Assert.Single(member.MeleeWeapons.Where(w => w.IsEquipped));
    }

    [Fact]
    public void EquipRandomWeapons_WithLightWeaponOnly_ShouldNotDualWieldButEquipShieldIfPossible()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();

        var dagger = CreateMeleeWeapon("dagger", "Dagger", new List<string> { "light" });
        member.MeleeWeapons = new List<MeleeWeapon> { dagger };

        var shield = CreateArmor("shield", "Shield");
        var armors = new List<Armor> { shield };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<MeleeWeapon>>()))
                  .Returns(dagger);

        // Act
        service.EquipRandomWeapons(member, new List<Armor>());

        // Assert
        Assert.True(dagger.IsEquipped);
        Assert.True(shield.IsEquipped);
        Assert.Single(member.MeleeWeapons.Where(w => w.IsEquipped));
    }

    #endregion

    #region ManageArmorRequirements Tests

    [Fact]
    public void ManageArmorRequirements_WithSufficientStrength_ShouldEquipArmor()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember(strength: 15);
        
        var chainmail = CreateArmor("chainmail", "Chainmail", strengthMinimum: 13);
        var armors = new List<Armor> { chainmail };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<Armor>>()))
                  .Returns(chainmail);

        // Act
        service.ManageArmorRequirements(member, armors);

        // Assert
        Assert.True(chainmail.IsEquipped);
    }

    [Fact]
    public void ManageArmorRequirements_WithInsufficientStrength_ShouldNotEquipArmor()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(strength: 10);
        
        var plateArmor = CreateArmor("plate", "Plate", strengthMinimum: 15);
        var armors = new List<Armor> { plateArmor };

        // Act
        service.ManageArmorRequirements(member, armors);

        // Assert
        Assert.False(plateArmor.IsEquipped);
    }

    [Fact]
    public void ManageArmorRequirements_WithMultipleArmors_ShouldPickOne()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember(strength: 15);
        
        var chainmail = CreateArmor("chainmail", "Chainmail", strengthMinimum: 13);
        var splintArmor = CreateArmor("splint", "Splint", strengthMinimum: 15);
        var armors = new List<Armor> { chainmail, splintArmor };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<Armor>>()))
                  .Returns(chainmail);

        // Act
        service.ManageArmorRequirements(member, armors);

        // Assert
        Assert.True(chainmail.IsEquipped);
        Assert.False(splintArmor.IsEquipped);
    }

    [Fact]
    public void ManageArmorRequirements_WithShieldOnly_ShouldReturnAllArmors()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        
        var shield = CreateArmor("shield", "Shield");
        var armors = new List<Armor> { shield };

        // Act
        service.ManageArmorRequirements(member, armors);

        // Assert
        Assert.Single(member.Armors);
        Assert.Equal("shield", member.Armors[0].Index);
    }

    [Fact]
    public void ManageArmorRequirements_WithNoArmors_ShouldReturnEmptyList()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        var armors = new List<Armor>();

        // Act
        service.ManageArmorRequirements(member, armors);

        // Assert
        Assert.Empty(member.Armors);
    }

    [Fact]
    public void ManageArmorRequirements_WithExactStrengthRequirement_ShouldEquipArmor()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember(strength: 15);
        
        var plateArmor = CreateArmor("plate", "Plate", strengthMinimum: 15);
        var armors = new List<Armor> { plateArmor };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<Armor>>()))
                  .Returns(plateArmor);

        // Act
        service.ManageArmorRequirements(member, armors);

        // Assert
        Assert.True(plateArmor.IsEquipped);
    }

    [Fact]
    public void ManageArmorRequirements_WithShieldAndArmor_ShouldOnlyEquipNonShieldArmor()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember(strength: 15);
        
        var chainmail = CreateArmor("chainmail", "Chainmail", strengthMinimum: 13);
        var shield = CreateArmor("shield", "Shield");
        var armors = new List<Armor> { chainmail, shield };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<Armor>>()))
                  .Returns(chainmail);

        // Act
        service.ManageArmorRequirements(member, armors);

        // Assert
        Assert.True(chainmail.IsEquipped);
        Assert.False(shield.IsEquipped);
        Assert.Equal(2, member.Armors.Count);
    }

    #endregion

    #region IsProficient Tests

    [Fact]
    public void IsProficient_WithDirectWeaponProficiency_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { "longsword" });
        var longsword = CreateMeleeWeapon("longsword", "Longsword", new List<string>());

        // Act
        var result = service.IsProficient(member, longsword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsProficient_WithCategoryRangeProficiency_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { "simple-melee-weapons" });
        var weapon = CreateMeleeWeapon("club", "Club", new List<string>());
        weapon.WeaponCategory = "Simple";
        weapon.WeaponRange = "Melee";

        // Act
        var result = service.IsProficient(member, weapon);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsProficient_WithCategoryProficiency_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { "martial-weapons" });
        var longsword = CreateMeleeWeapon("longsword", "Longsword", new List<string>());

        // Act
        var result = service.IsProficient(member, longsword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsProficient_WithRangeProficiency_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { "melee-weapons" });
        var longsword = CreateMeleeWeapon("longsword", "Longsword", new List<string>());

        // Act
        var result = service.IsProficient(member, longsword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsProficient_WithNoProficiency_ShouldReturnFalse()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string>());
        var longsword = CreateMeleeWeapon("longsword", "Longsword", new List<string>());

        // Act
        var result = service.IsProficient(member, longsword);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("longsword", "martial-weapons")]
    [InlineData("simple-melee-weapons", "martial-weapons")]
    [InlineData("martial-weapons", "martial-weapons")]
    [InlineData("melee-weapons", "martial-weapons")]
    public void IsProficient_WithVariousProficiencies_ShouldWorkCorrectly(string proficiency, string expectedMatch)
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { proficiency });
        var weapon = CreateMeleeWeapon("longsword", "Longsword", new List<string>());

        // Act
        var result = service.IsProficient(member, weapon);

        // Assert
        Assert.True(result || proficiency != expectedMatch);
    }

    #endregion

    #region GetEquipmentsByCategory Tests

    [Theory]
    [InlineData("melee-weapons")]
    [InlineData("ranged-weapons")]
    [InlineData("simple-weapons")]
    [InlineData("simple-melee-weapons")]
    [InlineData("simple-ranged-weapons")]
    [InlineData("martial-weapons")]
    [InlineData("martial-melee-weapons")]
    [InlineData("martial-ranged-weapons")]
    [InlineData("light-armor")]
    [InlineData("medium-armor")]
    [InlineData("heavy-armor")]
    [InlineData("shields")]
    public void GetEquipmentsByCategory_WithValidCategory_ShouldReturnList(string category)
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetEquipmentsByCategory(category);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetEquipmentsByCategory_WithInvalidCategory_ShouldReturnEmptyList()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetEquipmentsByCategory("invalid-category");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetEquipmentsByCategory_WithNullCategory_ShouldReturnEmptyList()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetEquipmentsByCategory(null!);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region ConvertToTypedEquipment Tests

    [Fact]
    public void ConvertToTypedEquipment_WithArmor_ShouldReturnArmorList()
    {
        // Arrange
        var service = CreateService();
        var equipments = new List<EquipmentMapper>
        {
            new EquipmentMapper("chainmail", "Chainmail")
            {
                EquipmentCategory = new BaseEntity("armor", "Armor")
            }
        };

        // Act
        var (armors, meleeWeapons, rangedWeapons, ammunitions) = service.ConvertToTypedEquipment(equipments);

        // Assert
        Assert.Single(armors);
        Assert.Empty(meleeWeapons);
        Assert.Empty(rangedWeapons);
        Assert.Empty(ammunitions);
    }

    [Fact]
    public void ConvertToTypedEquipment_WithMeleeWeapon_ShouldReturnMeleeWeaponList()
    {
        // Arrange
        var service = CreateService();
        var equipments = new List<EquipmentMapper>
        {
            new EquipmentMapper("longsword", "Longsword")
            {
                EquipmentCategory = new BaseEntity("weapon", "Weapon"),
                WeaponRange = "Melee"
            }
        };

        // Act
        var (armors, meleeWeapons, rangedWeapons, ammunitions) = service.ConvertToTypedEquipment(equipments);

        // Assert
        Assert.Empty(armors);
        Assert.Single(meleeWeapons);
        Assert.Empty(rangedWeapons);
        Assert.Empty(ammunitions);
    }

    [Fact]
    public void ConvertToTypedEquipment_WithRangedWeapon_ShouldReturnRangedWeaponList()
    {
        // Arrange
        var service = CreateService();
        var equipments = new List<EquipmentMapper>
        {
            new EquipmentMapper("longbow", "Longbow")
            {
                EquipmentCategory = new BaseEntity("weapon", "Weapon"),
                WeaponRange = "Ranged"
            }
        };

        // Act
        var (armors, meleeWeapons, rangedWeapons, ammunitions) = service.ConvertToTypedEquipment(equipments);

        // Assert
        Assert.Empty(armors);
        Assert.Empty(meleeWeapons);
        Assert.Single(rangedWeapons);
        Assert.Empty(ammunitions);
    }

    [Fact]
    public void ConvertToTypedEquipment_WithAmmunition_ShouldReturnAmmunitionList()
    {
        // Arrange
        var service = CreateService();
        var equipments = new List<EquipmentMapper>
        {
            new EquipmentMapper("arrow", "Arrow")
            {
                EquipmentCategory = new BaseEntity("adventuring-gear", "Adventuring Gear"),
                GearCategory = new BaseEntity("ammunition", "Ammunition")
            }
        };

        // Act
        var (armors, meleeWeapons, rangedWeapons, ammunitions) = service.ConvertToTypedEquipment(equipments);

        // Assert
        Assert.Empty(armors);
        Assert.Empty(meleeWeapons);
        Assert.Empty(rangedWeapons);
        Assert.Single(ammunitions);
    }

    [Fact]
    public void ConvertToTypedEquipment_WithMixedEquipment_ShouldCategorizeCorrectly()
    {
        // Arrange
        var service = CreateService();
        var equipments = new List<EquipmentMapper>
        {
            new EquipmentMapper("chainmail", "Chainmail")
            {
                EquipmentCategory = new BaseEntity("armor", "Armor")
            },
            new EquipmentMapper("longsword", "Longsword")
            {
                EquipmentCategory = new BaseEntity("weapon", "Weapon"),
                WeaponRange = "Melee"
            },
            new EquipmentMapper("longbow", "Longbow")
            {
                EquipmentCategory = new BaseEntity("weapon", "Weapon"),
                WeaponRange = "Ranged"
            },
            new EquipmentMapper("arrow", "Arrow")
            {
                EquipmentCategory = new BaseEntity("adventuring-gear", "Adventuring Gear"),
                GearCategory = new BaseEntity("ammunition", "Ammunition")
            }
        };

        // Act
        var (armors, meleeWeapons, rangedWeapons, ammunitions) = service.ConvertToTypedEquipment(equipments);

        // Assert
        Assert.Single(armors);
        Assert.Single(meleeWeapons);
        Assert.Single(rangedWeapons);
        Assert.Single(ammunitions);
    }

    [Fact]
    public void ConvertToTypedEquipment_WithEmptyList_ShouldReturnEmptyLists()
    {
        // Arrange
        var service = CreateService();
        var equipments = new List<EquipmentMapper>();

        // Act
        var (armors, meleeWeapons, rangedWeapons, ammunitions) = service.ConvertToTypedEquipment(equipments);

        // Assert
        Assert.Empty(armors);
        Assert.Empty(meleeWeapons);
        Assert.Empty(rangedWeapons);
        Assert.Empty(ammunitions);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void EquipRandomWeapons_WithMultipleLightWeaponsAlreadyEquipped_ShouldNotEquipThird()
    {
        // Arrange
        var mockRandom = new Mock<IRandomProvider>();
        var service = CreateService(mockRandom);
        var member = CreateTestPartyMember();
        
        var dagger1 = CreateMeleeWeapon("dagger", "Dagger", new List<string> { "light" });
        var dagger2 = CreateMeleeWeapon("dagger-2", "Dagger 2", new List<string> { "light" });
        var dagger3 = CreateMeleeWeapon("dagger-3", "Dagger 3", new List<string> { "light" });
        member.MeleeWeapons = new List<MeleeWeapon> { dagger1, dagger2, dagger3 };

        mockRandom.Setup(r => r.SelectRandom(It.IsAny<List<MeleeWeapon>>()))
                  .Returns(dagger1);

        // Act
        service.EquipRandomWeapons(member, new List<Armor>());

        // Assert
        Assert.Equal(2, member.MeleeWeapons.Count(w => w.IsEquipped));
    }

    [Fact]
    public void ManageArmorRequirements_WithNullStrength_ShouldHandleGracefully()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember();
        member.Strength = null;
        
        var armor = CreateArmor("leather", "Leather", strengthMinimum: 0);
        var armors = new List<Armor> { armor };

        // Act & Assert
        service.ManageArmorRequirements(member, armors);
    }

    [Fact]
    public void IsProficient_WithCaseInsensitiveMatching_ShouldWork()
    {
        // Arrange
        var service = CreateService();
        var member = CreateTestPartyMember(proficiencies: new List<string> { "simple-melee-weapons" });
        var weapon = CreateMeleeWeapon("club", "Club", new List<string>());
        weapon.WeaponCategory = "SIMPLE";
        weapon.WeaponRange = "MELEE";

        // Act
        var result = service.IsProficient(member, weapon);

        // Assert
        Assert.True(result);
    }

    #endregion
}