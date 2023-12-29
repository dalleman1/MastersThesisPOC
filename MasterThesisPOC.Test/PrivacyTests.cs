using Accord.Statistics.Testing;
using MastersThesisPOC;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using MastersThesisPOC.Helpers;
using NSubstitute;
using Serilog;

namespace MasterThesisPOC.Test
{
    [TestFixture]
    public class PrivacyTests
    {
        public ICompressionExecuter uut;
        public ICompressionMechanism mechanism;
        public IAlgorithmHelper helper;
        public ILaplaceNoiseGenerator laplaceNoiseGenerator;
        public IMetrics metrics;
        public ILogger logger;
        public ICsvReader csvReader;

        [SetUp]
        public void SetUp()
        {
            logger = Substitute.For<ILogger>();
            metrics = new Metrics();
            helper = new AlgorithmHelper();
            mechanism = new CompressionMechanism(helper);
            laplaceNoiseGenerator = new LaplaceNoiseGenerator();
            uut = new CompressionExecuter(mechanism, laplaceNoiseGenerator, metrics, logger);
            csvReader = new CsvReader();
        }

        [Test]
        [TestCase(0.1f)]
        [TestCase(0.5f)]
        [TestCase(1.0f)]
        [TestCase(2.0f)]
        [TestCase(5.0f)]
        [TestCase(10.0f)]
        public void DifferentialPrivacy_KolmogorovSmirnovTest(float Epsilon)
        {
            //Arrange
            var M = 3f;
            var patternOfM = "01";
            var Sensitivity = 5.0f;
            var NextBits = 20;
            var StartIndex = 12;

            var pondTempFloats = csvReader.ReadCsvColumPondTemp("C:\\Users\\mongl\\OneDrive\\Skrivebord\\IoTpond1.csv");
            pondTempFloats = pondTempFloats.Take(45000).ToList();
            Random rnd = new Random();
            int randomIndex = rnd.Next(pondTempFloats.Count);
            var pondTempFloatsAltered = new List<float>(pondTempFloats);
            pondTempFloatsAltered.RemoveAt(randomIndex);

            //Act
            var result1 = uut.RunProgramFastNoiseBeforePattern(M, patternOfM, pondTempFloats, 0, NextBits, Epsilon, Sensitivity, StartIndex);
            var result2 = uut.RunProgramFastNoiseBeforePattern(M, patternOfM, pondTempFloatsAltered, 0, NextBits, Epsilon, Sensitivity, StartIndex);

            var result1Reverted = new List<float>();
            var result2Reverted = new List<float>();

            foreach (var item in result1)
            {
                result1Reverted.Add(item / M);
            }

            foreach (var item in result2)
            {
                result2Reverted.Add(item / M);
            }

            //Assert
            // Perform the Kolmogorov-Smirnov Test
            bool ksTestResult = PerformKolmogorovSmirnovTest(result1Reverted, result2Reverted);

            // Assert that the distributions are similar
            Assert.IsTrue(ksTestResult, "Distributions should be similar according to the Kolmogorov-Smirnov Test");

        }

        [Test]
        [TestCase(0.1f)]
        [TestCase(0.5f)]
        [TestCase(1.0f)]
        [TestCase(2.0f)]
        [TestCase(5.0f)]
        [TestCase(10.0f)]
        public void DifferentialPrivacy_TTest(float Epsilon)
        {
            //Arrange
            var M = 3f;
            var patternOfM = "01";
            var Sensitivity = 5.0f;
            var NextBits = 20;
            var StartIndex = 12;

            var pondTempFloats = csvReader.ReadCsvColumPondTemp("C:\\Users\\mongl\\OneDrive\\Skrivebord\\IoTpond1.csv");
            pondTempFloats = pondTempFloats.Take(45000).ToList();
            Random rnd = new Random();
            int randomIndex = rnd.Next(pondTempFloats.Count);
            var pondTempFloatsAltered = new List<float>(pondTempFloats);
            pondTempFloatsAltered.RemoveAt(randomIndex);

            //Act
            var result1 = uut.RunProgramFastNoiseBeforePattern(M, patternOfM, pondTempFloats, 0, NextBits, Epsilon, Sensitivity, StartIndex);
            var result2 = uut.RunProgramFastNoiseBeforePattern(M, patternOfM, pondTempFloatsAltered, 0, NextBits, Epsilon, Sensitivity, StartIndex);

            var result1Reverted = new List<float>();
            var result2Reverted = new List<float>();

            foreach (var item in result1)
            {
                result1Reverted.Add(item / M);
            }

            foreach (var item in result2)
            {
                result2Reverted.Add(item / M);
            }

            //Assert
            // Perform the Kolmogorov-Smirnov Test
            bool tTestResult = PerformTTest(result1Reverted, result2Reverted);

            // Assert that the distributions are similar
            Assert.IsTrue(tTestResult, "Distributions should be similar according to the T-Test");

        }

        private bool PerformKolmogorovSmirnovTest(List<float> data1, List<float> data2)
        {
            // Convert Lists to Arrays as Accord's KS Test expects arrays
            double[] array1 = data1.Select(x => (double)x).ToArray();
            double[] array2 = data2.Select(x => (double)x).ToArray();

            // Create a new Kolmogorov-Smirnov test
            var ksTest = new TwoSampleKolmogorovSmirnovTest(array1, array2);

            Console.WriteLine($"PValue: {ksTest.PValue}");

            // Check if the distributions are similar
            // You might want to return ksTest.Significant to check if the difference is statistically significant
            return ksTest.PValue > 0.05; // Common threshold for statistical significance
        }

        private bool PerformTTest(List<float> data1, List<float> data2)
        {
            // Convert Lists to Arrays as Accord's KS Test expects arrays
            double[] array1 = data1.Select(x => (double)x).ToArray();
            double[] array2 = data2.Select(x => (double)x).ToArray();

            // Create a new Kolmogorov-Smirnov test
            var tTest = new TwoSampleTTest(array1, array2);

            Console.WriteLine($"PValue: {tTest.PValue}");

            // Check if the distributions are similar
            // You might want to return ksTest.Significant to check if the difference is statistically significant
            return tTest.PValue > 0.05; // Common threshold for statistical significance
        }

    }
}
