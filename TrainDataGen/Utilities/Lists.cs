using MongoDB.Driver;
using TrainDataGen.DataBase;
using TrainDataGen.Entities;
using TrainDataGen.Entities.Mappers;
using TrainDataGen.Entities.Mappers.Equipment;

namespace TrainDataGen.Utilities;

public static class Lists
{
    public readonly static List<RaceMapper> races = new Database().GetAllRaces();
    public readonly static List<SubraceMapper> subraces = new Database().GetAllSubraces();
    public readonly static List<TraitMapper> traits = new Database().GetAllTraits();
    public readonly static List<EquipmentMapper> weapons = new Database().GetAllWeapons();
    public readonly static List<ClassMapper> classes = new Database().GetAllClasses();
    public readonly static List<SpellMapper> spells = new Database().GetAllSpells();
    public readonly static List<BaseEntity> martialWeapons = new Database().GetAllMartialWeapons();
    public readonly static List<BaseEntity> martialMeleeWeapons = new Database().GetAllMartialMeleeWeapons();
    public readonly static List<BaseEntity> simpleWeapons = new Database().GetAllSimpleWeapons();
    public readonly static List<BaseEntity> simpleMeleeWeapons = new Database().GetAllSimpleMeleeWeapons();
    public readonly static List<string> weaponNames = weapons.Select(item => item.Name).ToList();

    public static List<BaseEntity> GetEquipmentsList(string index)
    {
        return index switch
        {
            "martial_weapons" => martialWeapons,
            "martial_melee_weapons" => martialMeleeWeapons,
            "simple_weapons" => simpleWeapons,
            "simple_melee_weapons" => simpleMeleeWeapons,
            _ => new List<BaseEntity>()
        };
    }
}