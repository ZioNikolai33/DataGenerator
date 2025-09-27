from database import *
from entities.AbilityBonus import *

class Subrace:
    def __init__ (self, subrace):
        self.name = subrace["index"]
        self.race = subrace["race"]["index"]
        self.abilityScores = [AbilityBonus(item) for item in subrace["ability_bonuses"]]
        self.proficiencies = [item["index"] for item in subrace["starting_proficiencies"]]
        self.traits = [item["index"] for item in subrace["racial_traits"]]

subraceStats = [Subrace(item) for item in list(Database.getAllSubraces(db))]

class Race:
    def __init__(self, race):
        self.name = race["index"]
        self.abilityBonuses = [AbilityBonus(item) for item in race["ability_bonuses"]]
        self.proficiencies = [item["index"] for item in race["starting_proficiencies"]]
        self.traits = [item["index"] for item in race["traits"]]
        self.subraces = [item for item in subraceStats if item.race == self.name]
        self.speed = race["speed"]

raceStats = [Race(item) for item in list(Database.getAllRaces(db))]