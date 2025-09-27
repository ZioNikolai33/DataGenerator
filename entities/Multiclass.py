from entities.Choice import *

class Multiclass:
    def __init__(self, classe):
        if "prerequisities" in classe:
            self.prerequisitiesAttribute = [item["ability_score"]["index"] for item in classe["prerequisities"] if "prerequisities" in classe]
            self.prerequisitiesValue = [item["minimum_score"] for item in classe["prerequisities"] if "prerequisities" in classe]
            self.numChoices = 0
        elif "prerequisite_options" in classe:
            self.prerequisitiesAttribute = [item["ability_score"]["index"] for item in classe["prerequisite_options"]["from"]["options"]]
            self.prerequisitiesValue = [item["minimum_score"] for item in classe["prerequisite_options"]["from"]["options"]]
            self.numChoices = classe["prerequisite_options"]["choose"]
        self.proficiencies = [item["index"] for item in classe["proficiencies"]]
        self.proficiencyChoices = []

        if "proficiency_choices" in classe:
            for prof in classe["proficiency_choices"]:
                self.proficiencyChoices.append(Choice(prof["choose"], [item["item"]["index"] for item in prof["from"]["options"]]))
