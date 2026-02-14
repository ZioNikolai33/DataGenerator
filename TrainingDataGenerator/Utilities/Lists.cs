using MongoDB.Driver;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.Utilities;

public static class Lists
{
    public readonly static List<MonsterMapper> monsters = new Database().GetAllMonsters().OrderBy(m => m.Name).ToList();
    public readonly static List<RaceMapper> races = new Database().GetAllRaces().OrderBy(r => r.Name).ToList();
    public readonly static List<SubraceMapper> subraces = new Database().GetAllSubraces().OrderBy(s => s.Name).ToList();
    public readonly static List<TraitMapper> traits = new Database().GetAllTraits().OrderBy(t => t.Name).ToList();
    public readonly static List<EquipmentMapper> weapons = new Database().GetAllWeapons().OrderBy(w => w.Name).ToList();
    public readonly static List<ClassMapper> classes = new Database().GetAllClasses().OrderBy(c => c.Name).ToList();
    public readonly static List<SubclassMapper> subclasses = new Database().GetAllSubclasses().OrderBy(s => s.Name).ToList();
    public readonly static List<SpellMapper> spells = new Database().GetAllSpells().OrderBy(s => s.Name).ToList();
    public readonly static List<FeatureMapper> features = new Database().GetAllFeatures().OrderBy(f => f.Name).ToList();
    public readonly static List<LevelMapper> levels = new Database().GetAllLevels().OrderBy(l => l.Index).ToList();
    public readonly static List<BaseEntity> meleeWeapons = new Database().GetAllMeleeWeapons().OrderBy(m => m.Name).ToList();
    public readonly static List<BaseEntity> rangedWeapons = new Database().GetAllRangedWeapons().OrderBy(r => r.Name).ToList();
    public readonly static List<BaseEntity> simpleWeapons = new Database().GetAllSimpleWeapons().OrderBy(s => s.Name).ToList();
    public readonly static List<BaseEntity> simpleMeleeWeapons = new Database().GetAllSimpleMeleeWeapons().OrderBy(s => s.Name).ToList();
    public readonly static List<BaseEntity> simpleRangedWeapons = new Database().GetAllSimpleRangedWeapons().OrderBy(s => s.Name).ToList();
    public readonly static List<BaseEntity> martialWeapons = new Database().GetAllMartialWeapons().OrderBy(m => m.Name).ToList();
    public readonly static List<BaseEntity> martialMeleeWeapons = new Database().GetAllMartialMeleeWeapons().OrderBy(m => m.Name).ToList();
    public readonly static List<BaseEntity> martialRangedWeapons = new Database().GetAllMartialRangedWeapons().OrderBy(m => m.Name).ToList();
    public readonly static List<BaseEntity> lightArmors = new Database().GetAllLightArmors().OrderBy(l => l.Name).ToList();
    public readonly static List<BaseEntity> mediumArmors = new Database().GetAllMediumArmors().OrderBy(m => m.Name).ToList();
    public readonly static List<BaseEntity> heavyArmors = new Database().GetAllHeavyArmors().OrderBy(h => h.Name).ToList();
    public readonly static List<BaseEntity> shields = new Database().GetAllShields().OrderBy(s => s.Name).ToList();
    public readonly static List<EquipmentMapper> equipments = new Database().GetAllEquipments().OrderBy(e => e.Name).ToList();

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