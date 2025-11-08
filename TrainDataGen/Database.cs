using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using TrainDataGen.Entities;
using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.DataBase;

internal class Config
{
    public DatabaseConfig Database { get; set; } = new DatabaseConfig();
}

internal class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

public class Database
{
    private readonly IMongoDatabase _db;

    public Database()
    {
        var configText = File.ReadAllText("appsettings.json");
        var config = JsonSerializer.Deserialize<Config>(configText);
        var client = new MongoClient(config.Database.ConnectionString);

        _db = client.GetDatabase(config.Database.DatabaseName);
    }

    public Dictionary<string, IMongoCollection<BsonDocument>> GetAllCollections()
    {
        return new Dictionary<string, IMongoCollection<BsonDocument>>
        {
            ["AbilityScores"] = _db.GetCollection<BsonDocument>("AbilityScores"),
            ["Alignments"] = _db.GetCollection<BsonDocument>("Alignments"),
            ["Backgrounds"] = _db.GetCollection<BsonDocument>("Backgrounds"),
            ["Classes"] = _db.GetCollection<BsonDocument>("Classes"),
            ["Conditions"] = _db.GetCollection<BsonDocument>("Conditions"),
            ["DamageTypes"] = _db.GetCollection<BsonDocument>("DamageTypes"),
            ["Equipments"] = _db.GetCollection<BsonDocument>("Equipments"),
            ["EquipmentCategories"] = _db.GetCollection<BsonDocument>("EquipmentCategories"),
            ["Feats"] = _db.GetCollection<BsonDocument>("Feats"),
            ["Languages"] = _db.GetCollection<BsonDocument>("Languages"),
            ["Levels"] = _db.GetCollection<BsonDocument>("Levels"),
            ["MagicItems"] = _db.GetCollection<BsonDocument>("MagicItems"),
            ["MagicSchools"] = _db.GetCollection<BsonDocument>("MagicSchools"),
            ["Monsters"] = _db.GetCollection<BsonDocument>("Monsters"),
            ["Proficiencies"] = _db.GetCollection<BsonDocument>("Proficiencies"),
            ["Races"] = _db.GetCollection<BsonDocument>("Races"),
            ["RuleSections"] = _db.GetCollection<BsonDocument>("RuleSections"),
            ["Rules"] = _db.GetCollection<BsonDocument>("Rules"),
            ["Skills"] = _db.GetCollection<BsonDocument>("Skills"),
            ["Spells"] = _db.GetCollection<BsonDocument>("Spells"),
            ["Subclasses"] = _db.GetCollection<BsonDocument>("Subclasses"),
            ["Subraces"] = _db.GetCollection<BsonDocument>("Subraces"),
            ["Traits"] = _db.GetCollection<BsonDocument>("Traits"),
            ["WeaponProperties"] = _db.GetCollection<BsonDocument>("WeaponProperties"),
        };
    }

    public List<Class> GetAllClasses() =>
        _db.GetCollection<Class>("Classes").Find(FilterDefinition<Class>.Empty).ToList();

    public List<Race> GetAllRaces() =>
        _db.GetCollection<Race>("Races").Find(FilterDefinition<Race>.Empty).ToList();

    public List<Subrace> GetAllSubraces() =>
        _db.GetCollection<Subrace>("Subraces").Find(FilterDefinition<Subrace>.Empty).ToList();

    public List<Subclass> GetAllSubclasses() =>
        _db.GetCollection<Subclass>("Subclasses").Find(FilterDefinition<Subclass>.Empty).ToList();

    public List<Monster> GetAllMonsters() =>
        _db.GetCollection<Monster>("Monsters").Find(FilterDefinition<Monster>.Empty).ToList();

    public List<Feature> GetAllFeatures() =>
        _db.GetCollection<Feature>("Features").Find(FilterDefinition<Feature>.Empty).ToList();

    public List<Equipment> GetAllEquipments() =>
        _db.GetCollection<Equipment>("Equipment").Find(FilterDefinition<Equipment>.Empty).ToList();

    public List<Equipment> GetAllWeapons() =>
        _db.GetCollection<Equipment>("Equipment").Find(Builders<Equipment>.Filter.Exists("weapon_category")).ToList();

    public List<Spell> GetAllSpells() =>
        _db.GetCollection<Spell>("Spells").Find(FilterDefinition<Spell>.Empty).ToList();

    public List<BaseMapper> GetAllMeleeWeapons() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "melee-weapons"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllRangedWeapons() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "ranged-weapons"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllLightArmors() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "light-armor"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllMediumArmors() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "medium-armor"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllHeavyArmors() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "heavy-armor"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllShields() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "shields"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllSimpleWeapons() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "simple-weapons"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllSimpleMeleeWeapons() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "simple-melee-weapons"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllMartialMeleeWeapons() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "martial-melee-weapons"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllMartialWeapons() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "martial-weapons"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllSimpleRangedWeapons() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "simple-ranged-weapons"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseMapper> GetAllMartialRangedWeapons() =>
        _db.GetCollection<BaseMapper>("EquipmentCategories")
        .Find(Builders<BaseMapper>.Filter.Eq("index", "martial-ranged-weapons"))
        .Project<BaseMapper>(Builders<BaseMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public IMongoDatabase GetInstance() => _db;
}