namespace MastersThesisPOC
{
    public interface IAlgorithm
    {
        (float result, string newMantissa, float delta) RunAlgorithm(string pattern, uint mantissa, int M, float x);
    }
}
