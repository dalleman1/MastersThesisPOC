namespace MastersThesisPOC
{
    public interface IAlgorithm
    {
        (float result, string newMantissa, float delta) RunAlgorithm(string pattern, uint mantissa, float M, float x);
    }
}
