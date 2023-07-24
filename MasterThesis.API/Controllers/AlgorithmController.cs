using MastersThesisPOC;
using Microsoft.AspNetCore.Mvc;

namespace MasterThesis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlgorithmController : Controller
    {
        private readonly IAlgorithm _algorithm;
        private readonly IAlgorithmHelper _algorithmHelper;

        public AlgorithmController(IAlgorithm algorithm, IAlgorithmHelper algorithmHelper)
        {
            _algorithm = algorithm;
            _algorithmHelper = algorithmHelper;
        }

        [HttpGet("/api/health")]
        public ActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpGet("/api/Algorithm/{number}/{M}")]
        public ActionResult<AlgorithmResult> GetAlgorithmResult(float number, float M)
        {
            var newFloat = new CustomFloat(number);

            //var result5 = pythonHelper!.GetStringPatternOfInteger(13)[..12];
            var result = GetStringPatternOfFloat(M)[..12];
            //var result6 = GetStringPatternOfInteger(13)[..12];

            var pattern = result;

            var patternsResult = _algorithmHelper.FindPatterns(pattern);

            var listOfResults = new List<Result>();

            foreach (var Currentpattern in patternsResult)
            {
                var temp = _algorithm.RunAlgorithm(Currentpattern, newFloat.GetMantissaAsUInt(), M, number);
                listOfResults.Add(new Result
                {
                    newNumber = temp.result,
                    newMantissa = temp.newMantissa,
                    delta = temp.delta
                });
            }

            var algorithmResult = new AlgorithmResult
            {
                originalNumber = number,
                originalNumberMantissaAsString = newFloat.MantissaAsBitString,
                originalNumberExponentAsString = newFloat.ExponentAsBitString,
                originalNumberSignAsString = newFloat.SignAsBitString,
                uintOfOriginalNumber = newFloat.GetMantissaAsUInt(),
                M = M,
                patternAsString = pattern,
                stringPatterns = patternsResult,
                results = listOfResults
            };

            return algorithmResult;
        }
        static string GetStringPatternOfFloat(float input)
        {
            byte[] fpNumberBytes;
            uint mantissaFloat;
            string mantissaFloatBinary;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(1.0f / input);
                    fpNumberBytes = memoryStream.ToArray();
                }
            }

            mantissaFloat = BitConverter.ToUInt32(fpNumberBytes, 0) & 0x007FFFFF;
            mantissaFloatBinary = Convert.ToString(mantissaFloat, 2).PadLeft(23, '0');

            return mantissaFloatBinary;
        }
    }
}
