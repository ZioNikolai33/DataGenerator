from database import *
from entities.AbilityBonus import *

class Race:
    def __init__(self, race):
        self.name = race["index"]
        self.abilityBonuses = [AbilityBonus(item) for item in race["ability_bonuses"]]
        self.proficiencies = [item["index"] for item in race["starting_proficiencies"]]
        self.traits = [item["index"] for item in race["traits"]]
        self.subraces = [item["index"] for item in race["subraces"]]
        self.speed = race["speed"]

raceStats = [Race(item) for item in list(Database.getAllRaces(db))]