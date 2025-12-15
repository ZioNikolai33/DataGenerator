using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class SpellMapper: BaseEntity
{
    [BsonElement("desc")]
    public List<string> Desc { get; set; }
    [BsonElement("higher_level")]
    public List<string>? HigherLevel { get; set; }
    [BsonElement("range")]
    public string Range { get; set; }
    [BsonElement("duration")]
    public string Duration { get; set; }
    [BsonElement("concentration")]
    public bool Concentration { get; set; }
    [BsonElement("casting_time")]
    public string CastingTime { get; set; }
    [BsonElement("level")]
    public byte Level { get; set; }
    [BsonElement("attack_type")]
    public string? AttackType { get; set; }
    [BsonElement("damage")]
    public DamageInfo? Damage { get; set; }
    [BsonElement("heal_at_slot_level")]
    public HealInfo? HealAtSlotLevel { get; set; }
    [BsonElement("dc")]
    public DifficultyClass? Dc { get; set; }
    [BsonElement("area_of_effect")]
    public Area? AreaOfEffect { get; set; }
    [BsonElement("school")]
    public BaseEntity School { get; set; }
    [BsonElement("classes")]
    public List<BaseEntity> Classes { get; set; }
    [BsonElement("subclasses")]
    public List<BaseEntity> Subclasses { get; set; }

    [BsonIgnoreExtraElements]
    public class DamageInfo
    {
        [BsonElement("damage_type")]
        public BaseEntity damage_type { get; set; }
        [BsonElement("damage_at_slot_level")]
        public Dictionary<string, string>? damage_at_slot_level { get; set; }
        [BsonElement("damage_at_character_level")]
        public Dictionary<string, string>? damage_at_character_level { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class HealInfo : Dictionary<string, string>
    {
        // e.g. { "1": "1d4 + 4", "2": "1d4 + 9", ... }
    }

    public SpellMapper(string index, string name) : base(index, name) { }
}