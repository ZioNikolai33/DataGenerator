import json

# Load Exp Points thresholds for each level
with open("data/expThreshold.json") as f:
    expPointsList = json.load(f)

# Load Multipliers for number of monsters
with open("data/expMonsterMultiplier.json") as f:
    multiplierList = json.load(f)

# Build lookup dictionaries for fast access
expPointsDict = {entry["level"]: entry for entry in expPointsList}
multiplierDict = {entry["level"]: entry["multiplier"] for entry in multiplierList}

def calculateDifficultiesExp(partyLevels):
    """
    Calculate the difficulty thresholds for a party based on their levels.
    Parameters:
    partyLevels (list): A list of integers representing the levels of each party member.
    Returns:
    dict: A dictionary containing the difficulty thresholds for 'easy', 'medium', 'hard', and 'deadly'.
    """
    totalExpPoints = {"easy": 0, "medium": 0, "hard": 0, "deadly": 0}

    for level in partyLevels:
        if level in expPointsDict:
            expPoints = expPointsDict[level]
            totalExpPoints["easy"] += expPoints["easy"]
            totalExpPoints["medium"] += expPoints["medium"]
            totalExpPoints["hard"] += expPoints["hard"]
            totalExpPoints["deadly"] += expPoints["deadly"]
        else:
            raise ValueError(f"Level {level} is out of range. Valid levels are 1 to 20.")

    return totalExpPoints

def calculateAdjustedExp(monsterExps):
    """
    Calculate the adjusted experience points for a list of monsters based on their individual experience points.
    Parameters:
    monsterExps (list): A list of integers representing the experience points of each monster.
    Returns:
    int: The adjusted experience points after applying the multiplier based on the number of monsters.
    """
    numMonsters = len(monsterExps)

    if numMonsters == 0:
        return 0

    totalExp = sum(monsterExps)

    # Use multiplier from multiplierDict if available, else use 4 for > 15 monsters
    if numMonsters in multiplierDict:
        multiplier = multiplierDict[numMonsters]
    elif numMonsters > 15:
        multiplier = 4
    else:
        raise ValueError(f"Number of monsters {numMonsters} is out of range. Valid range is 1 to 15 or more.")

    adjustedExp = int(totalExp * multiplier)

    return adjustedExp