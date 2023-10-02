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
/*
var listOfEpsilonsAndSpreads = new ListWithDuplicates();

listOfEpsilonsAndSpreads.Add(1.0f, 1.0f);
*/
/*
testDict.Add(7f, "001");
testDict.Add(11f, "0001011101");
testDict.Add(13f, "000100111011");
*/

//dict.Add(11f, "0001011101");
//dict.Add(13f, "000100111011");
testDict.Add(9f, "000111");

// Assuming you've already read in the list:
var temperatureFloats = csvReader.ReadCsvColumn("C:\\Users\\mongl\\source\\repos\\MastersThesisPOC\\MastersThesisPOC\\melbourne-smart_city.csv");
var humidityFloats = csvReader.ReadCsvColumnHumidity("C:\\Users\\mongl\\source\\repos\\MastersThesisPOC\\MastersThesisPOC\\melbourne-smart_city.csv");


// Splitting the list into quarters
int quarter = temperatureFloats.Count / 4;
var firstQuarter = temperatureFloats.Take(quarter).ToList();
var secondQuarter = temperatureFloats.Skip(quarter).Take(quarter).ToList();
var thirdQuarter = temperatureFloats.Skip(2 * quarter).Take(quarter).ToList();
var fourthQuarter = temperatureFloats.Skip(3 * quarter).ToList();

// Splitting the dataset based on exponent
var exponent2List = temperatureFloats.Where(x => ExtractExponent(x) == 2).ToList();
var exponent3List = temperatureFloats.Where(x => ExtractExponent(x) == 3).ToList();
var exponent4List = temperatureFloats.Where(x => ExtractExponent(x) == 4).ToList();
var exponent5List = temperatureFloats.Where(x => ExtractExponent(x) == 5).ToList();

// Splitting the dataset based on exponent
var humidityexponentList0 = humidityFloats.Where(x => ExtractExponent(x) == 0).ToList();
var humidityexponentList1 = humidityFloats.Where(x => ExtractExponent(x) == 1).ToList();
var humidityexponentList2 = humidityFloats.Where(x => ExtractExponent(x) == 2).ToList();
var humidityexponentList3 = humidityFloats.Where(x => ExtractExponent(x) == 3).ToList();
var humidityexponentList4 = humidityFloats.Where(x => ExtractExponent(x) == 4).ToList();
var humidityexponentList5 = humidityFloats.Where(x => ExtractExponent(x) == 5).ToList();
var humidityexponentList6 = humidityFloats.Where(x => ExtractExponent(x) == 6).ToList();

/*  
 * Humidity:
Console.WriteLine(humidityexponentList0.Count);
Console.WriteLine(humidityexponentList1.Count);
Console.WriteLine(humidityexponentList2.Count);
Console.WriteLine(humidityexponentList3.Count);
Console.WriteLine(humidityexponentList4.Count);
Console.WriteLine(humidityexponentList5.Count);
Console.WriteLine(humidityexponentList6.Count);
*/


//Temperature:
Console.WriteLine(exponent2List.Count);
Console.WriteLine(exponent3List.Count);
Console.WriteLine(exponent4List.Count);
Console.WriteLine(exponent5List.Count);

List<List<float>> listOfListsTemperature = new List<List<float>>() { exponent2List, exponent3List, exponent4List, exponent5List };
List<List<float>> listOfListsHumidity = new List<List<float>>() { humidityexponentList0, humidityexponentList1, humidityexponentList2, humidityexponentList3, humidityexponentList4, humidityexponentList5, humidityexponentList6};



// Calculating averages
double overallAverage = temperatureFloats.Average();

Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine($"Overall Average: {overallAverage}");

// Displaying the size, max, and min
Console.Write("Size of the dataset: ");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine(temperatureFloats.Count);
Console.ForegroundColor = ConsoleColor.White;
Console.Write("Max value: ");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine(temperatureFloats.Max());
Console.ForegroundColor = ConsoleColor.White;
Console.Write("Min value: ");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine(temperatureFloats.Min());
Console.ForegroundColor = ConsoleColor.White;

var resultFromProgram = new List<float>();

foreach (var q in listOfListsTemperature)
{
    foreach (var (M, pattern) in testDict)
    {
        resultFromProgram = RunUsingExtension(M, pattern, q, 0, 100);

        for (int i = 0; i < 15; i++)
        {
            //RunUsingReplaceOnce(M, pattern, temperatureFloats, i, 100);
            //RunDefault(M, pattern, temperatureFloats, i, 100);
        }

        AddLaplaceNoise(resultFromProgram, 1.0, 0.1, M);
    }
}




// Function to extract the exponent from a float
int ExtractExponent(float value)
{
    byte[] bytes = BitConverter.GetBytes(value);
    int intRepresentation = BitConverter.ToInt32(bytes, 0);
    int exponentPart = (intRepresentation >> 23) & 0xFF;  // Extract the 8-bit exponent
    return exponentPart - 127;  // Return the effective exponent
}


void AddLaplaceNoise(List<float> floats, double epsilon, double deltaF, float M)
{
    var noisyList = new List<float>();

    foreach (var value in floats)
    {
        var res = laplaceNoise.GenerateNoiseCenteredConsideringExponent(value, epsilon, deltaF);
        noisyList.Add(res);
    }

    var sharedIndexes = compressionMechanism.CalculateSharedIndexes(noisyList);

    Console.WriteLine("Added Laplace noise: \n");

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

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"\n\nAverage of Laplacian List: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(noisyList.Average() / M);
    Console.WriteLine("\n");
    Console.ForegroundColor = ConsoleColor.White;
}

List<float> RunDefault(float M, string pattern, List<float> floats, int i, int nextBits)
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

    return newFloats;
}

List<float> RunUsingExtension(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits)
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

    for (int i = 0; i < 15; i++)
    {
        var res = resultsFromExtension.Where(x => ExtractExponent(x) == i).ToList();

        if (res.Count != 0)
        {
            Console.WriteLine($"\n\nNumber of new floats in exponent {i}: {res.Count}");
        }

        
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

    return resultsFromExtension;
}

List<float> RunUsingReplaceOnce(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits)
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

    return resultsFromExtension;
}

