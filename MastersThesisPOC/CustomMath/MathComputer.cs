namespace MastersThesisPOC.CustomMath
{
    public interface IMathComputer
    {
        float AddLaplaceNoiseToMantissa(float epsilon, float originalValue, int bitCount, bool lowerBits);
        float GenerateLaplaceNoise(float epsilon);
    }
    public class MathComputer : IMathComputer
    {
        public float AddLaplaceNoiseToMantissa(float epsilon, float originalValue, int bitCount, bool lowerBits)
        {
            float laplaceNoise = GenerateLaplaceNoise(epsilon);
            Console.WriteLine($"Generated Laplace Noise: {laplaceNoise}");

            int scale = (1 << bitCount) - 1;
            int noiseInt = (int)(laplaceNoise * scale);
            Console.WriteLine($"Converted Noise to Int: {noiseInt}");

            int mask = noiseInt & scale;

            if (!lowerBits)
            {
                mask <<= (23 - bitCount);
            }

            Console.WriteLine($"Mask: {Convert.ToString(mask, 2).PadLeft(32, '0')}");

            int originalBits = BitConverter.SingleToInt32Bits(originalValue);
            Console.WriteLine($"Original Bits: {Convert.ToString(originalBits, 2).PadLeft(32, '0')}");

            originalBits |= mask;
            Console.WriteLine($"Modified Bits: {Convert.ToString(originalBits, 2).PadLeft(32, '0')}");

            return BitConverter.Int32BitsToSingle(originalBits);
        }

        public float GenerateLaplaceNoise(float epsilon)
        {
            Random random = new Random();
            var u = (float)random.NextDouble() - 0.5;
            
            var noise = -Math.Sign(u) * (1.0f / epsilon) * (float)Math.Log(1 - 2 * Math.Abs(u));

            Console.WriteLine(noise);

            return noise;
        }
    }
}
