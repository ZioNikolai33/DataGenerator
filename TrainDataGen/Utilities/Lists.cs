using MongoDB.Driver;
using TrainDataGen.DataBase;
using TrainDataGen.Entities;
using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Utilities;

public static class Lists
{
    public readonly static List<Race> races = new Database().GetAllRaces();
    public readonly static List<Equipment> weapons = new Database().GetAllWeapons();
    public readonly static List<Class> classes = new Database().GetAllClasses();
    public readonly static List<BaseMapper> martialWeapons = new Database().GetAllMartialWeapons();
    public readonly static List<BaseMapper> martialMeleeWeapons = new Database().GetAllMartialMeleeWeapons();
    public readonly static List<BaseMapper> simpleWeapons = new Database().GetAllSimpleWeapons();
    public readonly static List<BaseMapper> simpleMeleeWeapons = new Database().GetAllSimpleMeleeWeapons();
    public readonly static List<string> weaponNames = weapons.Select(item => item.Name).ToList();

    public static List<BaseMapper> GetEquipmentsList(string index)
    {
        return index switch
        {
            "martial_weapons" => martialWeapons,
            "martial_melee_weapons" => martialMeleeWeapons,
            "simple_weapons" => simpleWeapons,
            "simple_melee_weapons" => simpleMeleeWeapons,
            _ => new List<BaseMapper>()
        };
    }
}