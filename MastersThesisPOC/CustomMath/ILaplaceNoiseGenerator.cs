using System.Security.Cryptography;

namespace MastersThesisPOC.CustomMath
{
    public interface ILaplaceNoiseGenerator
    {
        float GenerateNoise(double epsilon, double deltaF, double mu = 0);
        float GenerateNoiseScaled(double epsilon, double deltaF, double scale, double mu = 0);
        float GenerateNoiseCentered(float centerValue, double epsilon, double deltaF);
        float GenerateNoiseCenteredConsideringExponent(float centerValue, double epsilon, double deltaF);
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
            var b = deltaF / epsilon;
            double u = GetRandomDouble() - 0.5;  // center around zero.
            return centerValue + (float)(-b * Math.Sign(u) * Math.Log(1 - 2 * Math.Abs(u)));
        }

        public float GenerateNoiseCenteredConsideringExponent(float centerValue, double epsilon, double deltaF)
        {
            // Determine boundaries based on the exponent of the centerValue
            float lowerBoundary = MathF.Pow(2, (int)MathF.Log2(centerValue));
            float upperBoundary = MathF.Pow(2, (int)MathF.Log2(centerValue) + 1);

            // Define a threshold for considering a value to be close to the boundary
            float threshold = 0.01f * (upperBoundary - lowerBoundary);

            var b = deltaF / epsilon;
            double u = GetRandomDouble() - 0.5;

            // If close to the lower boundary, generate only positive noise
            if (centerValue - lowerBoundary < threshold)
            {
                u = Math.Abs(u);
            }
            // If close to the upper boundary, generate only negative noise
            else if (upperBoundary - centerValue < threshold)
            {
                u = -Math.Abs(u);
            }
            //var res = centerValue + (float)(-b * Math.Sign(u) * Math.Log(1 - 2 * Math.Abs(u)));
            var res = (float)(centerValue - b * Math.Sign(u) * Math.Log(1 - 2 * Math.Abs(u)));
            return res;
        }
    }
}
