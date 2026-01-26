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

    #region Attribute Tests

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

    #endregion

    #region Hit Points Tests

    [Theory]
    [MemberData(nameof(GetConstructorTestData))]
    public void HitPoints_ShouldBePositiveAndUnderTheMax(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.True(member.HitPoints > 0);
        Assert.True(member.HitPoints < 440); // Max HP possible: 20 levels of Barbarian (12 HP per level) + 10 Con modifier (30 Con score) = 440
        Assert.True(member.HitPoints <= (classMapper.Hp * level) + (member.Constitution.Modifier * level));
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
}