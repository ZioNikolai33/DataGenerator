using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class LevelMapper
{
    [BsonElement("index")]
    public string Index { get; set; }
    [BsonElement("level")]
    public byte Level { get; set; }
    [BsonElement("ability_score_bonuses")]
    public byte? AbilityScoreBonuses { get; set; }
    [BsonElement("prof_bonus")]
    public byte? ProfBonus { get; set; }
    [BsonElement("features")]
    public List<BaseEntity> Features { get; set; }
    [BsonElement("spellcasting")]
    public SpellcastingInfo? Spellcasting { get; set; }
    [BsonElement("class_specific")]
    public Dictionary<string, object>? ClassSpecific { get; set; }
    [BsonElement("subclass_specific")]
    public Dictionary<string, object>? SubclassSpecific { get; set; }
    [BsonElement("class")]
    public BaseEntity Class { get; set; }
    [BsonElement("subclass")]
    public BaseEntity? Subclass { get; set; }

    [BsonIgnoreExtraElements]
    public class SpellcastingInfo
    {
        [BsonElement("cantrips_known")]
        public byte? CantripsKnown { get; set; }
        [BsonElement("spells_known")]
        public byte? SpellsKnown { get; set; }
        [BsonElement("spell_slots_level_1")]
        public byte? SpellSlotsLevel1 { get; set; }
        [BsonElement("spell_slots_level_2")]
        public byte? SpellSlotsLevel2 { get; set; }
        [BsonElement("spell_slots_level_3")]
        public byte? SpellSlotsLevel3 { get; set; }
        [BsonElement("spell_slots_level_4")]
        public byte? SpellSlotsLevel4 { get; set; }
        [BsonElement("spell_slots_level_5")]
        public byte? SpellSlotsLevel5 { get; set; }
        [BsonElement("spell_slots_level_6")]
        public byte? SpellSlotsLevel6 { get; set; }
        [BsonElement("spell_slots_level_7")]
        public byte? SpellSlotsLevel7 { get; set; }
        [BsonElement("spell_slots_level_8")]
        public byte? SpellSlotsLevel8 { get; set; }
        [BsonElement("spell_slots_level_9")]
        public byte? SpellSlotsLevel9 { get; set; }
    }
}