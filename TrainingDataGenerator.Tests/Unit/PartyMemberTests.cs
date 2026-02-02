using MongoDB.Bson.Serialization;
using System.Diagnostics;
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

    public static IEnumerable<object[]> GetConstructorTestDataLevel1Human()
    {
        int index = 1;

        foreach (var className in DataConstants.Classes)
            yield return new object[] { index++, 1, "human", className };
    }

    public static IEnumerable<object[]> GetConstructorTestDataHuman()
    {
        int index = 1;

        foreach (int i in Enumerable.Range(1, 20))
            foreach (var className in DataConstants.Classes)
                yield return new object[] { index++, i, "human", className };
    }

    public static IEnumerable<object[]> GetBarbarianTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "barbarian");

    public static IEnumerable<object[]> GetBardTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "bard");

    public static IEnumerable<object[]> GetClericTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "cleric");

    public static IEnumerable<object[]> GetDruidTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "druid");

    public static IEnumerable<object[]> GetFighterTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "fighter");

    public static IEnumerable<object[]> GetMonkTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "monk");

    public static IEnumerable<object[]> GetPaladinTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "paladin");

    public static IEnumerable<object[]> GetRogueTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "rogue");

    public static IEnumerable<object[]> GetSorcererTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "sorcerer");

    public static IEnumerable<object[]> GetWarlockTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "warlock");

    public static IEnumerable<object[]> GetWizardTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "wizard");

    public static IEnumerable<object[]> GetRangerTestData() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "ranger");

    public static IEnumerable<object[]> GetBarbarianTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "barbarian" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetBardTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "bard" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetClericTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "cleric" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetDruidTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "druid" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetFighterTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "fighter" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetMonkTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "monk" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetPaladinTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "paladin" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetRogueTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "rogue" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetSorcererTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "sorcerer" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetWarlockTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "warlock" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetWizardTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "wizard" && data[1]!.ToString() == "1");
    
    public static IEnumerable<object[]> GetRangerTestDataLevel1() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "ranger" && data[1]!.ToString() == "1");

    public static IEnumerable<object[]> GetBarbarianTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "barbarian" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetBardTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "bard" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetClericTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "cleric" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetDruidTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "druid" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetFighterTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "fighter" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetMonkTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "monk" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetPaladinTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "paladin" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetRogueTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "rogue" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetSorcererTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "sorcerer" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetWarlockTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "warlock" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetWizardTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "wizard" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetRangerTestDataLevel20() =>
        GetConstructorTestData().Where(data => data[3]!.ToString() == "ranger" && data[1]!.ToString() == "20");

    public static IEnumerable<object[]> GetHalfOrcTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "half-orc");

    public static IEnumerable<object[]> GetElfTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "elf");

    public static IEnumerable<object[]> GetHumanTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "human");

    public static IEnumerable<object[]> GetHalflingTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "halfling");

    public static IEnumerable<object[]> GetGnomeTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "gnome");

    public static IEnumerable<object[]> GetTieflingTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "tiefling");

    public static IEnumerable<object[]> GetDragonbornTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "dragonborn");

    public static IEnumerable<object[]> GetHalfElfTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "half-elf");

    public static IEnumerable<object[]> GetOrcTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "orc");

    public static IEnumerable<object[]> GetDwarfTestData() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() == "dwarf");

    public static IEnumerable<object[]> GetConstructorTestDataWithoutDwarvesAndSorcerers() =>
        GetConstructorTestData().Where(data => data[2]!.ToString() != "dwarf" && data[3]!.ToString() != "sorcerer");

    public static IEnumerable<object[]> GetProfBonusOf2() =>
        GetConstructorTestData().Where(data => data[1] is >= 1 and <= 4);

    public static IEnumerable<object[]> GetProfBonusOf3() =>
        GetConstructorTestData().Where(data => data[1] is >= 5 and <= 8);

    public static IEnumerable<object[]> GetProfBonusOf4() =>
        GetConstructorTestData().Where(data => data[1] is >= 9 and <= 12);

    public static IEnumerable<object[]> GetProfBonusOf5() =>
        GetConstructorTestData().Where(data => data[1] is >= 13 and <= 16);

    public static IEnumerable<object[]> GetProfBonusOf6() =>
        GetConstructorTestData().Where(data => data[1] is >= 17 and <= 20);

    public static IEnumerable<object[]> GetFirstLevels() =>
        GetConstructorTestData().Where(data => data[1] is 1);

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
        Assert.Equal(member.Strength.Value == 9 ? -1 : (member.Strength.Value - 10) / 2, member.Strength.Modifier);
        Assert.Equal(member.Dexterity.Value == 9 ? -1 : (member.Dexterity.Value - 10) / 2, member.Dexterity.Modifier);
        Assert.Equal(member.Constitution.Value == 9 ? -1 : (member.Constitution.Value - 10) / 2, member.Constitution.Modifier);
        Assert.Equal(member.Intelligence.Value == 9 ? -1 : (member.Intelligence.Value - 10) / 2, member.Intelligence.Modifier);
        Assert.Equal(member.Wisdom.Value == 9 ? -1 : (member.Wisdom.Value - 10) / 2, member.Wisdom.Modifier);
        Assert.Equal(member.Charisma.Value == 9 ? -1 : (member.Charisma.Value - 10) / 2, member.Charisma.Modifier);
    }

    #endregion

    #region Skills Tests

    [Theory]
    [MemberData(nameof(GetConstructorTestData))]
    public void Skills_ShouldHaveCorrectModifiers(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);
        var skills = member.Skills;

        // Assert
        Assert.NotNull(skills);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades") || f.Index.Equals("remarkable-athlete")) && !skills.Where(s => s.Index.Equals("skill-acrobatics")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-acrobatics")).First().Modifier == member.Dexterity.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-acrobatics")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-acrobatics")).First().Modifier == member.Dexterity.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-acrobatics")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-acrobatics")).First().Modifier == member.Dexterity.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-acrobatics")).First().Modifier == member.Dexterity.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-animal-handling")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-animal-handling")).First().Modifier == member.Wisdom.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-animal-handling")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-animal-handling")).First().Modifier == member.Wisdom.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-animal-handling")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-animal-handling")).First().Modifier == member.Wisdom.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-animal-handling")).First().Modifier == member.Wisdom.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-arcana")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-arcana")).First().Modifier == member.Intelligence.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-arcana")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-arcana")).First().Modifier == member.Intelligence.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-arcana")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-arcana")).First().Modifier == member.Intelligence.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-arcana")).First().Modifier == member.Intelligence.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades") || f.Index.Equals("remarkable-athlete")) && !skills.Where(s => s.Index.Equals("skill-athletics")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-athletics")).First().Modifier == member.Strength.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-athletics")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-athletics")).First().Modifier == member.Strength.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-athletics")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-athletics")).First().Modifier == member.Strength.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-athletics")).First().Modifier == member.Strength.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-deception")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-deception")).First().Modifier == member.Charisma.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-deception")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-deception")).First().Modifier == member.Charisma.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-deception")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-deception")).First().Modifier == member.Charisma.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-deception")).First().Modifier == member.Charisma.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-history")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-history")).First().Modifier == member.Intelligence.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-history")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-history")).First().Modifier == member.Intelligence.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-history")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-history")).First().Modifier == member.Intelligence.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-history")).First().Modifier == member.Intelligence.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-insight")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-insight")).First().Modifier == member.Wisdom.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-insight")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-insight")).First().Modifier == member.Wisdom.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-insight")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-insight")).First().Modifier == member.Wisdom.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-insight")).First().Modifier == member.Wisdom.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-intimidation")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-intimidation")).First().Modifier == member.Charisma.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-intimidation")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-intimidation")).First().Modifier == member.Charisma.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-intimidation")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-intimidation")).First().Modifier == member.Charisma.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-intimidation")).First().Modifier == member.Charisma.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-investigation")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-investigation")).First().Modifier == member.Intelligence.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-investigation")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-investigation")).First().Modifier == member.Intelligence.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-investigation")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-investigation")).First().Modifier == member.Intelligence.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-investigation")).First().Modifier == member.Intelligence.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-medicine")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-medicine")).First().Modifier == member.Wisdom.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-medicine")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-medicine")).First().Modifier == member.Wisdom.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-medicine")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-medicine")).First().Modifier == member.Wisdom.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-medicine")).First().Modifier == member.Wisdom.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-nature")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-nature")).First().Modifier == member.Intelligence.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-nature")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-nature")).First().Modifier == member.Intelligence.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-nature")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-nature")).First().Modifier == member.Intelligence.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-nature")).First().Modifier == member.Intelligence.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-perception")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-perception")).First().Modifier == member.Wisdom.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-perception")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-perception")).First().Modifier == member.Wisdom.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-perception")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-perception")).First().Modifier == member.Wisdom.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-perception")).First().Modifier == member.Wisdom.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-performance")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-performance")).First().Modifier == member.Charisma.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-performance")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-performance")).First().Modifier == member.Charisma.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-performance")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-performance")).First().Modifier == member.Charisma.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-performance")).First().Modifier == member.Charisma.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-persuasion")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-persuasion")).First().Modifier == member.Charisma.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-persuasion")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-persuasion")).First().Modifier == member.Charisma.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-persuasion")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-persuasion")).First().Modifier == member.Charisma.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-persuasion")).First().Modifier == member.Charisma.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-religion")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-religion")).First().Modifier == member.Intelligence.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-religion")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-religion")).First().Modifier == member.Intelligence.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-religion")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-religion")).First().Modifier == member.Intelligence.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-religion")).First().Modifier == member.Intelligence.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades") || f.Index.Equals("remarkable-athlete")) && !skills.Where(s => s.Index.Equals("skill-sleight-of-hand")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-sleight-of-hand")).First().Modifier == member.Dexterity.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-sleight-of-hand")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-sleight-of-hand")).First().Modifier == member.Dexterity.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-sleight-of-hand")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-sleight-of-hand")).First().Modifier == member.Dexterity.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-sleight-of-hand")).First().Modifier == member.Dexterity.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades") || f.Index.Equals("remarkable-athlete")) && !skills.Where(s => s.Index.Equals("skill-stealth")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-stealth")).First().Modifier == member.Dexterity.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-stealth")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-stealth")).First().Modifier == member.Dexterity.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-stealth")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-stealth")).First().Modifier == member.Dexterity.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-stealth")).First().Modifier == member.Dexterity.Modifier);

        if (member.Features.Any(f => f.Index.Equals("jack-of-all-trades")) && !skills.Where(s => s.Index.Equals("skill-survival")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-survival")).First().Modifier == member.Wisdom.Modifier + (member.ProficiencyBonus / 2));
        else if (skills.Where(s => s.Index.Equals("skill-survival")).First().IsExpert)
            Assert.True(skills.Where(s => s.Index.Equals("skill-survival")).First().Modifier == member.Wisdom.Modifier + (2 * member.ProficiencyBonus));
        else if (skills.Where(s => s.Index.Equals("skill-survival")).First().IsProficient)
            Assert.True(skills.Where(s => s.Index.Equals("skill-survival")).First().Modifier == member.Wisdom.Modifier + member.ProficiencyBonus);
        else
            Assert.True(skills.Where(s => s.Index.Equals("skill-survival")).First().Modifier == member.Wisdom.Modifier);
    }

    #endregion

    #region Hit Points Tests

    [Theory]
    [MemberData(nameof(GetConstructorTestDataWithoutDwarvesAndSorcerers))]
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

        if (member.Class.Equals("sorcerer"))
            Assert.True(member.HitPoints <= (classMapper.Hp * level) + (member.Constitution.Modifier * level) + level * 2);
        else
            Assert.True(member.HitPoints <= (classMapper.Hp * level) + (member.Constitution.Modifier * level) + level);
    }

    [Theory]
    [MemberData(nameof(GetSorcererTestData))]
    public void HitPoints_ShouldBePositiveAndUnderTheMaxOnlySorcerer(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        // Assert
        Assert.True(member.HitPoints > 0);

        if (member.Race.Equals("dwarf"))
            Assert.True(member.HitPoints <= (classMapper.Hp * level) + (member.Constitution.Modifier * level) + level * 2);
        else
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

    #region Equipment Tests

    [Theory]
    [MemberData(nameof(GetConstructorTestDataLevel1Human))]
    public void Equipment_ShouldHaveAtLeastOneWeaponEquipped(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);

        // Assert
        Assert.True(member.MeleeWeapons.Any(w => w.IsEquipped) || member.RangedWeapons.Any(w => w.IsEquipped));
    }

    #endregion

    #region Armor Class Tests

    [Theory]
    [MemberData(nameof(GetBarbarianTestDataLevel1))]
    public void ArmorClassBarbarian_ShouldBeCalculatedCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);

        // Assert
        Assert.Equal(member.ArmorClass, 10 + member.Dexterity.Modifier + member.Constitution.Modifier);
    }

    [Theory]
    [MemberData(nameof(GetMonkTestDataLevel1))]
    public void ArmorClassMonk_ShouldBeCalculatedCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);

        // Assert
        Assert.Equal(member.ArmorClass, 10 + member.Dexterity.Modifier + member.Wisdom.Modifier);
    }

    [Theory]
    [MemberData(nameof(GetBardTestDataLevel1))]
    [MemberData(nameof(GetDruidTestDataLevel1))]
    [MemberData(nameof(GetRogueTestDataLevel1))]
    [MemberData(nameof(GetWarlockTestDataLevel1))]
    public void ArmorClassLeatherArmor_ShouldBeCalculatedCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);

        // Assert
        Assert.Equal(member.ArmorClass, 11 + member.Dexterity.Modifier);
    }

    [Theory]
    [MemberData(nameof(GetClericTestDataLevel1))]
    public void ArmorClassCleric_ShouldBeCalculatedCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);
        var shieldValue = member.Armors.Any(a => a.IsEquipped && a.Index.Equals("shield")) ? 2 : 0;

        // Assert
        Assert.True(member.ArmorClass == 11 + member.Dexterity.Modifier + shieldValue ||
            member.ArmorClass == 14 + Math.Min(2, (int)member.Dexterity.Modifier) + shieldValue ||
            (member.Strength.Value >= 13 && member.ArmorClass == 16 + shieldValue) || 
            member.ArmorClass == 10 + member.Dexterity.Modifier + shieldValue);
    }

    [Theory]
    [MemberData(nameof(GetFighterTestDataLevel1))]
    public void ArmorClassFighter_ShouldBeCalculatedCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);
        var shieldValue = member.Armors.Any(a => a.IsEquipped && a.Index.Equals("shield")) ? 2 : 0;
        var defenseValue = member.Features.Any(a => a.Index.Equals("fighter-fighting-style-defense")) ? 1 : 0;

        // Assert
        Assert.True(member.ArmorClass == 11 + member.Dexterity.Modifier + shieldValue + defenseValue ||
            (member.Strength.Value >= 13 && member.ArmorClass == 16 + shieldValue + defenseValue) ||
            member.ArmorClass == 10 + member.Dexterity.Modifier + shieldValue);
    }

    [Theory]
    [MemberData(nameof(GetPaladinTestDataLevel1))]
    public void ArmorClassPaladin_ShouldBeCalculatedCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);
        var shieldValue = member.Armors.Any(a => a.IsEquipped && a.Index.Equals("shield")) ? 2 : 0;
        var defenseValue = member.Features.Any(a => a.Index.Equals("fighting-style-defense")) ? 1 : 0;

        // Assert
        Assert.True(member.Strength.Value >= 13 && member.ArmorClass == 16 + shieldValue + defenseValue ||
            member.ArmorClass == 10 + member.Dexterity.Modifier + shieldValue + defenseValue);
    }

    [Theory]
    [MemberData(nameof(GetRangerTestDataLevel1))]
    public void ArmorClassRanger_ShouldBeCalculatedCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);
        var defenseValue = member.Features.Any(a => a.Index.Equals("ranger-fighting-style-defense")) ? 1 : 0;
        var shieldValue = member.Armors.Any(a => a.IsEquipped && a.Index.Equals("shield")) ? 2 : 0;

        // Assert
        Assert.True(member.ArmorClass == 11 + member.Dexterity.Modifier + shieldValue + defenseValue ||
            member.ArmorClass == 14 + Math.Min(2, (int)member.Dexterity.Modifier) + shieldValue + defenseValue);
    }

    [Theory]
    [MemberData(nameof(GetSorcererTestDataLevel1))]
    public void ArmorClassSorcerer_ShouldBeCalculatedCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);

        // Assert
        Assert.True(member.ArmorClass == 10 + member.Dexterity.Modifier ||
            member.ArmorClass == 13 + member.Dexterity.Modifier);
    }

    [Theory]
    [MemberData(nameof(GetWizardTestDataLevel1))]
    public void ArmorClassWizard_ShouldBeCalculatedCorrectly(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember((byte)index, level, race, classMapper);

        // Assert
        Assert.Equal(member.ArmorClass, 10 + member.Dexterity.Modifier);
    }

    #endregion

    #region Features Tests

    [Theory]
    [MemberData(nameof(GetBardTestData))]
    public void Bard_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(2, member.Features.Count); break;
            case 2: Assert.Equal(4, member.Features.Count); break;
            case 3: Assert.Equal(8, member.Features.Count); break;
            case 4: Assert.Equal(9, member.Features.Count); break;
            case 5: Assert.Equal(11, member.Features.Count); break;
            case 6: Assert.Equal(13, member.Features.Count); break;
            case 7: Assert.Equal(13, member.Features.Count); break;
            case 8: Assert.Equal(14, member.Features.Count); break;
            case 9: Assert.Equal(15, member.Features.Count); break;
            case 10: Assert.Equal(18, member.Features.Count); break;
            case 11: Assert.Equal(18, member.Features.Count); break;
            case 12: Assert.Equal(19, member.Features.Count); break;
            case 13: Assert.Equal(20, member.Features.Count); break;
            case 14: Assert.Equal(22, member.Features.Count); break;
            case 15: Assert.Equal(23, member.Features.Count); break;
            case 16: Assert.Equal(24, member.Features.Count); break;
            case 17: Assert.Equal(25, member.Features.Count); break;
            case 18: Assert.Equal(26, member.Features.Count); break;
            case 19: Assert.Equal(27, member.Features.Count); break;
            case 20: Assert.Equal(28, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetWarlockTestData))]
    public void Warlock_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(3, member.Features.Count); break;
            case 2: Assert.Equal(6, member.Features.Count); break;
            case 3: Assert.Equal(8, member.Features.Count); break;
            case 4: Assert.Equal(9, member.Features.Count); break;
            case 5: Assert.Equal(10, member.Features.Count); break;
            case 6: Assert.Equal(11, member.Features.Count); break;
            case 7: Assert.Equal(12, member.Features.Count); break;
            case 8: Assert.Equal(13, member.Features.Count); break;
            case 9: Assert.Equal(14, member.Features.Count); break;
            case 10: Assert.Equal(15, member.Features.Count); break;
            case 11: Assert.Equal(16, member.Features.Count); break;
            case 12: Assert.Equal(18, member.Features.Count); break;
            case 13: Assert.Equal(19, member.Features.Count); break;
            case 14: Assert.Equal(20, member.Features.Count); break;
            case 15: Assert.Equal(22, member.Features.Count); break;
            case 16: Assert.Equal(23, member.Features.Count); break;
            case 17: Assert.Equal(24, member.Features.Count); break;
            case 18: Assert.Equal(25, member.Features.Count); break;
            case 19: Assert.Equal(26, member.Features.Count); break;
            case 20: Assert.Equal(27, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetRangerTestData))]
    public void Ranger_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(2, member.Features.Count); break;
            case 2: Assert.Equal(5, member.Features.Count); break;
            case 3: Assert.Equal(9, member.Features.Count); break;
            case 4: Assert.Equal(10, member.Features.Count); break;
            case 5: Assert.Equal(11, member.Features.Count); break;
            case 6: Assert.Equal(13, member.Features.Count); break;
            case 7: Assert.Equal(15, member.Features.Count); break;
            case 8: Assert.Equal(17, member.Features.Count); break;
            case 9: Assert.Equal(17, member.Features.Count); break;
            case 10: Assert.Equal(19, member.Features.Count); break;
            case 11: Assert.Equal(21, member.Features.Count); break;
            case 12: Assert.Equal(22, member.Features.Count); break;
            case 13: Assert.Equal(22, member.Features.Count); break;
            case 14: Assert.Equal(24, member.Features.Count); break;
            case 15: Assert.Equal(26, member.Features.Count); break;
            case 16: Assert.Equal(27, member.Features.Count); break;
            case 17: Assert.Equal(27, member.Features.Count); break;
            case 18: Assert.Equal(28, member.Features.Count); break;
            case 19: Assert.Equal(29, member.Features.Count); break;
            case 20: Assert.Equal(30, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetRogueTestData))]
    public void Rogue_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(3, member.Features.Count); break;
            case 2: Assert.Equal(4, member.Features.Count); break;
            case 3: Assert.Equal(7, member.Features.Count); break;
            case 4: Assert.Equal(8, member.Features.Count); break;
            case 5: Assert.Equal(9, member.Features.Count); break;
            case 6: Assert.Equal(10, member.Features.Count); break;
            case 7: Assert.Equal(11, member.Features.Count); break;
            case 8: Assert.Equal(12, member.Features.Count); break;
            case 9: Assert.Equal(13, member.Features.Count); break;
            case 10: Assert.Equal(14, member.Features.Count); break;
            case 11: Assert.Equal(15, member.Features.Count); break;
            case 12: Assert.Equal(16, member.Features.Count); break;
            case 13: Assert.Equal(17, member.Features.Count); break;
            case 14: Assert.Equal(18, member.Features.Count); break;
            case 15: Assert.Equal(19, member.Features.Count); break;
            case 16: Assert.Equal(20, member.Features.Count); break;
            case 17: Assert.Equal(21, member.Features.Count); break;
            case 18: Assert.Equal(22, member.Features.Count); break;
            case 19: Assert.Equal(23, member.Features.Count); break;
            case 20: Assert.Equal(24, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetSorcererTestData))]
    public void Sorcerer_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(5, member.Features.Count); break;
            case 2: Assert.Equal(8, member.Features.Count); break;
            case 3: Assert.Equal(12, member.Features.Count); break;
            case 4: Assert.Equal(13, member.Features.Count); break;
            case 5: Assert.Equal(13, member.Features.Count); break;
            case 6: Assert.Equal(14, member.Features.Count); break;
            case 7: Assert.Equal(14, member.Features.Count); break;
            case 8: Assert.Equal(15, member.Features.Count); break;
            case 9: Assert.Equal(15, member.Features.Count); break;
            case 10: Assert.Equal(17, member.Features.Count); break;
            case 11: Assert.Equal(17, member.Features.Count); break;
            case 12: Assert.Equal(18, member.Features.Count); break;
            case 13: Assert.Equal(18, member.Features.Count); break;
            case 14: Assert.Equal(19, member.Features.Count); break;
            case 15: Assert.Equal(19, member.Features.Count); break;
            case 16: Assert.Equal(20, member.Features.Count); break;
            case 17: Assert.Equal(22, member.Features.Count); break;
            case 18: Assert.Equal(23, member.Features.Count); break;
            case 19: Assert.Equal(24, member.Features.Count); break;
            case 20: Assert.Equal(25, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetBarbarianTestData))]
    public void Barbarian_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(2, member.Features.Count); break;
            case 2: Assert.Equal(4, member.Features.Count); break;
            case 3: Assert.Equal(6, member.Features.Count); break;
            case 4: Assert.Equal(7, member.Features.Count); break;
            case 5: Assert.Equal(9, member.Features.Count); break;
            case 6: Assert.Equal(10, member.Features.Count); break;
            case 7: Assert.Equal(11, member.Features.Count); break;
            case 8: Assert.Equal(12, member.Features.Count); break;
            case 9: Assert.Equal(13, member.Features.Count); break;
            case 10: Assert.Equal(14, member.Features.Count); break;
            case 11: Assert.Equal(15, member.Features.Count); break;
            case 12: Assert.Equal(16, member.Features.Count); break;
            case 13: Assert.Equal(17, member.Features.Count); break;
            case 14: Assert.Equal(18, member.Features.Count); break;
            case 15: Assert.Equal(19, member.Features.Count); break;
            case 16: Assert.Equal(20, member.Features.Count); break;
            case 17: Assert.Equal(21, member.Features.Count); break;
            case 18: Assert.Equal(22, member.Features.Count); break;
            case 19: Assert.Equal(23, member.Features.Count); break;
            case 20: Assert.Equal(24, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetFighterTestData))]
    public void Fighter_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(3, member.Features.Count); break;
            case 2: Assert.Equal(4, member.Features.Count); break;
            case 3: Assert.Equal(6, member.Features.Count); break;
            case 4: Assert.Equal(7, member.Features.Count); break;
            case 5: Assert.Equal(8, member.Features.Count); break;
            case 6: Assert.Equal(9, member.Features.Count); break;
            case 7: Assert.Equal(10, member.Features.Count); break;
            case 8: Assert.Equal(11, member.Features.Count); break;
            case 9: Assert.Equal(12, member.Features.Count); break;
            case 10: Assert.Equal(14, member.Features.Count); break;
            case 11: Assert.Equal(15, member.Features.Count); break;
            case 12: Assert.Equal(16, member.Features.Count); break;
            case 13: Assert.Equal(17, member.Features.Count); break;
            case 14: Assert.Equal(18, member.Features.Count); break;
            case 15: Assert.Equal(19, member.Features.Count); break;
            case 16: Assert.Equal(20, member.Features.Count); break;
            case 17: Assert.Equal(22, member.Features.Count); break;
            case 18: Assert.Equal(23, member.Features.Count); break;
            case 19: Assert.Equal(24, member.Features.Count); break;
            case 20: Assert.Equal(25, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetClericTestData))]
    public void Cleric_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(5, member.Features.Count); break;
            case 2: Assert.Equal(8, member.Features.Count); break;
            case 3: Assert.Equal(9, member.Features.Count); break;
            case 4: Assert.Equal(10, member.Features.Count); break;
            case 5: Assert.Equal(12, member.Features.Count); break;
            case 6: Assert.Equal(14, member.Features.Count); break;
            case 7: Assert.Equal(15, member.Features.Count); break;
            case 8: Assert.Equal(18, member.Features.Count); break;
            case 9: Assert.Equal(19, member.Features.Count); break;
            case 10: Assert.Equal(20, member.Features.Count); break;
            case 11: Assert.Equal(21, member.Features.Count); break;
            case 12: Assert.Equal(22, member.Features.Count); break;
            case 13: Assert.Equal(22, member.Features.Count); break;
            case 14: Assert.Equal(23, member.Features.Count); break;
            case 15: Assert.Equal(23, member.Features.Count); break;
            case 16: Assert.Equal(24, member.Features.Count); break;
            case 17: Assert.Equal(26, member.Features.Count); break;
            case 18: Assert.Equal(27, member.Features.Count); break;
            case 19: Assert.Equal(28, member.Features.Count); break;
            case 20: Assert.Equal(29, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetDruidTestData))]
    public void Druid_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(2, member.Features.Count); break;
            case 2: Assert.Equal(8, member.Features.Count); break;
            case 3: Assert.Equal(9, member.Features.Count); break;
            case 4: Assert.Equal(11, member.Features.Count); break;
            case 5: Assert.Equal(12, member.Features.Count); break;
            case 6: Assert.Equal(13, member.Features.Count); break;
            case 7: Assert.Equal(14, member.Features.Count); break;
            case 8: Assert.Equal(16, member.Features.Count); break;
            case 9: Assert.Equal(17, member.Features.Count); break;
            case 10: Assert.Equal(18, member.Features.Count); break;
            case 11: Assert.Equal(18, member.Features.Count); break;
            case 12: Assert.Equal(19, member.Features.Count); break;
            case 13: Assert.Equal(19, member.Features.Count); break;
            case 14: Assert.Equal(20, member.Features.Count); break;
            case 15: Assert.Equal(20, member.Features.Count); break;
            case 16: Assert.Equal(21, member.Features.Count); break;
            case 17: Assert.Equal(21, member.Features.Count); break;
            case 18: Assert.Equal(23, member.Features.Count); break;
            case 19: Assert.Equal(24, member.Features.Count); break;
            case 20: Assert.Equal(25, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetPaladinTestData))]
    public void Paladin_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(2, member.Features.Count); break;
            case 2: Assert.Equal(6, member.Features.Count); break;
            case 3: Assert.Equal(12, member.Features.Count); break;
            case 4: Assert.Equal(13, member.Features.Count); break;
            case 5: Assert.Equal(14, member.Features.Count); break;
            case 6: Assert.Equal(15, member.Features.Count); break;
            case 7: Assert.Equal(16, member.Features.Count); break;
            case 8: Assert.Equal(17, member.Features.Count); break;
            case 9: Assert.Equal(17, member.Features.Count); break;
            case 10: Assert.Equal(18, member.Features.Count); break;
            case 11: Assert.Equal(19, member.Features.Count); break;
            case 12: Assert.Equal(20, member.Features.Count); break;
            case 13: Assert.Equal(20, member.Features.Count); break;
            case 14: Assert.Equal(21, member.Features.Count); break;
            case 15: Assert.Equal(22, member.Features.Count); break;
            case 16: Assert.Equal(23, member.Features.Count); break;
            case 17: Assert.Equal(23, member.Features.Count); break;
            case 18: Assert.Equal(24, member.Features.Count); break;
            case 19: Assert.Equal(25, member.Features.Count); break;
            case 20: Assert.Equal(26, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetWizardTestData))]
    public void Wizard_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(2, member.Features.Count); break;
            case 2: Assert.Equal(5, member.Features.Count); break;
            case 3: Assert.Equal(5, member.Features.Count); break;
            case 4: Assert.Equal(6, member.Features.Count); break;
            case 5: Assert.Equal(6, member.Features.Count); break;
            case 6: Assert.Equal(7, member.Features.Count); break;
            case 7: Assert.Equal(7, member.Features.Count); break;
            case 8: Assert.Equal(8, member.Features.Count); break;
            case 9: Assert.Equal(8, member.Features.Count); break;
            case 10: Assert.Equal(9, member.Features.Count); break;
            case 11: Assert.Equal(9, member.Features.Count); break;
            case 12: Assert.Equal(10, member.Features.Count); break;
            case 13: Assert.Equal(10, member.Features.Count); break;
            case 14: Assert.Equal(11, member.Features.Count); break;
            case 15: Assert.Equal(11, member.Features.Count); break;
            case 16: Assert.Equal(12, member.Features.Count); break;
            case 17: Assert.Equal(12, member.Features.Count); break;
            case 18: Assert.Equal(13, member.Features.Count); break;
            case 19: Assert.Equal(14, member.Features.Count); break;
            case 20: Assert.Equal(15, member.Features.Count); break;
        }
    }

    [Theory]
    [MemberData(nameof(GetMonkTestData))]
    public void Monk_NumberOfFeatures_ShouldMatchLevel(int index, byte level, string raceName, string className)
    {
        // Arrange
        var race = CreateTestRace(raceName);
        var classMapper = CreateTestClass(className);
        var member = new PartyMember(index, level, race, classMapper);

        switch (level)
        {
            case 1: Assert.Equal(2, member.Features.Count); break;
            case 2: Assert.Equal(7, member.Features.Count); break;
            case 3: Assert.Equal(10, member.Features.Count); break;
            case 4: Assert.Equal(12, member.Features.Count); break;
            case 5: Assert.Equal(14, member.Features.Count); break;
            case 6: Assert.Equal(16, member.Features.Count); break;
            case 7: Assert.Equal(18, member.Features.Count); break;
            case 8: Assert.Equal(19, member.Features.Count); break;
            case 9: Assert.Equal(20, member.Features.Count); break;
            case 10: Assert.Equal(21, member.Features.Count); break;
            case 11: Assert.Equal(22, member.Features.Count); break;
            case 12: Assert.Equal(23, member.Features.Count); break;
            case 13: Assert.Equal(24, member.Features.Count); break;
            case 14: Assert.Equal(25, member.Features.Count); break;
            case 15: Assert.Equal(26, member.Features.Count); break;
            case 16: Assert.Equal(27, member.Features.Count); break;
            case 17: Assert.Equal(28, member.Features.Count); break;
            case 18: Assert.Equal(29, member.Features.Count); break;
            case 19: Assert.Equal(30, member.Features.Count); break;
            case 20: Assert.Equal(31, member.Features.Count); break;
        }
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
    [MemberData(nameof(GetBarbarianTestDataLevel20))]
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