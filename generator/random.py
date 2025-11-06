from entities.CRRatios import *
from entities.party import *

import random

def getRandomDifficulty():
    return random.choice(list(CRRatios))

def getRandomParty():
    numMembers = random.randint(1, 7) # Number of party members
    section = random.randint(1, 4) # Section of levels to randomize levels organically - 1: 1-5; 2: 6-10; 3: 11-15; 4: 16-20
    partyLevels = [random.randint((5*section)-4, 5*section) for _ in range(numMembers)] # Generate random levels for each member, related to section
    party = [Member(i, 20) for i in range(12)] #[Member(i, partyLevels[i]) for i in range(numMembers)]

    for item in party:
        print(str(item))