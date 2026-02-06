using MongoDB.Driver;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Utilities;

public static class Lists
{
    public static List<MonsterMapper> monsters = new Database().GetAllMonsters();
    public static List<RaceMapper> races = new Database().GetAllRaces();
    public static List<SubraceMapper> subraces = new Database().GetAllSubraces();
    public static List<TraitMapper> traits = new Database().GetAllTraits();
    public static List<EquipmentMapper> weapons = new Database().GetAllWeapons();
    public static List<ClassMapper> classes = new Database().GetAllClasses();
    public static List<SubclassMapper> subclasses = new Database().GetAllSubclasses();
    public static List<SpellMapper> spells = new Database().GetAllSpells();
    public static List<FeatureMapper> features = new Database().GetAllFeatures();
    public static List<LevelMapper> levels = new Database().GetAllLevels();
    public static List<BaseEntity> meleeWeapons = new Database().GetAllMeleeWeapons();
    public static List<BaseEntity> rangedWeapons = new Database().GetAllRangedWeapons();
    public static List<BaseEntity> simpleWeapons = new Database().GetAllSimpleWeapons();
    public static List<BaseEntity> simpleMeleeWeapons = new Database().GetAllSimpleMeleeWeapons();
    public static List<BaseEntity> simpleRangedWeapons = new Database().GetAllSimpleRangedWeapons();
    public static List<BaseEntity> martialWeapons = new Database().GetAllMartialWeapons();
    public static List<BaseEntity> martialMeleeWeapons = new Database().GetAllMartialMeleeWeapons();
    public static List<BaseEntity> martialRangedWeapons = new Database().GetAllMartialRangedWeapons();
    public static List<BaseEntity> lightArmors = new Database().GetAllLightArmors();
    public static List<BaseEntity> mediumArmors = new Database().GetAllMediumArmors();
    public static List<BaseEntity> heavyArmors = new Database().GetAllHeavyArmors();
    public static List<BaseEntity> shields = new Database().GetAllShields();
    public static List<EquipmentMapper> equipments = new Database().GetAllEquipments();

    public static List<BaseEntity> GetEquipmentsList(string index)
    {
        return index switch
        {
            "melee-weapons" => meleeWeapons,
            "ranged-weapons" => rangedWeapons,
            "simple-weapons" => simpleWeapons,
            "simple-melee-weapons" => simpleMeleeWeapons,
            "simple-ranged-weapons" => simpleRangedWeapons,
            "martial-weapons" => martialWeapons,
            "martial-melee-weapons" => martialMeleeWeapons,
            "martial-ranged-weapons" => martialRangedWeapons,
            "light-armor" => lightArmors,
            "medium-armor" => mediumArmors,
            "heavy-armor" => heavyArmors,
            "shields" => shields,
            _ => new List<BaseEntity>()
        };
    }
}