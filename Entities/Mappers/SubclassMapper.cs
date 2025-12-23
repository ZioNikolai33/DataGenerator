using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class SubclassMapper : BaseEntity
{
    [BsonElement("class")]
    public BaseEntity Class { get; set; } = new BaseEntity();
    [BsonElement("subclass_flavor")]
    public string SubclassFlavor { get; set; } = string.Empty;
    [BsonElement("spells")]
    public List<SubclassSpell> spells { get; set; } = new List<SubclassSpell>();
    [BsonIgnoreExtraElements]
    public class SubclassSpell
    {
        [BsonElement("prerequisites")]
        public List<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();
        [BsonElement("spell")]
        public BaseEntity Spell { get; set; } = new BaseEntity();
    }

    [BsonIgnoreExtraElements]
    public class Prerequisite : BaseEntity
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        public Prerequisite(string index, string name) : base(index, name) { }
    }

    public SubclassMapper(string index, string name) : base(index, name) { }
}
