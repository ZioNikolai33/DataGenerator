using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class ProficiencyChoiceMapper
{
    [BsonElement("desc")]
    public string Desc { get; set; } = string.Empty;
    [BsonElement("choose")]
    public byte Choose { get; set; } = 0;
    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;
    [BsonElement("from")]
    public ProficiencyFrom From { get; set; } = new ProficiencyFrom();

    [BsonIgnoreExtraElements]
    public class ProficiencyFrom
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<ProficiencyOption> Options { get; set; } = new List<ProficiencyOption>();
    }

    [BsonIgnoreExtraElements]
    public class ProficiencyOption
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; } = string.Empty;
        [BsonElement("item")]
        public BaseEntity? Item { get; set; }
    }

    public List<string> GetRandomChoice(List<string>? proficiencies)
    {
        var random = Random.Shared;
        var selectedProficiencies = new List<string>();

        if (this.From.Options.All(item => item.Item != null))            
            if (proficiencies == null)
                selectedProficiencies = this.From.Options
                    .OrderBy(_ => random.Next())
                    .Take(Choose)
                    .Select(option => option.Item?.Index )
                    .Where(index => index != null)
                    .Select(index => index!)
                    .ToList();
            else {
                var availableOptions = this.From.Options
                    .Where(option => !proficiencies.Contains(option.Item?.Index ?? string.Empty))
                    .ToList();

                selectedProficiencies = availableOptions
                    .OrderBy(_ => random.Next())
                    .Take(Choose)
                    .Select(option => option.Item?.Index)
                    .Where(index => index != null)
                    .Select(index => index!)
                    .ToList();
            }

        return selectedProficiencies;
    }
}