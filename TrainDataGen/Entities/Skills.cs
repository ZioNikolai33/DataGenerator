namespace TrainDataGen.Entities;

public class Skills
{
    public Skill Acrobatics { get; set; }
    public Skill AnimalHandling { get; set; }
    public Skill Arcana { get; set; }
    public Skill Athletics { get; set; }
    public Skill Deception { get; set; }
    public Skill History { get; set; }
    public Skill Insight { get; set; }
    public Skill Intimidation { get; set; }
    public Skill Investigation { get; set; }
    public Skill Medicine { get; set; }
    public Skill Nature { get; set; }
    public Skill Perception { get; set; }
    public Skill Performance { get; set; }
    public Skill Persuasion { get; set; }
    public Skill Religion { get; set; }
    public Skill SleightOfHand { get; set; }
    public Skill Stealth { get; set; }
    public Skill Survival { get; set; }

    public Skills(Skill acrobatics, Skill animalHandling, Skill arcana, Skill athletics, Skill deception, Skill history, Skill insight, Skill intimidation, Skill investigation, Skill medicine, Skill nature, Skill perception, Skill performance, Skill persuasion, Skill religion, Skill sleightOfHand, Skill stealth, Skill survival)
    {
        Acrobatics = acrobatics;
        AnimalHandling = animalHandling;
        Arcana = arcana;
        Athletics = athletics;
        Deception = deception;
        History = history;
        Insight = insight;
        Intimidation = intimidation;
        Investigation = investigation;
        Medicine = medicine;
        Nature = nature;
        Perception = perception;
        Performance = performance;
        Persuasion = persuasion;
        Religion = religion;
        SleightOfHand = sleightOfHand;
        Stealth = stealth;
        Survival = survival;
    }
}