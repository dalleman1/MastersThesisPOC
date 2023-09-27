using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using Microsoft.AspNetCore.Mvc;

namespace MasterThesis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlgorithmController : Controller
    {
        private readonly IAlgorithmHelper _algorithmHelper;
        private readonly IMathComputer _mathComputer;

        public AlgorithmController(IAlgorithmHelper algorithmHelper, IMathComputer mathComputer)
        {
            _algorithmHelper = algorithmHelper;
            _mathComputer = mathComputer;
        }

        [HttpGet("/api/health")]
        public ActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpGet("/api/LaplaceNoise")]
        public ActionResult<float> LaplaceNoise(float epsilon)
        {
            return _mathComputer.GenerateLaplaceNoise(epsilon);
        }

        [HttpGet("/api/ReplacePattern")]
        public ActionResult<List<string>> ReplacePattern(string pattern, string mantissa, int index, int nextBitsLength)
        {
            var res = _algorithmHelper.ReplacePattern(pattern, mantissa, index, nextBitsLength);

            return Ok(new List<string> { res.Item1, res.Item2 });
        }


        [HttpGet("/api/RoundMantissa")]
        public ActionResult<string> RoundMantissa(string mantissa, string nextBits)
        {
            return _algorithmHelper.RoundMantissaNew(mantissa, nextBits);
        }

        [HttpGet("/api/32Bit/M")]
        public ActionResult<string> GetPatternOfMasInt32Bit(float M)
        {
            return _algorithmHelper.StringPatternOfM32Bit(M);
        }

        [HttpGet("/api/64Bit/M")]
        public ActionResult<string> GetPatternOfMasInt64Bit(float M)
        {
            return _algorithmHelper.StringPatternOfM64Bit(M);
        }
    }
}
