namespace MastersThesisPOC
{
    public interface ICompressionMechanism
    {
        List<float> ComputeBasicCompressedList(float M, string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
        List<float> ComputeBasicCompressedListUsingExtension(float M, string pattern, List<float> numbers, int startIndexFromPattern, int amountOfRoundingBits);
        (List<float>, string) ComputeBasicCompressedListReplacingOnceWithOutMultiplication(string pattern, List<float> numbers, int startIndexFromPattern, int amountOfRoundingBits);
        List<float> AddCarryOverPropagationBits(List<float> numbers, string pattern);
        List<float> ComputeOriginalNumbersFromCompressedList(float M, List<float> compressedNumbers);
        Dictionary<int, char> CalculateSharedIndexes(List<float> numbers);
    }
}
