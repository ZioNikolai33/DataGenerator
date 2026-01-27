using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.MonsterEntities;

public class LegendaryAction
{
    public string Name { get; set; }
    public string Desc { get; set; }
    public int? AttackBonus { get; set; }
    public MonsterDC? Dc { get; set; }
    public List<Damage>? Damage { get; set; }

    public LegendaryAction(MonsterMapper.LegendaryAction action)
    {
        Name = action.Name;
        Desc = action.Desc;
        AttackBonus = action.AttackBonus;
        Dc = (action.Dc != null) ? new MonsterDC(action.Dc.DcType.Index, action.Dc.SuccessType, action.Dc.DcValue) : null;
        Damage = (action.Damage != null) ? action.Damage.Select(item => new Damage(item)).ToList() : null;
    }
}
