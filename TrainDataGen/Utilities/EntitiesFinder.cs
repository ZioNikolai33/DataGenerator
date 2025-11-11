using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Utilities;

public static class EntitiesFinder
{
    public static SubraceMapper GetEntityByIndex(List<SubraceMapper> subraceMappers, BaseEntity subrace)
    {
        return subraceMappers
            .Where(sr => sr.Index == subrace.Index)
            .Select(item => item)
            .FirstOrDefault();
    }

    public static TraitsMapper GetEntityByIndex(List<TraitsMapper> traitMappers, BaseEntity trait)
    {
        return traitMappers
            .Where(sr => sr.Index == trait.Index)
            .Select(item => item)
            .FirstOrDefault();
    }
}
