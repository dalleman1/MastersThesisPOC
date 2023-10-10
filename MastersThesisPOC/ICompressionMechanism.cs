namespace MastersThesisPOC
{
    public interface ICompressionMechanism
    {
        List<float> ComputeBasicCompressedList(float M, string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
        List<float> ComputeBasicCompressedListUsingExtension(float M, string pattern, List<float> numbers, int startIndexFromPattern, int amountOfRoundingBits);
        List<float> ComputeBasicCompressedListReplacingOnceWithOutMultiplication(string pattern, List<float> numbers, int startIndexFromPattern, int amountOfRoundingBits);
        List<float> ComputeOriginalNumbersFromCompressedList(float M, List<float> compressedNumbers);
        Dictionary<int, char> CalculateSharedIndexes(List<float> numbers);
        byte[] CompressMantissasUsingGZip(List<float> numbers, Dictionary<int, char> sharedIndexValues);
        int CalculateMantissasSizeInBytes(List<float> numbers);
        string DecompressMantissasUsingGZip(byte[] compressedData);
        List<string> ReconstructOriginalMantissas(string decompressedMantissas, Dictionary<int, char> sharedIndexValues);
    }
}
