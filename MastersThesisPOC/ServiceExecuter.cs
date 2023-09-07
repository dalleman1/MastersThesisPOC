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

    }
}
