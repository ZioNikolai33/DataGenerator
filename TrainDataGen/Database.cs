using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;
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

    public List<ClassMapper> GetAllClasses() =>
        _db.GetCollection<ClassMapper>("Classes").Find(FilterDefinition<ClassMapper>.Empty).ToList();

    public List<RaceMapper> GetAllRaces() =>
        _db.GetCollection<RaceMapper>("Races").Find(FilterDefinition<RaceMapper>.Empty).ToList();

    public List<SubraceMapper> GetAllSubraces() =>
        _db.GetCollection<SubraceMapper>("Subraces").Find(FilterDefinition<SubraceMapper>.Empty).ToList();

    public List<SubclassMapper> GetAllSubclasses() =>
        _db.GetCollection<SubclassMapper>("Subclasses").Find(FilterDefinition<SubclassMapper>.Empty).ToList();

    public List<MonsterMapper> GetAllMonsters() =>
        _db.GetCollection<MonsterMapper>("Monsters").Find(FilterDefinition<MonsterMapper>.Empty).ToList();

    public List<FeatureMapper> GetAllFeatures() =>
        _db.GetCollection<FeatureMapper>("Features").Find(FilterDefinition<FeatureMapper>.Empty).ToList();

    public List<FeatureMapper> GetAllLevels() =>
        _db.GetCollection<FeatureMapper>("Levels").Find(FilterDefinition<FeatureMapper>.Empty).ToList();

    public List<EquipmentMapper> GetAllEquipments() =>
        _db.GetCollection<EquipmentMapper>("Equipment").Find(FilterDefinition<EquipmentMapper>.Empty).ToList();

    public List<TraitMapper> GetAllTraits() =>
        _db.GetCollection<TraitMapper>("Traits").Find(FilterDefinition<TraitMapper>.Empty).ToList();

    public List<EquipmentMapper> GetAllWeapons() =>
        _db.GetCollection<EquipmentMapper>("Equipment").Find(Builders<EquipmentMapper>.Filter.Exists("weapon_category")).ToList();

    public List<SpellMapper> GetAllSpells() =>
        _db.GetCollection<SpellMapper>("Spells").Find(FilterDefinition<SpellMapper>.Empty).ToList();

    public List<EquipmentMapper> GetAllMeleeWeapons() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "melee-weapons"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<EquipmentMapper> GetAllRangedWeapons() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "ranged-weapons"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<EquipmentMapper> GetAllLightArmors() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "light-armor"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<EquipmentMapper> GetAllMediumArmors() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "medium-armor"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<EquipmentMapper> GetAllHeavyArmors() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "heavy-armor"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseEntity> GetAllShields() =>
        _db.GetCollection<BaseEntity>("EquipmentCategories")
        .Find(Builders<BaseEntity>.Filter.Eq("index", "shields"))
        .Project<BaseEntity>(Builders<BaseEntity>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<EquipmentMapper> GetAllSimpleWeapons() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "simple-weapons"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<EquipmentMapper> GetAllSimpleMeleeWeapons() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "simple-melee-weapons"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<EquipmentMapper> GetAllMartialMeleeWeapons() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "martial-melee-weapons"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<EquipmentMapper> GetAllMartialWeapons() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "martial-weapons"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<BaseEntity> GetAllSimpleRangedWeapons() =>
        _db.GetCollection<BaseEntity>("EquipmentCategories")
        .Find(Builders<BaseEntity>.Filter.Eq("index", "simple-ranged-weapons"))
        .Project<BaseEntity>(Builders<BaseEntity>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public List<EquipmentMapper> GetAllMartialRangedWeapons() =>
        _db.GetCollection<EquipmentMapper>("EquipmentCategories")
        .Find(Builders<EquipmentMapper>.Filter.Eq("index", "martial-ranged-weapons"))
        .Project<EquipmentMapper>(Builders<EquipmentMapper>.Projection
            .Include("equipment")
            .Exclude("_id"))
        .ToList();

    public IMongoDatabase GetInstance() => _db;
}