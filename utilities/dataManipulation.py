def getMonstersDifficultiesList(db):
    allMonsters = db.getAllMonsters()
    monstersList = [{"index": item["index"], "name": item["name"], "cr": item["challenge_rating"], "xp": item["xp"]} for item in allMonsters]
    
    print(monstersList)