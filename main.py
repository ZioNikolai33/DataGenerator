from generator.generator import *
from utilities.expOperations import *
from entities.party import *
from database import *

Database()

print([(item.name, item.hp, item.subclasses) for item in classStats])

#generate()