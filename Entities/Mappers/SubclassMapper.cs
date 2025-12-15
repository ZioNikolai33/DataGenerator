using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

[BsonIgnoreExtraElements]
public class SubclassMapper : BaseEntity
{
    [BsonElement("class")]
    public BaseEntity Class { get; set; }
    [BsonElement("subclass_flavor")]
    public string SubclassFlavor { get; set; }
    [BsonElement("spells")]
    public List<SubclassSpell> spells { get; set; }

    [BsonIgnoreExtraElements]
    public class SubclassSpell
    {
        [BsonElement("prerequisites")]
        public List<Prerequisite> Prerequisites { get; set; }
        [BsonElement("spell")]
        public BaseEntity Spell { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Prerequisite : BaseEntity
    {
        [BsonElement("type")]
        public string Type { get; set; }

        public Prerequisite(string index, string name) : base(index, name) { }
    }

    public SubclassMapper(string index, string name) : base(index, name) { }
}
