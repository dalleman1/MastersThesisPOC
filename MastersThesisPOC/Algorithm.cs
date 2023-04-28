using System.Text;
using System.Text.RegularExpressions;

namespace MastersThesisPOC
{
    public class Algorithm
    {
        public string ExtendMantissaAndGetStringRepresentation(uint mantissa, string pattern)
        {
            // Find the position of the first 1 in the pattern
            int firstOnePos = pattern.IndexOf('1');

            // If the pattern does not contain a 1, return the original mantissa
            if (firstOnePos == -1)
            {
                throw new Exception("No 1's appear in pattern");
            }

            // Extend the mantissa with the pattern up until the first 1 + the 1
            var extension = pattern.Substring(0, firstOnePos + 1);
            var mantissaString = Convert.ToString(mantissa & 0x7FFFFF, 2).PadLeft(23, '0');

            var result = extension + mantissaString;

            return result;
        }

        public string RemoveExtension(string extendedMantissa, string pattern)
        {
            int firstOnePos = pattern.IndexOf('1');

            var extensionLength = pattern.Substring(0, firstOnePos + 1).Length;

            return extendedMantissa.Substring(extensionLength);
        }

        public string GetExtendedMantissaAsString(string pattern, uint extendedMantissaInput)
        {
            int extensionLength = 23 - pattern.IndexOf('1');
            uint extension = extendedMantissaInput >> (23 - extensionLength);
            uint extendedMantissa = (extension << 23) | extendedMantissaInput;

            return Convert.ToString(extendedMantissa, 2).PadLeft(32, '0');
        }
        public float ConvertToFloat(string sign, string exponent, string mantissa)
        {
            string binary = sign + exponent + mantissa;
            int intRep = Convert.ToInt32(binary, 2);
            float floatRep = BitConverter.ToSingle(BitConverter.GetBytes(intRep), 0);
            return floatRep;
        }

        public string RotateBits(string pattern)
        {
            // Get the first bit of the pattern and add it to the end
            char firstBit = pattern[0];
            pattern = pattern.Substring(1) + firstBit;

            return pattern;
        }

        public (string, string) InfinitelyReplaceMantissaWithPattern(string pattern, string extendedMantissaString)
        {
            int mantissaLength = extendedMantissaString.Length;
            int patternLength = pattern.Length;
            int patternRepeats = (mantissaLength + patternLength - 1) / patternLength;

            string repeatedPattern = string.Concat(Enumerable.Repeat(pattern, patternRepeats));
            repeatedPattern = repeatedPattern.Substring(0, mantissaLength);

            int remainingBits = mantissaLength - repeatedPattern.Length;

            if (repeatedPattern.Length < mantissaLength)
            {
                repeatedPattern += pattern.Substring(0, remainingBits);
            }

            string nextBit = "0";
            if (remainingBits > 0)
            {
                nextBit = pattern.Substring(remainingBits, 1);
            }
            else
            {
                int index = repeatedPattern.Length - 1;
                while (index >= 0 && repeatedPattern[index] == '1')
                {
                    index--;
                }
                if (index >= 0)
                {
                    nextBit = "1";
                }
            }

            return (repeatedPattern, nextBit);
        }
    }
}
