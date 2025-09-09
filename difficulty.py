# Dictionary of Exp Points related to four difficulties and for each level
expPointsDict = {
    1:  [25, 50, 75, 100],
    2:  [50, 100, 150, 200],
    3:  [75, 150, 225, 400],
    4:  [125, 250, 375, 500],
    5:  [250, 500, 750, 1100],
    6:  [300, 600, 900, 1400],
    7:  [350, 750, 1100, 1700],
    8:  [450, 900, 1400, 2100],
    9:  [550, 1100, 1600, 2400],
    10: [600, 1200, 1900, 2800],
    11: [800, 1600, 2400, 3600],
    12: [1000, 2000, 3000, 4500],
    13: [1100, 2200, 3400, 5100],
    14: [1250, 2500, 3800, 5700],
    15: [1400, 2800, 4300, 6400],
    16: [1600, 3200, 4800, 7200],
    17: [2000, 3900, 5900, 8800],
    18: [2100, 4200, 6300, 9500],
    19: [2400, 4900, 7300, 10900],
    20: [2800, 5700, 8500, 12700]
}

# Dictionary of Multipliers related to number of monsters
multiplierDict = {
    1: 1,
    2: 1.5,
    3: 2,
    4: 2,
    5: 2,
    6: 2,
    7: 2.5,
    8: 2.5,
    9: 2.5,
    10: 2.5,
    11: 3,
    12: 3,
    13: 3,
    14: 3,
    15: 4
}

def calculateDifficultiesExp(partyLevels):
    """
    Calculate the difficulty thresholds for a party based on their levels.
    Parameters:
    partyLevels (list): A list of integers representing the levels of each party member.
    Returns:
    dict: A dictionary containing the difficulty thresholds for 'easy', 'medium', 'hard', and 'deadly'.
    """
    totalExpPoints = [0, 0, 0, 0]  # easy, medium, hard, deadly

    for level in partyLevels:
        if level in expPointsDict:
            expPoints = expPointsDict[level]

            for i in range(4):
                totalExpPoints[i] += expPoints[i]
        else:
            raise ValueError(f"Level {level} is out of range. Valid levels are 1 to 20.")
    
    return {
        "easy": totalExpPoints[0],
        "medium": totalExpPoints[1],
        "hard": totalExpPoints[2],
        "deadly": totalExpPoints[3]
    }

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
    
    if numMonsters in multiplierDict:
        multiplier = multiplierDict[numMonsters]
    elif numMonsters > 15:
        multiplier = 4
    else:
        raise ValueError(f"Number of monsters {numMonsters} is out of range. Valid range is 1 to 15 or more.")
    
    adjustedExp = int(totalExp * multiplier)
    
    return adjustedExp