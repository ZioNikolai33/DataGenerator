using MongoDB.Bson.Serialization.Attributes;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class EquipmentMapper : BaseEntity
{
    [BsonElement("equipment_category")]
    public BaseEntity EquipmentCategory { get; set; } = new BaseEntity();
    [BsonElement("cost")]
    public CostMapper Cost { get; set; } = new CostMapper();
    [BsonElement("quantity")]
    public short? Quantity { get; set; }

    [BsonElement("weapon_category")]
    public string? WeaponCategory { get; set; }
    [BsonElement("weapon_range")]
    public string? WeaponRange { get; set; }
    [BsonElement("category_range")]
    public string? CategoryRange { get; set; }
    [BsonElement("damage")]
    public DamageData? Damage { get; set; }
    [BsonElement("range")]
    public RangeData? Range { get; set; }
    [BsonElement("properties")]
    public List<BaseEntity>? Properties { get; set; }
    [BsonElement("gear_category")]
    public BaseEntity? GearCategory { get; set; }
    [BsonElement("throw_range")]
    public RangeData? ThrowRange { get; set; }
    [BsonElement("two_handed_damage")]
    public DamageData? TwoHandedDamage { get; set; }

    [BsonElement("armor_category")]
    public string? ArmorCategory { get; set; }
    [BsonElement("armor_class")]
    public ArmorData? ArmorClass { get; set; }
    [BsonElement("str_minimum")]
    public byte? StrengthMinimum { get; set; }
    [BsonElement("stealth_disadvantage")]
    public bool? IsStealthDisadvantage { get; set; }

    [BsonIgnoreExtraElements]
    public class ArmorData
    {
        [BsonElement("base")]
        public byte Base { get; set; }
        [BsonElement("dex_bonus")]
        public bool HasDexBonus { get; set; }
        [BsonElement("max_bonus")]
        public byte? MaxDexBonus { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class DamageData
    {
        [BsonElement("damage_type")]
        public BaseEntity DamageType { get; set; } = new BaseEntity();
        [BsonElement("damage_dice")]
        public string DamageDice { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class RangeData
    {
        [BsonElement("normal")]
        public short Normal { get; set; }
        [BsonElement("long")]
        public short? Long { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CostMapper
    {
        [BsonElement("quantity")]
        public short Quantity { get; set; }
        [BsonElement("unit")]
        public string Unit { get; set; } = string.Empty;
    }

    public EquipmentMapper(string index, string name) : base(index, name) { }
}