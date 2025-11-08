namespace TrainDataGen.Entities
{
    public class Slots
    {
        public int First { get; set; }
        public int Second { get; set; }
        public int Third { get; set; }
        public int Fourth { get; set; }
        public int Fifth { get; set; }
        public int Sixth { get; set; }
        public int Seventh { get; set; }
        public int Eighth { get; set; }
        public int Nineth { get; set; }

        public Slots(
            int first = 0, int second = 0, int third = 0, int fourth = 0,
            int fifth = 0, int sixth = 0, int seventh = 0, int eighth = 0, int nineth = 0)
        {
            First = first;
            Second = second;
            Third = third;
            Fourth = fourth;
            Fifth = fifth;
            Sixth = sixth;
            Seventh = seventh;
            Eighth = eighth;
            Nineth = nineth;
        }

        public bool HasEnoughSlots(int level, int numberToCheck)
        {
            return level switch
            {
                1 => First >= numberToCheck,
                2 => Second >= numberToCheck,
                3 => Third >= numberToCheck,
                4 => Fourth >= numberToCheck,
                5 => Fifth >= numberToCheck,
                6 => Sixth >= numberToCheck,
                7 => Seventh >= numberToCheck,
                8 => Eighth >= numberToCheck,
                9 => Nineth >= numberToCheck,
                _ => false
            };
        }

        public override string ToString()
        {
            var str = "Spell Slots:\n";
            if (First > 0) str += $" 1st Level: {First}\n";
            if (Second > 0) str += $" 2nd Level: {Second}\n";
            if (Third > 0) str += $" 3rd Level: {Third}\n";
            if (Fourth > 0) str += $" 4th Level: {Fourth}\n";
            if (Fifth > 0) str += $" 5th Level: {Fifth}\n";
            if (Sixth > 0) str += $" 6th Level: {Sixth}\n";
            if (Seventh > 0) str += $" 7th Level: {Seventh}\n";
            if (Eighth > 0) str += $" 8th Level: {Eighth}\n";
            if (Nineth > 0) str += $" 9th Level: {Nineth}\n";
            return str;
        }
    }
}