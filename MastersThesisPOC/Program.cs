using MastersThesisPOC;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using MastersThesisPOC.Helpers;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddScoped<IAlgorithmHelper, AlgorithmHelper>()
    .AddScoped<IMathComputer, MathComputer>()
    .AddScoped<ILaplaceNoiseGenerator, LaplaceNoiseGenerator>()
    .AddScoped<ICsvReader, CsvReader>()
    .AddScoped<ICompressionMechanism, CompressionMechanism>()
    .AddScoped<IMetrics, Metrics>();

var serviceProvider = services.BuildServiceProvider();

var compressionMechanism = serviceProvider.GetRequiredService<ICompressionMechanism>();
var metricsProvider = serviceProvider.GetRequiredService<IMetrics>();
var laplaceNoise = serviceProvider.GetRequiredService<ILaplaceNoiseGenerator>();
var csvReader = serviceProvider.GetRequiredService<ICsvReader>();

var dict = new Dictionary<float, string>();
var testDict = new Dictionary<float, string>();

dict.Add(3f, "01");
dict.Add(5f, "0011");
dict.Add(7f, "001");
dict.Add(9f, "000111");
dict.Add(11f, "0001011101");
dict.Add(13f, "000100111011");
dict.Add(15f, "0001");
dict.Add(17f, "11100001");
dict.Add(19f, "101011110010100001");
dict.Add(21f, "100001");
dict.Add(23f, "01100100001");

var listOfEpsilonsAndSpreads = new ListWithDuplicates();

listOfEpsilonsAndSpreads.Add(1.0f, 1.0f);

/*
testDict.Add(7f, "001");
testDict.Add(11f, "0001011101");
testDict.Add(13f, "000100111011");
*/
testDict.Add(21f, "100001");


var temperatureFloats = csvReader.ReadCsvColumn("C:\\Users\\mongl\\source\\repos\\MastersThesisPOC\\MastersThesisPOC\\melbourne-smart_city.csv");

Console.ForegroundColor = ConsoleColor.White;
Console.Write("Size of the dataset: ");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.Write(temperatureFloats.Count);
Console.WriteLine();
Console.ForegroundColor = ConsoleColor.White;
Console.Write("Max value: ");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.Write(temperatureFloats.Max());
Console.WriteLine();
Console.ForegroundColor = ConsoleColor.White;
Console.Write("Min value: ");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.Write(temperatureFloats.Min());
Console.ForegroundColor = ConsoleColor.White;


foreach (var (M, pattern) in testDict)
{
    RunUsingExtension(M, pattern, temperatureFloats, 1, 100);

    for (int i = 0; i < 15; i++)
    {
        //RunUsingReplaceOnce(M, pattern, temperatureFloats, i, 100);
        //RunDefault(M, pattern, temperatureFloats, i, 100);
    }
}




void RunDefault(float M, string pattern, List<float> floats, int i, int nextBits)
{
    Console.WriteLine("\n\n");

    // Set default color
    Console.ForegroundColor = ConsoleColor.White;

    Console.Write("Value of M: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(M);
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(" | Pattern: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(pattern); 
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(" | Starting Index: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(i);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("\n");

    var newFloats = compressionMechanism.ComputeBasicCompressedList(M, pattern, floats, i, nextBits);

    var sharedIndexes = compressionMechanism.CalculateSharedIndexes(newFloats);

    foreach (int index in Enumerable.Range(0, 23))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        if (sharedIndexes.Contains(index))
        {
            Console.ForegroundColor = ConsoleColor.Blue;
        }

        Console.Write($"[{index}] ");

        Console.ResetColor(); // Reset text color to default
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;

    var revertedList = compressionMechanism.ComputeOriginalNumbersFromCompressedList(M, newFloats);
    var averageError = metricsProvider.CalculateAverageErrorPercentDifference(floats, revertedList);
    var (startAverage, endAverage) = metricsProvider.CalculateAverages(floats, revertedList);

    Console.Write($"Average Individual Error %: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(averageError);
    Console.WriteLine("\n");
    Console.ForegroundColor = ConsoleColor.White;

    Console.Write($"Average of starting list: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(startAverage);
    Console.WriteLine("\n");

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"Average of compressed list: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(endAverage);
    Console.WriteLine("\n");
    Console.ForegroundColor = ConsoleColor.White;
}

void RunUsingExtension(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits)
{
    Console.WriteLine("\n\n");
    Console.ForegroundColor = ConsoleColor.White;

    Console.Write("Using Extension Starting From Pattern Index: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(patternStartIndex);
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(" | M: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(M);
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(" | Pattern: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(pattern);
    Console.WriteLine();
    Console.WriteLine();

    var resultsFromExtension = compressionMechanism.ComputeBasicCompressedListUsingExtension(M, pattern, floats, patternStartIndex, nextBits);

    var sharedIndexesFromExtension = compressionMechanism.CalculateSharedIndexes(resultsFromExtension);

    foreach (int index in Enumerable.Range(0, 23))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        if (sharedIndexesFromExtension.Contains(index))
        {
            Console.ForegroundColor = ConsoleColor.Blue;
        }

        Console.Write($"[{index}] ");

        Console.ResetColor(); // Reset text color to default
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;

    var revertedList = compressionMechanism.ComputeOriginalNumbersFromCompressedList(M, resultsFromExtension);
    var averageError = metricsProvider.CalculateAverageErrorPercentDifference(floats, revertedList);
    var (startAverage, endAverage) = metricsProvider.CalculateAverages(floats, revertedList);

    Console.Write($"Average Individual Error %: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(averageError);
    Console.WriteLine("\n");
    Console.ForegroundColor = ConsoleColor.White;

    Console.Write($"Average of starting list: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(startAverage);
    Console.WriteLine("\n");

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"Average of compressed list: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(endAverage);
    Console.WriteLine("\n");
    Console.ForegroundColor = ConsoleColor.White;
}

void RunUsingReplaceOnce(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits)
{
    Console.WriteLine("\n\n");
    Console.ForegroundColor = ConsoleColor.White;

    Console.Write("Replacing Pattern once from Index: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(patternStartIndex);
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(" | M: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(M);
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(" | Pattern: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(pattern);
    Console.WriteLine();
    Console.WriteLine();

    var resultsFromExtension = compressionMechanism.ComputeBasicCompressedListReplacingOnce(M, pattern, floats, patternStartIndex, nextBits);

    var sharedIndexesFromExtension = compressionMechanism.CalculateSharedIndexes(resultsFromExtension);

    foreach (int index in Enumerable.Range(0, 23))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        if (sharedIndexesFromExtension.Contains(index))
        {
            Console.ForegroundColor = ConsoleColor.Blue;
        }

        Console.Write($"[{index}] ");

        Console.ResetColor(); // Reset text color to default
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;

    var revertedList = compressionMechanism.ComputeOriginalNumbersFromCompressedList(M, resultsFromExtension);
    var averageError = metricsProvider.CalculateAverageErrorPercentDifference(floats, revertedList);
    var (startAverage, endAverage) = metricsProvider.CalculateAverages(floats, revertedList);

    Console.Write($"Average Individual Error %: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(averageError);
    Console.WriteLine("\n");
    Console.ForegroundColor = ConsoleColor.White;

    Console.Write($"Average of starting list: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(startAverage);
    Console.WriteLine("\n");

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"Average of compressed list: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(endAverage);
    Console.WriteLine("\n");
    Console.ForegroundColor = ConsoleColor.White;
}

