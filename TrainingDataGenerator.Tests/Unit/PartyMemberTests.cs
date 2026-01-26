using MongoDB.Bson.Serialization;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Tests.Unit;

public class PartyMemberTests
{
    private static readonly string classesFolder = Path.Combine("..", "..", "..", "TestData", "Classes");
    private static readonly string racesFolder = Path.Combine("..", "..", "..", "TestData", "Races");

    #region Helper Methods

    public static IEnumerable<object[]> GetConstructorTestData()
    {        
        int index = 1;

        foreach (int i in Enumerable.Range(1, 20))
            foreach (var race in DataConstants.Races)
                foreach (var className in DataConstants.Classes)
                    yield return new object[] { index++, i, race, className };
    }

    public static IEnumerable<object[]> GetBarbarianTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "barbarian").ToList();

    public static IEnumerable<object[]> GetBardTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "bard").ToList();

    public static IEnumerable<object[]> GetClericTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "cleric").ToList();

    public static IEnumerable<object[]> GetDruidTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "druid").ToList();

    public static IEnumerable<object[]> GetFighterTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "fighter").ToList();

    public static IEnumerable<object[]> GetMonkTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "monk").ToList();

    public static IEnumerable<object[]> GetPaladinTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "paladin").ToList();

    public static IEnumerable<object[]> GetRogueTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "rogue").ToList();

    public static IEnumerable<object[]> GetSorcererTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "sorcerer").ToList();

    public static IEnumerable<object[]> GetWarlockTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "warlock").ToList();

    public static IEnumerable<object[]> GetWizardTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "wizard").ToList();

    public static IEnumerable<object[]> GetHalfOrcTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "half-orc").ToList();

    public static IEnumerable<object[]> GetElfTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "elf").ToList();

    public static IEnumerable<object[]> GetHumanTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "human").ToList();

    public static IEnumerable<object[]> GetHalflingTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "halfling").ToList();

    public static IEnumerable<object[]> GetGnomeTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "gnome").ToList();

    public static IEnumerable<object[]> GetTieflingTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "tiefling").ToList();

    public static IEnumerable<object[]> GetDragonbornTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "dragonborn").ToList();

    public static IEnumerable<object[]> GetHalfElfTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "half-elf").ToList();

    public static IEnumerable<object[]> GetOrcTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "orc").ToList();

    public static IEnumerable<object[]> GetDwarfTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "dwarf").ToList();

    public static IEnumerable<object[]> GetConstructorTestDataWithoutDwarves() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() != "dwarf").ToList();

    public static IEnumerable<object[]> GetProfBonusOf2() =>
        GetConstructorTestData().Where(data => data[1] is >= 1 and <= 4).ToList();

    public static IEnumerable<object[]> GetProfBonusOf3() =>
        GetConstructorTestData().Where(data => data[1] is >= 5 and <= 8).ToList();

    public static IEnumerable<object[]> GetProfBonusOf4() =>
        GetConstructorTestData().Where(data => data[1] is >= 9 and <= 12).ToList();

    public static IEnumerable<object[]> GetProfBonusOf5() =>
        GetConstructorTestData().Where(data => data[1] is >= 13 and <= 16).ToList();

    public static IEnumerable<object[]> GetProfBonusOf6() =>
        GetConstructorTestData().Where(data => data[1] is >= 17 and <= 20).ToList();

    public static IEnumerable<object[]> GetFirstLevels() =>
        GetConstructorTestData().Where(data => data[1] is 1).ToList();

    private ClassMapper CreateTestClass(string className)
    {
        var classFilePath = Path.Combine(classesFolder, $"{className}.json");
        var classJson = File.ReadAllText(classFilePath);

        return BsonSerializer.Deserialize<ClassMapper>(classJson)!;
    }

    private RaceMapper CreateTestRace(string raceName)
    {
        var raceFilePath = Path.Combine(racesFolder, $"{raceName}.json");
        var raceJson = File.ReadAllText(raceFilePath);

        return BsonSerializer.Deserialize<RaceMapper>(raceJson)!;
    }

    #endregion

    #region Proficiency Bonus Tests

    [Theory]
    [MemberData(nameof(GetConstructorTestData))]
    public void Constructor_ShouldSetBasicProperties(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.Equal(index.ToString(), member.Index);
        Assert.Equal($"Member {index}", member.Name);
        Assert.Equal(level, member.Level);
        Assert.Equal(className, member.Class);
        Assert.Equal(raceName, member.Race);
    }

    [Theory]
    [MemberData(nameof(GetProfBonusOf2))]
    public void ShouldHaveProficiencyBonusOf2(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.Equal(2, member.ProficiencyBonus);
    }

    [Theory]
    [MemberData(nameof(GetProfBonusOf3))]
    public void ShouldHaveProficiencyBonusOf3(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.Equal(3, member.ProficiencyBonus);
    }

    [Theory]
    [MemberData(nameof(GetProfBonusOf4))]
    public void ShouldHaveProficiencyBonusOf4(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.Equal(4, member.ProficiencyBonus);
    }

    [Theory]
    [MemberData(nameof(GetProfBonusOf5))]
    public void ShouldHaveProficiencyBonusOf5(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.Equal(5, member.ProficiencyBonus);
    }

    [Theory]
    [MemberData(nameof(GetProfBonusOf6))]
    public void ShouldHaveProficiencyBonusOf6(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.Equal(6, member.ProficiencyBonus);
    }

    #endregion

    #region Attribute Tests

    [Theory]
    [MemberData(nameof(GetConstructorTestData))]
    public void Attributes_ShouldNotBeNull(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.NotNull(member.Strength);
        Assert.NotNull(member.Dexterity);
        Assert.NotNull(member.Constitution);
        Assert.NotNull(member.Intelligence);
        Assert.NotNull(member.Wisdom);
        Assert.NotNull(member.Charisma);
    }

    [Theory]
    [MemberData(nameof(GetConstructorTestData))]
    public void Attributes_ShouldBeInValidRange(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.InRange(member.Strength.Value, 1, 30);
        Assert.InRange(member.Dexterity.Value, 1, 30);
        Assert.InRange(member.Constitution.Value, 1, 30);
        Assert.InRange(member.Intelligence.Value, 1, 30);
        Assert.InRange(member.Wisdom.Value, 1, 30);
        Assert.InRange(member.Charisma.Value, 1, 30);
    }

    [Theory]
    [MemberData(nameof(GetConstructorTestData))]
    public void Attributes_ShouldHaveCorrectModifiers(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.Equal((member.Strength.Value - 10) / 2, member.Strength.Modifier);
        Assert.Equal((member.Dexterity.Value - 10) / 2, member.Dexterity.Modifier);
        Assert.Equal((member.Constitution.Value - 10) / 2, member.Constitution.Modifier);
        Assert.Equal((member.Intelligence.Value - 10) / 2, member.Intelligence.Modifier);
        Assert.Equal((member.Wisdom.Value - 10) / 2, member.Wisdom.Modifier);
        Assert.Equal((member.Charisma.Value - 10) / 2, member.Charisma.Modifier);
    }

    #endregion

    #region Hit Points Tests

    [Theory]
    [MemberData(nameof(GetConstructorTestDataWithoutDwarves))]
    public void HitPoints_ShouldBePositiveAndUnderTheMaxAnythingButDwarves(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.True(member.HitPoints > 0);
        Assert.True(member.HitPoints <= (classMapper.Hp * level) + (member.Constitution.Modifier * level));
    }

    [Theory]
    [MemberData(nameof(GetDwarfTestData))]
    public void HitPoints_ShouldBePositiveAndUnderTheMaxOnlyDwarves(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.True(member.HitPoints > 0);
        Assert.True(member.HitPoints <= (classMapper.Hp * level) + (member.Constitution.Modifier * level) + level);
    }

    [Theory]
    [MemberData(nameof(GetFirstLevels))]
    public void HitPoints_ShouldBeAtLeastHitDiePlusConMod(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);
        var minHP = classMapper.Hp + member.Constitution.Modifier;

        // Assert
        Assert.Equal(1, level);
        Assert.True(member.HitPoints >= minHP);
    }

    #endregion

    #region Monk Feature Tests

    [Theory]
    [MemberData(nameof(GetMonkTestData))]
    public void Monk_UnarmoredDefense_ShouldCalculateCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Act & Assert
        if (member.Features.Any(f => f.Index == "unarmored-defense-monk") && member.Armors.All(a => !a.IsEquipped))
        {
            var expectedAC = 10 + member.Dexterity.Modifier + member.Wisdom.Modifier;
            Assert.Equal(expectedAC, member.ArmorClass);
        }
    }

    [Theory]
    [MemberData(nameof(GetMonkTestData))]
    public void Monk_DiamondSoul_ShouldAddProficiencyToAllSaves(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Act & Assert
        if (member.Features.Any(f => f.Index == "diamond-soul"))
        {
            Assert.True(member.Level >= 14);
            Assert.Equal(member.Strength.Modifier + member.ProficiencyBonus, member.Strength.Save);
            Assert.Equal(member.Dexterity.Modifier + member.ProficiencyBonus, member.Dexterity.Save);
            Assert.Equal(member.Constitution.Modifier + member.ProficiencyBonus, member.Constitution.Save);
            Assert.Equal(member.Intelligence.Modifier + member.ProficiencyBonus, member.Intelligence.Save);
            Assert.Equal(member.Wisdom.Modifier + member.ProficiencyBonus, member.Wisdom.Save);
            Assert.Equal(member.Charisma.Modifier + member.ProficiencyBonus, member.Charisma.Save);
        }
    }

    #endregion

    #region Barbarian Feature Tests

    [Theory]
    [MemberData(nameof(GetBarbarianTestData))]
    public void Barbarian_PrimalChampion_ShouldIncreaseStrengthAndConstitution(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Act & Assert
        if (member.Features.Any(f => f.Index == "primal-champion"))
        {
            Assert.Equal(20, member.Level);
            Assert.True(member.Strength.Value >= 12);
            Assert.True(member.Constitution.Value >= 12);
        }
    }

    [Theory]
    [MemberData(nameof(GetBarbarianTestData))]
    public void Barbarian_UnarmoredDefense_ShouldUseConstitutionModifier(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Act & Assert
        if (member.Features.Any(f => f.Index == "barbarian-unarmored-defense") && member.Armors.Where(a => a.Index != "shield").All(a => !a.IsEquipped))
        {
            var expectedAC = 10 + member.Dexterity.Modifier + member.Constitution.Modifier;
            Assert.Equal(expectedAC, member.ArmorClass);
        }
    }

    #endregion
}