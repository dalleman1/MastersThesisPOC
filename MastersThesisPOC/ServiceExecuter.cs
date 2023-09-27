namespace MastersThesisPOC
{

    public class ServiceExecuter : IServiceExecuter
    {
        private readonly IProgramInstances _trailingZeros;
        private readonly IProgramInstances _trailingOnes;
        public ServiceExecuter(IProgramInstances trailingZeros, IProgramInstances trailingOnes)
        {
            _trailingZeros = trailingZeros;
            _trailingOnes = trailingOnes;
        }

        public List<float> ComputeBasicCompressedList(float M, string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits, bool extension)
        {
            return _trailingZeros.ComputeBasicCompressedList(M, pattern, numbers, patternStartIndex, amountOfRoundingBits, extension);
        }

        public List<float> ComputeOriginalNumbersFromCompressedList(float M, List<float> compressedNumbers)
        {
            return _trailingZeros.ComputeOriginalNumbersFromCompressedList(M, compressedNumbers);
        }

        public (Dictionary<float, int>, Dictionary<float, float>) ExecutePrivatizedM(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits, float epsilon)
        {
            return _trailingZeros.ComputeUsingPrivateM(basePatternDictionary, numbers, patternStartIndex, amountOfRoundingBits, epsilon);
        }

        public (Dictionary<float, int>, Dictionary<float, float>) ExecuteWithTrailingOnesWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex)
        {
            return _trailingOnes.ComputeBestMWithNoRounding(basePatternDictionary, numbers, patternStartIndex, null);
        }

        public (Dictionary<float, int>, Dictionary<float, float>) ExecuteWithTrailingOnesWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            return _trailingOnes.ComputeBestMWithRounding(basePatternDictionary, numbers, patternStartIndex, amountOfRoundingBits);
        }

        public (Dictionary<float, int>, Dictionary<float, float>) ExecuteWithTrailingZerosWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex)
        {
            return _trailingZeros.ComputeBestMWithNoRounding(basePatternDictionary, numbers, patternStartIndex, null);
        }

        public (Dictionary<float, int>, Dictionary<float, float>) ExecuteWithTrailingZerosWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            return _trailingZeros.ComputeBestMWithRounding(basePatternDictionary, numbers, patternStartIndex, amountOfRoundingBits);
        }

        public List<float> GenerateFloats(int amount)
        {
            Random random = new Random();
            List<float> floatList = new List<float>();

            for (int i = 0; i < amount; i++)
            {
                float randomFloat = (float)random.NextDouble() * 49 + 1; // Random float between 1 and 50
                floatList.Add(randomFloat);
            }

            return floatList;
        }

        public void PrintMPerformance(Dictionary<float, int> Mperformance, Dictionary<float, float> errorPerformance, string type)
        {
            foreach (var (M, performance) in Mperformance)
            {
                Console.WriteLine($"Value of M: {M} | {performance} amount of trailing {type}");
            }

            Console.WriteLine("\n");

            foreach (var (M, error) in errorPerformance)
            {
                Console.WriteLine($"Value of M: {M} | {error}% max error");
            }
        }
        /// <summary>
        ///     It starts with the bit at index ii of the first mantissa.
        ///     It uses the All method to determine if every mantissa in the list has that same bit value at the same position ii.
        ///     If all mantissas have the same bit at that position, the index ii is added to the sharedIndexes list.
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public List<int> CalculateSharedIndexes(List<float> numbers)
        {
            List<string> mantissas = numbers.Select(GetMantissaBits).ToList();

            List<int> sharedIndexes = new List<int>();

            foreach (int i in Enumerable.Range(0, 23))
            {
                char bit = mantissas[0][i];

                if (mantissas.All(m => m[i] == bit))
                {
                    sharedIndexes.Add(i);
                }
            }

            return sharedIndexes;
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
