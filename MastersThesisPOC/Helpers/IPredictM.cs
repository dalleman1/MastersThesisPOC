public interface IPredictM
{
    Dictionary<string, int> PredictBestM(List<float> floats);
    List<(float, string, int)> DetermineBestM(Dictionary<string, int> mostOccuringMSB, Dictionary<float, string> dictionaryOfMs);
}

public class PredictM : IPredictM
{
    public List<(float, string, int)> DetermineBestM(Dictionary<string, int> mostOccuringMSB, Dictionary<float, string> dictionaryOfMs)
    {
        Dictionary<int, int> positionWeights = new Dictionary<int, int>
    {
        {0, 3},
        {1, 2},
        {2, 1}
    };

        List<(float M, string pattern, int score)> scoresList = new List<(float, string, int)>();

        foreach (var entry in dictionaryOfMs)
        {
            float M = entry.Key;
            string originalPattern = entry.Value;

            // Generate shifted versions of the pattern
            var shiftedPatterns = GenerateShiftedPatterns(originalPattern);

            foreach (var pattern in shiftedPatterns)
            {
                int score = 0;

                // Compare the shifted pattern against the top patterns from mostOccuringMSB
                foreach (var topPattern in mostOccuringMSB)
                {
                    int minLen = Math.Min(topPattern.Key.Length, pattern.Length);
                    int currentPatternScore = 0; // This is the score for this specific topPattern
                    for (int i = 0; i < minLen; i++)
                    {
                        if (topPattern.Key[i] == pattern[i])
                        {
                            currentPatternScore += positionWeights[i];
                        }
                    }

                    // Multiply by the frequency of the topPattern in the dataset
                    score += currentPatternScore * topPattern.Value;
                }

                scoresList.Add((M, pattern, score));
            }
        }

        // Order the list by score and take the top 3
        var top3 = scoresList.OrderByDescending(item => item.score).Take(3).ToList();

        return top3;
    }



    private const int ComparisonLength = 3;  // The length of the MSBs you are comparing against

    private IEnumerable<string> GenerateShiftedPatterns(string pattern)
    {
        List<string> shiftedPatterns = new List<string>();

        // If the pattern is shorter than the desired length, repeat it
        while (pattern.Length < ComparisonLength)
        {
            pattern += pattern;
        }

        for (int i = 0; i < pattern.Length; i++)
        {
            string shiftedPattern = pattern.Substring(i) + pattern.Substring(0, i);
            shiftedPatterns.Add(shiftedPattern);
        }

        return shiftedPatterns;
    }



    public Dictionary<string, int> PredictBestM(List<float> floats)
    {
        Dictionary<string, int> patternCounts = new Dictionary<string, int>
        {
            {"000", 0},
            {"001", 0},
            {"010", 0},
            {"011", 0},
            {"100", 0},
            {"101", 0},
            {"110", 0},
            {"111", 0}
        };

        foreach (float number in floats)
        {
            string mantissa = GetMantissaBits(number);
            string msb3 = mantissa.Substring(0, 3);

            if (patternCounts.ContainsKey(msb3))
            {
                patternCounts[msb3]++;
            }
        }

        // Finding the pattern with the highest count
        var topPatterns = patternCounts.OrderByDescending(p => p.Value).Take(8)
                           .ToDictionary(pair => pair.Key, pair => pair.Value);

        return topPatterns;
    }

    private string GetMantissaBits(float number)
    {
        byte[] bytes = BitConverter.GetBytes(number);
        uint intRepresentation = BitConverter.ToUInt32(bytes, 0);

        // Get the last 23 bits (mantissa)
        uint mantissa = intRepresentation & 0x7FFFFF;

        return Convert.ToString(mantissa, 2).PadLeft(23, '0');
    }
}