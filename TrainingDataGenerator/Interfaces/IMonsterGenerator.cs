using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Interfaces;

public interface IMonsterGenerator
{
    List<Monster> GenerateRandomMonsters(CRRatios difficulty, List<byte> partyLevels, List<MonsterDifficulty> availableMonsters);
}