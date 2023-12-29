namespace MastersThesisPOC
{
    public interface ICompressionExecuter
    {
        List<float> RunProgram(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits, float epsilon, float sensitivity);
        List<float> RunProgramOnlyPrivacy(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits, float epsilon, float sensitivity);
        List<float> RunProgramFast(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits, float epsilon, float sensitivity);
        List<float> RunProgramFastNoiseBeforePattern(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits, float epsilon, float sensitivity, int mantissaStart);
    }
}
