using MastersThesisPOC.Algorithm;
using MastersThesisPOC.Float;
using System.IO.Compression;
using System.Text;

namespace MastersThesisPOC
{

    public class CompressionMechanism : ICompressionMechanism
    {
        private readonly IAlgorithmHelper _algorithmHelper;
        public CompressionMechanism(IAlgorithmHelper algorithmHelper)
        {
            _algorithmHelper = algorithmHelper;
        }

        public List<float> ComputeBasicCompressedList(float M, string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            List<float> newListOfNumbers = new List<float>();

            foreach (var number in numbers)
            {
                var customFloat = new CustomFloat(number);

                var (newMantissa, nextBits) = _algorithmHelper.ReplacePattern(pattern, customFloat.MantissaAsBitString, patternStartIndex, amountOfRoundingBits);

                var roundedMantissa = _algorithmHelper.RoundMantissaNew(newMantissa, nextBits);

                var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, roundedMantissa);

                var MultipliedFloat = newFloat * M;

                newListOfNumbers.Add(MultipliedFloat);
            }

            return newListOfNumbers;
        }

        public (List<float>, string) ComputeBasicCompressedListReplacingOnceWithOutMultiplication(string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            List<float> newListOfNumbers = new List<float>();
            var shiftPattern = "";

            foreach (var number in numbers)
            {
                var customFloat = new CustomFloat(number);

                var (newMantissa, shiftedPattern) = _algorithmHelper.ReplacePatternTestMethod(pattern, customFloat.MantissaAsBitString, patternStartIndex, amountOfRoundingBits);

                shiftPattern = shiftedPattern;

                var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, newMantissa);

                newListOfNumbers.Add(newFloat);
            }

            return (newListOfNumbers, shiftPattern);
        }

        public List<float> AddCarryOverPropagationBits(List<float> numbers, string pattern)
        {
            List<float> newListOfNumbers = new List<float>();

            var res = CalculateSharedIndexes(numbers);

            var indexAfterLongestSharedSequence = FindIndexAfterLongestSharedSequence(res);

            foreach (var number in numbers)
            {
                var customFloat = new CustomFloat(number);  // Assuming CustomFloat is a class that parses the float into its components.

                var mantissa = customFloat.MantissaAsBitString;

                // Ensure we're not replacing within the sequence itself by starting from the next index.
                indexAfterLongestSharedSequence = indexAfterLongestSharedSequence <= mantissa.Length ? indexAfterLongestSharedSequence : mantissa.Length;

                // Replace bits at the specified indices with '0'.
                char[] mantissaArray = mantissa.ToCharArray();
                for (int i = 0; i < 2 && indexAfterLongestSharedSequence + i < mantissa.Length; i++) // Ensure we don't exceed the mantissa length.
                {
                    mantissaArray[indexAfterLongestSharedSequence + i] = '0';
                }
                var newMantissa = new string(mantissaArray);

                var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, newMantissa);

                newListOfNumbers.Add(newFloat);

            }

            return newListOfNumbers;
        }

        private int FindIndexAfterLongestSharedSequence(Dictionary<int, char> sharedIndexes)
        {
            int lastIndex = -1;

            // This will ensure the indexes are processed in order, 
            // which is crucial for finding the continuous sequence.
            var orderedKeys = sharedIndexes.Keys.OrderBy(x => x);

            // Check if the sequence starts from 0 and is continuous.
            foreach (int key in orderedKeys)
            {
                if (key == lastIndex + 1)
                {
                    lastIndex = key;
                }
                else
                {
                    // If there's a gap in the sequence, we stop.
                    break;
                }
            }

            return lastIndex + 1; // We return the index after the longest shared sequence.
        }


        public List<float> ComputeBasicCompressedListUsingExtension(float M, string pattern, List<float> numbers, int startIndexFromPattern, int amountOfRoundingBits)
        {
            List<float> newListOfNumbers = new List<float>();

            foreach (var number in numbers)
            {
                var customFloat = new CustomFloat(number);

                var (newMantissa, nextBits) = _algorithmHelper.ReplacePatternWithExtension(pattern, customFloat.MantissaAsBitString, startIndexFromPattern, amountOfRoundingBits);

                var roundedMantissa = _algorithmHelper.RoundMantissaNew(newMantissa, nextBits);

                var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, roundedMantissa);

                var MultipliedFloat = newFloat * M;

                newListOfNumbers.Add(MultipliedFloat);
            }

            return newListOfNumbers;
        }

        public List<float> ComputeOriginalNumbersFromCompressedList(float M, List<float> compressedNumbers)
        {
            List<float> revertedList = new List<float>();

            foreach (var item in compressedNumbers)
            {
                var res = item / M;
                revertedList.Add(res);
            }

            return revertedList;
        }

        /// <summary>
        ///     It starts with the bit at index ii of the first mantissa.
        ///     It uses the All method to determine if every mantissa in the list has that same bit value at the same position ii.
        ///     If all mantissas have the same bit at that position, the index ii is added to the sharedIndexes list.
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public Dictionary<int, char> CalculateSharedIndexes(List<float> numbers)
        {
            List<string> mantissas = numbers.Select(GetMantissaBits).ToList();

            Dictionary<int, char> sharedIndexValues = new Dictionary<int, char>();

            foreach (int i in Enumerable.Range(0, 23))
            {
                char bit = mantissas[0][i];

                if (mantissas.All(m => m[i] == bit))
                {
                    sharedIndexValues[i] = bit;
                }
            }

            return sharedIndexValues;
        }


        // Extract the mantissa bits from a float
        private string GetMantissaBits(float number)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            uint intRepresentation = BitConverter.ToUInt32(bytes, 0);

            // Get the last 23 bits (mantissa)
            uint mantissa = intRepresentation & 0x7FFFFF;

            return Convert.ToString(mantissa, 2).PadLeft(23, '0');
        }

        private string RemoveSharedBitsFromMantissa(string mantissa, Dictionary<int, char> sharedIndexValues)
        {
            StringBuilder modifiedMantissa = new StringBuilder(mantissa);

            // Reverse the dictionary to process from highest index to lowest, 
            // this ensures we don't mess up the subsequent indexes as we modify the string
            foreach (var item in sharedIndexValues.OrderByDescending(x => x.Key))
            {
                modifiedMantissa.Remove(item.Key, 1);
            }

            return modifiedMantissa.ToString();
        }

    }
}
