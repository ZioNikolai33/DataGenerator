class Slots():
    def __init__(self, first=0, second=0, third=0, fourth=0, fifth=0, sixth=0, seventh=0, eighth=0, nineth=0):
        self.first = first
        self.second = second
        self.third = third
        self.fourth = fourth
        self.fifth = fifth
        self.sixth = sixth
        self.seventh = seventh
        self.eighth = eighth
        self.nineth = nineth

    def __str__(self):
        string = "Spell Slots:\n"
        string += f" 1st Level: {self.first}\n" if self.first > 0 else ""
        string += f" 2nd Level: {self.second}\n" if self.second > 0 else ""
        string += f" 3rd Level: {self.third}\n" if self.third > 0 else ""
        string += f" 4th Level: {self.fourth}\n" if self.fourth > 0 else ""
        string += f" 5th Level: {self.fifth}\n" if self.fifth > 0 else ""
        string += f" 6th Level: {self.sixth}\n" if self.sixth > 0 else ""
        string += f" 7th Level: {self.seventh}\n" if self.seventh > 0 else ""
        string += f" 8th Level: {self.eighth}\n" if self.eighth > 0 else ""
        string += f" 9th Level: {self.nineth}\n" if self.nineth > 0 else ""

        return string
