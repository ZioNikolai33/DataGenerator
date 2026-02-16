using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities.PartyEntities;

namespace TrainingDataGenerator.Interfaces;

public interface IPartyGenerator
{
    List<PartyMember> GenerateRandomParty(Database database);
}