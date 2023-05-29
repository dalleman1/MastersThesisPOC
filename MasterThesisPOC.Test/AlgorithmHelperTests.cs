using MastersThesisPOC;

namespace MasterThesisPOC.Test
{
    [TestFixture]
    public class AlgorithmHelperTests
    {
        [Test]
        public void ReplaceMantissaInfinitely_Returns_Correct_Mantissa_And_Next20Bits()
        {
            //Arrange
            AlgorithmHelper uut = new AlgorithmHelper();
            var pattern = "000100111011";
            var mantissa = "00111001100110011001101";
            var actual = "00010011101100010011101";
            var actualNext20Bits = "10001001110110001001";

            //Act
            var res = uut.InfinitelyReplaceMantissaWithPattern(pattern, mantissa);

            //Assert
            Assert.That(actual, Is.EqualTo(res.Item1));
            Assert.That(actualNext20Bits, Is.EqualTo(res.Item2));
        }

        [Test]
        public void RoundMantissa_Based_On_20_Next_Bits()
        {
            //Arrange
            AlgorithmHelper uut = new AlgorithmHelper();
            var mantissa = "00010011101100010011101";
            var Next20Bits = "10001001110110001001";
            var roundedMantissa = "00010011101100010011110";

            //Act
            var res = uut.RoundMantissa(mantissa, Next20Bits);

            //Assert
            Assert.That(roundedMantissa, Is.EqualTo(res));
        }

        [Test]
        public void RoundMantissa_Based_On_20_Next_Bits_Where_Bits_Are_Zero()
        {
            //Arrange
            AlgorithmHelper uut = new AlgorithmHelper();
            var mantissa = "00010011101100010011101";
            var Next20Bits = "00000000000000000000";
            var roundedMantissa = "00010011101100010011101";

            //Act
            var res = uut.RoundMantissa(mantissa, Next20Bits);

            //Assert
            Assert.That(roundedMantissa, Is.EqualTo(res));
        }
    }
}
