using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TrainDataGen.Entities;

namespace TrainDataGen.Utilities;

public static class ExpOperations
{
    private static readonly List<ExpThreshold> ExpPointsList;
    private static readonly List<MonsterMultiplier> MultiplierList;

    static ExpOperations()
    {
        var expThresholdJson = File.ReadAllText("data/expThreshold.json");
        ExpPointsList = JsonSerializer.Deserialize<List<ExpThreshold>>(expThresholdJson);

        var multiplierJson = File.ReadAllText("data/expMonsterMultiplier.json");
        MultiplierList = JsonSerializer.Deserialize<List<MonsterMultiplier>>(multiplierJson);
    }

    public static Difficulty CalculateDifficultiesExp(IEnumerable<int> partyLevels)
    {
        var totalExpPoints = new Difficulty
        {
            Easy = 0,
            Medium = 0,
            Hard = 0,
            Deadly = 0
        };

        foreach (var level in partyLevels)
        {
            var levelExps = ExpPointsList.FirstOrDefault(entry => entry.Level == level);
            if (levelExps != null)
            {
                totalExpPoints.Easy += levelExps.Easy;
                totalExpPoints.Medium += levelExps.Medium;
                totalExpPoints.Hard += levelExps.Hard;
                totalExpPoints.Deadly += levelExps.Deadly;
            }
            else
            {
                throw new ArgumentException($"Level {level} not found in expPointsList.");
            }
        }

        return totalExpPoints;
    }

    public static int CalculateAdjustedExp(IEnumerable<int> monsterExps)
    {
        var totalExp = monsterExps.Sum();
        var numMonsters = monsterExps.Count();
        var numMultiplier = MultiplierList.FirstOrDefault(entry => entry.Number == numMonsters);

        if (numMultiplier == null)
            throw new ArgumentException($"Number of monsters {numMonsters} is out of range. Valid range is 1 to 15 or more.");

        var adjustedExp = (int)(totalExp * numMultiplier.Multiplier);
        return adjustedExp;
    }
}