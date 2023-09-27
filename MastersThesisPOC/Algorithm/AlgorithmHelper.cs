namespace MastersThesisPOC.Algorithm
{
    public class AlgorithmHelper : IAlgorithmHelper
    {

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
            Console.WriteLine("Prepend string: " + prependStr);

            string extendedMantissa = mantissa;

            if (prependStr != "1")
            {
                extendedMantissa = prependStr + mantissa;  // Only prepend if prependStr is not just a '1'
                Console.WriteLine("Extended mantissa : " + extendedMantissa);
            }

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
    }
}
