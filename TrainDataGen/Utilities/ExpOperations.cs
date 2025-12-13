using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TrainDataGen.Entities;

namespace TrainDataGen.Utilities;

public static class ExpOperations
{
    public static readonly List<ExpThreshold> ExpPointsList = JsonSerializer.Deserialize<List<ExpThreshold>>(File.ReadAllText("data/expThreshold.json"));
    public static readonly List<MonsterMultiplier> MultiplierList = JsonSerializer.Deserialize<List<MonsterMultiplier>>(File.ReadAllText("data/expMonsterMultiplier.json"));

    public static Difficulty CalculateDifficultiesExp(List<byte> partyLevels)
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

    public static int CalculateAdjustedExp(List<int> monsterExps)
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