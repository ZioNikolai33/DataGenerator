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

    public static int GetDamageValue(string damageDice)
    {
        var value = 0;
        var diceParts = damageDice.Trim().Split("d");

        if (damageDice.Contains("+"))
            value = int.Parse(diceParts[0]) + int.Parse(diceParts[1].Split("+")[0]);

        //WIP

        return value;
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
            case "forcecage":
                return (int)Entities.Enums.Spells.Forcecage;
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
        switch (feature)
        {
            case "rage":
                return (int)Entities.Enums.Features.Rage;
            case "reckless-attack":
                return (int)Entities.Enums.Features.RecklessAttack;
            case "frenzy":
                return (int)Entities.Enums.Features.Frenzy;
            case "mindless-rage":
                return (int)Entities.Enums.Features.MindlessRage;
            case "feral-instinct":
                return (int)Entities.Enums.Features.FeralInstinct;
            case "brutal-critical-1-die":
                return (int)Entities.Enums.Features.BrutalCritical1Die;
            case "intimidating-presence":
                return (int)Entities.Enums.Features.IntimidatingPresence;
            case "relentless-rage":
                return (int)Entities.Enums.Features.RelentlessRage;
            case "brutal-critical-2-die":
                return (int)Entities.Enums.Features.BrutalCritical2Die;
            case "retaliation":
                return (int)Entities.Enums.Features.Retaliation;
            case "persistent-rage":
                return (int)Entities.Enums.Features.PersistentRage;
            case "brutal-critical-3-die":
                return (int)Entities.Enums.Features.BrutalCritical3Die;
            case "indomitable-might":
                return (int)Entities.Enums.Features.IndomitableMight;
            case "cutting-words":
                return (int)Entities.Enums.Features.CuttingWords;
            case "font-of-inspiration":
                return (int)Entities.Enums.Features.FontOfInspiration;
            case "countercharm":
                return (int)Entities.Enums.Features.Countercharm;
            case "peerless-skill":
                return (int)Entities.Enums.Features.PeerlessSkill;
            case "superior-inspiration":
                return (int)Entities.Enums.Features.SuperiorInspiration;
            case "disciple-of-life":
                return (int)Entities.Enums.Features.DiscipleOfLife;
            case "channel-divinity-1-rest":
                return (int)Entities.Enums.Features.ChannelDivinity1Rest;
            case "channel-divinity-preserve-life":
                return (int)Entities.Enums.Features.ChannelDivinityPreserveLife;
            case "channel-divinity-turn-undead":
                return (int)Entities.Enums.Features.ChannelDivinityTurnUndead;
            case "destroy-undead-cr-1-or-2-or-below":
                return (int)Entities.Enums.Features.DestroyUndeadCr1Or2OrBelow;
            case "blessed-healer":
                return (int)Entities.Enums.Features.BlessedHealer;
            case "channel-divinity-2-rest":
                return (int)Entities.Enums.Features.ChannelDivinity2Rest;
            case "destroy-undead-cr-1-or-below":
                return (int)Entities.Enums.Features.DestroyUndeadCr1OrBelow;
            case "divine-strike":
                return (int)Entities.Enums.Features.DivineStrike;
            case "divine-intervention":
                return (int)Entities.Enums.Features.DivineIntervention;
            case "destroy-undead-cr-2-or-below":
                return (int)Entities.Enums.Features.DestroyUndeadCr2OrBelow;
            case "destroy-undead-cr-3-or-below":
                return (int)Entities.Enums.Features.DestroyUndeadCr3OrBelow;
            case "supreme-healing":
                return (int)Entities.Enums.Features.SupremeHealing;
            case "channel-divinity-3-rest":
                return (int)Entities.Enums.Features.ChannelDivinity3Rest;
            case "divine-intervention-improvement":
                return (int)Entities.Enums.Features.DivineInterventionImprovement;
            case "wild-shape-cr-1-4-or-below-no-flying-no-swim-speed":
                return (int)Entities.Enums.Features.WildShapeCr14OrBelowNoFlyingNoSwimSpeed;
            case "wild-shape-cr-1-2-or-below-no-flying-speed":
                return (int)Entities.Enums.Features.WildShapeCr12OrBelowNoFlyingSpeed;
            case "druid-lands-stride":
                return (int)Entities.Enums.Features.DruidLandsStride;
            case "wild-shape-cr-1-or-below":
                return (int)Entities.Enums.Features.WildShapeCr1OrBelow;
            case "natures-ward":
                return (int)Entities.Enums.Features.NaturesWard;
            case "natures-sanctuary":
                return (int)Entities.Enums.Features.NaturesSanctuary;
            case "archdruid":
                return (int)Entities.Enums.Features.Archdruid;
            case "second-wind":
                return (int)Entities.Enums.Features.SecondWind;
            case "action-surge-1-use":
                return (int)Entities.Enums.Features.ActionSurge1Use;
            case "improved-critical":
                return (int)Entities.Enums.Features.ImprovedCritical;
            case "indomitable-1-use":
                return (int)Entities.Enums.Features.Indomitable1Use;
            case "superior-critical":
                return (int)Entities.Enums.Features.SuperiorCritical;
            case "action-surge-2-use":
                return (int)Entities.Enums.Features.ActionSurge2Use;
            case "indomitable-2-use":
                return (int)Entities.Enums.Features.Indomitable2Use;
            case "intolable-3-use":
                return (int)Entities.Enums.Features.Intolable3Use;
            case "survivor":
                return (int)Entities.Enums.Features.Survivor;
            case "martial-arts":
                return (int)Entities.Enums.Features.MartialArts;
            case "step-of-the-wind":
                return (int)Entities.Enums.Features.StepOfTheWind;
            case "patient-defense":
                return (int)Entities.Enums.Features.PatientDefense;
            case "flurry-of-blows":
                return (int)Entities.Enums.Features.FlurryOfBlows;
            case "deflect-missiles":
                return (int)Entities.Enums.Features.DeflectMissiles;
            case "slow-fall":
                return (int)Entities.Enums.Features.SlowFall;
            case "stunning-strike":
                return (int)Entities.Enums.Features.StunningStrike;
            case "ki-empowered-strikes":
                return (int)Entities.Enums.Features.KiEmpoweredStrikes;
            case "wholeness-of-body":
                return (int)Entities.Enums.Features.WholenessOfBody;
            case "stillness-of-mind":
                return (int)Entities.Enums.Features.StillnessOfMind;
            case "monk-evasion":
                return (int)Entities.Enums.Features.MonkEvasion;
            case "unarmored-movement-2":
                return (int)Entities.Enums.Features.UnarmoredMovement2;
            case "diamond-soul":
                return (int)Entities.Enums.Features.DiamondSoul;
            case "quivering-palm":
                return (int)Entities.Enums.Features.QuiveringPalm;
            case "empty-body":
                return (int)Entities.Enums.Features.EmptyBody;
            case "perfect-self":
                return (int)Entities.Enums.Features.PerfectSelf;
            case "lay-on-hands":
                return (int)Entities.Enums.Features.LayOnHands;
            case "divine-sense":
                return (int)Entities.Enums.Features.DivineSense;
            case "divine-smite":
                return (int)Entities.Enums.Features.DivineSmite;
            case "channel-divinity-turn-the-unholy":
                return (int)Entities.Enums.Features.ChannelDivinityTurnTheUnholy;
            case "channel-divinity-sacred-weapon":
                return (int)Entities.Enums.Features.ChannelDivinitySacredWeapon;
            case "channel-divinity":
                return (int)Entities.Enums.Features.ChannelDivinity;
            case "aura-of-protection":
                return (int)Entities.Enums.Features.AuraOfProtection;
            case "aura-of-devotion":
                return (int)Entities.Enums.Features.AuraOfDevotion;
            case "aura-of-courage":
                return (int)Entities.Enums.Features.AuraOfCourage;
            case "improved-divine-smite":
                return (int)Entities.Enums.Features.ImprovedDivineSmite;
            case "cleansing-touch":
                return (int)Entities.Enums.Features.CleansingTouch;
            case "purity-of-spirit":
                return (int)Entities.Enums.Features.PurityOfSpirit;
            case "aura-improvements":
                return (int)Entities.Enums.Features.AuraImprovements;
            case "holy-nimbus":
                return (int)Entities.Enums.Features.HolyNimbus;
            case "hunters-prey-colossus-slayer":
                return (int)Entities.Enums.Features.HuntersPreyColossusSlayer;
            case "hunters-prey-giant-killer":
                return (int)Entities.Enums.Features.HuntersPreyGiantKiller;
            case "hunters-prey-horde-breaker":
                return (int)Entities.Enums.Features.HuntersPreyHordeBreaker;
            case "primeval-awareness":
                return (int)Entities.Enums.Features.PrimevalAwareness;
            case "defensive-tactics-escape-the-horde":
                return (int)Entities.Enums.Features.DefensiveTacticsEscapeTheHorde;
            case "defensive-tactics-multiattack-defense":
                return (int)Entities.Enums.Features.DefensiveTacticsMultiattackDefense;
            case "defensive-tactics-steel-will":
                return (int)Entities.Enums.Features.DefensiveTacticsSteelWill;
            case "ranger-lands-stride":
                return (int)Entities.Enums.Features.RangerLandsStride;
            case "hide-in-plain-sight":
                return (int)Entities.Enums.Features.HideInPlainSight;
            case "multiattack-volley":
                return (int)Entities.Enums.Features.MultiattackVolley;
            case "multiattack-whirlwind-attack":
                return (int)Entities.Enums.Features.MultiattackWhirlwindAttack;
            case "vanish":
                return (int)Entities.Enums.Features.Vanish;
            case "superior-hunters-defense-stand-against-the-tide":
                return (int)Entities.Enums.Features.SuperiorHuntersDefenseStandAgainstTheTide;
            case "superior-hunters-defense-evasion":
                return (int)Entities.Enums.Features.SuperiorHuntersDefenseEvasion;
            case "superior-hunters-defense-uncanny-dodge":
                return (int)Entities.Enums.Features.SuperiorHuntersDefenseUncannyDodge;
            case "feral-senses":
                return (int)Entities.Enums.Features.FeralSenses;
            case "foe-slayer":
                return (int)Entities.Enums.Features.FoeSlayer;
            case "sneak-attack":
                return (int)Entities.Enums.Features.SneakAttack;
            case "cunning-action":
                return (int)Entities.Enums.Features.CunningAction;
            case "fast-hands":
                return (int)Entities.Enums.Features.FastHands;
            case "uncanny-dodge":
                return (int)Entities.Enums.Features.UncannyDodge;
            case "rogue-evasion":
                return (int)Entities.Enums.Features.RogueEvasion;
            case "supreme-sneak":
                return (int)Entities.Enums.Features.SupremeSneak;
            case "reliable-talent":
                return (int)Entities.Enums.Features.ReliableTalent;
            case "blindsense":
                return (int)Entities.Enums.Features.Blindsense;
            case "thiefs-reflexes":
                return (int)Entities.Enums.Features.ThiefsReflexes;
            case "elusive":
                return (int)Entities.Enums.Features.Elusive;
            case "stroke-of-luck":
                return (int)Entities.Enums.Features.StrokeOfLuck;
            case "draconic-resilience":
                return (int)Entities.Enums.Features.DraconicResilience;
            case "flexible-casting-creating-spell-slots":
                return (int)Entities.Enums.Features.FlexibleCastingCreatingSpellSlots;
            case "flexible-casting-converting-spell-slots":
                return (int)Entities.Enums.Features.FlexibleCastingConvertingSpellSlots;
            case "metamagic-subtle-spell":
                return (int)Entities.Enums.Features.MetamagicSubtleSpell;
            case "metamagic-twinned-spell":
                return (int)Entities.Enums.Features.MetamagicTwinnedSpell;
            case "metamagic-quickened-spell":
                return (int)Entities.Enums.Features.MetamagicQuickenedSpell;
            case "metamagic-empowered-spell":
                return (int)Entities.Enums.Features.MetamagicEmpoweredSpell;
            case "metamagic-distant-spell":
                return (int)Entities.Enums.Features.MetamagicDistantSpell;
            case "metamagic-extended-spell":
                return (int)Entities.Enums.Features.MetamagicExtendedSpell;
            case "metamagic-heightened-spell":
                return (int)Entities.Enums.Features.MetamagicHeightenedSpell;
            case "metamagic-careful-spell":
                return (int)Entities.Enums.Features.MetamagicCarefulSpell;
            case "elemental-affinity":
                return (int)Entities.Enums.Features.ElementalAffinity;
            case "dragon-wings":
                return (int)Entities.Enums.Features.DragonWings;
            case "draconic-presence":
                return (int)Entities.Enums.Features.DraconicPresence;
            case "sorcerous-restoration":
                return (int)Entities.Enums.Features.SorcerousRestoration;
            case "dark-one-blessing":
                return (int)Entities.Enums.Features.DarkOneBlessing;
            case "eldritch-invocation-armor-of-shadows":
                return (int)Entities.Enums.Features.EldritchInvocationArmorOfShadows;
            case "eldritch-invocation-beast-speech":
                return (int)Entities.Enums.Features.EldritchInvocationBeastSpeech;
            case "eldritch-invocation-devils-sight":
                return (int)Entities.Enums.Features.EldritchInvocationDevilsSight;
            case "eldritch-invocation-repelling-blast":
                return (int)Entities.Enums.Features.EldritchInvocationRepellingBlast;
            case "eldritch-invocation-thief-of-five-fates":
                return (int)Entities.Enums.Features.EldritchInvocationThiefOfFiveFates;
            case "eldritch-invocation-voice-of-the-chain-master":
                return (int)Entities.Enums.Features.EldritchInvocationVoiceOfTheChainMaster;
            case "eldritch-invocation-book-of-ancient-secrets":
                return (int)Entities.Enums.Features.EldritchInvocationBookOfAncientSecrets;
            case "eldritch-invocation-beguiling-influence":
                return (int)Entities.Enums.Features.EldritchInvocationBeguilingInfluence;
            case "eldritch-invocation-eldritch-sight":
                return (int)Entities.Enums.Features.EldritchInvocationEldritchSight;
            case "eldritch-invocation-fiendish-vigor":
                return (int)Entities.Enums.Features.EldritchInvocationFiendishVigor;
            case "eldritch-invocation-misty-visions":
                return (int)Entities.Enums.Features.EldritchInvocationMistyVisions;
            case "eldritch-invocation-agonizing-blast":
                return (int)Entities.Enums.Features.EldritchInvocationAgonizingBlast;
            case "eldritch-invocation-eldritch-spear":
                return (int)Entities.Enums.Features.EldritchInvocationEldritchSpear;
            case "eldritch-invocation-eye-of-the-rune-keeper":
                return (int)Entities.Enums.Features.EldritchInvocationEyeOfTheRuneKeeper;
            case "eldritch-invocation-gaze-of-two-minds":
                return (int)Entities.Enums.Features.EldritchInvocationGazeOfTwoMinds;
            case "eldritch-invocation-mask-of-many-faces":
                return (int)Entities.Enums.Features.EldritchInvocationMaskOfManyFaces;
            case "eldritch-invocation-mire-the-mind":
                return (int)Entities.Enums.Features.EldritchInvocationMireTheMind;
            case "eldritch-invocation-sign-of-ill-omen":
                return (int)Entities.Enums.Features.EldritchInvocationSignOfIllOmen;
            case "eldritch-invocation-thirsting-blade":
                return (int)Entities.Enums.Features.EldritchInvocationThirstingBlade;
            case "eldritch-invocation-one-with-shadow":
                return (int)Entities.Enums.Features.EldritchInvocationOneWithShadow;
            case "eldritch-invocation-sculptor-of-flesh":
                return (int)Entities.Enums.Features.EldritchInvocationSculptorOfFlesh;
            case "eldritch-invocation-bewitching-whisper":
                return (int)Entities.Enums.Features.EldritchInvocationBewitchingWhisper;
            case "eldritch-invocation-dreadful-word":
                return (int)Entities.Enums.Features.EldritchInvocationDreadfulWord;
            case "eldritch-invocation-ascendant-step":
                return (int)Entities.Enums.Features.EldritchInvocationAscendantStep;
            case "eldritch-invocation-otherworldly-leap":
                return (int)Entities.Enums.Features.EldritchInvocationOtherworldlyLeap;
            case "eldritch-invocation-whispers-of-the-grave":
                return (int)Entities.Enums.Features.EldritchInvocationWhispersOfTheGrave;
            case "eldritch-invocation-minion-of-chaos":
                return (int)Entities.Enums.Features.EldritchInvocationMinionOfChaos;
            case "eldritch-invocation-lifedrinker":
                return (int)Entities.Enums.Features.EldritchInvocationLifedrinker;
            case "eldritch-invocation-vision-of-distant-realms":
                return (int)Entities.Enums.Features.EldritchInvocationVisionOfDistantRealms;
            case "eldritch-invocation-witch-sight":
                return (int)Entities.Enums.Features.EldritchInvocationWitchSight;
            case "eldritch-invocation-chains-of-carceri":
                return (int)Entities.Enums.Features.EldritchInvocationChainsOfCarceri;
            case "eldritch-invocation-master-of-myriad-forms":
                return (int)Entities.Enums.Features.EldritchInvocationMasterOfMyriadForms;
            case "hurl-through-hell":
                return (int)Entities.Enums.Features.HurlThroughHell;
            case "dark-one-own-luck":
                return (int)Entities.Enums.Features.DarkOneOwnLuck;
            case "eldritch-master":
                return (int)Entities.Enums.Features.EldritchMaster;
            case "arcane-recovery":
                return (int)Entities.Enums.Features.ArcaneRecovery;
            case "potent-cantrip":
                return (int)Entities.Enums.Features.PotentCantrip;
            case "empowered-evocation":
                return (int)Entities.Enums.Features.EmpoweredEvocation;
            case "overchannel":
                return (int)Entities.Enums.Features.Overchannel;
            case "spell-mastery":
                return (int)Entities.Enums.Features.SpellMastery;
            case "signature-spells":
                return (int)Entities.Enums.Features.SignatureSpells;
            default:
                return 0;
        }
    }
}