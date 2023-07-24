namespace MastersThesisPOC
{
    public class Algorithm : IAlgorithm
    {
        private readonly IAlgorithmHelper _algorithmHelper;
        public Algorithm(IAlgorithmHelper algorithmHelper)
        {
            _algorithmHelper = algorithmHelper;
        }

        public (float result, string newMantissa, float delta) RunAlgorithm(string pattern, uint mantissa, float M, float x)
        {
            //Refactor this
            var newFloat = new CustomFloat(x);

            var res = _algorithmHelper.ExtendMantissaAndGetStringRepresentation(mantissa, pattern);

            //Console.WriteLine("Extended mantissa as a string: " + res + "\n");

            var (res2, nextbit) = _algorithmHelper.InfinitelyReplaceMantissaWithPattern(pattern, res);

            //Console.WriteLine("Pattern repeated in mantissa: " + res2 + "\n");
            //Console.WriteLine("The next bit would have been:" + nextbit + "\n");

            var res3 = _algorithmHelper.RemoveExtension(res2, pattern);

            //Console.WriteLine("Removed Extension: " + res3 + "\n");

            var roundedMantissa = _algorithmHelper.RoundMantissa(res3, nextbit);

            //Console.WriteLine("Rounded mantissa as a string: " + roundedMantissa + "\n");

            //Console.WriteLine("Uint of rounded mantissa: " + Convert.ToUInt32(roundedMantissa, 2) + "\n");

            var result = _algorithmHelper.ConvertToFloat(newFloat.SignAsBitString, newFloat.ExponentAsBitString, roundedMantissa);

            //Console.WriteLine("New float value with new mantissa:" + result + "\n");

            var newResult = result * M;

            //Console.WriteLine($"Float after being multiplied by M {M}: " + newResult + "\n");

            var customNewResult = new CustomFloat(newResult);

            //Console.WriteLine($"Sign of the result: {customNewResult.SignAsBitString}, Exponent: {customNewResult.ExponentAsBitString}, Mantissa: {customNewResult.MantissaAsBitString}" + "\n");

            var delta = ComputeDelta(x, result);

            //Console.WriteLine("The delta of the new result is: " + delta + "\n");

            return (result, customNewResult.MantissaAsBitString, delta);
        }

        private float ComputeDelta(float number, float result)
        {
            return Math.Abs(number - result);
        }
    }
}
