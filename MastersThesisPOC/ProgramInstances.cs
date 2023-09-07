using MastersThesisPOC.Algorithm;
using MastersThesisPOC.Float;

namespace MastersThesisPOC
{
    public class ProgramInstances : IProgramInstances
    {
        private readonly IAlgorithmHelper _algorithmHelper;
        private readonly ITrailingStrategy _trailingStrategy;
        public ProgramInstances(IAlgorithmHelper algorithmHelper, ITrailingStrategy trailingStrategy)
        {
            _algorithmHelper = algorithmHelper;
            _trailingStrategy = trailingStrategy;
        }

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
                        if (MPerformances[M] > _trailingStrategy.CountTrailing(newCustomFloat.MantissaAsBitString))
                        {
                            MPerformances[M] = _trailingStrategy.CountTrailing(newCustomFloat.MantissaAsBitString);
                        }
                    }
                    else
                    {
                        MPerformances.Add(M, _trailingStrategy.CountTrailing(newCustomFloat.MantissaAsBitString));
                    }
                }
            }

            return (MPerformances, MErrorMargin);
        }


        public (Dictionary<float, int>, Dictionary<float, float>) ComputeBestMWithNoRounding(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int? amountOfRoundingBits)
        {
            Dictionary<float, int> MPerformances = new Dictionary<float, int>();
            Dictionary<float, float> MErrorMargin = new Dictionary<float, float>();

            foreach (var (M, pattern) in basePatternDictionary)
            {
                foreach (var number in numbers)
                {
                    var customFloat = new CustomFloat(number);

                    var (newMantissa, nextBits) = _algorithmHelper.ReplacePattern(pattern, customFloat.MantissaAsBitString, patternStartIndex, amountOfRoundingBits);

                    var newFloat = _algorithmHelper.ConvertToFloat(customFloat.SignAsBitString, customFloat.ExponentAsBitString, newMantissa);

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
                        if (MPerformances[M] > _trailingStrategy.CountTrailing(newCustomFloat.MantissaAsBitString))
                        {
                            MPerformances[M] = _trailingStrategy.CountTrailing(newCustomFloat.MantissaAsBitString);
                        }
                    }
                    else
                    {
                        MPerformances.Add(M, _trailingStrategy.CountTrailing(newCustomFloat.MantissaAsBitString));
                    }
                }
            }

            return (MPerformances, MErrorMargin);
        }

        public float CalculatePercentDifference(float value1, float value2)
        {
            return Math.Abs((value1 - value2) / ((value1 + value2) / 2)) * 100;
        }
    }
}
