from database import *
from entities.Equipment import *
from entities.Multiclass import *
from entities.Spell import *
from entities.Feature import *
from entities.Choice import *

categories = list(Database.getAllCategories(db))
weapons = list(Database.getAllWeapons(db))
classes = list(Database.getAllClasses(db))
weaponNames = [item["index"] for item in weapons]
proficiencyChoices = {}

for item in classes:
    class_index = item["index"]
    profs = []    
    proficiencyOptions = [i for i in item["proficiency_choices"][0]["from"]["options"]]

    for options in proficiencyOptions:
        profs.append(options["item"]["index"])
                        
    proficiencyChoices[class_index] = profs

class Class:
    def __init__(self, classe):
        self.name = classe["index"]
        self.hp = classe["hit_die"]
        self.subclasses = [item["index"] for item in classe["subclasses"]]
        self.proficiencyChoices = [Choice(classe["proficiency_choices"][0]["choose"], proficiencyChoices[classe["index"]])]
        self.savingThrows = [item["index"] for item in classe["saving_throws"]]
        self.proficiencies = [item["index"] for item in classe["proficiencies"]]
        self.features = [feature for feature in featureStats if feature.classe == self.name]
        self.startingEquipments = [Equipment(item["equipment"]["index"], item["equipment"]["quantity"]) for item in classe["starting_equipment"] if item["equipment"]["index"] in weapons]
        self.startingEquipmentsOptions = self.getEquipmentOptions(classe["starting_equipment_options"])
        self.multiclassing = Multiclass(classe["multi_classing"])
        self.spellcastingAbility = classe["spellcasting"]["spellcasting_ability"]["index"] if "spellcasting" in classe else None
        self.spells = [item for item in spellStats if self.name in item.classes]

    def getEquipmentOptions(self, startingEquip):
        equipmentOptions = []

        for item in startingEquip:
            numChoices = item["choose"]
            optionList = []

            if "options" in item["from"]:
                for option in item["from"]["options"]:
                    if option["option_type"] == "counted_reference" and option["of"]["index"] in weapons:
                        equipmentOptions.append(Choice(numChoices, Equipment(option["of"]["index"], option["count"])))
                    elif option["option_type"] == "choice":
                        choiceNum = option["choice"]["choose"]
                        if option["choice"]["from"]["option_set_type"] == "equipment_category":
                            toChooseEquip = [item for item in categories if item["index"] == option["choice"]["from"]["equipment_category"]["index"]]
                            equipmentOptions.append(Choice(choiceNum, [Equipment(item["index"], 1) for item in toChooseEquip if item["index"] in weapons]))

        return equipmentOptions

classStats = [Class(item) for item in classes]