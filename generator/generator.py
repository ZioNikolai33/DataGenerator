from utilities.dataManipulation import *
from database import *
import pandas as pd
import json

def generate():
    db = database()
    monstersList = getMonstersDifficultiesList(db)