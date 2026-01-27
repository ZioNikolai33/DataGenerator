using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.MonsterEntities;

public class Damage
{
    public string Type { get; set; } = string.Empty;
    public string DamageType { get; set; } = string.Empty;
    public string DamageDice { get; set; } = string.Empty;
    public MonsterDC? Dc { get; set; }
    public byte? Choose { get; set; }
    public DamageOptionSet From { get; set; } = new DamageOptionSet(new MonsterMapper.DamageOptionSet());

    public class DamageOptionSet
    {
        public string Option_Set_Type { get; set; }
        public List<DamageOption> Options { get; set; }

        public DamageOptionSet(MonsterMapper.DamageOptionSet damage)
        {
            Option_Set_Type = damage.Option_Set_Type;
            Options = damage.Options.Select(item => new DamageOption
            {
                Option_Type = item.Option_Type,
                Notes = item.Notes,
                Damage_Type = item.Damage_Type.Index,
                Damage_Dice = item.Damage_Dice
            }).ToList();
        }
    }

    public class DamageOption
    {
        public string Desc { get; set; } = string.Empty;
        public string Option_Type { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Damage_Type { get; set; } = string.Empty;
        public string Damage_Dice { get; set; } = string.Empty;
    }

    public Damage(MonsterMapper.Damage? damage)
    {
        if (damage != null)
        {
            Type = damage.Type;
            DamageType = damage.DamageType?.Index ?? string.Empty;
            DamageDice = damage.DamageDice;
            Dc = (damage.Dc != null) ? new MonsterDC(damage.Dc.DcType.Index, damage.Dc.SuccessType, damage.Dc.DcValue) : null;
            Choose = damage.Choose != null ? (byte?)damage.Choose : null;
            From = damage.From != null ? new DamageOptionSet(damage.From) : new DamageOptionSet(new MonsterMapper.DamageOptionSet());
        }
    }
}
