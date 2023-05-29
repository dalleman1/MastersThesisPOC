using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace MastersThesisPOC
{
    public class AlgorithmHelper : IAlgorithmHelper
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

        public List<string> FindPatterns(string pattern)
        {
            var result = new List<string>();
            var currentPattern = pattern;

            // Iterate over the pattern, rotating it on each iteration
            for (int i = 0; i < pattern.Length - 1; i++)
            {
                currentPattern = RotateBits(currentPattern);

                // Check if the rotated pattern matches the specified structure
                if (currentPattern.StartsWith("0") && currentPattern.Contains("1"))
                {
                    result.Add(currentPattern);
                }
            }

            return result;
        }


        public string RoundMantissa(string mantissaString, string nextBits)
        {
            string roundedMantissa = mantissaString;

            if (nextBits.Length >= 20)
            {
                string roundingBits = nextBits.Substring(0, 20); // Consider the first 20 bits for rounding

                if (HasRoundingBit(roundingBits))
                {
                    roundedMantissa = IncrementMantissa(mantissaString);
                }
            }

            return roundedMantissa;
        }

        private bool HasRoundingBit(string roundingBits)
        {
            foreach (char bit in roundingBits)
            {
                if (bit == '1')
                {
                    return true;
                }
            }

            return false;
        }

        private string IncrementMantissa(string mantissaString)
        {
            char[] mantissa = mantissaString.ToCharArray();
            int index = mantissa.Length - 1;

            while (index >= 0 && mantissa[index] == '1')
            {
                mantissa[index] = '0';
                index--;
            }

            if (index >= 0)
            {
                mantissa[index] = '1';
            }

            return new string(mantissa);
        }

        public (string, string) InfinitelyReplaceMantissaWithPattern(string pattern, string extendedMantissaString)
        {
            int mantissaLength = extendedMantissaString.Length;
            int patternLength = pattern.Length;
            int patternRepeats = (mantissaLength + patternLength - 1) / patternLength;

            string repeatedPattern = string.Concat(Enumerable.Repeat(pattern, patternRepeats));
            

            int remainingBits = Math.Abs(mantissaLength - repeatedPattern.Length);

            var leftOverBits = repeatedPattern.Substring(mantissaLength, remainingBits);

            repeatedPattern = repeatedPattern.Substring(0, mantissaLength);
            
            string nextBits = "";

            if (remainingBits > 0)
            {
                nextBits = leftOverBits + repeatedPattern.Substring(0, 20-leftOverBits.Length);
            }
            else if (remainingBits == 0)
            {
                nextBits = repeatedPattern.Substring(0, 20);
            }

            return (repeatedPattern, nextBits);
        }
    }
}
