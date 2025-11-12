using TrainDataGen.Entities.Enums;
using TrainDataGen.Entities.Mappers;
using TrainDataGen.Utilities;

namespace TrainDataGen.Entities;

public class Subrace: BaseEntity
{
    public BaseEntity Race { get; set; }
    public List<AbilityBonus> AbilityBonuses { get; set; }
    public List<Skills> Proficiencies { get; set; }
    public List<Trait> Traits { get; set; }

    public Subrace(string index, string name, SubraceMapper subrace) : base(index, name) {
        Index = subrace.Index;
        Name = subrace.Name;
        Race = subrace.Race;
        AbilityBonuses = subrace.AbilityBonuses;
        Proficiencies = EntitiesMapper.FromStringMultipleSkills(subrace.StartingProficiencies.Select(item => item.Index).ToList());
        Traits = subrace.RacialTraits.Select(item => new Trait(item.Index, item.Name, EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(subrace.Index, subrace.Name), item))).ToList();
    }
}
