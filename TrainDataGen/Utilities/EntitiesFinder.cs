using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Utilities;

public static class EntitiesFinder
{
    public static SubraceMapper GetEntityByIndex(List<SubraceMapper> subraceMappers, BaseEntity race, BaseEntity subrace)
    {
        return subraceMappers
            .Where(sr => sr.Index == subrace.Index && sr.Race == race)
            .Select(item => item)
            .FirstOrDefault();
    }

    public static TraitMapper GetEntityByIndex(List<TraitMapper> traitMappers, BaseEntity race, BaseEntity trait)
    {
        return traitMappers
            .Where(sr => sr.Index == trait.Index && sr.Races.Contains(race))
            .Select(item => item)
            .FirstOrDefault();
    }

    public static SpellMapper GetEntityByIndex(List<SpellMapper> spellMapper, BaseEntity spell)
    {
        return spellMapper
            .Where(sr => sr.Index == spell.Index)
            .Select(item => item)
            .FirstOrDefault();
    }

    public static SubclassMapper GetEntityByIndex(List<SubclassMapper> subclassMappers, BaseEntity cl, BaseEntity subclass)
    {
        return subclassMappers
            .Where(sr => sr.Index == subclass.Index && sr.Class == cl)
            .Select(item => item)
            .FirstOrDefault();
    }

    public static EquipmentMapper GetEntityByIndex(List<EquipmentMapper> equipmentMappers, BaseEntity equipment)
    {
        return equipmentMappers
            .Where(sr => sr.Index == equipment.Index)
            .Select(item => item)
            .FirstOrDefault();
    }
}
