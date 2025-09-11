class Difficulty():
    def __init__(self, easy=0, medium=0, hard=0, deadly=0):
        self.easy = easy
        self.medium = medium
        self.hard = hard
        self.deadly = deadly

    def __str__(self):
        return f"easy={self.easy}, medium={self.medium}, hard={self.hard}, deadly={self.deadly}"
