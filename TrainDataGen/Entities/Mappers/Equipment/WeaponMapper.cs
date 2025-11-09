using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers.Equipment;

public class WeaponMapper : EquipmentMapper
{
    [BsonElement("weapon_category")]
    public string WeaponCategory { get; set; }
    [BsonElement("weapon_range")]
    public string WeaponRange { get; set; }
    [BsonElement("category_range")]
    public string CategoryRange { get; set; }
    [BsonElement("damage")]
    public DamageData Damage { get; set; }
    [BsonElement("range")]
    public RangeData Range { get; set; }
    [BsonElement("properties")]
    public List<BaseEntity> Properties { get; set; }
    [BsonElement("throw_range")]
    public RangeData? ThrowRange { get; set; }
    [BsonElement("two_handed_damage")]
    public DamageData? TwoHandedDamage { get; set; }


    public class DamageData
    {
        [BsonElement("damage_type")]
        public BaseEntity DamageType { get; set; }
        [BsonElement("damage_dice")]
        public string DamageDice { get; set; }
    }

    public class RangeData
    {
        [BsonElement("normal")]
        public short Normal { get; set; }
        [BsonElement("long")]
        public short? Long { get; set; }
    }

    public WeaponMapper(string index, string name) : base(index, name) { }
}
