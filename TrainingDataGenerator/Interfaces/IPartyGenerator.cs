using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities;

namespace TrainingDataGenerator.Interfaces;

public interface IPartyGenerator
{
    List<PartyMember> GenerateRandomParty(Database database);
}