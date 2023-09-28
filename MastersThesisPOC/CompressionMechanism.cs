using MastersThesisPOC.Algorithm;
using MastersThesisPOC.Float;

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

        public List<float> ComputeBasicCompressedListReplacingOnce(float M, string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            List<float> newListOfNumbers = new List<float>();

            foreach (var number in numbers)
            {
                var customFloat = new CustomFloat(number);

                var (newMantissa, nextBits) = _algorithmHelper.ReplacePatternOnce(pattern, customFloat.MantissaAsBitString, patternStartIndex, amountOfRoundingBits);

                var roundedMantissa = _algorithmHelper.RoundMantissaNew(newMantissa, nextBits);

                var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, roundedMantissa);

                var MultipliedFloat = newFloat * M;

                newListOfNumbers.Add(MultipliedFloat);
            }

            return newListOfNumbers;
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
        public List<int> CalculateSharedIndexes(List<float> numbers)
        {
            List<string> mantissas = numbers.Select(GetMantissaBits).ToList();

            List<int> sharedIndexes = new List<int>();

            foreach (int i in Enumerable.Range(0, 23))
            {
                char bit = mantissas[0][i];

                if (mantissas.All(m => m[i] == bit))
                {
                    sharedIndexes.Add(i);
                }
            }

            return sharedIndexes;
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
    }
}
