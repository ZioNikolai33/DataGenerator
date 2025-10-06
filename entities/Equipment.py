from utilities.lists import *

class Equipment:
    def __init__(self, index, quantity):
        self.name = index
        self.quantity = quantity

        for item in weapons:
            if item["index"] == self.name:
                weapon = item
                break

        self.equipmentCategory = item["equipment_category"]["index"]
        self.weaponCategory = item["weapon_category"] if "weapon_category" in item else None
        self.weaponRange = item["weapon_range"] if "weapon_range" in item else None
        self.categoryRange = item["category_range"]
        self.cost = item["cost"]["quantity"]
        self.costUnit = item["cost"]["unit"]
        self.damage = item["damage"]["damage_dice"] if "damage" in item else None
        self.damageType = item["damage"]["damage_type"]["index"] if "damage" in item else None
        self.rangeNormal = item["range"]["normal"] if "range" in item else None
        self.properties = [item["index"] for item in item["properties"]]

    def __str__(self):
        return f"{self.name}"