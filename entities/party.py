from hmac import new

from pymongo.synchronous.helpers import F
from database import *
from entities.Race import *
from entities.Class import *
from entities.Attribute import *
import random

class Member:
    def __init__(self, id, level):
        randomRace = random.choice(raceStats)
        randomClass = random.choice(classStats)
        attributes = self.addAbilityScores(randomRace, self.assumeAttributes(randomClass))

        self.id = id
        self.name = "Member " + str(id)
        self.level = level
        self.race = randomRace.name
        self.subrace = random.choice(randomRace.subraces) if len(randomRace.subraces) > 0 else ""
        self.speed = randomRace.speed
        self.classe = randomClass.name
        self.strength = Attribute(attributes[0])
        self.dexterity = Attribute(attributes[1])
        self.constitution = Attribute(attributes[2])
        self.intelligence = Attribute(attributes[3])
        self.wisdom = Attribute(attributes[4])
        self.charisma = Attribute(attributes[5])
        self.hp = self.calculateRandomHp(randomClass.hp)
        self.subclass = random.choice(randomClass.subclasses)
        self.initiative = self.dexterity.modifier
        self.proficiencyBonus = self.getProfBonus()
        self.proficiencies = randomClass.proficiencies
        self.proficiencies.extend(randomRace.proficiencies)

        for item in randomClass.proficiencyChoices:
            chosen = item.getRandomChoice(self.proficiencies)
            self.proficiencies.extend(chosen)

        self.equipments = randomClass.startingEquipments

        if len(randomClass.startingEquipmentsOptions) > 0:
            for item in randomClass.startingEquipmentsOptions:
                self.equipments.append(item)

        self.features = [item for item in randomClass.features if item.level == self.level]
        self.featureSpecifics = []
        self.masteries = []
        self.vulnerabilities = []
        self.resistances = []
        self.immunities = []

        for item in randomClass.proficiencyChoices:
            self.proficiencies.extend(random.sample(item.choices, item.number))

        for feature in self.features:
            for item in feature.featureSpecificChoices:
                if feature.featureSpecificType == "subfeature_options":
                    self.featureSpecifics.extend(random.sample(item.choices, item.number))
                elif feature.featureSpecificType == "expertise":
                    self.masteries.extend(random.sample(item.choices, item.number))
                elif feature.featureSpecificType == "enemy_type_options":
                    self.featureSpecifics.extend(random.sample(item.choices, item.number))
                elif feature.featureSpecificType == "terrain_type_options":
                    self.featureSpecifics.extend(random.sample(item.choices, item.number))
                elif feature.featureSpecificType == "invocations":
                    self.featureSpecifics.extend(random.sample(item.choices, item.number))

        self.addProfToSavings(randomClass)
        self.skills = self.getSkillsDict()
        self.spellSlots = randomClass.getSpellSlots(self.level)

    def calculateRandomHp(self, hitDie):
        hp = hitDie + self.constitution.modifier

        for i in range(2, self.level):
            hp += random.randint(1, hitDie) + self.constitution.modifier

        return hp

    def assumeAttributes(self, randomClass):
        attributes = [15, 14, 13, 12, 10, 8] # Just assume standard array
        random.shuffle(attributes)

        return attributes

    def getProfBonus(self):
        return 2 + ((self.level - 1) // 4)

    def addProfToSavings(self, classe):
        self.strength.save += self.proficiencyBonus if "str" in classe.savingThrows else 0
        self.dexterity.save += self.proficiencyBonus if "dex" in classe.savingThrows else 0
        self.constitution.save += self.proficiencyBonus if "con" in classe.savingThrows else 0
        self.intelligence.save += self.proficiencyBonus if "int" in classe.savingThrows else 0
        self.wisdom.save += self.proficiencyBonus if "wis" in classe.savingThrows else 0
        self.charisma.save += self.proficiencyBonus if "cha" in classe.savingThrows else 0

    def addAbilityScores(self, race, attributes):
        for item in race.abilityBonuses:
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

    def getSkillsDict(self):
        skills = {
            "acrobatics": self.dexterity.modifier,
            "animal_handling": self.wisdom.modifier,
            "arcana": self.intelligence.modifier,
            "athletics": self.strength.modifier,
            "deception": self.charisma.modifier,
            "history": self.intelligence.modifier,
            "insight": self.wisdom.modifier,
            "intimidation": self.charisma.modifier,
            "investigation": self.intelligence.modifier,
            "medicine": self.wisdom.modifier,
            "nature": self.intelligence.modifier,
            "perception": self.wisdom.modifier,
            "performance": self.charisma.modifier,
            "persuasion": self.charisma.modifier,
            "religion": self.intelligence.modifier,
            "sleight_of_hand": self.dexterity.modifier,
            "stealth": self.dexterity.modifier,
            "survival": self.wisdom.modifier
        }

        for index, item in enumerate(skills):
            if "skill-" + item in self.proficiencies:
                skills[item] += self.proficiencyBonus

        for index, item in enumerate(skills):
            if "skill-" + item in self.masteries:
                skills[item] += self.proficiencyBonus

        return skills

    def __str__(self):
        subrace_str = f" {self.subrace.name}" if self.subrace != "" else ""

        string = f"{self.name} | {self.race}{subrace_str} | Lv{self.level} {self.classe}\n"
        string += f"HP: {self.hp} | Speed: {self.speed} | Initiative: {self.initiative} | Proficiency Bonus: +{self.proficiencyBonus}\n"
        string += f"STR: {self.strength.value} ({self.strength.modifier}) | DEX: {self.dexterity.value} ({self.dexterity.modifier}) | CON: {self.constitution.value} ({self.constitution.modifier}) | INT: {self.intelligence.value} ({self.intelligence.modifier}) | WIS: {self.wisdom.value} ({self.wisdom.modifier}) | CHA: {self.charisma.value} ({self.charisma.modifier})\n"
        string += f"Saving Throws: STR {self.strength.save}, DEX {self.dexterity.save}, CON {self.constitution.save}, INT {self.intelligence.save}, WIS {self.wisdom.save}, CHA {self.charisma.save}\n"
        string += f"Skills: {', '.join([f'{key.replace('_', ' ').title()} {value}' for key, value in self.skills.items()])}\n"
        string += f"Proficiencies: {', '.join([item if isinstance(item, str) else ', '.join(item) for item in self.proficiencies])}\n"
        if len(self.masteries) > 0:
            string += f"Masteries: {', '.join([item if isinstance(item, str) else ', '.join(item) for item in self.masteries])}\n"
        string += f"Traits: {', '.join(self.subrace.traits) if self.subrace != '' else 'None'}\n"
        string += f"Features: {', '.join([item.name for item in self.features])}\n"
        string += f"Equipments: {', '.join([f'{item.quantity} x {item.name.replace('-', ' ').title()}' for item in self.equipments])}\n"
        if len(self.vulnerabilities) > 0:
            string += f"Vulnerabilities: {', '.join(self.vulnerabilies)}\n"
        if len(self.resistances) > 0:
            string += f"Resistances: {', '.join(self.resistances)}\n"
        if len(self.immunities) > 0:
            string += f"Immunities: {', '.join(self.immunities)}\n"
        string += str(self.spellSlots) if self.spellSlots.first > 0 else ""
        string += "\n------------------------------------------------------------------------------------------------------------------------\n"

        return string