from database import *

categories = list(Database.getAllCategories(db))
weapons = list(Database.getAllWeapons(db))
classes = list(Database.getAllClasses(db))
martialWeapons = list(Database.getAllMartialWeapons(db))
martialMeleeWeapons = list(Database.getAllMartialMeleeWeapons(db))
simpleWeapons = list(Database.getAllSimpleWeapons(db))
simpleMeleeWeapons = list(Database.getAllSimpleMeleeWeapons(db))

weaponNames = [item["index"] for item in weapons]
proficiencyChoices = {}