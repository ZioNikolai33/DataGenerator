from hmac import new
from database import *

import random

classes = list(Database.getAllClasses(db))
proficiencyChoices = {}

for item in classes:
    class_index = item["index"]
    profs = []    
    proficiencyOptions = [i for i in item["proficiency_choices"][0]["from"]["options"]]

    for options in proficiencyOptions:
        profs.append(options["item"]["index"])
                        
    proficiencyChoices[class_index] = profs

class Attribute:
    def __init__(self, value):
        self.value = value
        self.modifier = (value - 10) // 2
        self.save = (value - 10) // 2

class Equipment:
    def __init__(self, equipment):
        self.name = equipment["equipment"]["index"]
        self.quantity = equipment["quantity"]

class Class:
    def __init__(self, classe):
        self.name = classe["index"]
        self.hp = classe["hit_die"]
        self.subclasses = [item["index"] for item in classe["subclasses"]]
        self.proficiencyChoices = proficiencyChoices[classe["index"]]
        self.savingThrows = [item["index"] for item in classe["saving_throws"]]
        self.proficiencies = [item["index"] for item in classe["proficiencies"]]
        self.startingEquipments = [Equipment(item) for item in classe["starting_equipment"]]

classStats = [Class(item) for item in classes]
#print([(item.name, item.hp, item.subclasses, item.proficiencyChoices, item.savingThrows, item.proficiencies) for item in classStats])

class Member:
    def __init__(self, id, level):
        randomClass = random.choice(classStats)
        attributes = self.assumeAttributes(randomClass)

        self.id = id
        self.name = "Member " + str(id)
        self.level = level
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