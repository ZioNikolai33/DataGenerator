from database import *

def getMonstersDifficultiesList():
    allMonsters = Database.getAllMonsters(db)
    filteredMonster = [{item["index"], item["name"], item["challenge_rating"], item["xp"]} for item in allMonsters]

    print([item for item in allMonsters])
    #print(filteredMonster)

    return allMonsters
    
    return allMonsters