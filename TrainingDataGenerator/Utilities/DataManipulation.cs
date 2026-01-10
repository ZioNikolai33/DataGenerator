using TrainingDataGenerator.DataBase;

namespace TrainingDataGenerator.Utilities;

public static class DataManipulation
{
    public static List<MonsterDifficulty> GetMonstersDifficultiesList(Database db)
    {
        var filteredMonsters = new List<MonsterDifficulty>();

        foreach (var item in Lists.monsters)
        {
            filteredMonsters.Add(new MonsterDifficulty
            {
                Index = item.Index,
                Name = item.Name,
                ChallengeRating = item.ChallengeRating,
                Xp = item.Xp
            });
        }

        return filteredMonsters;
    }

    public static short CalculateHitPercentage(int armorClass, int attackBonus)
    {
        var hitPercentage = 0;

        for (var roll = 1; roll <= 20; roll++)
        {
            if (roll == 1)
                continue;

            if (roll == 20)
            {
                hitPercentage += 5;
                continue;
            }

            if (roll + attackBonus >= armorClass)
                hitPercentage += 5;
        }

        return (short)hitPercentage;
    }

    public static string ConvertAbilityIndex(string index)
    {
        switch (index)
        {
            case "str":
                return "strength";
            case "dex":
                return "dexterity";
            case "con":
                return "constitution";
            case "int":
                return "intelligence";
            case "wis":
                return "wisdom";
            case "cha":
                return "charisma";
        }

        return string.Empty;
    }

    public static int GetSpellsPowerValue(string spell)
    {
        switch (spell)
        {
            case "shillelagh":
                return (int)Entities.Enums.Spells.Shillelagh;
            case "resistance":
                return (int)Entities.Enums.Spells.Resistance;
            case "guidance":
                return (int)Entities.Enums.Spells.Guidance;
            case "spare-the-dying":
                return (int)Entities.Enums.Spells.SpareTheDying;
            case "longstrider":
                return (int)Entities.Enums.Spells.Longstrider;
            case "protection-from-evil-and-good":
                return (int)Entities.Enums.Spells.ProtectionFromEvilAndGood;
            case "shield":
                return (int)Entities.Enums.Spells.Shield;
            case "shield-of-faith":
                return (int)Entities.Enums.Spells.ShieldOfFaith;
            case "command":
                return (int)Entities.Enums.Spells.Command;
            case "color-spray":
                return (int)Entities.Enums.Spells.ColorSpray;
            case "hunters-mark":
                return (int)Entities.Enums.Spells.HuntersMark;
            case "feather-fall":
                return (int)Entities.Enums.Spells.FeatherFall;
            case "entangle":
                return (int)Entities.Enums.Spells.Entangle;
            case "false-life":
                return (int)Entities.Enums.Spells.FalseLife;
            case "bane":
                return (int)Entities.Enums.Spells.Bane;
            case "detect-magic":
                return (int)Entities.Enums.Spells.DetectMagic;
            case "heroism":
                return (int)Entities.Enums.Spells.Heroism;
            case "healing-word":
                return (int)Entities.Enums.Spells.HealingWord;
            case "bless":
                return (int)Entities.Enums.Spells.Bless;
            case "expeditious-retreat":
                return (int)Entities.Enums.Spells.ExpeditiousRetreat;
            case "sanctuary":
                return (int)Entities.Enums.Spells.Sanctuary;
            case "fog-cloud":
                return (int)Entities.Enums.Spells.FogCloud;
            case "charm-person":
                return (int)Entities.Enums.Spells.CharmPerson;
            case "grease":
                return (int)Entities.Enums.Spells.Grease;
            case "silent-image":
                return (int)Entities.Enums.Spells.SilentImage;
            case "mage-armor":
                return (int)Entities.Enums.Spells.MageArmor;
            case "jump":
                return (int)Entities.Enums.Spells.Jump;
            case "hideous-laughter":
                return (int)Entities.Enums.Spells.HideousLaughter;
            case "unseen-servant":
                return (int)Entities.Enums.Spells.UnseenServant;
            case "zone-of-truth":
                return (int)Entities.Enums.Spells.ZoneOfTruth;
            case "web":
                return (int)Entities.Enums.Spells.Web;
            case "warding-bond":
                return (int)Entities.Enums.Spells.WardingBond;
            case "gentle-repose":
                return (int)Entities.Enums.Spells.GentleRepose;
            case "suggestion":
                return (int)Entities.Enums.Spells.Suggestion;
            case "silence":
                return (int)Entities.Enums.Spells.Silence;
            case "alter-self":
                return (int)Entities.Enums.Spells.AlterSelf;
            case "enhance-ability":
                return (int)Entities.Enums.Spells.EnhanceAbility;
            case "see-invisibility":
                return (int)Entities.Enums.Spells.SeeInvisibility;
            case "ray-of-enfeeblement":
                return (int)Entities.Enums.Spells.RayOfEnfeeblement;
            case "invisibility":
                return (int)Entities.Enums.Spells.Invisibility;
            case "levitate":
                return (int)Entities.Enums.Spells.Levitate;
            case "hold-person":
                return (int)Entities.Enums.Spells.HoldPerson;
            case "spike-growth":
                return (int)Entities.Enums.Spells.SpikeGrowth;
            case "misty-step":
                return (int)Entities.Enums.Spells.MistyStep;
            case "protection-from-poison":
                return (int)Entities.Enums.Spells.ProtectionFromPoison;
            case "magic-weapon":
                return (int)Entities.Enums.Spells.MagicWeapon;
            case "pass-without-trace":
                return (int)Entities.Enums.Spells.PassWithoutTrace;
            case "spider-climb":
                return (int)Entities.Enums.Spells.SpiderClimb;
            case "lesser-restoration":
                return (int)Entities.Enums.Spells.LesserRestoration;
            case "mirror-image":
                return (int)Entities.Enums.Spells.MirrorImage;
            case "darkness":
                return (int)Entities.Enums.Spells.Darkness;
            case "find-steed":
                return (int)Entities.Enums.Spells.FindSteed;
            case "enthrall":
                return (int)Entities.Enums.Spells.Enthrall;
            case "find-traps":
                return (int)Entities.Enums.Spells.FindTraps;
            case "enlarge-reduce":
                return (int)Entities.Enums.Spells.EnlargeReduce;
            case "arcanists-magic-aura":
                return (int)Entities.Enums.Spells.ArcanistsMagicAura;
            case "arcane-lock":
                return (int)Entities.Enums.Spells.ArcaneLock;
            case "detect-thoughts":
                return (int)Entities.Enums.Spells.DetectThoughts;
            case "gust-of-wind":
                return (int)Entities.Enums.Spells.GustOfWind;
            case "continual-flame":
                return (int)Entities.Enums.Spells.ContinualFlame;
            case "darkvision":
                return (int)Entities.Enums.Spells.Darkvision;
            case "blur":
                return (int)Entities.Enums.Spells.Blur;
            case "barkskin":
                return (int)Entities.Enums.Spells.Barkskin;
            case "calm-emotions":
                return (int)Entities.Enums.Spells.CalmEmotions;
            case "blindness-deafness":
                return (int)Entities.Enums.Spells.BlindnessDeafness;
            case "speak-with-plants":
                return (int)Entities.Enums.Spells.SpeakWithPlants;
            case "gaseous-form":
                return (int)Entities.Enums.Spells.GaseousForm;
            case "revivify":
                return (int)Entities.Enums.Spells.Revivify;
            case "stinking-cloud":
                return (int)Entities.Enums.Spells.StinkingCloud;
            case "water-breathing":
                return (int)Entities.Enums.Spells.WaterBreathing;
            case "nondetection":
                return (int)Entities.Enums.Spells.Nondetection;
            case "major-image":
                return (int)Entities.Enums.Spells.MajorImage;
            case "animate-dead":
                return (int)Entities.Enums.Spells.AnimateDead;
            case "bestow-curse":
                return (int)Entities.Enums.Spells.BestowCurse;
            case "meld-into-stone":
                return (int)Entities.Enums.Spells.MeldIntoStone;
            case "haste":
                return (int)Entities.Enums.Spells.Haste;
            case "phantom-steed":
                return (int)Entities.Enums.Spells.PhantomSteed;
            case "plant-growth":
                return (int)Entities.Enums.Spells.PlantGrowth;
            case "protection-from-energy":
                return (int)Entities.Enums.Spells.ProtectionFromEnergy;
            case "blink":
                return (int)Entities.Enums.Spells.Blink;
            case "sleet-storm":
                return (int)Entities.Enums.Spells.SleetStorm;
            case "conjure-animals":
                return (int)Entities.Enums.Spells.ConjureAnimals;
            case "slow":
                return (int)Entities.Enums.Spells.Slow;
            case "remove-curse":
                return (int)Entities.Enums.Spells.RemoveCurse;
            case "fly":
                return (int)Entities.Enums.Spells.Fly;
            case "counterspell":
                return (int)Entities.Enums.Spells.Counterspell;
            case "beacon-of-hope":
                return (int)Entities.Enums.Spells.BeaconOfHope;
            case "daylight":
                return (int)Entities.Enums.Spells.Daylight;
            case "hypnotic-pattern":
                return (int)Entities.Enums.Spells.HypnoticPattern;
            case "water-walk":
                return (int)Entities.Enums.Spells.WaterWalk;
            case "dispel-magic":
                return (int)Entities.Enums.Spells.DispelMagic;
            case "fear":
                return (int)Entities.Enums.Spells.Fear;
            case "spirit-guardians":
                return (int)Entities.Enums.Spells.SpiritGuardians;
            case "banishment":
                return (int)Entities.Enums.Spells.Banishment;
            case "stone-shape":
                return (int)Entities.Enums.Spells.StoneShape;
            case "arcane-eye":
                return (int)Entities.Enums.Spells.ArcaneEye;
            case "resilient-sphere":
                return (int)Entities.Enums.Spells.ResilientSphere;
            case "polymorph":
                return (int)Entities.Enums.Spells.Polymorph;
            case "dominate-beast":
                return (int)Entities.Enums.Spells.DominateBeast;
            case "giant-insect":
                return (int)Entities.Enums.Spells.GiantInsect;
            case "freedom-of-movement":
                return (int)Entities.Enums.Spells.FreedomOfMovement;
            case "stoneskin":
                return (int)Entities.Enums.Spells.Stoneskin;
            case "dimension-door":
                return (int)Entities.Enums.Spells.DimensionDoor;
            case "greater-invisibility":
                return (int)Entities.Enums.Spells.GreaterInvisibility;
            case "conjure-woodland-beings":
                return (int)Entities.Enums.Spells.ConjureWoodlandBeings;
            case "confusion":
                return (int)Entities.Enums.Spells.Confusion;
            case "compulsion":
                return (int)Entities.Enums.Spells.Compulsion;
            case "death-ward":
                return (int)Entities.Enums.Spells.DeathWard;
            case "planar-binding":
                return (int)Entities.Enums.Spells.PlanarBinding;
            case "hold-monster":
                return (int)Entities.Enums.Spells.HoldMonster;
            case "modify-memory":
                return (int)Entities.Enums.Spells.ModifyMemory;
            case "hallow":
                return (int)Entities.Enums.Spells.Hallow;
            case "greater-restoration":
                return (int)Entities.Enums.Spells.GreaterRestoration;
            case "geas":
                return (int)Entities.Enums.Spells.Geas;
            case "seeming":
                return (int)Entities.Enums.Spells.Seeming;
            case "contagion":
                return (int)Entities.Enums.Spells.Contagion;
            case "telekinesis":
                return (int)Entities.Enums.Spells.Telekinesis;
            case "animate-objects":
                return (int)Entities.Enums.Spells.AnimateObjects;
            case "wall-of-force":
                return (int)Entities.Enums.Spells.WallOfForce;
            case "dominate-person":
                return (int)Entities.Enums.Spells.DominatePerson;
            case "mislead":
                return (int)Entities.Enums.Spells.Mislead;
            case "antilife-shell":
                return (int)Entities.Enums.Spells.AntilifeShell;
            case "dispel-evil-and-good":
                return (int)Entities.Enums.Spells.DispelEvilAndGood;
            case "wall-of-stone":
                return (int)Entities.Enums.Spells.WallOfStone;
            case "irresistible-dance":
                return (int)Entities.Enums.Spells.IrresistibleDance;
            case "eyebite":
                return (int)Entities.Enums.Spells.Eyebite;
            case "true-seeing":
                return (int)Entities.Enums.Spells.TrueSeeing;
            case "mass-suggestion":
                return (int)Entities.Enums.Spells.MassSuggestion;
            case "programmed-illusion":
                return (int)Entities.Enums.Spells.ProgrammedIllusion;
            case "flesh-to-stone":
                return (int)Entities.Enums.Spells.FleshToStone;
            case "globe-of-invulnerability":
                return (int)Entities.Enums.Spells.GlobeOfInvulnerability;
            case "force-cage":
                return (int)Entities.Enums.Spells.ForceCage;
            case "place-shift":
                return (int)Entities.Enums.Spells.PlaceShift;
            case "project-image":
                return (int)Entities.Enums.Spells.ProjectImage;
            case "sequester":
                return (int)Entities.Enums.Spells.Sequester;
            case "reverse-gravity":
                return (int)Entities.Enums.Spells.ReverseGravity;
            case "divine-word":
                return (int)Entities.Enums.Spells.DivineWord;
            case "earthquake":
                return (int)Entities.Enums.Spells.Earthquake;
            case "animal-shapes":
                return (int)Entities.Enums.Spells.AnimalShapes;
            case "antimagic-field":
                return (int)Entities.Enums.Spells.AntimagicField;
            case "power-word-stun":
                return (int)Entities.Enums.Spells.PowerWordStun;
            case "maze":
                return (int)Entities.Enums.Spells.Maze;
            case "dominate-monster":
                return (int)Entities.Enums.Spells.DominateMonster;
            case "glibness":
                return (int)Entities.Enums.Spells.Glibness;
            case "holy-aura":
                return (int)Entities.Enums.Spells.HolyAura;
            case "mind-blank":
                return (int)Entities.Enums.Spells.MindBlank;
            case "shapechange":
                return (int)Entities.Enums.Spells.ShapeChange;
            case "wish":
                return (int)Entities.Enums.Spells.Wish;
            case "time-stop":
                return (int)Entities.Enums.Spells.TimeStop;
            case "weird":
                return (int)Entities.Enums.Spells.Weird;
            case "prismatic-wall":
                return (int)Entities.Enums.Spells.PrismaticWall;
            case "true-polymorph":
                return (int)Entities.Enums.Spells.TruePolymorph;
            case "power-word-kill":
                return (int)Entities.Enums.Spells.PowerWordKill;
            default:
                return 0;
        }
    }

    public static int GetFeaturePowerValue(string feature)
    {
        return 0;
    }
}