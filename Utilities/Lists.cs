using MongoDB.Driver;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Utilities;

public static class Lists
{
    public readonly static List<MonsterMapper> monsters = new Database().GetAllMonsters();
    public readonly static List<RaceMapper> races = new Database().GetAllRaces();
    public readonly static List<SubraceMapper> subraces = new Database().GetAllSubraces();
    public readonly static List<TraitMapper> traits = new Database().GetAllTraits();
    public readonly static List<EquipmentMapper> weapons = new Database().GetAllWeapons();
    public readonly static List<ClassMapper> classes = new Database().GetAllClasses();
    public readonly static List<SubclassMapper> subclasses = new Database().GetAllSubclasses();
    public readonly static List<SpellMapper> spells = new Database().GetAllSpells();
    public readonly static List<FeatureMapper> features = new Database().GetAllFeatures();
    public readonly static List<LevelMapper> levels = new Database().GetAllLevels();
    public readonly static List<BaseEntity> meleeWeapons = new Database().GetAllMeleeWeapons();
    public readonly static List<BaseEntity> rangedWeapons = new Database().GetAllRangedWeapons();
    public readonly static List<BaseEntity> simpleWeapons = new Database().GetAllSimpleWeapons();
    public readonly static List<BaseEntity> simpleMeleeWeapons = new Database().GetAllSimpleMeleeWeapons();
    public readonly static List<BaseEntity> simpleRangedWeapons = new Database().GetAllSimpleRangedWeapons();
    public readonly static List<BaseEntity> martialWeapons = new Database().GetAllMartialWeapons();
    public readonly static List<BaseEntity> martialMeleeWeapons = new Database().GetAllMartialMeleeWeapons();
    public readonly static List<BaseEntity> martialRangedWeapons = new Database().GetAllMartialRangedWeapons();
    public readonly static List<BaseEntity> lightArmors = new Database().GetAllLightArmors();
    public readonly static List<BaseEntity> mediumArmors = new Database().GetAllMediumArmors();
    public readonly static List<BaseEntity> heavyArmors = new Database().GetAllHeavyArmors();
    public readonly static List<BaseEntity> shields = new Database().GetAllShields();
    public readonly static List<EquipmentMapper> equipments = new Database().GetAllEquipments();

    public static List<BaseEntity> GetEquipmentsList(string index)
    {
        return index switch
        {
            "melee-weapons" => meleeWeapons,
            "ranged-weapons" => rangedWeapons,
            "simple_weapons" => simpleWeapons,
            "simple_melee_weapons" => simpleMeleeWeapons,
            "simple-ranged-weapons" => simpleRangedWeapons,
            "martial_weapons" => martialWeapons,
            "martial_melee_weapons" => martialMeleeWeapons,
            "martial-ranged-weapons" => martialRangedWeapons,
            "light-armor" => lightArmors,
            "medium-armor" => mediumArmors,
            "heavy-armor" => heavyArmors,
            "shields" => shields,
            _ => new List<BaseEntity>()
        };
    }
}