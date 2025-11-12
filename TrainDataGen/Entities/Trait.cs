using System.Diagnostics;
using TrainDataGen.Entities.Enums;
using TrainDataGen.Entities.Mappers;
using TrainDataGen.Utilities;

namespace TrainDataGen.Entities;

public class Trait: BaseEntity
{
    public List<BaseEntity> Races { get; set; }
    public List<BaseEntity> Subraces { get; set; }
    public List<Skills> Proficiencies { get; set; }
    public Trait? Parent { get; set; }
    public TraitSpecific? TraitSpec { get; set; }

    public Trait(string index, string name, TraitMapper trait) : base(index, name) {
        Index = trait.Index;
        Name = trait.Name;
        Races = trait.Races;
        Subraces = trait.Subraces;
        Proficiencies = EntitiesMapper.FromStringMultipleSkills(trait.Proficiencies.Select(item => item.Index).ToList());
        Parent = trait.Parent != null ? new Trait(trait.Parent.Index, trait.Parent.Name, EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(trait.Index, trait.Name), trait.Parent)) : null;
        TraitSpec = trait.TraitSpec != null ? new TraitSpecific(Index, Name, trait.TraitSpec) : null;

        Proficiencies.AddRange(EntitiesMapper.FromStringMultipleSkills(trait.ProficiencyChoice.GetRandomChoice(Proficiencies).Select(item => item.Index).ToList()));
    }
}
