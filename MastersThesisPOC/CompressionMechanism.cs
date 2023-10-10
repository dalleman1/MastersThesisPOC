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

        public List<float> ComputeBasicCompressedListReplacingOnceWithOutMultiplication(string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            List<float> newListOfNumbers = new List<float>();

            foreach (var number in numbers)
            {
                var customFloat = new CustomFloat(number);

                var (newMantissa, nextBits) = _algorithmHelper.ReplacePatternWithExtensionOnce(pattern, customFloat.MantissaAsBitString, patternStartIndex, amountOfRoundingBits);

                //var roundedMantissa = _algorithmHelper.RoundMantissaNew(newMantissa, nextBits);

                var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, newMantissa);

                newListOfNumbers.Add(newFloat);
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

        //Sign = True for negative, false for positive.
        private (List<bool>, List<string>, List<string>) CompressFloats(List<float> numbers, Dictionary<int, char> sharedIndexValues)
        {
            // Lists to store signs, exponents, and modified mantissas
            List<bool> signs = new List<bool>();
            List<string> exponents = new List<string>();
            List<string> modifiedMantissas = new List<string>();

            foreach (var number in numbers)
            {
                // Convert float to its binary representation
                byte[] bytes = BitConverter.GetBytes(number);
                string binaryFloat = string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

                // Extract and store the sign bit
                bool sign = binaryFloat[0] == '1';
                signs.Add(sign);

                // Extract and store the exponent
                string exponent = binaryFloat.Substring(1, 8);
                exponents.Add(exponent);

                // Extract and modify the mantissa
                string mantissa = binaryFloat.Substring(9);
                string modifiedMantissa = RemoveSharedBitsFromMantissa(mantissa, sharedIndexValues);
                modifiedMantissas.Add(modifiedMantissa);
            }

            return (signs, exponents, modifiedMantissas);
        }

        public byte[] CompressMantissasUsingGZip(List<float> numbers, Dictionary<int, char> sharedIndexValues)
        {
            var (signs, exponents, modifiedMantissas) = CompressFloats(numbers, sharedIndexValues);

            // Convert the list of modified mantissas into a single string
            string mantissasString = string.Join("", modifiedMantissas);

            // Convert the string into bytes
            var bytes = Encoding.UTF8.GetBytes(mantissasString);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return mso.ToArray();
            }
        }

        public string DecompressMantissasUsingGZip(byte[] compressedData)
        {
            using (var msi = new MemoryStream(compressedData))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }

                // Convert the bytes back into a string
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public List<string> ReconstructOriginalMantissas(string decompressedMantissas, Dictionary<int, char> sharedIndexValues)
        {
            int chunkSize = 23 - sharedIndexValues.Count;
            List<string> reconstructedMantissas = new List<string>();

            for (int i = 0; i < decompressedMantissas.Length; i += chunkSize)
            {
                StringBuilder mantissaChunk = new StringBuilder(decompressedMantissas.Substring(i, chunkSize));

                // Insert the shared bits at the appropriate positions
                foreach (var item in sharedIndexValues.OrderBy(x => x.Key))
                {
                    mantissaChunk.Insert(item.Key, item.Value);
                }

                reconstructedMantissas.Add(mantissaChunk.ToString());
            }

            return reconstructedMantissas;
        }


        public int CalculateMantissasSizeInBytes(List<float> numbers)
        {
            // Extract the mantissas from the floats
            List<string> mantissas = numbers.Select(GetMantissaBits).ToList();

            // Convert the list of mantissas into a single string
            string mantissasString = string.Join("", mantissas);

            // Convert the string into bytes
            var bytes = Encoding.UTF8.GetBytes(mantissasString);

            return bytes.Length;
        }

    }
}
