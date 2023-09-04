namespace MastersThesisPOC.Algorithm
{
    public interface IAlgorithmHelper
    {
        float ConvertToFloat(string sign, string exponent, string mantissa);
        string ExtendMantissaAndGetStringRepresentation(uint mantissa, string pattern);
        string GetExtendedMantissaAsString(string pattern, uint extendedMantissaInput);
        (string, string) InfinitelyReplaceMantissaWithPattern(string pattern, string extendedMantissaString);
        string RemoveExtension(string extendedMantissa, string pattern);
        string RotateBits(string pattern);
        string RoundMantissa(string mantissaString, string nextBits);
        List<string> FindPatterns(string pattern);
        string StringPatternOfM32Bit(int M);
        string StringPatternOfM64Bit(int M);
        Dictionary<string, int> FindRepeatingPattern(string mantissa);
        (string, string) ReplacePattern(string pattern, string mantissa, int placement, int nextBitsLength);

        string RoundMantissaNew(string mantissa, string nextBits);
    }
}