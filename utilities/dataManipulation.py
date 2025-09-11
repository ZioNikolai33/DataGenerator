import pandas as pd

def getMonstersDifficultiesList(db):
    allMonsters = db.getAllMonsters()
    df = pd.DataFrame(allMonsters)

    monstersList = df[["index", "name", "challenge_rating", "xp"]].rename(
        columns={"challenge_rating": "cr"}
    ).to_dict(orient="records")
    
    return monstersList