from math import e
from tkinter import SEL
from database import *
from entities.Equipment import *
from entities.Multiclass import *
from entities.Spell import *
from entities.Feature import *
from entities.Choice import *
from entities.Slots import *
from utilities.lists import *

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
        self.startingEquipmentsOptions = self.getEquipmentOptions()
        self.multiclassing = Multiclass(classe["multi_classing"])
        self.spellcastingAbility = classe["spellcasting"]["spellcasting_ability"]["index"] if "spellcasting" in classe else None
        self.spells = [item for item in spellStats if self.name in item.classes]

    def getEquipmentOptions(self):
        equipmentOptions = []
        choices = []

        if self.name == "barbarian":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("greataxe", 1))
            else:
                for item in martialMeleeWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("handaxe", 2))
            else:
                for item in simpleWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

        if self.name == "bard":
            selection = random.randint(0, 2)
            if selection == 0:
                equipmentOptions.append(Equipment("rapier", 1))
            elif selection == 1:
                equipmentOptions.append(Equipment("longsword", 1))
            else:
                for item in simpleWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

        if self.name == "cleric":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("mace", 1))
            else:
                equipmentOptions.append(Equipment("warhammer", 1))

            selection = random.randint(0, 2)
            if selection == 0:
                equipmentOptions.append(Equipment("scale-mail", 1))
            elif selection == 1:
                equipmentOptions.append(Equipment("leather-armor", 1))
            else:
                equipmentOptions.append(Equipment("chain-mail", 1))

            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("light-crossbow", 1))
                equipmentOptions.append(Equipment("crossbow-bolt", 20))
            else:
                for item in simpleWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

        if self.name == "druid":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("wooden-shield", 1))
            else:
                for item in simpleWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("scimitar", 1))
            else:
                for item in simpleMeleeWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

        if self.name == "fighter":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("chain-mail", 1))
            else:
                equipmentOptions.append(Equipment("leather-armor", 1))
                equipmentOptions.append(Equipment("longbow", 1))
                equipmentOptions.append(Equipment("arrow", 20))

            selection = random.randint(0, 1)
            if selection == 0:
                for item in martialWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)
                equipmentOptions.append(Equipment("shield", 1))
            else:
                for item in martialWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 2)

            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("light-crossbow", 1))
                equipmentOptions.append(Equipment("crossbow-bolt", 20))
            else:
                equipmentOptions.append(Equipment("handaxe", 2))

        if self.name == "monk":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("shortsword", 1))
            else:
                for item in simpleWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

        if self.name == "paladin":
            selection = random.randint(0, 1)
            if selection == 0:
                for item in martialWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)
                equipmentOptions.append(Equipment("shield", 1))
            else:
                for item in martialWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 2)

            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("javelin", 5))
            else:
                for item in simpleMeleeWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

        if self.name == "ranger":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("scale-mail", 1))
            else:
                equipmentOptions.append(Equipment("leather-armor", 1))

            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("shortsword", 2))
            else:
                for item in simpleMeleeWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 2)
            
        if self.name == "rogue":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("rapier", 1))
            else:
                equipmentOptions.append(Equipment("shortsword", 1))

            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("shortbow", 1))
                equipmentOptions.append(Equipment("arrow", 20))
            else:
                equipmentOptions.append(Equipment("shortsword", 1))

        if self.name == "sorcerer":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("light-crossbow", 1))
                equipmentOptions.append(Equipment("crossbow-bolt", 20))
            else:
                for item in simpleWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

        if self.name == "warlock":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("light-crossbow", 1))
                equipmentOptions.append(Equipment("crossbow-bolt", 20))
            else:
                for item in simpleWeapons:
                    choices.append(Choice(Equipment(item["index"], 1)), 1)

            for item in simpleWeapons:
                choices.append(Choice(Equipment(item["index"], 1)), 1)

        if self.name == "wizard":
            selection = random.randint(0, 1)
            if selection == 0:
                equipmentOptions.append(Equipment("quarterstaff", 1))
            else:
                equipmentOptions.append(Equipment("dagger", 1))

        if len(choices) > 0:
            for choice in choices:
                equipmentOptions.extend(choice.getRandomChoice(equipmentOptions))

        return equipmentOptions

    def getSpellSlots(self, level):
        spellSlots = Slots()

        if self.name in ["bard", "cleric", "druid", "sorcerer", "wizard"]:
            if level == 1:
                spellSlots = Slots(2)
            elif level == 2:
                spellSlots = Slots(3)
            elif level == 3:
                spellSlots = Slots(4, 2)
            elif level == 4:
                spellSlots = Slots(4, 3)
            elif level == 5:
                spellSlots = Slots(4, 3, 2)
            elif level == 6:
                spellSlots = Slots(4, 3, 3)
            elif level == 7:
                spellSlots = Slots(4, 3, 3, 1)
            elif level == 8:
                spellSlots = Slots(4, 3, 3, 2)
            elif level == 9:
                spellSlots = Slots(4, 3, 3, 3, 1)
            elif level == 10:
                spellSlots = Slots(4, 3, 3, 3, 2)
            elif level == 11 or level == 12:
                spellSlots = Slots(4, 3, 3, 3, 2, 1)
            elif level == 13 or level == 14:
                spellSlots = Slots(4, 3, 3, 3, 2, 1, 1)
            elif level == 15 or level == 16:
                spellSlots = Slots(4, 3, 3, 3, 2, 1, 1, 1)
            elif level == 17:
                spellSlots = Slots(4, 3, 3, 3, 2, 1, 1, 1, 1)
            elif level == 18:
                spellSlots = Slots(4, 3, 3, 3, 3, 1, 1, 1, 1)
            elif level == 19:
                spellSlots = Slots(4, 3, 3, 3, 3, 2, 1, 1, 1)
            elif level == 20:
                spellSlots = Slots(4, 3, 3, 3, 3, 2, 2, 1, 1)
        elif self.name in ["paladin", "ranger"]:
            if level == 2:
                spellSlots = Slots(2)
            elif level == 3:
                spellSlots = Slots(3)
            elif level == 4:
                spellSlots = Slots(3)
            elif level == 5 or level == 6:
                spellSlots = Slots(4, 2)
            elif level == 7 or level == 8:
                spellSlots = Slots(4, 3)
            elif level == 9 or level == 10:
                spellSlots = Slots(4, 3, 2)
            elif level == 11 or level == 12:
                spellSlots = Slots(4, 3, 3)
            elif level == 13 or level == 14:
                spellSlots = Slots(4, 3, 3, 1)
            elif level == 15 or level == 16:
                spellSlots = Slots(4, 3, 3, 2)
            elif level == 17 or level == 18:
                spellSlots = Slots(4, 3, 3, 3, 1)
            elif level == 19 or level == 20:
                spellSlots = Slots(4, 3, 3, 3, 2)

        return spellSlots


classStats = [Class(item) for item in classes]