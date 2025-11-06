import random

class Choice:
    def __init__(self, number, choices):
        self.number = number
        self.choices = choices

    def getRandomChoice(self, items = None):
        if items is not None:
            filtered_choices = [choice for choice in self.choices if choice not in items]
        else:
            filtered_choices = self.choices

        if len(filtered_choices) <= self.number:
            return filtered_choices

        return random.sample(filtered_choices, self.number)

    def getRandomChoiceWithoutCheck(self):
        if len(self.choices) <= self.number:
            return self.choices

        return random.sample(self.choices, self.number)

    def __str__(self):
        return f"{self.number} - {', '.join(str(choice) for choice in self.choices)}"