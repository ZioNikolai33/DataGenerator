import random

class Choice:
    def __init__(self, number, choices):
        self.number = number
        self.choices = choices

    def getRandomChoice(self):
        return random.sample(self.choices, self.number)