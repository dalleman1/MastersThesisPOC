namespace MastersThesisPOC
{
    public interface IProgramInstances
    {
        float CalculatePercentDifference(float value1, float value2);
        (Dictionary<float, int>, Dictionary<float, float>) ComputeBestMWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
        (Dictionary<float, int>, Dictionary<float, float>) ComputeBestMWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int? amountOfRoundingBits);
    }
}
