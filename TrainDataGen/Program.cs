using TrainDataGen.DataBase;
using TrainDataGen.Generator;

namespace TrainDataGen;

internal static class Program
{
    private static void Main()
    {
        var database = new Database();

        DataGenerator.Generate(database);
    }
}