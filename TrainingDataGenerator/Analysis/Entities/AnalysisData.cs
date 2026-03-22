using System.Data;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.PartyEntities;

namespace TrainingDataGenerator.Analysis.Entities;

public class AnalysisData
{
    public string Difficulty { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; }
    public string Size { get; set; } = string.Empty;
    public int HitPoints { get; set; }
    public byte ArmorClass { get; set; }
    public byte Strength { get; set; }
    public byte Dexterity { get; set; }
    public byte Constitution { get; set; }
    public byte Intelligence { get; set; }
    public byte Wisdom { get; set; }
    public byte Charisma { get; set; }
    public byte? Level { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string? Subrace { get; set; }
    public string Subclass { get; set; } = string.Empty;
    public double? ChallengeRating { get; set; }
    public int? Exp { get; set; }
    public byte PartyMemberCount { get; set; }
    public int BaseStatsPower { get; set; }
    public int OffensivePower { get; set; }
    public int HealingPower { get; set; }
    public string Result { get; set; } = string.Empty;
    public short TotalRounds { get; set; }

    public AnalysisData(CRRatios difficulty, string id, PartyMember member, Result result, int partyMemberCount)
    {
        Difficulty = difficulty.ToString();
        Id = id;
        Size = member.Size;
        HitPoints = member.HitPoints;
        PartyMemberCount = (byte)partyMemberCount;
        ArmorClass = member.ArmorClass;
        Strength = member.Strength.Value;
        Dexterity = member.Dexterity.Value;
        Constitution = member.Constitution.Value;
        Intelligence = member.Intelligence.Value;
        Wisdom = member.Wisdom.Value;
        Charisma = member.Charisma.Value;
        Level = member.Level;
        Class = member.Class;
        Race = member.Race;
        Subrace = member.Subrace;
        Subclass = member.Subclass;
        BaseStatsPower = member.BaseStats;
        OffensivePower = member.OffensivePower;
        HealingPower = member.HealingPower;
        Result = result.Outcome.ToString();
        TotalRounds = result.TotalRounds;
    }

    public AnalysisData(CRRatios difficulty, string id, Monster monster, Result result, int partyMemberCount)
    {
        Difficulty = difficulty.ToString();
        Id = id;
        Name = monster.Name;
        Size = monster.Size;
        HitPoints = monster.HitPoints;
        PartyMemberCount = (byte)partyMemberCount;
        ArmorClass = monster.ArmorClass;
        Strength = monster.Strength.Value;
        Dexterity = monster.Dexterity.Value;
        Constitution = monster.Constitution.Value;
        Intelligence = monster.Intelligence.Value;
        Wisdom = monster.Wisdom.Value;
        Charisma = monster.Charisma.Value;
        ChallengeRating = monster.ChallengeRating;
        Exp = monster.Xp;
        BaseStatsPower = monster.BaseStats;
        OffensivePower = monster.OffensivePower;
        HealingPower = monster.HealingPower;
        Result = result.Outcome.ToString();
        TotalRounds = result.TotalRounds;
    }
}
