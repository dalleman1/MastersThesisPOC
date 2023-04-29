namespace MastersThesisPOC
{
    public interface IAlgorithmHelper
    {
        float ConvertToFloat(string sign, string exponent, string mantissa);
        string ExtendMantissaAndGetStringRepresentation(uint mantissa, string pattern);
        string GetExtendedMantissaAsString(string pattern, uint extendedMantissaInput);
        (string, string) InfinitelyReplaceMantissaWithPattern(string pattern, string extendedMantissaString);
        string RemoveExtension(string extendedMantissa, string pattern);
        string RotateBits(string pattern);
        string RoundMantissa(string mantissaString, string nextBit);
        List<string> FindPatterns(string pattern);
    }
}