from hmac import new
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