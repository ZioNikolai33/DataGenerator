using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Entities.MonsterEntities;

public class NormalAction
{
    public string Name { get; set; }
    public string Desc { get; set; }
    public List<MultiAction> Actions { get; set; }
    public byte? AttackBonus { get; set; }
    public List<Damage> Damage { get; set; }
    public MonsterDC? Dc { get; set; }
    public Usage? Usage { get; set; }

    public class MultiAction
    {
        public string ActionName { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public NormalAction(MonsterMapper.NormalAction action)
    {
        Name = action.Name;
        Desc = action.Desc;
        Actions = action.Actions?.Select(item => new MultiAction
        {
            ActionName = item.ActionName,
            Count = (item.Count != null) ? (item.Count is string ? -1 : (int)item.Count) : 0,
            Type = item.Type
        }).ToList() ?? new List<MultiAction>();
        AttackBonus = action.AttackBonus != null ? (byte?)action.AttackBonus : null;
        Damage = action.Damage?.Select(item => new Damage(item)).ToList() ?? new List<Damage>();
        Dc = new MonsterDC(action.Dc.DcType.Index, action.Dc.SuccessType, action.Dc.DcValue);
        Usage = (action.Usage != null) ? new Usage(action.Usage.Type, action.Usage.Times, action.Usage.RestTypes, action.Usage.Dice, action.Usage.MinValue) : null;

        if (action.ActionOptions != null && action.ActionOptions.From.Options.Count > 0)
        {
            var random = Random.Shared;
            var choose = action.ActionOptions.Choose;

            var selectedOptions = action.ActionOptions.From.Options.OrderBy(_ => random.Next()).Take(choose).ToList();

            foreach (var option in selectedOptions)
            {
                if (option.OptionType.Equals("multiple", StringComparison.OrdinalIgnoreCase))
                {
                    Actions.AddRange(option.Items.Select(item => new MultiAction
                    {
                        ActionName = item.ActionName,
                        Count = (item.Count != null) ? (int)item.Count : 0,
                        Type = item.Type
                    }).ToList());
                }
                else
                {
                    Actions.Add(new MultiAction
                    {
                        ActionName = option.ActionName,
                        Count = (option.Count != null) ? (int)option.Count : 0,
                        Type = option.Type
                    });
                }
            }
        }
    }
}
