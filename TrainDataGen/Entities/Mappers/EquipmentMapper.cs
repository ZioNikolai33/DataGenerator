using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

public class EquipmentMapper : BaseMapper
{
    [BsonElement("equipment_category")]
    public BaseMapper EquipmentCategory { get; set; }
    [BsonElement("weapon_category")]
    public string? WeaponCategory { get; set; }
    [BsonElement("weapon_range")]
    public string? WeaponRange { get; set; }
    [BsonElement("category_range")]
    public string CategoryRange { get; set; }
    [BsonElement("cost")]
    public CostMapper Cost { get; set; }
    [BsonElement("damage")]
    public DamageData? Damage { get; set; }
    [BsonElement("range")]
    public DamageData? DamageType { get; set; }
    [BsonElement("properties")]
    public List<BaseMapper> Properties { get; set; }

    public class CostMapper
    {
        [BsonElement("quantity")]
        public byte Quantity { get; set; }
        [BsonElement("unit")]
        public string Unit { get; set; }
    }

    public class DamageData
    {
        public string DamageDice { get; set; }
        public BaseMapper DamageType { get; set; }
    }

    public class RangeData
    {
        public byte Normal { get; set; }
    }

    public EquipmentMapper(string index, int quantity, List<WeaponData> weapons)
    {
        Name = index;
        Quantity = quantity;

        var weapon = weapons.FirstOrDefault(item => item.Index == Name);
        if (weapon == null)
            throw new KeyNotFoundException($"Weapon with index '{Name}' not found.");

        EquipmentCategory = weapon.EquipmentCategory;
        WeaponCategory = weapon.WeaponCategory;
        WeaponRange = weapon.WeaponRange;
        CategoryRange = weapon.CategoryRange;
        Cost = weapon.Cost.Quantity;
        CostUnit = weapon.Cost.Unit;
        Damage = weapon.Damage?.DamageDice;
        DamageType = weapon.Damage?.DamageType?.Index;
        RangeNormal = weapon.Range?.Normal;
        Properties = weapon.Properties?.Select(p => p.Index).ToList() ?? new List<string>();
    }

    public override string ToString()
    {
        return Name;
    }
}