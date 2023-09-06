using MastersThesisPOC.Algorithm;
using MastersThesisPOC.Float;

namespace MastersThesisPOC
{
    public interface IProgramInstances
    {
        float CalculatePercentDifference(float value1, float value2);
        void PrintMPerformance(Dictionary<float, int> performance, Dictionary<float, float> errorPerformance);
        List<float> GenerateFloats(int amount);
        (Dictionary<float, int>, Dictionary<float, float>) ComputeBestMWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
        Dictionary<float, int> ComputeBestMWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits);
    }


    public class ProgramInstances : IProgramInstances
    {
        private readonly IAlgorithmHelper _algorithmHelper;
        public ProgramInstances(IAlgorithmHelper algorithmHelper)
        {
            _algorithmHelper = algorithmHelper;
        }

        public List<float> GenerateFloats(int amount)
        {
            Random random = new Random();
            List<float> floatList = new List<float>();

            for (int i = 0; i < amount; i++)
            {
                float randomFloat = (float)random.NextDouble() * 49 + 1; // Random float between 1 and 50
                floatList.Add(randomFloat);
            }

            return floatList;
        }

        //This method computes the best M based on the number of trailing zeros. A similiar method should be tested based on trailing ones.
        public (Dictionary<float, int>, Dictionary<float, float>) ComputeBestMWithRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            Dictionary<float, int> MPerformances = new Dictionary<float, int>();
            Dictionary<float, float> MErrorMargin = new Dictionary<float, float>();

            foreach (var (M, pattern) in basePatternDictionary)
            {
                foreach (var number in numbers)
                {
                    var customFloat = new CustomFloat(number);

                    var (newMantissa, nextBits) = _algorithmHelper.ReplacePattern(pattern, customFloat.MantissaAsBitString, patternStartIndex, amountOfRoundingBits);

                    var roundedMantissa = _algorithmHelper.RoundMantissaNew(newMantissa, nextBits);

                    var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, roundedMantissa);

                    var MultipliedFloat = newFloat * M;

                    var newCustomFloat = new CustomFloat(MultipliedFloat);

                    var error = CalculatePercentDifference(number, newFloat);

                    if (MErrorMargin.ContainsKey(M))
                    {
                        if (MErrorMargin[M] < error)
                        {
                            MErrorMargin[M] = error;
                        }
                    }
                    else
                    {
                        MErrorMargin[M] = error;
                    }

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

            return (MPerformances, MErrorMargin);
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

        public Dictionary<float, int> ComputeBestMWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
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

            return MPerformances;
        }

        public void PrintMPerformance(Dictionary<float, int> Mperformance, Dictionary<float, float> errorPerformance)
        {
            foreach (var (M, performance) in Mperformance)
            {
                Console.WriteLine($"Value of M: {M} | {performance} amount of trailing zeros");
            }

            Console.WriteLine("\n");

            foreach (var (M, error) in errorPerformance)
            {
                Console.WriteLine($"Value of M: {M} | {error}% max error");
            }
        }

        public float CalculatePercentDifference(float value1, float value2)
        {
            return Math.Abs((value1 - value2) / ((value1 + value2) / 2)) * 100;
        }
    }
}
