from hmac import new

from pymongo.synchronous.helpers import F
from database import *
from entities.Race import *
from entities.Class import *
from entities.Attribute import *
import random

class Member:
    def __init__(self, id, level):
        randomRace = random.choice(raceStats)
        randomClass = random.choice(classStats)
        attributes = self.addAbilityScores(randomRace, self.assumeAttributes(randomClass))

        self.id = id
        self.name = "Member " + str(id)
        self.level = level
        self.race = randomRace.name
        self.subrace = random.choice(randomRace.subraces) if len(randomRace.subraces) > 0 else ""
        self.speed = randomRace.speed
        self.classe = randomClass.name
        self.strength = Attribute(attributes[0])
        self.dexterity = Attribute(attributes[1])
        self.constitution = Attribute(attributes[2])
        self.intelligence = Attribute(attributes[3])
        self.wisdom = Attribute(attributes[4])
        self.charisma = Attribute(attributes[5])
        self.hp = self.calculateRandomHp(randomClass.hp)
        self.subclass = random.choice(randomClass.subclasses)
        self.initiative = self.dexterity.modifier
        self.proficiencyBonus = self.getProfBonus()
        self.proficiencies = randomClass.proficiencies
        self.proficiencies.extend(randomRace.proficiencies)

        for item in randomClass.proficiencyChoices:
            chosen = item.getRandomChoice(self.proficiencies)
            self.proficiencies.extend(chosen)

        self.equipments = randomClass.startingEquipments

        if len(randomClass.startingEquipmentsOptions) > 0:
            for item in randomClass.startingEquipmentsOptions:
                self.equipments.append(item)

        self.features = [item for item in randomClass.features if item.level == self.level]

        self.featureSpecifics = []
        self.masteries = []
        self.vulnerabilities = []
        self.resistances = []
        self.immunities = []

        for item in randomClass.proficiencyChoices:
            self.proficiencies.extend(random.sample(item.choices, item.number))

        self.setFeatureSpecifics()
        self.addProfToSavings(randomClass)
        self.skills = self.getSkillsDict()
        self.spellSlots = randomClass.getSpellSlots(self.level)
        self.spells = [item for item in randomClass.spells if self.spellSlots[item.level] > 0]

    def calculateRandomHp(self, hitDie):
        hp = hitDie + self.constitution.modifier

        for i in range(2, self.level):
            hp += random.randint(1, hitDie) + self.constitution.modifier

        return hp

    def assumeAttributes(self, randomClass):
        attributes = [15, 14, 13, 12, 10, 8] # Just assume standard array
        random.shuffle(attributes)

        return attributes

    def getProfBonus(self):
        return 2 + ((self.level - 1) // 4)

    def addProfToSavings(self, classe):
        self.strength.save += self.proficiencyBonus if "str" in classe.savingThrows else 0
        self.dexterity.save += self.proficiencyBonus if "dex" in classe.savingThrows else 0
        self.constitution.save += self.proficiencyBonus if "con" in classe.savingThrows else 0
        self.intelligence.save += self.proficiencyBonus if "int" in classe.savingThrows else 0
        self.wisdom.save += self.proficiencyBonus if "wis" in classe.savingThrows else 0
        self.charisma.save += self.proficiencyBonus if "cha" in classe.savingThrows else 0

    def addAbilityScores(self, race, attributes):
        for item in race.abilityBonuses:
            if item.name == "str":
                attributes[0] += item.bonus
            elif item.name == "dex":
                attributes[1] += item.bonus
            elif item.name == "con":
                attributes[2] += item.bonus
            elif item.name == "int":
                attributes[3] += item.bonus
            elif item.name == "wis":
                attributes[4] += item.bonus
            elif item.name == "cha":
                attributes[5] += item.bonus

        return attributes

    def getSkillsDict(self):
        skills = {
            "acrobatics": self.dexterity.modifier,
            "animal_handling": self.wisdom.modifier,
            "arcana": self.intelligence.modifier,
            "athletics": self.strength.modifier,
            "deception": self.charisma.modifier,
            "history": self.intelligence.modifier,
            "insight": self.wisdom.modifier,
            "intimidation": self.charisma.modifier,
            "investigation": self.intelligence.modifier,
            "medicine": self.wisdom.modifier,
            "nature": self.intelligence.modifier,
            "perception": self.wisdom.modifier,
            "performance": self.charisma.modifier,
            "persuasion": self.charisma.modifier,
            "religion": self.intelligence.modifier,
            "sleight_of_hand": self.dexterity.modifier,
            "stealth": self.dexterity.modifier,
            "survival": self.wisdom.modifier
        }

        for index, item in enumerate(skills):
            if "skill-" + item in self.proficiencies:
                skills[item] += self.proficiencyBonus

        for index, item in enumerate(skills):
            if "skill-" + item in self.masteries:
                skills[item] += self.proficiencyBonus

        return skills

    def setFeatureSpecifics(self):
        for feature in self.features:
            for item in feature.featureSpecificChoices:
                if feature.featureSpecificType == "subfeature_options":
                    choices = item.choices

                    for choice in item.choices:
                        if choice in self.featureSpecifics:
                            choices = [item for item in choices if item != choice]

                    self.featureSpecifics.extend(random.sample(item.choices, item.number))
                elif feature.featureSpecificType == "expertise":
                    choices = item.choices

                    for choice in item.choices:
                        if choice in self.masteries:
                            choices = [item for item in choices if item != choice]

                    self.masteries.extend(random.sample(item.choices, item.number))
                elif feature.featureSpecificType == "enemy_type_options":
                    choices = item.choices

                    for choice in item.choices:
                        if choice in self.featureSpecifics:
                            choices = [item for item in choices if item != choice]

                    self.featureSpecifics.extend(random.sample(item.choices, item.number))
                elif feature.featureSpecificType == "terrain_type_options":
                    choices = item.choices

                    for choice in item.choices:
                        if choice in self.featureSpecifics:
                            choices = [item for item in choices if item != choice]

                    self.featureSpecifics.extend(random.sample(item.choices, item.number))
                elif feature.featureSpecificType == "invocations":
                    choices = item.choices

                    for choice in item.choices:
                        if choice in self.featureSpecifics:
                            choices = [item for item in choices if item != choice]

                    self.featureSpecifics.extend(random.sample(item.choices, item.number))

    def removeNotUsedFeature(self):
        if self.classe == "bard":
            self.features = [feature for feature in self.features if feature.name != "spellcasting-bard"]

            if self.level >= 15:
                self.features = [feature for feature in self.features if feature.name != "bardic-inspiration-d10" and feature.name != "bardic-inspiration-d8" and feature.name != "bardic-inspiration-d6"]
            elif self.level >= 10:
                self.features = [feature for feature in self.features if feature.name != "bardic-inspiration-d8" and feature.name != "bardic-inspiration-d6"]
            elif self.level >= 5:
                self.features = [feature for feature in self.features if feature.name != "bardic-inspiration-d6"]

            if self.level >= 17:
                self.features = [feature for feature in self.features if feature.name != "song-of-rest-d10" and feature.name != "song-of-rest-d8" and feature.name != "song-of-rest-d6"]
            elif self.level >= 13:
                self.features = [feature for feature in self.features if feature.name != "song-of-rest-d8" and feature.name != "song-of-rest-d6"]
            elif self.level >= 9:
                self.features = [feature for feature in self.features if feature.name != "song-of-rest-d6"]

        if self.classe == "cleric":
            self.features = [feature for feature in self.features if feature.name != "spellcasting-cleric"]

            if self.level >= 18:
                self.features = [feature for feature in self.features if feature.name != "channel-divinity-2-rest" and feature.name != "channel-divinity-1-rest"]
            elif self.level >= 6:
                self.features = [feature for feature in self.features if feature.name != "channel-divinity-1-rest"]

            if self.level >= 17:
                self.features = [feature for feature in self.features if feature.name != "destroy-undead-cr-3-or-below" and feature.name != "destroy-undead-cr-2-or-below" and feature.name != "destroy-undead-cr-1-or-below" and feature.name != "destroy-undead-cr-1-2-or-below"]
            elif self.level >= 14:
                self.features = [feature for feature in self.features if feature.name != "destroy-undead-cr-2-or-below" and feature.name != "destroy-undead-cr-1-or-below" and feature.name != "destroy-undead-cr-1-2-or-below"]
            elif self.level >= 11:
                self.features = [feature for feature in self.features if feature.name != "destroy-undead-cr-1-or-below" and feature.name != "destroy-undead-cr-1-2-or-below"]
            elif self.level >= 8:
                self.features = [feature for feature in self.features if feature.name != "destroy-undead-cr-1-2-or-below"]

        if self.classe == "druid":
            self.features = [feature for feature in self.features if feature.name != "spellcasting-druid"]

            if self.level >= 2:
                self.features = [feature for feature in self.features if feature.name != "circle-of-the-land-arctic" and feature.name != "circle-of-the-land-coast" and feature.name != "circle-of-the-land-desert" and feature.name != "circle-of-the-land-forest" and feature.name != "circle-of-the-land-grassland" and feature.name != "circle-of-the-land-mountain" and feature.name != "circle-of-the-land-swamp"]

            if self.level >= 8:
                self.features = [feature for feature in self.features if feature.name != "wild-shape-cr-1-2-or-below-no-flying-speed" and feature.name != "wild-shape-cr-1-4-or-below-no-flying-or-swim-speed"]
            elif self.level >= 4:
                self.features = [feature for feature in self.features if feature.name != "wild-shape-cr-1-2-or-below-no-flying-speed"]

        if self.classe == "fighter":
            self.features = [feature for feature in self.features if feature.name != "fighter-fighting-style-archery" and feature.name != "fighter-fighting-style-defense" and feature.name != "fighter-fighting-style-dueling" and feature.name != "fighter-fighting-style-great-weapon-fighting" and feature.name != "fighter-fighting-style-protection" and feature.name != "fighter-fighting-style-two-weapon-fighting"]

            if self.level >= 17:
                self.features = [feature for feature in self.features if feature.name != "action-surge-1-use"]

            if self.level >= 20:
                self.features = [feature for feature in self.features if feature.name != "extra-attack-2" and feature.name != "extra-attack-1"]
            elif self.level >= 11:
                self.features = [feature for feature in self.features if feature.name != "extra-attack-1"]

            if self.level >= 17:
                self.features = [feature for feature in self.features if feature.name != "indomitable-2-uses" and feature.name != "indomitable-1-use"]
            elif self.level >= 13:
                self.features = [feature for feature in self.features if feature.name != "indomitable-1-use"]

        if self.classe == "monk":
            if self.level >= 9:
                self.features = [feature for feature in self.features if feature.name != "unarmored-movement-1"]

        if self.classe == "paladin":
            if self.level >= 2:
                self.features = [feature for feature in self.features if feature.name != "spellcasting-paladin"]

            self.features = [feature for feature in self.features if feature.name != "fighting-style-defense" and feature.name != "fighting-style-dueling" and feature.name != "fighting-style-great-weapon-fighting" and feature.name != "fighting-style-protection"]

            if self.level >= 11:
                self.features = [feature for feature in self.features if feature.name != "divine-smite"]

        if self.classe == "ranger":
            if self.level >= 2:
                self.features = [feature for feature in self.features if feature.name != "spellcasting-ranger"]

            if self.level >= 2:
                self.features = [feature for feature in self.features if feature.name != "ranger-fighting-style-archery" and feature.name != "ranger-fighting-style-defense" and feature.name != "ranger-fighting-style-dueling" and feature.name != "ranger-fighting-style-two-weapon-fighting"]

            if self.level >= 3:
                self.features = [feature for feature in self.features if feature.name != "hunters-prey-colossus-slayer" and feature.name != "hunters-prey-giant-killer" and feature.name != "hunters-prey-horde-breaker"]

            if self.level >= 14:
                self.features = [feature for feature in self.features if feature.name != "favored-enemy-2-enemies" and feature.name != "favored-enemy-1-enemies"]
            elif self.level >= 6:
                self.features = [feature for feature in self.features if feature.name != "favored-enemy-1-enemies"]

            if self.level >= 10:
                self.features = [feature for feature in self.features if feature.name != "natural-explorer-2-terrain-types" and feature.name != "natural-explorer-1-terrain-types"]
            elif self.level >= 6:
                self.features = [feature for feature in self.features if feature.name != "natural-explorer-1-terrain-types"]

            if self.level >= 7:
                self.features = [feature for feature in self.features if feature.name != "defensive-tactics-escape-the-horde" and feature.name != "defensive-tactics-multiattack-defense" and feature.name != "defensive-tactics-steel-will"]

            if self.level >= 11:
                self.features = [feature for feature in self.features if feature.name != "multiattack-volley" and feature.name != "multiattack-whirlwind-attack"]

            if self.level >= 15:
                self.features = [feature for feature in self.features if feature.name != "superior-hunters-defense-evasion" and feature.name != "superior-hunters-defense-stand-against-the-tide" and feature.name != "superior-hunters-defense-uncanny-dodge"]

        if self.classe == "sorcerer":
            self.features = [feature for feature in self.features if feature.name != "spellcasting-sorcerer"]
            self.features = [feature for feature in self.features if feature.name != "dragon-ancestor-black---acid-damage" and feature.name != "dragon-ancestor-blue---lightning-damage" and feature.name != "dragon-ancestor-brass---fire-damage" and feature.name != "dragon-ancestor-bronze---lightning-damage" and feature.name != "dragon-ancestor-copper---acid-damage" and feature.name != "dragon-ancestor-gold---fire-damage" and feature.name != "dragon-ancestor-green---poison-damage" and feature.name != "dragon-ancestor-red---fire-damage" and feature.name != "dragon-ancestor-silver---cold-damage" and feature.name != "dragon-ancestor-white---cold-damage"]
            
            if self.level >= 3:
                self.features = [feature for feature in self.features if feature.name != "metamagic-careful-spell" and feature.name != "metamagic-distant-spell" and feature.name != "metamagic-empowered-spell" and feature.name != "metamagic-extended-spell" and feature.name != "metamagic-heightened-spell" and feature.name != "metamagic-quickened-spell" and feature.name != "metamagic-subtle-spell" and feature.name != "metamagic-twinned-spell" ]

        if self.classe == "warlock":
            self.features = [feature for feature in self.features if feature.name != "spellcasting-warlock"]

            if self.level >= 2:
                self.features = [feature for feature in self.features if feature.name != "eldritch-invocation-agonizing-blast" and feature.name != "eldritch-invocation-armor-of-shadows" and feature.name != "eldritch-invocation-beast-speech" and feature.name != "eldritch-invocation-beguiling-influence" and feature.name != "eldritch-invocation-book-of-ancient-secrets" and feature.name != "eldritch-invocation-devils-sight" and feature.name != "eldritch-invocation-eldritch-sight" and feature.name != "eldritch-invocation-eldritch-spear" and feature.name != "eldritch-invocation-eyes-of-the-rune-keeper" and feature.name != "eldritch-invocation-fiendish-vigor"]
                self.features = [feature for feature in self.features if feature.name != "eldritch-invocation-gaze-of-two-minds" and feature.name != "eldritch-invocation-mask-of-many-faces" and feature.name != "eldritch-invocation-misty-visions" and feature.name != "eldritch-invocation-repelling-blast" and feature.name != "eldritch-invocation-thief-of-five-fates" and feature.name != "eldritch-invocation-agonizing-blast" and feature.name != "eldritch-invocation-voice-of-the-chain-master" and feature.name != "eldritch-invocation-mire-the-mind" and feature.name != "eldritch-invocation-armor-of-shadows" and feature.name != "eldritch-invocation-one-with-shadows"]
                self.features = [feature for feature in self.features if feature.name != "eldritch-invocation-sign-of-ill-omen" and feature.name != "eldritch-invocation-thirsting-blade" and feature.name != "eldritch-invocation-bewitching-whispers" and feature.name != "eldritch-invocation-dreadful-word" and feature.name != "eldritch-invocation-sculptor-of-flesh" and feature.name != "eldritch-invocation-agonizing-blast" and feature.name != "eldritch-invocation-ascendant-step" and feature.name != "eldritch-invocation-minions-of-chaos" and feature.name != "eldritch-invocation-otherworldly-leap" and feature.name != "eldritch-invocation-whispers-of-the-grave"]
                self.features = [feature for feature in self.features if feature.name != "eldritch-invocation-lifedrinker" and feature.name != "eldritch-invocation-chains-of-carceri" and feature.name != "eldritch-invocation-master-of-myriad-forms" and feature.name != "eldritch-invocation-visions-of-distant-realms" and feature.name != "eldritch-invocation-witch-sight"]
            
            if self.level >= 3:
                self.features = [feature for feature in self.features if feature.name != "pact-of-the-chain" and feature.name != "pact-of-the-blade" and feature.name != "pact-of-the-tome"]

            if self.level >= 17:
                self.features = [feature for feature in self.features if feature.name != "mystic-arcanum-8th-level" and feature.name != "mystic-arcanum-7th-level" and feature.name != "mystic-arcanum-6th-level"]
            elif self.level >= 15:
                self.features = [feature for feature in self.features if feature.name != "mystic-arcanum-7th-level" and feature.name != "mystic-arcanum-6th-level"]
            elif self.level >= 13:
                self.features = [feature for feature in self.features if feature.name != "mystic-arcanum-6th-level"]

        if self.classe == "wizard":
            self.features = [feature for feature in self.features if feature.name != "spellcasting-wizard"]

    def __str__(self):
        subrace_str = f" {self.subrace.name}" if self.subrace != "" else ""

        string = f"{self.name} | {self.race}{subrace_str} | Lv{self.level} {self.classe}\n"
        string += f"HP: {self.hp} | Speed: {self.speed} | Initiative: {self.initiative} | Proficiency Bonus: +{self.proficiencyBonus}\n"
        string += f"STR: {self.strength.value} ({self.strength.modifier}) | DEX: {self.dexterity.value} ({self.dexterity.modifier}) | CON: {self.constitution.value} ({self.constitution.modifier}) | INT: {self.intelligence.value} ({self.intelligence.modifier}) | WIS: {self.wisdom.value} ({self.wisdom.modifier}) | CHA: {self.charisma.value} ({self.charisma.modifier})\n"
        string += f"Saving Throws: STR {self.strength.save}, DEX {self.dexterity.save}, CON {self.constitution.save}, INT {self.intelligence.save}, WIS {self.wisdom.save}, CHA {self.charisma.save}\n"
        string += f"Skills: {', '.join([f'{key.replace('_', ' ').title()} {value}' for key, value in self.skills.items()])}\n"
        string += f"Proficiencies: {', '.join([item if isinstance(item, str) else ', '.join(item) for item in self.proficiencies])}\n"
        if len(self.masteries) > 0:
            string += f"Masteries: {', '.join([item if isinstance(item, str) else ', '.join(item) for item in self.masteries])}\n"
        string += f"Traits: {', '.join(self.subrace.traits) if self.subrace != '' else 'None'}\n"
        string += f"Features: {', '.join([item.name for item in self.features])}\n"
        string += f"Equipments: {', '.join([f'{item.quantity} x {item.name.replace('-', ' ').title()}' for item in self.equipments])}\n"
        if len(self.vulnerabilities) > 0:
            string += f"Vulnerabilities: {', '.join(self.vulnerabilies)}\n"
        if len(self.resistances) > 0:
            string += f"Resistances: {', '.join(self.resistances)}\n"
        if len(self.immunities) > 0:
            string += f"Immunities: {', '.join(self.immunities)}\n"
        string += str(self.spellSlots) if self.spellSlots.first > 0 else ""
        string += "\n------------------------------------------------------------------------------------------------------------------------\n"

        return string