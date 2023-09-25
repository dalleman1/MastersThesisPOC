using MastersThesisPOC;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using MastersThesisPOC.Helpers;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddScoped<IAlgorithmHelper, AlgorithmHelper>()
    .AddScoped<IMathComputer, MathComputer>()
    .AddScoped<ILaplaceNoiseGenerator, LaplaceNoiseGenerator>()
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
var laplaceNoise = serviceProvider.GetRequiredService<ILaplaceNoiseGenerator>();


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


testDict.Add(13f, "000100111011");

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
MTimesTwoPlusOneDict.Add(8191f, "0000000000001");
MTimesTwoPlusOneDict.Add(16383f, "00000000000001");

var listOfFloats = executer.GenerateFloats(10);

var listOfEpsilonsAndSpreads = new ListWithDuplicates();

listOfEpsilonsAndSpreads.Add(1.0f, 1.0f);
listOfEpsilonsAndSpreads.Add(0.8f, 1.0f);
listOfEpsilonsAndSpreads.Add(0.5f, 1.0f);
listOfEpsilonsAndSpreads.Add(0.3f, 1.0f);
listOfEpsilonsAndSpreads.Add(0.1f, 1.0f);

listOfEpsilonsAndSpreads.Add(1.0f, 0.5f);
listOfEpsilonsAndSpreads.Add(0.8f, 0.5f);
listOfEpsilonsAndSpreads.Add(0.5f, 0.5f);
listOfEpsilonsAndSpreads.Add(0.3f, 0.5f);
listOfEpsilonsAndSpreads.Add(0.1f, 0.5f);

listOfEpsilonsAndSpreads.Add(1.0f, 0.2f);
listOfEpsilonsAndSpreads.Add(0.8f, 0.2f);
listOfEpsilonsAndSpreads.Add(0.5f, 0.2f);
listOfEpsilonsAndSpreads.Add(0.3f, 0.2f);
listOfEpsilonsAndSpreads.Add(0.1f, 0.2f);

listOfEpsilonsAndSpreads.Add(1.0f, 0.1f);
listOfEpsilonsAndSpreads.Add(0.8f, 0.1f);
listOfEpsilonsAndSpreads.Add(0.5f, 0.1f);
listOfEpsilonsAndSpreads.Add(0.3f, 0.1f);
listOfEpsilonsAndSpreads.Add(0.1f, 0.1f);

foreach (var (M, pattern) in testDict)
{
    foreach (var (epsilon, spread) in listOfEpsilonsAndSpreads)
    {
        for (int i = 3; i < 5; i++)
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

            var newFloats = executer.ComputeBasicCompressedList(M, pattern, listOfFloats, i, 100);

            var sharedIndexes = executer.CalculateSharedIndexes(newFloats);

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
            Console.Write("Adding Laplacian Noise | Epsilon: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(epsilon);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" | Spread: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(spread);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n");

            var newFloatsWithLaplace = new List<float>();

            foreach (var newFloat in newFloats)
            {
                //var noise = laplaceNoise.GenerateNoiseScaled(epsilon, spread, M);
                var noise = laplaceNoise.GenerateNoise(epsilon, spread);
                //Console.WriteLine(noise);
                newFloatsWithLaplace.Add(newFloat + noise);
            }

            var newSharedIndexes = executer.CalculateSharedIndexes(newFloatsWithLaplace);

            foreach (int index in Enumerable.Range(0, 23))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (newSharedIndexes.Contains(index))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;

                }

                Console.Write($"[{index}] ");

                Console.ResetColor(); // Reset text color to default
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            var revertedList = executer.ComputeOriginalNumbersFromCompressedList(M, newFloats);

            var revertedLaplacianList = executer.ComputeOriginalNumbersFromCompressedList(M, newFloatsWithLaplace);

            var averageError = metricsProvider.CalculateAverageErrorPercentDifference(listOfFloats, revertedList);

            var averageErrorLaplace = metricsProvider.CalculateAverageErrorPercentDifference(listOfFloats, revertedLaplacianList);

            var (startAverageLaplace, endAverageLaplace) = metricsProvider.CalculateAverages(listOfFloats, revertedLaplacianList);

            var (startAverage, endAverage) = metricsProvider.CalculateAverages(listOfFloats, revertedList);

            Console.Write($"Average Individual Error %: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(averageError);
            Console.WriteLine("\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write($"Average Individual Error % with Laplacian Noise: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(averageErrorLaplace);
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

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Average of compressed list with Laplacian Noise: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(endAverageLaplace);
            Console.WriteLine("\n");
            Console.ForegroundColor = ConsoleColor.White;


        }
    }
}



