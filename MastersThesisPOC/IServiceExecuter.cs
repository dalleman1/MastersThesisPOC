namespace MastersThesisPOC
{
    public interface IServiceExecuter
    {
        (Dictionary<float, int>, Dictionary<float, float>) 
            ExecutePrivatizedM(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits, float epsilon);

        (Dictionary<float, int>, Dictionary<float, float>) 
            ExecuteWithTrailingZerosWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);

        (Dictionary<float, int>, Dictionary<float, float>) 
            ExecuteWithTrailingZerosWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex);

        (Dictionary<float, int>, Dictionary<float, float>) 
            ExecuteWithTrailingOnesWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);

        (Dictionary<float, int>, Dictionary<float, float>) 
            ExecuteWithTrailingOnesWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex);

        List<float> ComputeBasicCompressedList(float M, string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits, bool extension);
        List<float> ComputeOriginalNumbersFromCompressedList(float M, List<float> compressedNumbers);

        List<float> GenerateFloats(int amount);

        void PrintMPerformance(Dictionary<float, int> performance, Dictionary<float, float> errorPerformance, string type);

        List<int> CalculateSharedIndexes(List<float> numbers);
    }
}
