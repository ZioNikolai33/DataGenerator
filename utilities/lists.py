from database import *

categories = list(Database.getAllCategories(db))
weapons = list(Database.getAllWeapons(db))
classes = list(Database.getAllClasses(db))
martialWeapons = list(Database.getAllMartialWeapons(db))[0]["equipment"]
martialMeleeWeapons = list(Database.getAllMartialMeleeWeapons(db))[0]["equipment"]
simpleWeapons = list(Database.getAllSimpleWeapons(db))[0]["equipment"]
simpleMeleeWeapons = list(Database.getAllSimpleMeleeWeapons(db))[0]["equipment"]

weaponNames = [item["index"] for item in weapons]
proficiencyChoices = {}