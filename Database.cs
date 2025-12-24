using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using TrainingDataGenerator.Entities.Mappers;

namespace TrainingDataGenerator.DataBase;

public class Config
{
    [JsonPropertyName("database")]
    public DatabaseConfig Database { get; set; } = new DatabaseConfig();
    [JsonPropertyName("numberOfCycles")]
    public int NumberOfCycles { get; set; } = 10;
}

public class DatabaseConfig
{
    [JsonPropertyName("connectionString")]
    public string ConnectionString { get; set; } = string.Empty;
    [JsonPropertyName("databaseName")]
    public string DatabaseName { get; set; } = string.Empty;
}

public class Database
{
    private readonly IMongoDatabase _db;

    public Database()
    {
        var configText = File.ReadAllText("appsettings.json");
        var config = JsonSerializer.Deserialize<Config>(configText);
        var client = new MongoClient(config?.Database.ConnectionString);

        _db = client.GetDatabase(config?.Database.DatabaseName);
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

    public List<LevelMapper> GetAllLevels() =>
        _db.GetCollection<LevelMapper>("Levels").Find(FilterDefinition<LevelMapper>.Empty).ToList();

    public List<EquipmentMapper> GetAllEquipments() =>
        _db.GetCollection<EquipmentMapper>("Equipment").Find(FilterDefinition<EquipmentMapper>.Empty).ToList();

    public List<TraitMapper> GetAllTraits() =>
        _db.GetCollection<TraitMapper>("Traits").Find(FilterDefinition<TraitMapper>.Empty).ToList();

    public List<EquipmentMapper> GetAllWeapons() =>
        _db.GetCollection<EquipmentMapper>("Equipment").Find(Builders<EquipmentMapper>.Filter.Exists("weapon_category")).ToList();

    public List<SpellMapper> GetAllSpells() =>
        _db.GetCollection<SpellMapper>("Spells").Find(FilterDefinition<SpellMapper>.Empty).ToList();

    public List<BaseEntity> GetAllMeleeWeapons() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "melee-weapons"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllRangedWeapons() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "ranged-weapons"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllLightArmors() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "light-armor"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllMediumArmors() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "medium-armor"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllHeavyArmors() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "heavy-armor"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllShields() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "shields"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllSimpleWeapons() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "simple-weapons"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllSimpleMeleeWeapons() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "simple-melee-weapons"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllMartialWeapons() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "martial-weapons"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllMartialMeleeWeapons() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "martial-melee-weapons"))
            .FirstOrDefault().Equipment;    

    public List<BaseEntity> GetAllSimpleRangedWeapons() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "simple-ranged-weapons"))
            .FirstOrDefault().Equipment;

    public List<BaseEntity> GetAllMartialRangedWeapons() =>
        _db.GetCollection<EquipmentCategoryMapper>("EquipmentCategories")
            .Find(Builders<EquipmentCategoryMapper>.Filter.Eq("index", "martial-ranged-weapons"))
            .FirstOrDefault().Equipment;

    public IMongoDatabase GetInstance() => _db;
}