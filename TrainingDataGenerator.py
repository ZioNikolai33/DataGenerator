from pymongo import MongoClient
import pandas as pd
import json

# Open config file
with open("config.json") as f:
    config = json.load(f)

# Get database connection details
connection_string = config["database"]["connectionString"]
database_name = config["database"]["databaseName"]

# Connect to MongoDB
client = MongoClient(connection_string)
db = client[database_name]

# Access all collections
abilityScores = db["AbilityScores"].find()
alignments = db["Alignments"].find()
backgrounds = db["Backgrounds"].find()
classes = db["Classes"].find()
conditions = db["Conditions"].find()
damageTypes = db["DamageTypes"].find()
equipments = db["Equipments"].find()
equipmentCategories = db["EquipmentCategories"].find()
feats = db["Feats"].find()
languages = db["Languages"].find()
levels = db["Levels"].find()
magicItems = db["MagicItems"].find()
magicSchools = db["MagicSchools"].find()
monsters = db["Monsters"].find()
proficiencies = db["Proficiencies"].find()
races = db["Races"].find()
ruleSections = db["RuleSections"].find()
rules = db["Rules"].find()
skills = db["Skills"].find()
spells = db["Spells"].find()
subclasses = db["Subclasses"].find()
subraces = db["Subraces"].find()
traits = db["Traits"].find()
weaponProperties = db["WeaponProperties"].find()

print([dt["name"] for dt in damageTypes])