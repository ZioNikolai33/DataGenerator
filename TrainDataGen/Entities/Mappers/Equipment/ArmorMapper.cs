using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers.Equipment;

public class ArmorMapper : EquipmentMapper
{
    [BsonElement("armor_category")]
    public string ArmorCategory { get; set; }
    [BsonElement("armor_class")]
    public ArmorData ArmorClass { get; set; }
    [BsonElement("str_minimum")]
    public byte StrengthMinimum { get; set; }
    [BsonElement("stealth_disadvantage")]
    public bool IsStealthDisadvantage { get; set; }

    public class ArmorData
    {
        [BsonElement("base")]
        public byte Base { get; set; }
        [BsonElement("dex_bonus")]
        public bool HasDexBonus { get; set; }
        [BsonElement("max_bonus")]
        public byte? MaxDexBonus { get; set; }
    }

    public ArmorMapper(string index, string name) : base(index, name) { }
}
