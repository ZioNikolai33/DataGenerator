class Attribute:
    def __init__(self, value):
        self.value = value
        self.modifier = (value - 10) // 2
        self.save = (value - 10) // 2
