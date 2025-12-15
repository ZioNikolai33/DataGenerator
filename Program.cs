using TrainDataGen.DataBase;
using TrainDataGen.Generator;

namespace TrainDataGen;

internal static class Program
{
    private static void Main()
    {
        var database = new Database();
        var startTime = DateTime.Now;

        DataGenerator.Generate(database, startTime);
    }
}