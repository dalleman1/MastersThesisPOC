using MastersThesisPOC.CustomMath;

namespace MasterThesisPOC.Test
{
    [TestFixture]
    public class LaplaceNoiseTests
    {
        public ILaplaceNoiseGenerator uut;


        [SetUp]
        public void SetUp()
        {
            uut = new LaplaceNoiseGenerator();
        }

        [Test]
        public void LaplaceNoise_99Certainty_For_7_9_Test()
        {
            float epsilon = 1.0f;
            float scale = 0.021f;
            int samples = 1000000;
            float originalValue = 7.9f;

            int outliers = 0;
            for (int i = 0; i < samples; i++)
            {
                float noise = uut.GenerateLaplaceNoiseTest(epsilon, scale);
                float noisyValue = originalValue + noise;

                // Check if the noisy value is outside the range [7.8, 8.0]
                if (noisyValue < 7.8f || noisyValue > 8.0f)
                {
                    outliers++;
                }
            }

            float outlierPercentage = 100f * outliers / samples;
            Console.WriteLine($"Outlier Percentage: {outlierPercentage}%");

            // Assert that no more than 1% of the values are outliers
            Assert.IsTrue(outlierPercentage <= 1.0);
        }


        [Test]
        public void PrivacyTest()
        {
            float epsilon = 1.0f;
            float senstivity = 5.0f;
            float x = 25.0f;
            float y = 24.5f;


            for (int i = 0; i < 1000; i++)
            {
                var resx = uut.GenerateNoiseCentered(x, epsilon, senstivity);
                var resy = uut.GenerateNoiseCentered(y, epsilon, senstivity);
                Console.WriteLine($"xtilde: {resx}");
                Console.WriteLine($"ytilde: {resx}");
            }
        }

    }
}
