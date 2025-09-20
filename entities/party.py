from hmac import new
from database import *

import random

classes = list(Database.getAllClasses(db))
races = list(Database.getAllRaces(db))
features = list(Database.getAllFeatures(db))

proficiencyChoices = {}

for item in classes:
    class_index = item["index"]
    profs = []    
    proficiencyOptions = [i for i in item["proficiency_choices"][0]["from"]["options"]]

    for options in proficiencyOptions:
        profs.append(options["item"]["index"])
                        
    proficiencyChoices[class_index] = profs

class Feature:
    def __init__(self, feature):
        self.name = feature["index"]
        self.classe = feature["class"]["index"]
        self.subclass = feature["subclass"]["index"] if feature["subclass"] else None
        self.level = feature["level"]
        self.prerequisites = feature["prerequisites"]
        self.featureNumChoices, self.featureSpecific = self.getFeatureSpecific(feature)

    def getFeatureSpecific(self, feature):
        numChoices = 0
        featureSpecific = []

        if feature["feature_specific"]["expertise_options"]:
            numChoices = feature["feature_specific"]["expertise_options"]["choose"]
            featureSpecific = [item["item"]["index"] for item in feature["feature_specific"]["expertise_options"]["from"]["options"]]
        elif feature["feature_specific"]["subfeature_options"]:
            numChoices = feature["feature_specific"]["subfeature_options"]["choose"]
            featureSpecific = [item["item"]["index"] for item in feature["feature_specific"]["subfeature_options"]["from"]["options"]]
        elif feature["feature_specific"]["enemy_type_options"]:
            numChoices = feature["feature_specific"]["enemy_type_options"]["choose"]
            featureSpecific = [item["item"]["index"] for item in feature["feature_specific"]["enemy_type_options"]["from"]["options"]]
        elif feature["feature_specific"]["terrain_type_options"]:
            numChoices = feature["feature_specific"]["terrain_type_options"]["choose"]
            featureSpecific = [item["item"]["index"] for item in feature["feature_specific"]["terrain_type_options"]["from"]["options"]]
        elif feature["feature_specific"]["invocations"]:
            numChoices = self.getInvocationsNum()
            featureSpecific = [item["item"]["index"] for item in feature["feature_specific"]["invocations"]["from"]["options"]]

        return numChoices, featureSpecific

    def getInvocationsNum(self):
        if self.level > 17:
            return 8
        elif self.level > 14:
            return 7
        elif self.level > 11:
            return 6
        elif self.level > 8:
            return 5
        elif self.level > 6:
            return 4
        elif self.level > 3:
            return 3
        elif self.level > 1:
            return 2
        else:
            return 0

    def randomizeFeature(self):
        self.featureSpecific = random.sample(self.featureSpecific, self.featureNumChoices) if self.featureNumChoices > 0 else self.featureSpecific

class Attribute:
    def __init__(self, value):
        self.value = value
        self.modifier = (value - 10) // 2
        self.save = (value - 10) // 2

class AbilityBonus:
    def __init__(self, item):
        self.name = item["ability_score"]["index"]
        self.bonus = item["bonus"]

class Equipment:
    def __init__(self, equipment):
        self.name = equipment["equipment"]["index"]
        self.quantity = equipment["quantity"]

class Race:
    def __init__(self, race):
        self.name = race["index"]
        self.abilityBonuses = [AbilityBonus(item) for item in race["ability_bonuses"]]
        self.proficiencies = [item["index"] for item in race["starting_proficiencies"]]
        self.traits = [item["index"] for item in race["traits"]]
        self.subraces = [item["index"] for item in race["subraces"]]
        self.speed = race["speed"]

class Class:
    def __init__(self, classe):
        self.name = classe["index"]
        self.hp = classe["hit_die"]
        self.subclasses = [item["index"] for item in classe["subclasses"]]
        self.numProficiencies = classe["proficiency_choices"][0]["choose"]
        self.proficiencyChoices = proficiencyChoices[classe["index"]]
        self.savingThrows = [item["index"] for item in classe["saving_throws"]]
        self.proficiencies = [item["index"] for item in classe["proficiencies"]]
        self.features = [feature for feature in featureStats if feature.classe == self.name]
        

classStats = [Class(item) for item in classes]
raceStats = [Race(item) for item in races]
featureStats = [Feature(item) for item in features]

print([item for item in classStats])

class Member:
    def __init__(self, id, level):
        randomRace = random.choice(raceStats)
        randomClass = random.choice(classStats)
        attributes = self.addAbilityScores(randomRace, self.assumeAttributes(randomClass))

        self.id = id
        self.name = "Member " + str(id)
        self.level = level
        self.race = randomRace.name
        self.subrace = random.choice(randomRace.subraces) if len(randomRace.subraces) > 0 else None
        self.speed = randomRace.speed
        self.classe = randomClass.name
        self.strength = Attribute(attributes[0])
        self.dexterity = Attribute(attributes[1])
        self.constitution = Attribute(attributes[2])
        self.intelligence = Attribute(attributes[3])
        self.wisdom = Attribute(attributes[4])
        self.charisma = Attribute(attributes[5])
        self.hp = self.calculateRandomHp(randomClass.hp) + ((self.level - 1) * (self.constitution.modifier - 10) // 2)
        self.subclass = random.choice(randomClass.subclasses)
        self.initiative = self.dexterity.modifier
        self.proficiencyBonus = self.getProfBonus()
        self.proficiencies = random.sample(randomClass.proficiencyChoices, randomClass.numProficiencies)
        self.proficiencies.append(randomClass.proficiencies).append(randomRace.proficiencies)
        self.features = [item for item in randomClass.features if item.level <= self.level]

        for item in self.features:
            item.randomizeFeature()

        self.masteries = []
        self.vulnerabilities = []
        self.resistances = []
        self.immunities = []

        self.addProfToSavings(self, randomClass)

    def calculateRandomHp(self, hitDie):
        hp = hitDie

        for i in range(2, self.level):
            hp += random.randint(1, hitDie)

        return hp

    def assumeAttributes(self, randomClass):
        # Just assume standard array
        attributes = [15, 14, 13, 12, 10, 8]
        random.shuffle(attributes)

        return attributes

    def getProfBonus(self):
        return 2 + ((self.level - 1) // 4)

    def addProfToSavings(self, classe):
        self.strength += self.proficiencyBonus if "str" in classe.savingThrows else 0
        self.dexterity += self.proficiencyBonus if "dex" in classe.savingThrows else 0
        self.constitution += self.proficiencyBonus if "con" in classe.savingThrows else 0
        self.intelligence += self.proficiencyBonus if "int" in classe.savingThrows else 0
        self.wisdom += self.proficiencyBonus if "wis" in classe.savingThrows else 0
        self.charisma += self.proficiencyBonus if "cha" in classe.savingThrows else 0

    def addAbilityScores(self, abilityScores, attributes):
        for item in abilityScores:
            if item.name == "str":
                attributes[0] += item.bonus
            elif item.name == "dex":
                attributes[1] += item.bonus
            elif item.name == "con":
                attributes[2] += item.bonus
            elif item.name == "int":
                attributes[3] += item.bonus
            elif item.name == "wis":
                attributes[4] += item.bonus
            elif item.name == "cha":
                attributes[5] += item.bonus

        return attributes