using TrainDataGen.Entities.Enums;
using TrainDataGen.Entities.Mappers;
using TrainDataGen.Utilities;

namespace TrainDataGen.Entities;

public class Race: BaseEntity
{
    public Size Size { get; set; }
    public short Speed { get; set; }

    public Race(RaceMapper race) : base(race.Index, race.Name) {
        Index = race.Index;
        Name = race.Name;
        Speed = race.Speed;
        Size = race.Size;
    }
}