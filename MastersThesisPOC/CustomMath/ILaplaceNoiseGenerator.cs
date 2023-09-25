using System.Security.Cryptography;

namespace MastersThesisPOC.CustomMath
{
    public interface ILaplaceNoiseGenerator
    {
        float GenerateNoise(double epsilon, double deltaF, double mu = 0);
        float GenerateNoiseScaled(double epsilon, double deltaF, double scale, double mu = 0);
    }

    public class LaplaceNoiseGenerator : ILaplaceNoiseGenerator
    {
        private double GetRandomDouble()
        {
            int randInt = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
            return (double)(randInt - (long)int.MinValue) / ((long)int.MaxValue - int.MinValue);
        }

        public float GenerateNoise(double epsilon, double deltaF, double mu = 0)
        {
            var b = deltaF / epsilon;
            double u = GetRandomDouble() - 0.5;
            return (float)(mu - b * Math.Sign(u) * Math.Log(1 - 2 * Math.Abs(u)));
        }


        //b=deltaF/ϵ
        //Generating a Uniform Random Number:
        //noise=μ−b×sign(u)×ln(1−2∣u∣)
        //scaling:
        //scaled_noise=noise/scale​​
        public float GenerateNoiseScaled(double epsilon, double deltaF, double scale, double mu = 0)
        {
            var b = deltaF / epsilon;
            double u = GetRandomDouble() - 0.5;
            double noise = mu - b * Math.Sign(u) * Math.Log(1 - 2 * Math.Abs(u));

            return (float)(noise / scale);  // Scale the noise
        }
    }
}
