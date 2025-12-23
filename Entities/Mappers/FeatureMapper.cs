using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class FeatureMapper: BaseEntity
{
    [BsonElement("class")]
    public BaseEntity Class { get; set; } = new BaseEntity();
    [BsonElement("subclass")]
    public BaseEntity? Subclass { get; set; }
    [BsonElement("level")]
    public byte Level { get; set; }
    [BsonElement("prerequisites")]
    public List<FeaturePrerequisite> Prerequisites { get; set; } = new List<FeaturePrerequisite>();
    [BsonElement("desc")]
    public List<string> Desc { get; set; } = new List<string>();
    [BsonElement("feature_specific")]
    public FeatureSpecific? FeatureSpec { get; set; }
    [BsonElement("parent")]
    public BaseEntity? Parent { get; set; }

    [BsonIgnoreExtraElements]
    public class FeatureSpecific
    {
        [BsonElement("expertise_options")]
        public ExpertiseOptions? ExpertiseOptions { get; set; }
        [BsonElement("enemy_type_options")]
        public EnemyTypeOptions? EnemyTypeOptions { get; set; }
        [BsonElement("terrain_type_options")]
        public TerrainTypeOptions? TerrainTypeOptions { get; set; }
        [BsonElement("subfeature_options")]
        public SubfeatureOptions? SubfeatureOptions { get; set; }
        [BsonElement("invocations")]
        public List<BaseEntity>? Invocations { get; set; }

        public List<string> GetRandomChoice(List<string> proficiencies)
        {
            if (ExpertiseOptions != null)
                return ExpertiseOptions.GetRandomChoice(proficiencies);
            else if (EnemyTypeOptions != null)
                return EnemyTypeOptions.GetRandomChoice();
            else if (TerrainTypeOptions != null)
                return TerrainTypeOptions.GetRandomChoice();
            else if (SubfeatureOptions != null)
                return SubfeatureOptions.GetRandomChoice();

            return new List<string>();
        }
    }

    [BsonIgnoreExtraElements]
    public class ExpertiseOptions
    {
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("from")]
        public OptionSet From { get; set; } = new OptionSet();

        public List<string> GetRandomChoice(List<string> proficiencies)
        {
            var random = new Random();
            var selectedOptions = new List<string>();
            var selectableOptions = From.Options
                .Where(o => proficiencies.Any(p => o.Item != null && o.Item.Item != null && p == o.Item.Item.Index))
                .ToList();

            var selected = selectableOptions
                .OrderBy(x => random.Next())
                .Take(Choose)
                .ToList();

            foreach (var item in selected)
                if (item.Choice != null)
                    selectedOptions.AddRange(item.Choice.GetRandomChoice());
                else if (item.Items != null)
                    foreach (var subItem in item.Items)
                        if (subItem.Choice != null)
                            selectedOptions.AddRange(subItem.Choice.GetRandomChoice());
                        else if (subItem.Item != null)
                            selectedOptions.Add(subItem.Item.Item.Index);

            return selectedOptions;
        }
    }

    [BsonIgnoreExtraElements]
    public class EnemyTypeOptions
    {
        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("from")]
        public OptionSetString From { get; set; } = new OptionSetString();

        public List<string> GetRandomChoice()
        {
            var random = new Random();

            var selectedOptions = From.Options
                .OrderBy(x => random.Next())
                .Take(Choose)
                .ToList();

            return selectedOptions;
        }
    }

    [BsonIgnoreExtraElements]
    public class TerrainTypeOptions
    {
        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("from")]
        public OptionSetString From { get; set; } = new OptionSetString();

        public List<string> GetRandomChoice()
        {
            var random = new Random();

            var selectedOptions = From.Options
                .OrderBy(x => random.Next())
                .Take(Choose)
                .ToList();

            return selectedOptions;
        }
    }

    [BsonIgnoreExtraElements]
    public class SubfeatureOptions
    {
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("from")]
        public OptionSetReference From { get; set; } = new OptionSetReference();

        public List<string> GetRandomChoice()
        {
            var random = new Random();

            var selectedOptions = From.Options
                .OrderBy(x => random.Next())
                .Take(Choose)
                .Select(item => item.Item.Index)
                .ToList();
            return selectedOptions;
        }
    }

    [BsonIgnoreExtraElements]
    public class OptionSet
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<Option> Options { get; set; } = new List<Option>();
    }

    [BsonIgnoreExtraElements]
    public class OptionSetString
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<string> Options { get; set; } = new List<string>();
    }

    [BsonIgnoreExtraElements]
    public class OptionSetReference
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<OptionReference> Options { get; set; } = new List<OptionReference>();
    }

    [BsonIgnoreExtraElements]
    public class Option
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; } = string.Empty;
        [BsonElement("choice")]
        public Choice? Choice { get; set; }
        [BsonElement("items")]
        public List<Option>? Items { get; set; }
        [BsonElement("item")]
        public OptionReference? Item { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Choice
    {
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("from")]
        public OptionSet From { get; set; } = new OptionSet();

        public List<string> GetRandomChoice()
        {
            var random = new Random();

            var selectedOptions = From.Options
                .OrderBy(x => random.Next())
                .Take(Choose)
                .Select(item => item.Item?.Item.Index)
                .Where(index => index != null)
                .Select(index => index!)
                .ToList();

            return selectedOptions;
        }
    }

    [BsonIgnoreExtraElements]
    public class OptionReference
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; } = string.Empty;
        [BsonElement("item")]
        public BaseEntity Item { get; set; } = new BaseEntity();
    }

    [BsonIgnoreExtraElements]
    public class FeaturePrerequisite
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("level")]
        public byte? Level { get; set; }
        [BsonElement("feature")]
        public string? Feature { get; set; }
        [BsonElement("spell")]
        public string? Spell { get; set; }
    }

    public FeatureMapper(string index, string name) : base(index, name) { }
}