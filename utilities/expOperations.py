import pandas as pd
import json
from entities.Difficulty import Difficulty

# Load Exp Points thresholds for each level
with open("data/expThreshold.json") as f:
    expPointsList = json.load(f)

# Load Multipliers for number of monsters
with open("data/expMonsterMultiplier.json") as f:
    multiplierList = json.load(f)

def calculateDifficultiesExp(partyLevels):
    totalExpPoints = Difficulty(easy=0, medium=0, hard=0, deadly=0)

    for level in partyLevels:
        levelExps = next((entry for entry in expPointsList if entry["level"] == level), None)

        if any(entry["level"] == level for entry in expPointsList):
            totalExpPoints.easy += levelExps["easy"]
            totalExpPoints.medium += levelExps["medium"]
            totalExpPoints.hard += levelExps["hard"]
            totalExpPoints.deadly += levelExps["deadly"]
        else:
            raise ValueError(f"Level {level} not found in expPointsList.")

    return totalExpPoints

def calculateAdjustedExp(monsterExps):
    totalExp = sum(monsterExps)
    numMonsters = len(monsterExps)
    numMultiplier = next((entry for entry in multiplierList if entry["number"] == numMonsters), None)

    if numMultiplier is None:
        raise ValueError(f"Number of monsters {numMonsters} is out of range. Valid range is 1 to 15 or more.")

    adjustedExp = int(totalExp * numMultiplier["multiplier"])

    return adjustedExp