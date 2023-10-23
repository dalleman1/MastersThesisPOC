using MastersThesisPOC.CustomMath;
using Serilog;

namespace MastersThesisPOC
{

    public class CompressionExecuter : ICompressionExecuter
    {
        private readonly ICompressionMechanism _compressionMechanism;
        private readonly ILaplaceNoiseGenerator _laplaceNoiseGenerator;
        private readonly IMetrics _metrics;
        private readonly ILogger _logger;

        public CompressionExecuter(ICompressionMechanism compressionMechanism, ILaplaceNoiseGenerator laplaceNoiseGenerator, IMetrics metrics, ILogger logger)
        {
            _compressionMechanism = compressionMechanism;
            _laplaceNoiseGenerator = laplaceNoiseGenerator;
            _metrics = metrics;
            _logger = logger;
        }

        public List<float> RunProgram(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits)
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


            var (resultsFromExtension, shiftedPattern) = _compressionMechanism.ComputeBasicCompressedListReplacingOnceWithOutMultiplication(pattern, floats, patternStartIndex, nextBits);
            Console.WriteLine("Average After Applying Pattern:" + resultsFromExtension.Average());

            _logger.Information($"M: {M} | Pattern: {pattern} | ShiftedPattern: {shiftedPattern}");

            var sharedIndexesFromExtension = _compressionMechanism.CalculateSharedIndexes(resultsFromExtension);

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

            _logger.Information($"Number of shared indexes: {sharedIndexesFromExtension.Count}");

            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Adding Noise\n");

            var noisyNumbers = new List<float>();

            foreach (var result in resultsFromExtension)
            {
                var noisyNumber = _laplaceNoiseGenerator.GenerateNoiseCentered(result, 0.5f, 0.01f);
                //Console.WriteLine(noisyNumber);
                noisyNumbers.Add(noisyNumber);

            }

            var sharedIndexesFromNoise = _compressionMechanism.CalculateSharedIndexes(noisyNumbers);
            Console.WriteLine();
            foreach (int index in Enumerable.Range(0, 23))
            {

                Console.ForegroundColor = ConsoleColor.Green;
                if (sharedIndexesFromNoise.Keys.Contains(index))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.Write($"[{index}] ");

                Console.ResetColor(); // Reset text color to default
            }
            Console.WriteLine();
            Console.WriteLine();
            foreach (var (key, value) in sharedIndexesFromNoise)
            {
                Console.WriteLine($"Index: {key}, value: {value}");
            }

            _logger.Information($"Number of shared indexes after adding noise: {sharedIndexesFromNoise.Count}");

            Console.WriteLine("Average After Adding Noise:" + noisyNumbers.Average());
            Console.WriteLine("Largest Value After Adding Noise:" + noisyNumbers.Max());
            Console.WriteLine("Smallest Value After Adding Noise:" + noisyNumbers.Min());


            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Adding Propagation Zeros\n");

            var propagationList = _compressionMechanism.AddCarryOverPropagationBits(noisyNumbers, shiftedPattern);
            var sharedIndexesFromPropagation = _compressionMechanism.CalculateSharedIndexes(propagationList);
            Console.WriteLine();
            foreach (int index in Enumerable.Range(0, 23))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (sharedIndexesFromPropagation.Keys.Contains(index))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.Write($"[{index}] ");

                Console.ResetColor(); // Reset text color to default
            }
            Console.WriteLine();
            Console.WriteLine();
            foreach (var (key, value) in sharedIndexesFromPropagation)
            {
                Console.WriteLine($"Index: {key}, value: {value}");
            }

            _logger.Information($"Number of shared indexes after adding propagation zeros: {sharedIndexesFromPropagation.Count}");

            Console.WriteLine("Average After Adding Propagation Zeros:" + propagationList.Average());
            Console.WriteLine("Largest Value After Adding Propagation Zeros:" + propagationList.Max());
            Console.WriteLine("Smallest Value After Adding Propagation Zeros:" + propagationList.Min());

            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Multiplication \n");


            var multipliedList = new List<float>();

            foreach (var item in propagationList)
            {

                multipliedList.Add(item * M);

            }

            Console.WriteLine();

            var sharedIndexesFromMultiplication = _compressionMechanism.CalculateSharedIndexes(multipliedList);
            Console.WriteLine();
            foreach (int index in Enumerable.Range(0, 23))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (sharedIndexesFromMultiplication.Keys.Contains(index))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.Write($"[{index}] ");

                Console.ResetColor(); // Reset text color to default
            }
            Console.WriteLine();
            Console.WriteLine();
            foreach (var (key, value) in sharedIndexesFromMultiplication)
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

            _logger.Information($"Number of shared indexes after multiplication: {sharedIndexesFromMultiplication.Count}");

            Console.WriteLine("Average After Multiplication:" + multipliedList.Average());
            Console.WriteLine("Largest After Multiplied: " + multipliedList.Max());
            Console.WriteLine("Smallest After Multiplied: " + multipliedList.Min());

            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            var revertedList = _compressionMechanism.ComputeOriginalNumbersFromCompressedList(M, resultsFromExtension);
            var (startAverage, endAverage) = _metrics.CalculateAverages(floats, revertedList);

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

            var difference = Math.Abs((((multipliedList.Average() / M) - startAverage) / startAverage) * 100);
            var individualDifference = _metrics.CalculateAverageErrorPercentDifference(floats, multipliedList, M);

            _logger.Information($"Difference in Average: {difference} %");

            _logger.Information($"Average difference of the individual numbers: {individualDifference} %");

            return multipliedList;
        }

        // Function to extract the exponent from a float
        int ExtractExponent(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            int intRepresentation = BitConverter.ToInt32(bytes, 0);
            int exponentPart = (intRepresentation >> 23) & 0xFF;  // Extract the 8-bit exponent
            return exponentPart - 127;  // Return the effective exponent
        }

    }
}
