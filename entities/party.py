from database import *

import random

classes = Database.getAllClasses(db)

class Class:
    def __init__(self, classe):
        self.name = classe["index"]
        self.hp = classe["hit_die"]
        self.subclasses = classe["subclasses"]

classStats = [Class(item) for item in classes]

class Member:
    def __init__(self, id, level):
        self.id = id
        self.name = "Member " + str(id)
        self.level = level
        self.classe = random.choice(classes)
        self.hp = self.getHpClass(self.classe)