from database import *

def getMonstersDifficultiesList():
    allMonsters = Database.getAllMonsters(db)
    filteredMonster = [{item["index"], item["name"], item["challenge_rating"], item["xp"]} for item in allMonsters]

    return filteredMonster