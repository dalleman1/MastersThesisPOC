namespace MastersThesisPOC
{
    public interface IProgramInstances
    {

        (Dictionary<float, int>, Dictionary<float, float>) 
            ComputeUsingPrivateM(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits, float epsilon);

        (Dictionary<float, int>, Dictionary<float, float>) 
            ComputeBestMWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);

        (Dictionary<float, int>, Dictionary<float, float>) 
            ComputeBestMWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int? amountOfRoundingBits);

        List<float> ComputeBasicCompressedList(float M, string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
        List<float> ComputeOriginalNumbersFromCompressedList(float M, List<float> compressedNumbers);
    }
}
