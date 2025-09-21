import json
from pymongo import MongoClient

class Database:
    def __init__(self):
        with open("config.json") as f:
            config = json.load(f)

        self.client = MongoClient(config["database"]["connectionString"])
        self.db = self.client[config["database"]["databaseName"]]
    
    @staticmethod
    def getAllCollections(self):
        return {
            "AbilityScores": self.db["AbilityScores"].find(),
            "Alignments": self.db["Alignments"].find(),
            "Backgrounds": self.db["Backgrounds"].find(),
            "Classes": self.db["Classes"].find(),
            "Conditions": self.db["Conditions"].find(),
            "DamageTypes": self.db["DamageTypes"].find(),
            "Equipments": self.db["Equipments"].find(),
            "EquipmentCategories": self.db["EquipmentCategories"].find(),
            "Feats": self.db["Feats"].find(),
            "Languages": self.db["Languages"].find(),
            "Levels": self.db["Levels"].find(),
            "MagicItems": self.db["MagicItems"].find(),
            "MagicSchools": self.db["MagicSchools"].find(),
            "Monsters": self.db["Monsters"].find(),
            "Proficiencies": self.db["Proficiencies"].find(),
            "Races": self.db["Races"].find(),
            "RuleSections": self.db["RuleSections"].find(),
            "Rules": self.db["Rules"].find(),
            "Skills": self.db["Skills"].find(),
            "Spells": self.db["Spells"].find(),
            "Subclasses": self.db["Subclasses"].find(),
            "Subraces": self.db["Subraces"].find(),
            "Traits": self.db["Traits"].find(),
            "WeaponProperties": self.db["WeaponProperties"].find(),
        }

    @staticmethod
    def getAllClasses(self):
        return self.db["Classes"].find()

    @staticmethod
    def getAllRaces(self):
        return self.db["Races"].find()

    @staticmethod
    def getAllSubclasses(self):
        return self.db["Subclasses"].find()

    @staticmethod
    def getAllMonsters(self):
        return self.db["Monsters"].find()

    @staticmethod
    def getAllFeatures(self):
        return self.db["Features"].find()

    @staticmethod
    def getAllEquipments(self):
        return self.db["Equipment"].find()

    @staticmethod
    def getAllWeapons(self):
        return self.db["Equipment"].find({'weapon_category': {'$exists': True}})

db = Database()