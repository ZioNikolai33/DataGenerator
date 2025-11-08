using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

public class MultiClassing
{
    [BsonElement("prerequisites")]
    public List<Prerequisite>? Prerequisites { get; set; }
    [BsonElement("prerequisite_options")]
    public PrerequisiteOptions? PrerequisiteOptions { get; set; }
    [BsonElement("proficiencies")]
    public List<BaseMapper> Proficiencies { get; set; }
    [BsonElement("proficiency_choices")]
    public List<ProficiencyChoice>? ProficiencyChoices { get; set; }
}

public class Prerequisite
{
    [BsonElement("ability_score")]
    public BaseMapper AbilityScore { get; set; }
    [BsonElement("minimum_score")]
    public byte MinimumScore { get; set; }
}

public class PrerequisiteOptions
{
    [BsonElement("type")]
    public string Type { get; set; }
    [BsonElement("choose")]
    public byte Choose { get; set; }
    [BsonElement("from")]
    public PrerequisiteFrom From { get; set; }
}

public class PrerequisiteFrom
{
    [BsonElement("option_set_type")]
    public string OptionSetType { get; set; }
    [BsonElement("options")]
    public List<ScorePrerequisiteOption> Options { get; set; }
}

public class ScorePrerequisiteOption
{
    [BsonElement("option_type")]
    public string OptionType { get; set; }
    [BsonElement("ability_score")]
    public BaseMapper AbilityScore { get; set; }
    [BsonElement("minimum_score")]
    public byte MinimumScore { get; set; }
}

public class ProficiencyChoice
{
    [BsonElement("desc")]
    public string Desc { get; set; }
    [BsonElement("choose")]
    public byte Choose { get; set; }
    [BsonElement("type")]
    public string Type { get; set; }
    [BsonElement("from")]
    public ProficiencyFrom From { get; set; }
}

public class ProficiencyFrom
{
    [BsonElement("option_set_type")]
    public string OptionSetType { get; set; }
    [BsonElement("options")]
    public List<ProficiencyOption> Options { get; set; }
}

public class ProficiencyOption
{
    [BsonElement("option_type")]
    public string OptionType { get; set; }
    [BsonElement("item")]
    public BaseMapper Item { get; set; }
}
