using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace MastersThesisPOC.Algorithm
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
            uint extension = extendedMantissaInput >> 23 - extensionLength;
            uint extendedMantissa = extension << 23 | extendedMantissaInput;

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
                string roundingBits = nextBits.Substring(0, nextBits.Length); // Consider the length of the nextbits

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
                nextBits = leftOverBits + repeatedPattern.Substring(0, 20 - leftOverBits.Length);
            }
            else if (remainingBits == 0)
            {
                nextBits = repeatedPattern.Substring(0, 20);
            }

            return (repeatedPattern, nextBits);
        }

        public string StringPatternOfM32Bit(float M)
        {
            byte[] fpNumberBytes;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(1.0f / M);
                    fpNumberBytes = memoryStream.ToArray();
                }
            }

            uint mantissaInt = BitConverter.ToUInt32(fpNumberBytes, 0) & 0x007FFFFF;
            var mantissaIntBinaryString = Convert.ToString(mantissaInt, 2).PadLeft(23, '0');

            return mantissaIntBinaryString;
        }

        public string StringPatternOfM64Bit(float M)
        {
            byte[] fpNumberBytes;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(1.0 / M);
                    fpNumberBytes = memoryStream.ToArray();
                }
            }

            ulong doubleAsUInt64 = BitConverter.ToUInt64(fpNumberBytes, 0);
            ulong mantissaInt = doubleAsUInt64 & 0xFFFFFFFFFFFFF; // Masking to get the last 52 bits
            var mantissaIntBinaryString = Convert.ToString((long)mantissaInt, 2).PadLeft(52, '0');

            return mantissaIntBinaryString;
        }

        public Dictionary<string, int> FindRepeatingPattern(string mantissa)
        {
            Dictionary<string, int> frequencyMap = new Dictionary<string, int>();
            int length = mantissa.Length;

            for (int start = 0; start < length; start++) // Iterate through starting points
            {
                for (int i = 2; i <= (length - start) / 2; i++)  // Iterate through possible pattern lengths
                {
                    string pattern = mantissa.Substring(start, i); // Extract a potential pattern

                    if (pattern.Length < 2)
                        continue; // Skip too short patterns

                    int repeats = 0;

                    // Count the occurrences of the pattern in the string
                    int index = 0;
                    while ((index = mantissa.IndexOf(pattern, index)) != -1)
                    {
                        repeats++;
                        index += pattern.Length;
                    }

                    if (repeats > 1)
                    {
                        // Add or update the pattern frequency in the map
                        frequencyMap[pattern] = repeats;
                    }
                }
            }

            return frequencyMap;
        }

        public (string, string) ReplacePattern(string pattern, string mantissa, int placement, int? nextBitsLength)
        {
            // Check if the placement index is within the range of the mantissa
            if (placement < 0 || placement >= mantissa.Length)
            {
                return ("Invalid placement index", "");
            }

            // Initialize an empty string to store the new mantissa and the next bits
            string newMantissa = "";
            string nextBits = "";

            // Add the portion of the original mantissa that comes before the placement index
            newMantissa += mantissa.Substring(0, placement);

            // Loop to fill the rest of the mantissa with the pattern
            int patternIndex = 0;
            for (int i = placement; i < mantissa.Length; i++)
            {
                newMantissa += pattern[patternIndex];
                patternIndex = (patternIndex + 1) % pattern.Length; // Cycle back to the beginning of the pattern when we reach the end
            }

            // Loop to fill the next bits with the pattern
            for (int i = 0; i < nextBitsLength; i++)
            {
                nextBits += pattern[patternIndex];
                patternIndex = (patternIndex + 1) % pattern.Length; // Cycle back to the beginning of the pattern when we reach the end
            }

            return (newMantissa, nextBits);
        }

        public (string, string) ReplacePatternWithExtension(string pattern, string mantissa, int patternStartIndex, int? nextBitsLength)
        {
            // Locate the first '1' after the starting index
            int indexOfOne = pattern.IndexOf('1', patternStartIndex);

            // If '1' is not found after startIndex, wrap around to the start of the pattern
            if (indexOfOne == -1)
            {
                indexOfOne = pattern.IndexOf('1');
            }

            // Extract the segment to prepend
            string prependStr = pattern.Substring(patternStartIndex, indexOfOne - patternStartIndex + 1);

            string extendedMantissa = prependStr + mantissa;

            // Deduce placement based on the prependStr length
            int placement = prependStr.Length;

            // Replace pattern
            string newMantissa = extendedMantissa.Substring(0, placement);
            int patternIndex = (indexOfOne + 1) % pattern.Length;

            for (int i = placement; i < extendedMantissa.Length; i++)
            {
                newMantissa += pattern[patternIndex];
                patternIndex = (patternIndex + 1) % pattern.Length;
            }

            // Trim to original length
            newMantissa = newMantissa.Substring(newMantissa.Length - mantissa.Length);

            // Continue pattern for next bits
            string nextBits = "";
            for (int i = 0; i < nextBitsLength; i++)
            {
                nextBits += pattern[patternIndex];
                patternIndex = (patternIndex + 1) % pattern.Length;
            }

            return (newMantissa, nextBits);
        }

        public string RoundMantissaNew(string mantissa, string nextBits)
        {
            // Initialize an empty string to store the new mantissa
            string newMantissa = mantissa;

            // Check if the first bit of nextBits is '1'
            if (nextBits[0] == '1')
            {
                // Round up the last bit of the mantissa
                for (int i = mantissa.Length - 1; i >= 0; i--)
                {
                    if (mantissa[i] == '0')
                    {
                        newMantissa = mantissa.Substring(0, i) + '1' + mantissa.Substring(i + 1);
                        break;
                    }
                    else if (mantissa[i] == '1')
                    {
                        newMantissa = mantissa.Substring(0, i) + '0' + mantissa.Substring(i + 1);

                        // If this was the last '1', then we've rounded up to an even number
                        if (i == 0)
                        {
                            newMantissa = '1' + newMantissa;
                        }
                    }
                }
            }
            // Else, we keep the mantissa as is

            return newMantissa;
        }

        public string FindRepeatingPatternBinary(float value, int maxIterations = 64)
        {
            Dictionary<float, int> seenPositions = new Dictionary<float, int>();
            StringBuilder fractionalPart = new StringBuilder();

            for (int i = 0; i < maxIterations; i++)
            {
                value *= 10;
                int intPart = (int)value;
                fractionalPart.Append(intPart);
                value -= intPart;

                if (seenPositions.ContainsKey(value))
                {
                    int start = seenPositions[value];
                    return fractionalPart.ToString().Substring(start);
                }

                seenPositions[value] = i;
            }

            return "";  // If no repeating pattern found within maxIterations
        }

        private static string GetRepeatingPattern(string s)
        {
            for (int length = 1; length <= s.Length / 2; length++)
            {
                string pattern = s.Substring(0, length);
                if (IsRepeatingPattern(s, pattern))
                    return pattern;
            }
            return "";
        }

        private static bool IsRepeatingPattern(string s, string pattern)
        {
            for (int i = 0; i < s.Length - pattern.Length + 1; i += pattern.Length)
            {
                if (s.Substring(i, pattern.Length) != pattern)
                    return false;
            }
            return true;
        }
    }
}
