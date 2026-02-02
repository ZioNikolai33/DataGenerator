using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.Equip;

public class Armor : Equipment
{
    public string ArmorCategory { get; set; }
    public ArmorData ArmorClass { get; set; }
    public byte StrengthMinimum { get; set; }
    public bool IsStealthDisadvantage { get; set; }

    public class ArmorData
    {
        public byte Base { get; set; }
        public bool HasDexBonus { get; set; }
        public byte? MaxDexBonus { get; set; }
    }

    public Armor(EquipmentMapper equipment) : base(equipment)
    {
        ArmorCategory = equipment.ArmorCategory ?? "unknown";
        ArmorClass = new ArmorData
        {
            Base = equipment.ArmorClass?.Base ?? 10,
            HasDexBonus = equipment.ArmorClass?.HasDexBonus ?? false,
            MaxDexBonus = equipment.ArmorClass?.MaxDexBonus
        };
        StrengthMinimum = equipment.StrengthMinimum ?? 0;
        IsStealthDisadvantage = equipment.IsStealthDisadvantage ?? false;
    }
}
