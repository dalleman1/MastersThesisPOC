using MastersThesisPOC.Algorithm;
using MastersThesisPOC.Float;

namespace MastersThesisPOC
{
    public interface IProgramInstances
    {
        void ComputeBestMWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
        void ComputeBestMWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
    }


    public class ProgramInstances : IProgramInstances
    {
        private readonly IAlgorithmHelper _algorithmHelper;
        public ProgramInstances(IAlgorithmHelper algorithmHelper)
        {
            _algorithmHelper = algorithmHelper;
        }

        //This method computes the best M based on the number of trailing zeros. A similiar method should be tested based on trailing ones.
        public void ComputeBestMWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            Dictionary<float, int> MPerformances = new Dictionary<float, int>();

            foreach (var (M, pattern) in basePatternDictionary)
            {
                foreach (var number in numbers)
                {
                    var customFloat = new CustomFloat(number);
                    //Console.WriteLine(customFloat.SignAsBitString + " " + customFloat.ExponentAsBitString + " " + customFloat.MantissaAsBitString);

                    var (newMantissa, nextBits) = _algorithmHelper.ReplacePattern(pattern, customFloat.MantissaAsBitString, patternStartIndex, amountOfRoundingBits);

                    var roundedMantissa = _algorithmHelper.RoundMantissaNew(newMantissa, nextBits);

                    var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, roundedMantissa);

                    var MultipliedFloat = newFloat * M;

                    var newCustomFloat = new CustomFloat(MultipliedFloat);

                    if (MPerformances.ContainsKey(M))
                    {
                        if (MPerformances[M] > CountTrailingZeros(newCustomFloat.MantissaAsBitString))
                        {
                            MPerformances[M] = CountTrailingZeros(newCustomFloat.MantissaAsBitString);
                        }
                    }
                    else
                    {
                        MPerformances.Add(M, CountTrailingZeros(newCustomFloat.MantissaAsBitString));
                    }
                }
            }

            foreach (var (M, performance) in MPerformances)
            {
                Console.WriteLine($"Value of M: {M} | {performance} amount of trailing zeros");
            }
        }

        public void ComputeBestMWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            Dictionary<float, int> MPerformances = new Dictionary<float, int>();

            foreach (var (M, pattern) in basePatternDictionary)
            {
                foreach (var number in numbers)
                {
                    var customFloat = new CustomFloat(number);
                    //Console.WriteLine(customFloat.SignAsBitString + " " + customFloat.ExponentAsBitString + " " + customFloat.MantissaAsBitString);

                    var (newMantissa, nextBits) = _algorithmHelper.ReplacePattern(pattern, customFloat.MantissaAsBitString, patternStartIndex, amountOfRoundingBits);

                    var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, newMantissa);

                    var MultipliedFloat = newFloat * M;

                    var newCustomFloat = new CustomFloat(MultipliedFloat);

                    if (MPerformances.ContainsKey(M))
                    {
                        if (MPerformances[M] > CountTrailingZeros(newCustomFloat.MantissaAsBitString))
                        {
                            MPerformances[M] = CountTrailingZeros(newCustomFloat.MantissaAsBitString);
                        }
                    }
                    else
                    {
                        MPerformances.Add(M, CountTrailingZeros(newCustomFloat.MantissaAsBitString));
                    }
                }
            }

            foreach (var (M, performance) in MPerformances)
            {
                Console.WriteLine($"Value of M: {M} | {performance} amount of trailing zeros");
            }
        }

        static int CountTrailingZeros(string mantissa)
        {
            int count = 0;
            for (int i = mantissa.Length - 1; i >= 0; i--)
            {
                if (mantissa[i] == '0')
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        static int CountTrailingOnes(string mantissa)
        {
            int count = 0;
            for (int i = mantissa.Length - 1; i >= 0; i--)
            {
                if (mantissa[i] == '1')
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }
    }
}
