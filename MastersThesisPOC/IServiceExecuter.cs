namespace MastersThesisPOC
{
    public interface IServiceExecuter
    {
        (Dictionary<float, int>, Dictionary<float, float>) ExecuteWithTrailingZerosWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
        (Dictionary<float, int>, Dictionary<float, float>) ExecuteWithTrailingZerosWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex);
        (Dictionary<float, int>, Dictionary<float, float>) ExecuteWithTrailingOnesWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
        (Dictionary<float, int>, Dictionary<float, float>) ExecuteWithTrailingOnesWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex);
        List<float> GenerateFloats(int amount);
        void PrintMPerformance(Dictionary<float, int> performance, Dictionary<float, float> errorPerformance, string type);
    }
}
