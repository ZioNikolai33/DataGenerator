using TrainDataGen.Entities.Mappers;

namespace TrainDataGen.Entities
{
    public class Slots
    {
        public byte First { get; set; }
        public byte Second { get; set; }
        public byte Third { get; set; }
        public byte Fourth { get; set; }
        public byte Fifth { get; set; }
        public byte Sixth { get; set; }
        public byte Seventh { get; set; }
        public byte Eighth { get; set; }
        public byte Nineth { get; set; }

        public Slots(LevelMapper.SpellcastingInfo spellcastingInfo)
        {
            First = spellcastingInfo.SpellSlotsLevel1 ?? 0;
            Second = spellcastingInfo.SpellSlotsLevel2 ?? 0;
            Third = spellcastingInfo.SpellSlotsLevel3 ?? 0;
            Fourth = spellcastingInfo.SpellSlotsLevel4 ?? 0;
            Fifth = spellcastingInfo.SpellSlotsLevel5 ?? 0;
            Sixth = spellcastingInfo.SpellSlotsLevel6 ?? 0;
            Seventh = spellcastingInfo.SpellSlotsLevel7 ?? 0;
            Eighth = spellcastingInfo.SpellSlotsLevel8 ?? 0;
            Nineth = spellcastingInfo.SpellSlotsLevel9 ?? 0;
        }

        public Slots(MonsterMapper.SpellSlots spellcasting)
        {
            First = spellcasting._1 ?? 0;
            Second = spellcasting._2 ?? 0;
            Third = spellcasting._3 ?? 0;
            Fourth = spellcasting._4 ?? 0;
            Fifth = spellcasting._4 ?? 0;
            Sixth = spellcasting._5 ?? 0;
            Seventh = spellcasting._6 ?? 0;
            Eighth = spellcasting._7 ?? 0;
            Nineth = spellcasting._8 ?? 0;
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