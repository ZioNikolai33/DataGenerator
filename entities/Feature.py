from database import *
from entities.Choice import *
import random

class Feature:
    def __init__(self, feature):
        self.name = feature["index"]
        self.classe = feature["class"]["index"]
        self.subclass = feature["subclass"]["index"] if "subclass" in feature else None
        self.level = feature["level"]
        self.prerequisites = feature["prerequisites"]
        self.featureSpecificChoices, self.featureSpecificType = self.getFeatureSpecific(feature)

    def getFeatureSpecific(self, feature):
        featureSpecificType = ""
        featureSpecificChoices = []

        if "feature_specific" in feature:
            if "expertise_options" in feature["feature_specific"]:
                featureSpecificType = "expertise"
                expertiseOptions = feature["feature_specific"]["expertise_options"]["from"]["options"]

                if "items" in expertiseOptions[1] and "choice" in expertiseOptions[0]:
                    featureSpecificChoices.append(Choice(expertiseOptions[0]["choice"]["choose"], [item["item"]["index"] for item in expertiseOptions[0]["choice"]["from"]["options"]]))
                    featureSpecificChoices.append(Choice(expertiseOptions[1]["items"][0]["choice"]["choose"], [item["item"]["index"] for item in expertiseOptions[1]["items"][0]["choice"]["from"]["options"]].append([item["item"]["index"] for item in expertiseOptions[1]["items"][0]["choice"]["from"]["options"]])))
                else:
                    featureSpecificChoices.append(Choice(feature["feature_specific"]["expertise_options"]["choose"], [item["item"]["index"] for item in expertiseOptions]))
            elif "subfeature_options" in feature["feature_specific"]:
                featureSpecificType = "subfeature_options"
                featureSpecificChoices.append(Choice(feature["feature_specific"]["subfeature_options"]["choose"], [item["item"]["index"] for item in feature["feature_specific"]["subfeature_options"]["from"]["options"]]))
            elif "enemy_type_options" in feature["feature_specific"]:
                featureSpecificType = "enemy_type_options"
                featureSpecificChoices.append(Choice(feature["feature_specific"]["enemy_type_options"]["choose"], [item for item in feature["feature_specific"]["enemy_type_options"]["from"]["options"]]))
            elif "terrain_type_options" in feature["feature_specific"]:
                featureSpecificType = "terrain_type_options"
                featureSpecificChoices.append(Choice(feature["feature_specific"]["terrain_type_options"]["choose"], [item for item in feature["feature_specific"]["terrain_type_options"]["from"]["options"]]))
            elif "invocations" in feature["feature_specific"]:
                featureSpecificType = "invocations"
                featureSpecificChoices.append(Choice(self.getInvocationsNum(), [item["index"] for item in feature["feature_specific"]["invocations"]]))

        return featureSpecificChoices, featureSpecificType

    def getInvocationsNum(self):
        if self.level > 17:
            return 8
        elif self.level > 14:
            return 7
        elif self.level > 11:
            return 6
        elif self.level > 8:
            return 5
        elif self.level > 6:
            return 4
        elif self.level > 3:
            return 3
        elif self.level > 1:
            return 2
        else:
            return 0

featureStats = [Feature(item) for item in list(Database.getAllFeatures(db))]