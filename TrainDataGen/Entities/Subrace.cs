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
        Proficiencies = (List<Skills>)subrace.StartingProficiencies.Select(item => EntitiesMapper.FromString(item.Index));
        Traits = (List<Trait>)subrace.RacialTraits.Select(item => item.Index);
    }
}
