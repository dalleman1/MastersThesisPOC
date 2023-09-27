using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using MastersThesisPOC.Float;

namespace MastersThesisPOC
{
    public class ProgramInstances : IProgramInstances
    {
        private readonly IAlgorithmHelper _algorithmHelper;
        private readonly ITrailingStrategy _trailingStrategy;
        private readonly IMathComputer _mathComputer;
        private readonly IMetrics _metrics;

        public ProgramInstances(IAlgorithmHelper algorithmHelper, ITrailingStrategy trailingStrategy, IMathComputer mathComputer, IMetrics metrics)
        {
            _algorithmHelper = algorithmHelper;
            _trailingStrategy = trailingStrategy;
            _mathComputer = mathComputer;
            _metrics = metrics;
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

                    var error = _metrics.CalculatePercentDifference(number, newFloat);

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

                    var error = _metrics.CalculatePercentDifference(number, newFloat);

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

        public (Dictionary<float, int>, Dictionary<float, float>) ComputeUsingPrivateM(Dictionary<float, string> basePatternDictionary, List<float> numbers, int patternStartIndex, int amountOfRoundingBits, float epsilon)
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

                    var privatizedM = M + _mathComputer.GenerateLaplaceNoise(epsilon);

                    var MultipliedFloat = newFloat * privatizedM;

                    var newCustomFloat = new CustomFloat(MultipliedFloat);

                    var newFloatAfterDivision = MultipliedFloat / M;

                    var error = _metrics.CalculatePercentDifference(number, newFloatAfterDivision);

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


        public List<float> ComputeBasicCompressedList(float M, string pattern, List<float> numbers, int patternStartIndex, int amountOfRoundingBits)
        {
            List<float> newListOfNumbers = new List<float>();

            foreach (var number in numbers)
            {
                var customFloat = new CustomFloat(number);

                var (newMantissa, nextBits) = _algorithmHelper.ReplacePatternWithExtension(pattern, customFloat.MantissaAsBitString, 4, amountOfRoundingBits);

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
    }
}
