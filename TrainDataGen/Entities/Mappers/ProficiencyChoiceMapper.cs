using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

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
        public ProficiencyItem? Item { get; set; }
    }

    public class ProficiencyItem
    {
        [BsonElement("index")]
        public string Index { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
    }

    public List<BaseMapper> GetRandomChoice(List<BaseMapper>? proficiencies)
    {
        var random = new Random();
        var selectedProficiencies = new List<BaseMapper>();

        if (this.From.Options.All(item => item.Item != null))            
            if (proficiencies == null)
                selectedProficiencies = this.From.Options
                    .OrderBy(_ => random.Next())
                    .Take(Choose)
                    .Select(option => new BaseMapper(option.Item.Index, option.Item.Name))
                    .ToList();
            else {
                var availableOptions = this.From.Options
                    .Where(option => !proficiencies.Select(item => item.Index).Contains(option.Item.Index))
                    .ToList();

                selectedProficiencies = availableOptions
                    .OrderBy(_ => random.Next())
                    .Take(Choose)
                    .Select(option => new BaseMapper(option.Item.Index, option.Item.Name))
                    .ToList();
            }

        return selectedProficiencies;
    }
}