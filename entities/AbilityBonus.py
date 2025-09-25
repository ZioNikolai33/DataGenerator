class AbilityBonus:
    def __init__(self, item):
        self.name = item["ability_score"]["index"]
        self.bonus = item["bonus"]