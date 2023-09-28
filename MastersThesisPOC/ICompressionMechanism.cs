namespace MastersThesisPOC
{
    public interface ICompressionMechanism
    {
        List<float> ComputeBasicCompressedList(float M, string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
        List<float> ComputeBasicCompressedListUsingExtension(float M, string pattern, List<float> numbers, int startIndexFromPattern, int amountOfRoundingBits);
        List<float> ComputeBasicCompressedListReplacingOnce(float M, string pattern, List<float> numbers, int startIndexFromPattern, int amountOfRoundingBits);
        List<float> ComputeOriginalNumbersFromCompressedList(float M, List<float> compressedNumbers);
        List<int> CalculateSharedIndexes(List<float> numbers);
    }
}
