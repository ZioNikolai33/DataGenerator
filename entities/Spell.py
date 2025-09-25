from entities.Area import *
from entities.DifficultyClass import *
from database import *

class SpellDamage:
    def __init__(self, damage):
        self.damageType = damage["damage_type"]["index"] if "damage_type" in damage else None
        self.damageSlots = damage["damage_at_slot_level"] if "damage_at_slot_level" in damage else None
        self.damageAtCharacterLevel = damage["damage_at_character_level"] if "damage_at_character_level" in damage else None

class Spell:
    def __init__(self, spell):
        self.name = spell["index"]
        self.range = spell["range"]
        self.ritual = spell["ritual"]
        self.duration = spell["duration"]
        self.concentration = spell["concentration"]
        self.castingTime = spell["casting_time"]
        self.healAtSlotLevel = spell["heal_at_slot_level"] if "heal_at_slot_level" in spell else None
        self.school = spell["school"]["index"]
        self.classes = [item["index"] for item in spell["classes"]]
        self.subclasses = [item["index"] for item in spell["subclasses"]] if "subclasses" in spell else None
        self.areaEffect = Area(spell["area_of_effect"]) if "area_of_effect" in spell else None
        self.dc = DifficultyClass(spell["dc"]) if "dc" in spell else None
        self.damage = SpellDamage(spell["damage"]) if "damage" in spell else None
        self.attackType = spell["attack_type"] if "attack_type" in spell else None

spellStats = [Spell(item) for item in list(Database.getAllSpells(db))]