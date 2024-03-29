﻿namespace MastersThesisPOC.Algorithm
{
    public interface IAlgorithmHelper
    {
        float ConvertToFloat(string sign, string exponent, string mantissa);
        string RotateBits(string pattern);
        string StringPatternOfM32Bit(float M);
        string StringPatternOfM64Bit(float M);
        (string, string) ReplacePattern(string pattern, string mantissa, int placement, int? nextBitsLength);
        (string, string) ReplacePatternWithExtension(string pattern, string mantissa, int patternStartIndex, int? nextBitsLength);
        (string, string) ReplacePatternWithExtensionOnce(string pattern, string mantissa, int patternStartIndex, int? nextBitsLength);
        (string, string) ReplacePatternOnce(string pattern, string mantissa, int placement, int? nextBitsLength);
        string RoundMantissaNew(string mantissa, string nextBits);
        (string, string) ReplacePatternTestMethod(string pattern, string mantissa, int patternStartIndex, int? nextBitsLength);
        (string, string, string) ReplacePatternNoiseBeforePattern(string pattern, string mantissa, int patternStartIndex, int mantissaStartIndex, int nextBitsLength);
    }
}