import pandas as pd
import json
from types.difficulty import Difficulty

# Load Exp Points thresholds for each level
with open("data/expThreshold.json") as f:
    expPointsList = pd.DataFrame(json.load(f))

# Load Multipliers for number of monsters
with open("data/expMonsterMultiplier.json") as f:
    multiplierList = pd.DataFrame(json.load(f))

def calculateDifficultiesExp(partyLevels):
    totalExpPoints = Difficulty(easy=0, medium=0, hard=0, deadly=0)

    for level in partyLevels:
        if level in expPointsList["level"].to_list():
            totalExpPoints.easy += expPointsList["easy"][level]
            totalExpPoints.medium += expPointsList["medium"][level]
            totalExpPoints.hard += expPointsList["hard"][level]
            totalExpPoints.deadly += expPointsList["deadly"][level]
        else:
            raise ValueError(f"Level {level} is out of range. Valid levels are 1 to 20.")

    return totalExpPoints

def calculateAdjustedExp(monsterExps):
    numMonsters = len(monsterExps)

    if numMonsters == 0:
        return 0

    totalExp = sum(monsterExps)

    if numMonsters in multiplierList["number"]:
        multiplier = multiplierList["number"][numMonsters]
    elif numMonsters > 15:
        multiplier = 4
    else:
        raise ValueError(f"Number of monsters {numMonsters} is out of range. Valid range is 1 to 15 or more.")

    adjustedExp = int(totalExp * multiplier)

    return adjustedExp