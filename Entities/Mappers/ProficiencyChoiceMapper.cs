using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class ProficiencyChoiceMapper
{
    [BsonElement("desc")]
    public string Desc { get; set; }
    [BsonElement("choose")]
    public byte Choose { get; set; }
    [BsonElement("type")]
    public string Type { get; set; }
    [BsonElement("from")]
    public ProficiencyFrom From { get; set; }

    [BsonIgnoreExtraElements]
    public class ProficiencyFrom
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; }
        [BsonElement("options")]
        public List<ProficiencyOption> Options { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ProficiencyOption
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; }
        [BsonElement("item")]
        public BaseEntity? Item { get; set; }
    }

    public List<string> GetRandomChoice(List<string>? proficiencies)
    {
        var random = new Random();
        var selectedProficiencies = new List<string>();

        if (this.From.Options.All(item => item.Item != null))            
            if (proficiencies == null)
                selectedProficiencies = this.From.Options
                    .OrderBy(_ => random.Next())
                    .Take(Choose)
                    .Select(option => option.Item.Index)
                    .ToList();
            else {
                var availableOptions = this.From.Options
                    .Where(option => !proficiencies.Contains(option.Item.Index))
                    .ToList();

                selectedProficiencies = availableOptions
                    .OrderBy(_ => random.Next())
                    .Take(Choose)
                    .Select(option => option.Item.Index)
                    .ToList();
            }

        return selectedProficiencies;
    }
}