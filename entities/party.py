from hmac import new
from database import *

import random

classes = list(Database.getAllClasses(db))
races = list(Database.getAllRaces(db))
features = list(Database.getAllFeatures(db))
weapons = list(Database.getAllWeapons(db))
categories = list(Database.getAllCategories(db))
spells = list(Database.getAllSpells(db))

weaponNames = [item["index"] for item in weapons]
print(weapons)

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
        self.subclass = feature["subclass"]["index"] if "subclass" in feature else None
        self.level = feature["level"]
        self.prerequisites = feature["prerequisites"]
        self.featureNumChoices, self.featureSpecific = self.getFeatureSpecific(feature)

    def getFeatureSpecific(self, feature):
        numChoices = 0
        featureSpecific = []

        if "feature_specific" in feature:
            if "expertise_options" in feature["feature_specific"]:
                expertiseOptions = feature["feature_specific"]["expertise_options"]["from"]["options"]

                if "items" in expertiseOptions[1] and "choice" in expertiseOptions[0]:
                    numChoices = [expertiseOptions[0]["choice"]["choose"]]
                    numChoices += [expertiseOptions[1]["items"][0]["choice"]["choose"]]

                    featureSpecific = [item["item"]["index"] for item in expertiseOptions[0]["choice"]["from"]["options"]]
                    featureSpecific += [item["item"]["index"] for item in expertiseOptions[1]["items"][0]["choice"]["from"]["options"]]
                    featureSpecific[1] += expertiseOptions[1]["items"][1]["item"]["index"]
                else:
                    numChoices = feature["feature_specific"]["expertise_options"]["choose"]
                    featureSpecific = [item["item"]["index"] for item in expertiseOptions]
            elif "subfeature_options" in feature["feature_specific"]:
                numChoices = feature["feature_specific"]["subfeature_options"]["choose"]
                featureSpecific = [item["item"]["index"] for item in feature["feature_specific"]["subfeature_options"]["from"]["options"]]
            elif "enemy_type_options" in feature["feature_specific"]:
                numChoices = feature["feature_specific"]["enemy_type_options"]["choose"]
                featureSpecific = [item for item in feature["feature_specific"]["enemy_type_options"]["from"]["options"]]
            elif "terrain_type_options" in feature["feature_specific"]:
                numChoices = feature["feature_specific"]["terrain_type_options"]["choose"]
                featureSpecific = [item for item in feature["feature_specific"]["terrain_type_options"]["from"]["options"]]
            elif "invocations" in feature["feature_specific"]:
                numChoices = self.getInvocationsNum()
                featureSpecific = [item["index"] for item in feature["feature_specific"]["invocations"]]

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
    def __init__(self, index, quantity):
        self.name = index
        self.quantity = quantity

        for item in weapons:
            if item["index"] == self.name:
                weapon = item
                break

        self.equipmentCategory = item["equipment_category"]["index"]
        self.weaponCategory = item["weapon_category"] if "weapon_category" in item else None
        self.weaponRange = item["weapon_range"]
        self.categoryRange = item["category_range"]
        self.cost = item["cost"]["quantity"]
        self.costUnit = item["cost"]["unit"]
        self.damage = item["damage"]["damage_dice"] if "damage" in item else None
        self.damageType = item["damage"]["damage_type"]["index"] if "damage" in item else None
        self.rangeNormal = item["range"]["normal"] if "range" in item else None
        self.properties = [item["index"] for item in item["properties"]]

class Race:
    def __init__(self, race):
        self.name = race["index"]
        self.abilityBonuses = [AbilityBonus(item) for item in race["ability_bonuses"]]
        self.proficiencies = [item["index"] for item in race["starting_proficiencies"]]
        self.traits = [item["index"] for item in race["traits"]]
        self.subraces = [item["index"] for item in race["subraces"]]
        self.speed = race["speed"]

class Multiclass:
    def __init__(self, classe):
        self.prerequisitiesAttribute = [item["ability_scores"]["index"] for item in classe["prerequisities"]]
        self.prerequisitiesValue = [item["minimum_score"] for item in classe["prerequisities"]]
        self.proficiencies = [item["index"] for item in classe["proficiencies"]]
        self.proficiencyChoices = [prof["index"] for prof in [item["from"]["options"] for item in classe["proficiencies_choices"]]]
        self.proficiencyChoicesNum = [item["choose"] for item in classe["proficiencies_choices"]["choose"]]

class Area:
    def _init_(self, area):
        self.type = area["type"]
        self.size = area["size"]

class SpellDamage:
    def _init_(self, damage):
        self.damageType = damage["damage_type"]["index"]
        self.damageSlots = damage["damage_at_slot_level"]
        self.damageAtCharacterLevel = damage["damage_at_character_level"]

class Dc:
    def _init_(self, dc):
        self.dcType = dc["dc_type"]
        self.dcSuccess = dc["dc_success"]

class Spell:
    def __init__(self, spell):
        self.name = spell["index"]
        self.range = spell["range"]
        self.ritual = spell["ritual"]
        self.duration = spell["duration"]
        self.concentration = spell["concentration"]
        self.castingTime = spell["casting_time"]
        self.healAtSlotLevel = spell["heal_at_slot_level"] if "heal_at_slot_level" in spell else None
        self.school = spell["school"]["index"]
        self.classes = [item["index"] for item in spell["classes"]]
        self.subclasses = [item["index"] for item in spell["subclasses"]] if "subclasses" in spell else None
        self.areaEffect = Area(spell["area_of_effect"]) if "area_of_effect" in spell else None
        self.dc = Dc(spell["dc"]) if "dc" in spell else None
        self.damage = SpellDamage(spell["damage"]) if "damage" in spell else None
        self.attackType = spell["attack_type"] if "attack_type" in spell else None

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
        self.startingEquipments = [Equipment(item["equipment"]["index"], item["equipment"]["quantity"]) for item in classe["starting_equipment"] if item["equipment"]["index"] in weapons]
        self.startingEquipmentsOptions = self.getEquipmentOptions(classe["starting_equipment_options"])
        self.multiclassing = Multiclass(classe["multi_classing"])
        self.spellcastingAbility = classe["spellcasting"]["spellcasting_ability"]["index"] if "spellcasting" in classe else None
        self.spells = [Spell(item) for item in spellStats if self.name in item["classes"]]

    def getEquipmentOptions(self, startingEquip):
        equipmentOptions = []

        for item in startingEquip:
            numChoices = item["choose"]
            optionList = []

            for option in item["from"]["options"]:
                if option["option_type"] == "counted_reference" and option["of"]["index"] in weapons:
                    optionList.append(Equipment(option["of"]["index"], option["count"]))
                elif option["option_type"] == "choice":
                    choiceNum = option["choice"]["choose"]
                    if options["choice"]["from"]["option_set_type"] == "equipment_category":
                        toChooseEquip = [item for item in categories if item["index"] == option["choice"]["from"]["equipment_category"]["index"]]
                        optionList.append([Equipment(item["index"], 1) for item in toChooseEquip if item["index"] in weapons])

            equipmentOptions.append(optionList)

        return equipmentOptions

featureStats = [Feature(item) for item in features]
classStats = [Class(item) for item in classes]
raceStats = [Race(item) for item in races]
spellStats = [Spell(item) for item in spells]

print([item.__str__() for item in classStats])
print([item.__str__() for item in raceStats])
print([item.__str__() for item in featureStats])

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