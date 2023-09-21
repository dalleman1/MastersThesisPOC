using MastersThesisPOC;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddScoped<IAlgorithmHelper, AlgorithmHelper>()
    .AddScoped<IMathComputer, MathComputer>()
    .AddScoped<IAlgorithm, Algorithm>()
    .AddScoped<TrailingZerosStrategy>()
    .AddScoped<TrailingOnesStrategy>()
    .AddScoped<IMetrics, Metrics>()
    .AddScoped<IServiceExecuter, ServiceExecuter>(serviceProvider =>
            {
                var algorithmHelper = serviceProvider.GetRequiredService<IAlgorithmHelper>();
                var trailingZeros = new ProgramInstances(algorithmHelper, serviceProvider.GetRequiredService<TrailingZerosStrategy>(), serviceProvider.GetRequiredService<IMathComputer>(), serviceProvider.GetRequiredService<IMetrics>());
                var trailingOnes = new ProgramInstances(algorithmHelper, serviceProvider.GetRequiredService<TrailingOnesStrategy>(), serviceProvider.GetRequiredService<IMathComputer>(), serviceProvider.GetRequiredService<IMetrics>());
                return new ServiceExecuter(trailingZeros, trailingOnes);
            });

var serviceProvider = services.BuildServiceProvider();

var executer = serviceProvider.GetRequiredService<IServiceExecuter>();
var metricsProvider = serviceProvider.GetRequiredService<IMetrics>();
var algoHelper = serviceProvider.GetRequiredService<IAlgorithmHelper>();



var dict = new Dictionary<float, string>();
var floatDict = new Dictionary<float, string>();
var MTimesTwoPlusOneDict = new Dictionary<float, string>();
var testDict = new Dictionary<float, string>();

floatDict.Add(3.5f, "001");
floatDict.Add(5.5f, "0111010001");
floatDict.Add(7.5f, "0001");
floatDict.Add(8.5f, "11100001");
floatDict.Add(9.5f, "101011110010100001");
floatDict.Add(10.5f, "100001");
floatDict.Add(13.5f, "001011110110100001");

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


testDict.Add(63f, "00001");

MTimesTwoPlusOneDict.Add(3f, "01");
MTimesTwoPlusOneDict.Add(7f, "001");
MTimesTwoPlusOneDict.Add(15f, "0001");
MTimesTwoPlusOneDict.Add(31f, "00001");
MTimesTwoPlusOneDict.Add(63f, "000001");
MTimesTwoPlusOneDict.Add(127f, "0000001");
MTimesTwoPlusOneDict.Add(255f, "00000001");
MTimesTwoPlusOneDict.Add(511f, "000000001");
MTimesTwoPlusOneDict.Add(1023f, "0000000001");
MTimesTwoPlusOneDict.Add(2047f, "00000000001");
MTimesTwoPlusOneDict.Add(4095f, "000000000001");



var listOfFloats = executer.GenerateFloats(2000);



foreach (var (M, pattern) in testDict)
{
    for (int i = 0; i < 8; i++)
    {
        Console.WriteLine("\n\n");
        Console.WriteLine($"Value of M: {M} | Pattern: {pattern}");
        Console.WriteLine("Starting from index: " + i + "\n");

        //var (res, res2) = executer.ExecuteWithTrailingZerosWithRounding(dict, listOfFloats, i, 100);
        //var(res, res2) = executer.ExecutePrivatizedM(dict, listOfFloats, i, 100, 20.0f);

        //executer.PrintMPerformance(res, res2, "zeros");

        var newFloats = executer.ComputeBasicCompressedList(M, pattern, listOfFloats, i, 100);

        var result = executer.CalculateSharedIndexes(newFloats);

        var revertedList = executer.ComputeOriginalNumbersFromCompressedList(M, newFloats);

        var averageError = metricsProvider.CalculateAverageErrorPercentDifference(listOfFloats, revertedList);

        Console.WriteLine($"Average Individual Error %: {averageError}\n");

        var (startAverage, endAverage) = metricsProvider.CalculateAverages(listOfFloats, revertedList);

        Console.WriteLine($"Average of starting list: {startAverage}\n");
        Console.WriteLine($"Average of compressed list: {endAverage}\n");

        foreach (int index in Enumerable.Range(0, 23))
        {
            if (result.Contains(index))
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;

            }

            Console.Write($"[{index}] ");

            Console.ResetColor(); // Reset text color to default
        }
        Console.WriteLine();
    }
}



