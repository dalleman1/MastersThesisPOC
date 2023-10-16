namespace MastersThesisPOC
{
    public interface ICompressionExecuter
    {
        List<float> RunProgram(float M, string pattern, List<float> floats, int patternStartIndex, int nextBits);
    }
}
