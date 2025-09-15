from utilities.dataManipulation import *
from entities.CRRatios import *
from generator.random import *

import pandas as pd
import random
import json

def generate():
    # Select a random difficulty bucket - bucket.value to get number
    bucket = random.choice(list(CRRatios))    
    party = getRandomParty()
    monstersList = getMonstersDifficultiesList()