using CsvHelper;
using MastersThesisPOC.CustomMath;
using MastersThesisPOC.Helpers;
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

        public List<float> RunProgram(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits, float epsilon, float sensitivity)
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

            _logger.Information($"Epsilon: {epsilon}, Sensitivity: {sensitivity}");
            foreach (var result in resultsFromExtension)
            {
                var noisyNumber = _laplaceNoiseGenerator.GenerateNoiseCentered(result, epsilon, sensitivity);
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

                var multipliedNumber = item * M;
                //var roundedNumber = (float)Math.Round(multipliedNumber, 1, MidpointRounding.AwayFromZero);
                multipliedList.Add(multipliedNumber);

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
                    _logger.Information($"\n\nNumber of multiplied floats in exponent {i}: {res.Count}");
                }
            }

            _logger.Information($"Number of shared indexes after multiplication: {sharedIndexesFromMultiplication.Count}");

            Console.WriteLine("Average After Multiplication:" + multipliedList.Average());
            Console.WriteLine("Largest After Multiplied: " + multipliedList.Max());
            Console.WriteLine("Smallest After Multiplied: " + multipliedList.Min());

            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            var revertedList = _compressionMechanism.ComputeOriginalNumbersFromCompressedList(M, multipliedList);
            var (startAverage, endAverage) = _metrics.CalculateAverages(floats, revertedList);

            var revertedList10 = revertedList.Take(10).ToList();
            var floats10 = floats.Take(10).ToList();

            for (int i = 0; i < revertedList10.Count; i++)
            {
                _logger.Information($"{revertedList[i]}");
            }

            for (int i = 0; i < floats10.Count; i++)
            {
                _logger.Information($"{floats10[i]}");
            }

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

            _logger.Information($"Min of reverted multiplied: {revertedList.Min()}");

            _logger.Information($"Max of reverted multiplied: {revertedList.Max()}");

            _logger.Information($"Avg of reverted multiplied: {revertedList.Average()}");

            _logger.Information($"Difference in Average: {difference} %");

            _logger.Information($"Average difference of the individual numbers: {individualDifference} %");

            // Calculate MRE
            double mre = floats.Zip(revertedList, (original, noisy) => Math.Abs(original - noisy) / original)
                                     .Average();

            _logger.Information($"Mean Relative Error (MRE): {mre * 100} %");

            // Calculate RMSE
            double rmse = Math.Sqrt(floats.Zip(revertedList, (original, noisy) => Math.Pow(original - noisy, 2))
                                               .Average());

            _logger.Information($"Root Mean Square Error (RMSE): {rmse}");

            using (StreamWriter writer = new StreamWriter($"C:\\Users\\mongl\\OneDrive\\Skrivebord\\logs\\log-{DateTime.Now:yyyyMMdd-HHmmss}-DATA.txt"))
            {
                for (int i = 0; i < Math.Max(revertedList.Count, floats.Count); i++)
                {
                    string revertedValue = i < revertedList.Count ? revertedList[i].ToString() : "";
                    string floatValue = i < floats.Count ? floats[i].ToString() : "";

                    writer.WriteLine($"{revertedValue}\t{floatValue}");
                }
            }

            return multipliedList;
        }

        public List<float> RunProgramFast(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits, float epsilon, float sensitivity)
        {
            
            var (resultsFromExtension, shiftedPattern) = _compressionMechanism.ComputeBasicCompressedListReplacingOnceWithOutMultiplication(pattern, floats, patternStartIndex, nextBits);

           
            
            Console.WriteLine();

            var noisyNumbers = new List<float>();

            _logger.Information($"Epsilon: {epsilon}, Sensitivity: {sensitivity}");

            _logger.Information($"M: {M} | Pattern: {pattern} | ShiftedPattern: {shiftedPattern}");

            foreach (var result in resultsFromExtension)
            {
                var noisyNumber = _laplaceNoiseGenerator.GenerateNoiseCentered(result, epsilon, sensitivity);
                
                noisyNumbers.Add(noisyNumber);

            }

           
            var propagationList = _compressionMechanism.AddCarryOverPropagationBits(noisyNumbers, shiftedPattern);


            var multipliedList = new List<float>();

            foreach (var item in propagationList)
            {
                var multipliedNumber = item * M;
                
                multipliedList.Add(multipliedNumber);

            }

            var revertedList = _compressionMechanism.ComputeOriginalNumbersFromCompressedList(M, multipliedList);
            var (startAverage, endAverage) = _metrics.CalculateAverages(floats, revertedList);
            var difference = Math.Abs((((multipliedList.Average() / M) - startAverage) / startAverage) * 100);
            var individualDifference = _metrics.CalculateAverageErrorPercentDifference(floats, multipliedList, M);

            _logger.Information($"Min of reverted multiplied: {revertedList.Min()}");

            _logger.Information($"Max of reverted multiplied: {revertedList.Max()}");

            _logger.Information($"Avg of reverted multiplied: {revertedList.Average()}");

            _logger.Information($"Difference in Average: {difference} %");

            _logger.Information($"Average difference of the individual numbers: {individualDifference} %");


            double averagePercentageDifference = resultsFromExtension.Zip(floats, (a, b) =>
            (Math.Abs(a - b) / ((Math.Abs(a) + Math.Abs(b)) / 2)) * 100).Average();

            _logger.Information($"Average individual percentage difference after applying substitution before noise: {averagePercentageDifference}%");


            // Calculate MRE
            double mre = floats.Zip(revertedList, (original, noisy) => Math.Abs(original - noisy) / original)
                                     .Average();

            _logger.Information($"Mean Relative Error (MRE): {mre * 100} %");

            // Calculate RMSE
            double rmse = Math.Sqrt(floats.Zip(revertedList, (original, noisy) => Math.Pow(original - noisy, 2))
                                               .Average());

            _logger.Information($"Root Mean Square Error (RMSE): {rmse}");


            using (StreamWriter writer = new StreamWriter($"C:\\Users\\mongl\\OneDrive\\Skrivebord\\logs\\log-{DateTime.Now:yyyyMMdd-HHmmss}-DATA.txt"))
            {
                for (int i = 0; i < Math.Max(revertedList.Count, floats.Count); i++)
                {
                    string revertedValue = i < revertedList.Count ? revertedList[i].ToString() : "";
                    string floatValue = i < floats.Count ? floats[i].ToString() : "";

                    writer.WriteLine($"{revertedValue}\t{floatValue}");
                }
            }

            return multipliedList;
        }

        public List<float> RunProgramFastNoiseBeforePattern(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits, float epsilon, float sensitivity, int mantissaStart)
        {

            var noisyNumbers = new List<float>();

            _logger.Information($"Epsilon: {epsilon}, Sensitivity: {sensitivity}");

            foreach (var number in floats)
            {
                var noisyNumber = _laplaceNoiseGenerator.GenerateNoiseCentered(number, epsilon, sensitivity);

                noisyNumbers.Add(noisyNumber);

            }
            var second10 = noisyNumbers.Take(10);

            foreach (var item in second10)
            {
                Console.WriteLine($"{GetMantissaBits(item)}");
            }

            Console.WriteLine();

            var (resultsFromExtension, shiftedPattern) = _compressionMechanism.ComputeBasicCompressedListReplacingOnceWithOutMultiplicationAfterNoise(pattern, noisyNumbers, patternStartIndex, nextBits, mantissaStart);

            _logger.Information($"M: {M} | Pattern: {pattern} | ShiftedPattern: {shiftedPattern}");

            var first10 = resultsFromExtension.Take(10);

            foreach (var item in first10)
            {
                Console.WriteLine($"{GetMantissaBits(item)}");
            }

            Console.WriteLine();

            var multipliedList = new List<float>();

            foreach (var item in resultsFromExtension)
            {
                var multipliedNumber = item * M;

                multipliedList.Add(multipliedNumber);

            }

            var fourth10 = multipliedList.Take(10);

            foreach (var item in fourth10)
            {
                Console.WriteLine($"{GetMantissaBits(item)}");
            }

            var revertedList = _compressionMechanism.ComputeOriginalNumbersFromCompressedList(M, multipliedList);
            var (startAverage, endAverage) = _metrics.CalculateAverages(floats, revertedList);
            var difference = Math.Abs((((multipliedList.Average() / M) - startAverage) / startAverage) * 100);
            var individualDifference = _metrics.CalculateAverageErrorPercentDifference(floats, multipliedList, M);

            _logger.Information($"Min of reverted multiplied: {revertedList.Min()}");

            _logger.Information($"Max of reverted multiplied: {revertedList.Max()}");

            _logger.Information($"Avg of reverted multiplied: {revertedList.Average()}");

            _logger.Information($"Difference in Average: {difference} %");

            _logger.Information($"Average difference of the individual numbers: {individualDifference} %");

            double averagePercentageDifference = resultsFromExtension.Zip(noisyNumbers, (a, b) =>
            (Math.Abs(a - b) / ((Math.Abs(a) + Math.Abs(b)) / 2)) * 100).Average();

            _logger.Information($"Average individual percentage difference after applying substitution before noise: {averagePercentageDifference}%");

            // Calculate MRE
            double mre = floats.Zip(revertedList, (original, noisy) => Math.Abs(original - noisy) / original)
                                     .Average();

            _logger.Information($"Mean Relative Error (MRE): {mre * 100} %");

            // Calculate RMSE
            double rmse = Math.Sqrt(floats.Zip(revertedList, (original, noisy) => Math.Pow(original - noisy, 2))
                                               .Average());

            _logger.Information($"Root Mean Square Error (RMSE): {rmse}");

            using (StreamWriter writer = new StreamWriter($"C:\\Users\\mongl\\OneDrive\\Skrivebord\\logs\\log-{DateTime.Now:yyyyMMdd-HHmmss}-DATA.txt"))
            {
                for (int i = 0; i < Math.Max(revertedList.Count, floats.Count); i++)
                {
                    string revertedValue = i < revertedList.Count ? revertedList[i].ToString() : "";
                    string floatValue = i < floats.Count ? floats[i].ToString() : "";

                    writer.WriteLine($"{revertedValue}\t{floatValue}");
                }
            }

            return multipliedList;
        }

        public List<float> RunProgramOnlyPrivacy(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits, float epsilon, float sensitivity)
        {
            var noisyNumbers = new List<float>();

            _logger.Information($"Epsilon: {epsilon}, Sensitivity: {sensitivity}");

            foreach (var result in floats)
            {
                var noisyNumber = _laplaceNoiseGenerator.GenerateNoiseCentered(result, epsilon, sensitivity);

                noisyNumbers.Add(noisyNumber);

            }

            var multipliedList = new List<float>();

            foreach (var item in noisyNumbers)
            {
                var multNumber = item * M;
                
                multipliedList.Add(multNumber);

            }

            var revertedList = _compressionMechanism.ComputeOriginalNumbersFromCompressedList(M, multipliedList);
            var (startAverage, endAverage) = _metrics.CalculateAverages(floats, revertedList);


            var difference = Math.Abs((((multipliedList.Average() / M) - startAverage) / startAverage) * 100);
            var individualDifference = _metrics.CalculateAverageErrorPercentDifference(floats, multipliedList, M);

            _logger.Information($"Min of reverted multiplied: {revertedList.Min()}");

            _logger.Information($"Max of reverted multiplied: {revertedList.Max()}");

            _logger.Information($"Avg of reverted multiplied: {revertedList.Average()}");

            _logger.Information($"Difference in Average: {difference} %");

            _logger.Information($"Average difference of the individual numbers: {individualDifference} %");

            // Calculate MRE
            double mre = floats.Zip(revertedList, (original, noisy) => Math.Abs(original - noisy) / original)
                                     .Average();

            _logger.Information($"Mean Relative Error (MRE): {mre * 100} %");

            // Calculate RMSE
            double rmse = Math.Sqrt(floats.Zip(revertedList, (original, noisy) => Math.Pow(original - noisy, 2))
                                               .Average());

            _logger.Information($"Root Mean Square Error (RMSE): {rmse}");

            return noisyNumbers;
        }

        // Function to extract the exponent from a float
        int ExtractExponent(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            int intRepresentation = BitConverter.ToInt32(bytes, 0);
            int exponentPart = (intRepresentation >> 23) & 0xFF;  // Extract the 8-bit exponent
            return exponentPart - 127;  // Return the effective exponent
        }
        // Extract the mantissa bits from a float
        private string GetMantissaBits(float number)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            uint intRepresentation = BitConverter.ToUInt32(bytes, 0);

            // Get the last 23 bits (mantissa)
            uint mantissa = intRepresentation & 0x7FFFFF;

            return Convert.ToString(mantissa, 2).PadLeft(23, '0');
        }

    }
}
