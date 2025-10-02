import random

class Choice:
    def __init__(self, number, choices):
        self.number = number
        self.choices = choices

    def getRandomChoice(self, proficiencies):
        filtered_choices = [choice for choice in self.choices if choice not in proficiencies]

        return random.sample(filtered_choices, self.number)