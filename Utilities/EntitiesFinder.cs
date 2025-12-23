using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Utilities;

public static class EntitiesFinder
{
    public static SubraceMapper GetEntityByIndex(List<SubraceMapper> subraceMappers, BaseEntity race, BaseEntity subrace)
    {
        return subraceMappers
            .Where(sr => sr.Index == subrace.Index && sr.Race.Index == race.Index)
            .Select(item => item)
            .First() ?? throw new Exception($"Subrace not found: {subrace.Index}");
    }

    public static TraitMapper GetEntityByIndex(List<TraitMapper> traitMappers, BaseEntity race, BaseEntity subrace, BaseEntity trait)
    {
        return traitMappers
            .Where(sr => sr.Index == trait.Index && (sr.Races.Select(item => item.Index).ToList().Contains(race.Index) || sr.Subraces.Select(item => item.Index).ToList().Contains(subrace.Index)))
            .Select(item => item)
            .FirstOrDefault() ?? throw new Exception($"Trait not found: {trait.Index}");
    }

    public static SpellMapper GetEntityByIndex(List<SpellMapper> spellMapper, BaseEntity spell)
    {
        return spellMapper
            .Where(sr => sr.Index == spell.Index)
            .Select(item => item)
            .First() ?? throw new Exception($"Spell not found: {spell.Index}");
    }

    public static SubclassMapper GetEntityByIndex(List<SubclassMapper> subclassMappers, BaseEntity cl, BaseEntity subclass)
    {
        return subclassMappers
            .Where(sr => sr.Index == subclass.Index && sr.Class.Index == cl.Index)
            .Select(item => item)
            .First() ?? throw new Exception($"Subclass not found: {subclass.Index}");
    }

    public static EquipmentMapper GetEntityByIndex(List<EquipmentMapper> equipmentMappers, BaseEntity equipment)
    {
        return equipmentMappers
            .Where(sr => sr.Index == equipment.Index)
            .Select(item => item)
            .FirstOrDefault() ?? throw new Exception($"Equipment not found: {equipment.Index}");
    }

    public static MonsterMapper GetEntityByIndex(List<MonsterMapper> monsterMappers, BaseEntity monster)
    {
        return monsterMappers
            .Where(sr => sr.Index == monster.Index)
            .Select(item => item)
            .FirstOrDefault() ?? throw new Exception($"Monster not found: {monster.Index}");
    }
}
