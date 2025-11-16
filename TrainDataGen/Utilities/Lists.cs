using MongoDB.Driver;
using TrainDataGen.DataBase;
using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Utilities;

public static class Lists
{
    public readonly static List<RaceMapper> races = new Database().GetAllRaces();
    public readonly static List<SubraceMapper> subraces = new Database().GetAllSubraces();
    public readonly static List<TraitMapper> traits = new Database().GetAllTraits();
    public readonly static List<EquipmentMapper> weapons = new Database().GetAllWeapons();
    public readonly static List<ClassMapper> classes = new Database().GetAllClasses();
    public readonly static List<SubclassMapper> subclasses = new Database().GetAllSubclasses();
    public readonly static List<SpellMapper> spells = new Database().GetAllSpells();
    public readonly static List<FeatureMapper> features = new Database().GetAllFeatures();
    public readonly static List<LevelMapper> levels = new Database().GetAllLevels();
    public readonly static List<EquipmentMapper> martialWeapons = new Database().GetAllMartialWeapons();
    public readonly static List<EquipmentMapper> martialMeleeWeapons = new Database().GetAllMartialMeleeWeapons();
    public readonly static List<EquipmentMapper> simpleWeapons = new Database().GetAllSimpleWeapons();
    public readonly static List<EquipmentMapper> simpleMeleeWeapons = new Database().GetAllSimpleMeleeWeapons();
    public readonly static List<EquipmentMapper> equipments = new Database().GetAllEquipments();
    public readonly static List<string> weaponNames = weapons.Select(item => item.Name).ToList();

    public static List<EquipmentMapper> GetEquipmentsList(string index)
    {
        return index switch
        {
            "martial_weapons" => martialWeapons,
            "martial_melee_weapons" => martialMeleeWeapons,
            "simple_weapons" => simpleWeapons,
            "simple_melee_weapons" => simpleMeleeWeapons,
            _ => new List<EquipmentMapper>()
        };
    }
}