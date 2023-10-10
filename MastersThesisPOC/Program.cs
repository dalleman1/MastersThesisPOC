using MastersThesisPOC;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using MastersThesisPOC.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

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
dict.Add(3.5f, "001");
dict.Add(5f, "0011");
dict.Add(7f, "001");
dict.Add(7.5f, "0001");
dict.Add(9f, "000111");
dict.Add(11f, "0001011101");
dict.Add(13f, "000100111011");
dict.Add(15f, "0001");
dict.Add(15.5f, "00001");
dict.Add(17f, "11100001");
dict.Add(19f, "101011110010100001");
dict.Add(19.5f, "101001000001");
dict.Add(21f, "100001");
dict.Add(23f, "01100100001");
dict.Add(31f, "00001");
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
testDict.Add(13f, "000100111011");

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


List<List<float>> listOfListsTemperature = new List<List<float>>() { exponent2List, exponent3List, exponent4List, exponent5List };
List<List<float>> listOfListsHumidity = new List<List<float>>() { humidityexponentList0, humidityexponentList1, humidityexponentList2, humidityexponentList3, humidityexponentList4, humidityexponentList5, humidityexponentList6};


var resultFromProgram = new List<float>();


var size = compressionMechanism.CalculateMantissasSizeInBytes(exponent2List);

Console.WriteLine($"Size of all of the mantissas of exponent2List: {size} bytes");

foreach (var (M, pattern) in testDict)
{
     //resultFromProgram = RunUsingReplaceOnce(M, pattern, exponent2List, 0, 100);

     for (int i = 0; i < 5; i++)
     {
        resultFromProgram = RunUsingReplaceOnce(M, pattern, exponent2List, i, 100);
        //RunUsingReplaceOnce(M, pattern, temperatureFloats, i, 100);
        //RunDefault(M, pattern, temperatureFloats, i, 100);
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

    var newFloats = compressionMechanism.ComputeBasicCompressedListUsingExtension(M, pattern, floats, i, nextBits);

    var sharedIndexes = compressionMechanism.CalculateSharedIndexes(newFloats);

    foreach (int index in Enumerable.Range(0, 23))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        if (sharedIndexes.Keys.Contains(index))
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
    Console.Write($"Average of multiplied list: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(endAverage);
    Console.WriteLine("\n");
    Console.ForegroundColor = ConsoleColor.White;

    var noisyNumbers = new List<float>();

    foreach (var result in newFloats)
    {

        var noisyNumber = laplaceNoise.GenerateNoiseCentered(result, 1.0f, 0.01f);

        noisyNumbers.Add(noisyNumber);

    }
    Console.WriteLine("Average Noise:" + noisyNumbers.Average());
    Console.WriteLine("Largest Noise:" + noisyNumbers.Max());
    Console.WriteLine("Smallest Noise:" + noisyNumbers.Min());

    Console.WriteLine();

    var sharedIndexesFromNoisyNumbers = compressionMechanism.CalculateSharedIndexes(noisyNumbers);

    foreach (int index in Enumerable.Range(0, 23))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        if (sharedIndexesFromNoisyNumbers.Keys.Contains(index))
        {
            Console.ForegroundColor = ConsoleColor.Blue;
        }

        Console.Write($"[{index}] ");

        Console.ResetColor(); // Reset text color to default
    }
    Console.WriteLine();
    Console.WriteLine();
    foreach (var (key, value) in sharedIndexesFromNoisyNumbers)
    {
        Console.WriteLine($"Index: {key}, value: {value}");
    }

    for (int j = 0; j < 15; j++)
    {
        var res = noisyNumbers.Where(x => ExtractExponent(x) == j).ToList();

        if (res.Count != 0)
        {
            Console.WriteLine($"\n\nNumber of multiplied floats in exponent {j}: {res.Count}");
        }


    }

    Console.WriteLine();
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;

    var compressed = compressionMechanism.CompressMantissasUsingGZip(noisyNumbers, sharedIndexesFromNoisyNumbers);

    Console.WriteLine($"Size of all compressed mantissas: {compressed.Length} bytes\n");

    var decompressed = compressionMechanism.DecompressMantissasUsingGZip(compressed);

    var ogdata = compressionMechanism.ReconstructOriginalMantissas(decompressed, sharedIndexesFromNoisyNumbers);

    // Convert the list of mantissas into a single string
    string mantissasString = string.Join("", ogdata);

    // Convert the string into bytes
    var bytes = Encoding.UTF8.GetBytes(mantissasString);

    Console.WriteLine($"Size of all of the mantissas of exponent2List: {bytes.Length} bytes\n");

    double compressionRatio = (double)size / compressed.Length;
    Console.WriteLine($"Compression Ratio: {compressionRatio:0.##} | {1 / compressionRatio} % of the original data");

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
        if (sharedIndexesFromExtension.Keys.Contains(index))
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

    Console.WriteLine("Average Start:" + floats.Average());


    var resultsFromExtension = compressionMechanism.ComputeBasicCompressedListReplacingOnceWithOutMultiplication(pattern, floats, patternStartIndex, nextBits);
    Console.WriteLine("Average Extensions:" + resultsFromExtension.Average());


    var noisyNumbers = new List<float>();
    
    foreach (var result in resultsFromExtension)
    {
        var noisyNumber = laplaceNoise.GenerateNoiseCentered(result, 1.0f, 0.01f);

        noisyNumbers.Add(noisyNumber);

    }
    Console.WriteLine("Average Noise:" + noisyNumbers.Average());
    Console.WriteLine("Largest Noise:" + noisyNumbers.Max());
    Console.WriteLine("Smallest Noise:" + noisyNumbers.Min());
    var multipliedList = new List<float>();

    foreach (var item in noisyNumbers)
    {
        multipliedList.Add(item * M);

        
    }
    Console.WriteLine("Average End:" + multipliedList.Average());
    Console.WriteLine("Largest Multiplied: " + multipliedList.Max());
    Console.WriteLine("Smallest Multiplied: " + multipliedList.Min());

    Console.WriteLine();

    var sharedIndexesFromExtension = compressionMechanism.CalculateSharedIndexes(multipliedList);

    foreach (int index in Enumerable.Range(0, 23))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        if (sharedIndexesFromExtension.Keys.Contains(index))
        {
            Console.ForegroundColor = ConsoleColor.Blue;
        }

        Console.Write($"[{index}] ");

        Console.ResetColor(); // Reset text color to default
    }
    Console.WriteLine();
    Console.WriteLine();
    foreach (var (key, value) in sharedIndexesFromExtension)
    {
        Console.WriteLine($"Index: {key}, value: {value}");
    }

    for (int i = 0; i < 15; i++)
    {
        var res = multipliedList.Where(x => ExtractExponent(x) == i).ToList();

        if (res.Count != 0)
        {
            Console.WriteLine($"\n\nNumber of multiplied floats in exponent {i}: {res.Count}");
        }


    }

    Console.WriteLine();
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;

    var revertedList = compressionMechanism.ComputeOriginalNumbersFromCompressedList(M, resultsFromExtension);
    var (startAverage, endAverage) = metricsProvider.CalculateAverages(floats, revertedList);

    Console.Write($"Average of starting list: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(startAverage);
    Console.WriteLine("\n");

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"Average of multiplied list: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(multipliedList.Average() / M);
    Console.WriteLine("\n");
    Console.ForegroundColor = ConsoleColor.White;


    var compressed = compressionMechanism.CompressMantissasUsingGZip(multipliedList, sharedIndexesFromExtension);

    Console.WriteLine($"Size of all compressed mantissas: {compressed.Length} bytes\n");

    var decompressed = compressionMechanism.DecompressMantissasUsingGZip(compressed);

    var ogdata = compressionMechanism.ReconstructOriginalMantissas(decompressed, sharedIndexesFromExtension);

    // Convert the list of mantissas into a single string
    string mantissasString = string.Join("", ogdata);

    // Convert the string into bytes
    var bytes = Encoding.UTF8.GetBytes(mantissasString);

    Console.WriteLine($"Size of all of the mantissas of exponent2List: {bytes.Length} bytes\n");

    double compressionRatio = (double)size / compressed.Length;
    Console.WriteLine($"Compression Ratio: {compressionRatio:0.##} | {1/compressionRatio} % of the original data");

    return resultsFromExtension;
}

