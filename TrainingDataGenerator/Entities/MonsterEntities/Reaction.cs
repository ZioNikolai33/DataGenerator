using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.MonsterEntities;

public class Reaction
{
    public string Name { get; set; }
    public string Desc { get; set; }
    public int? AttackBonus { get; set; }
    public MonsterDC? Dc { get; set; }
    public List<Damage> Damage { get; set; } = new List<Damage>();

    public Reaction(MonsterMapper.Reaction reaction)
    {
        Name = reaction.Name;
        Desc = reaction.Desc;
        AttackBonus = reaction.AttackBonus;
        Dc = (reaction.Dc != null) ? new MonsterDC(reaction.Dc.DcType.Index, reaction.Dc.SuccessType, reaction.Dc.DcValue) : null;
        Damage = (reaction.Damage != null) ? reaction.Damage.Select(item => new Damage(item)).ToList() : new List<Damage>();
    }
}
