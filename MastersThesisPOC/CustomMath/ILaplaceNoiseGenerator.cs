using MathNet.Numerics.Distributions;
using System.Security.Cryptography;

namespace MastersThesisPOC.CustomMath
{
    public interface ILaplaceNoiseGenerator
    {
        float GenerateNoise(double epsilon, double deltaF, double mu = 0);
        float GenerateNoiseScaled(double epsilon, double deltaF, double scale, double mu = 0);
        float GenerateNoiseCentered(float centerValue, double epsilon, double deltaF);
        float GenerateLaplaceNoiseTest(float epsilon, float scale);
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
            double u = GetRandomDouble() - 0.5; // center around zero.
            double noise = mu - b * Math.Sign(u) * Math.Log(1 - 2 * Math.Abs(u));

            return (float)(noise / scale);  // Scale the noise
        }
    
        public float GenerateNoiseCentered(float centerValue, double epsilon, double deltaF)
        {
            double scale = deltaF / epsilon;
            return (float)Laplace.Sample(centerValue, scale);
        }

        private readonly Random random = new Random();

        // Laplace Probability Density Function
        private float LaplacePDF(float x, float scale)
        {
            return (1f / (2f * scale)) * (float)Math.Exp(-Math.Abs(x) / scale);
        }

        // Laplace Cumulative Distribution Function
        private float LaplaceCDF(float x, float scale)
        {
            if (x < 0)
            {
                return 0.5f * (float)Math.Exp(x / scale);
            }
            else
            {
                return 1f - 0.5f * (float)Math.Exp(-x / scale);
            }
        }

        // Inverse Laplace CDF
        private float InverseLaplaceCDF(float p, float scale)
        {
            if (p <= 0.5f)
            {
                return scale * (float)Math.Log(2f * p);
            }
            else
            {
                return -scale * (float)Math.Log(2f * (1f - p));
            }
        }

        // Generate Laplace noise
        public float GenerateLaplaceNoiseTest(float epsilon, float scale)
        {
            float p = (float)random.NextDouble();  // Uniform random number between 0 and 1
            return InverseLaplaceCDF(p, scale / epsilon);
        }
    }
}
