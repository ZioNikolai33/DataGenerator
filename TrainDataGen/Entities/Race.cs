using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TrainDataGen.Entities.Enums;
using TrainDataGen.Entities.Mappers;
using TrainDataGen.Utilities;

namespace TrainDataGen.Entities;

public class Race: BaseEntity
{
    public Size Size { get; set; }
    public List<AbilityBonus> AbilityBonuses { get; set; }
    public List<Skills> Proficiencies { get; set; }
    public List<Trait> Traits { get; set; }
    public List<Subrace> Subraces { get; set; }
    public short Speed { get; set; }

    public Race(RaceMapper race) : base(race.Index, race.Name) {
        Index = race.Index;
        Name = race.Name;
        Speed = race.Speed;
        Size = race.Size;
        AbilityBonuses = race.AbilityBonuses;
        Proficiencies = (List<Skills>)race.StartingProficiences.Select(item => EntitiesMapper.FromStringSchool(item.Index));
        Subraces = race.Subraces.Select(item => new Subrace(item.Index, item.Name, EntitiesFinder.GetEntityByIndex(Lists.subraces, new BaseEntity(race.Index, race.Name), item))).ToList();
        Traits = race.Traits.Select(item => new Trait(item.Index, item.Name, EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(race.Index, race.Name), item))).ToList();

        AbilityBonuses.AddRange(race.GetRandomAbility());
        Proficiencies.AddRange(EntitiesMapper.FromStringMultipleSkills(race.StartingProficiencesOptions.GetRandomChoice(Proficiencies).Select(item => item.Index).ToList()));
    }
}