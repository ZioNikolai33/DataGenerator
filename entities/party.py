from hmac import new
from database import *

import random

classes = Database.getAllClasses(db)
proficiencyChoices = {}

for item in classes:
    class_index = item["index"]
    profs = []    
    proficiencyOptions = [i for i in item["proficiency_choices"][0]["from"]["options"]]

    for options in proficiencyOptions:
        profs.append(options["item"]["index"])
                        
    proficiencyChoices[class_index] = profs

print(proficiencyChoices)

class Class:
    def __init__(self, classe):
        self.name = classe["index"]
        self.hp = classe["hit_die"]
        self.subclasses = classe["subclasses"]

class Attribute:
    def __init__(self, value):
        self.value = value
        self.modifier = (value - 10) // 2
        self.save = (value - 10) // 2

classStats = [Class(item) for item in classes]

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