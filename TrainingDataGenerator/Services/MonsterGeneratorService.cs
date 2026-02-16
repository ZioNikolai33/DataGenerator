using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Services;

public class MonsterGeneratorService : IMonsterGenerator
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;

    public MonsterGeneratorService(
        ILogger logger,
        IRandomProvider random)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public List<Monster> GenerateRandomMonsters(CRRatios ratio, List<byte> levels, List<MonsterDifficulty> monsters)
    {
        var monstersFiltered = new List<MonsterDifficulty>();
        var randomMonsters = new List<MonsterDifficulty>();
        var countSelectionForRetrying = 0;

        // Random
        var randomNumMonsters = _random.Next(1, 16);

        // Data parameters
        var expThresholds = ExpOperations.CalculateDifficultiesExp(levels);
        var numMultiplier = GetMultiplierForNumMonsters(randomNumMonsters);

        // Target Experience
        var targetExpBeforeMultiplier = 0;
        var floorTargetExpBeforeMultiplier = 0;        

        _logger.Verbose($"EXP Thresholds calculated for party levels {string.Join(", ", levels)}: Cakewalk {expThresholds.Cakewalk}, Easy {expThresholds.Easy}, Medium {expThresholds.Medium}, Hard {expThresholds.Hard}, Deadly {expThresholds.Deadly}, Impossible {expThresholds.Impossible}");
        _logger.Information($"Selecting monsters: ratio {ratio} - {randomNumMonsters} monsters - multiplier {numMultiplier}");

        GetExpFloorAndTarget(ratio, expThresholds, numMultiplier, out targetExpBeforeMultiplier, out floorTargetExpBeforeMultiplier);

        _logger.Verbose($"Floor Target XP before multiplier: {floorTargetExpBeforeMultiplier}");
        _logger.Verbose($"Target XP before multiplier: {targetExpBeforeMultiplier}");

        do
        {
            monstersFiltered = (targetExpBeforeMultiplier == -1) ? monsters : monsters.Where(m => m.Xp <= targetExpBeforeMultiplier).ToList();
            _logger.Verbose($"Found {monstersFiltered.Count} monsters matching criteria.");

            if (monstersFiltered.Count == 0)
            {
                randomNumMonsters = _random.Next(1, 16);
                numMultiplier = GetMultiplierForNumMonsters(randomNumMonsters);
                _logger.Warning($"No suitable monsters found. Selecting new {randomNumMonsters} number of monsters and changing multiplier to {numMultiplier}");

                GetExpFloorAndTarget(ratio, expThresholds, numMultiplier, out targetExpBeforeMultiplier, out floorTargetExpBeforeMultiplier);

                _logger.Verbose($"New Floor Target XP before multiplier: {floorTargetExpBeforeMultiplier}");
                _logger.Verbose($"New Target XP before multiplier: {targetExpBeforeMultiplier}");
            }
        }
        while (monstersFiltered.Count == 0);

        do
        {
            randomMonsters = _random.SelectRandom(monstersFiltered, randomNumMonsters);

            countSelectionForRetrying++;

            if (countSelectionForRetrying > 1000)
            {
                countSelectionForRetrying = 0;

                randomNumMonsters = _random.Next(1, 16);
                numMultiplier = GetMultiplierForNumMonsters(randomNumMonsters);
                _logger.Warning($"Maximum tries reached. Selecting new {randomNumMonsters} number of monsters and changing multiplier to {numMultiplier}");

                GetExpFloorAndTarget(ratio, expThresholds, numMultiplier, out targetExpBeforeMultiplier, out floorTargetExpBeforeMultiplier);

                _logger.Verbose($"New Floor Target XP before multiplier: {floorTargetExpBeforeMultiplier}");
                _logger.Verbose($"New Target XP before multiplier: {targetExpBeforeMultiplier}");

                monstersFiltered = (targetExpBeforeMultiplier == -1) ? monsters : monsters.Where(m => m.Xp <= targetExpBeforeMultiplier).ToList();
            }
        }
        while (randomMonsters.Sum(m => m.Xp) <= floorTargetExpBeforeMultiplier || (targetExpBeforeMultiplier != -1 && randomMonsters.Sum(m => m.Xp) > targetExpBeforeMultiplier));

        _logger.Verbose($"Selected monsters XP sum: {randomMonsters.Sum(m => m.Xp)}");
        _logger.Verbose($"Selected Monsters XP after multiplier: {randomMonsters.Sum(m => m.Xp) * numMultiplier}");

        List<Monster> monstersSelected = randomMonsters.Select(item => CreateMonster(EntitiesFinder.GetEntityByIndex(Lists.monsters, new Entities.Mappers.BaseEntity(item.Index, item.Name)))).ToList();
        _logger.Information($"Selected monsters: {string.Join(", ", monstersSelected.Select(m => m.Name))}");

        return monstersSelected;
    }

    private double GetMultiplierForNumMonsters(int numMonsters) =>
        ExpOperations.MultiplierList.FirstOrDefault(entry => entry.Number == numMonsters)?.Multiplier ?? 1;

    private void GetExpFloorAndTarget(CRRatios ratio, Difficulty expThresholds, double numMultiplier, out int targetExpBeforeMultiplier, out int floorTargetExpBeforeMultiplier)
    {
        switch (ratio)
        {
            case CRRatios.Cakewalk:
                targetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Easy / numMultiplier);
                floorTargetExpBeforeMultiplier = 0;
                break;
            case CRRatios.Easy:
                targetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Medium / numMultiplier);
                floorTargetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Easy / numMultiplier);
                break;
            case CRRatios.Normal:
                targetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Hard / numMultiplier);
                floorTargetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Medium / numMultiplier);
                break;
            case CRRatios.Hard:
                targetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Deadly / numMultiplier);
                floorTargetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Hard / numMultiplier);
                break;
            case CRRatios.Deadly:
                targetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Impossible / numMultiplier);
                floorTargetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Deadly / numMultiplier);
                break;
            case CRRatios.Impossible:
                targetExpBeforeMultiplier = -1; // No upper limit
                floorTargetExpBeforeMultiplier = (int)Math.Floor(expThresholds.Impossible / numMultiplier);
                break;
            default:
                targetExpBeforeMultiplier = 0;
                floorTargetExpBeforeMultiplier = 0;
                break;
        }
    }

    private Monster CreateMonster(MonsterMapper monster)
    {
        return new Monster(
            monster,
            _logger,
            _random);
    }
}