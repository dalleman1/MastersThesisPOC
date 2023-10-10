using System.Security.Cryptography;

namespace MastersThesisPOC.CustomMath
{
    public interface ILaplaceNoiseGenerator
    {
        float GenerateNoise(double epsilon, double deltaF, double mu = 0);
        float GenerateNoiseScaled(double epsilon, double deltaF, double scale, double mu = 0);
        float GenerateNoiseCentered(float centerValue, double epsilon, double deltaF);
        float GenerateNoiseCenteredConsideringExponent(float centerValue, double epsilon, double deltaF);
        float GenerateWith99PercentCertaintyNoiseCentered(float centerValue, double epsilon, double deltaF);
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
            var b = deltaF / epsilon;
            double u = GetRandomDouble() - 0.5;  // center around zero.
            var res = (float)(centerValue - b * Math.Sign(u) * Math.Log(1 - 2 * Math.Abs(u)));
            return res;
        }

        public float GenerateWith99PercentCertaintyNoiseCentered(float centerValue, double epsilon, double deltaF)
        {
            var b = deltaF / epsilon;

            // 1. Calculate Boundaries for the Noise
            float lowerBoundary = MathF.Pow(2, (int)MathF.Log2(centerValue) - 1);
            float upperBoundary = MathF.Pow(2, (int)MathF.Log2(centerValue));

            float maxNoiseUpper = upperBoundary - centerValue;
            float maxNoiseLower = centerValue - lowerBoundary;

            // 2. Calculate the Noise Range for 99% Probability
            double noiseRangeUpper = b * Math.Log(50); // F^-1(0.99) for Laplace CDF
            double noiseRangeLower = -b * Math.Log(50); // F^-1(0.01) for Laplace CDF

            // Adjust noise range to ensure it doesn't exceed our calculated boundaries
            noiseRangeUpper = Math.Min(noiseRangeUpper, maxNoiseUpper);
            noiseRangeLower = Math.Max(noiseRangeLower, -maxNoiseLower);


            // Ensure noise range does not exceed boundaries
            if (noiseRangeUpper > maxNoiseUpper)
            {
                noiseRangeUpper = maxNoiseUpper;
            }
            if (noiseRangeLower < -maxNoiseLower)
            {
                noiseRangeLower = -maxNoiseLower;
            }

            // Ensure noiseRangeLower is less than or equal to noiseRangeUpper
            if (noiseRangeLower > noiseRangeUpper)
            {
                var temp = noiseRangeLower;
                noiseRangeLower = noiseRangeUpper;
                noiseRangeUpper = temp;
            }
            // 3. Add the Noise
            double u = GetRandomDouble() - 0.5;  // center around zero.
            double noise = -b * Math.Sign(u) * Math.Log(1 - 2 * Math.Abs(u));
            // Ensure noise doesn't exceed the range
            noise = Math.Clamp(noise, noiseRangeLower, noiseRangeUpper);

            return centerValue + (float)noise;
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
