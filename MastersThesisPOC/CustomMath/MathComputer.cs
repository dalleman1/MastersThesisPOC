namespace MastersThesisPOC.CustomMath
{
    public interface IMathComputer
    {
        float AddLaplaceNoise(float epsilon, float originalValue);
    }
    public class MathComputer : IMathComputer
    {
        public float AddLaplaceNoise(float epsilon, float originalValue)
        {
            Random random = new Random();
            var u = (float)random.NextDouble() - 0.5;

            var laplaceNoise = -Math.Sign(u) * (1.0f / epsilon) * (float)Math.Log(1 - 2 * Math.Abs(u));

            return laplaceNoise + originalValue;
        }
    }
}
