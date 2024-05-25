using Microsoft.AspNetCore.Mvc;
using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Web.Dto;

namespace PlaygroundService.Web.Controllers
{
    [Route("linter")]
    [ApiController]
    public class AnalyzeController : ControllerBase
    {
        private readonly IAnalyzeLogic _analyzeLogic;

        public AnalyzeController(IAnalyzeLogic analyzeLogic)
        {
            _analyzeLogic = analyzeLogic;
        }

        /// <summary>
        /// Analyze Code
        /// </summary>
        /// <param name="codeDto"></param>
        /// <returns></returns>
        [HttpPost("analyze")]
        public async Task<ActionResult> Analyze([FromBody] CodeDtoSingle codeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _analyzeLogic.Analyze(codeDto.ToCode());

            if (result is null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }
    }
}
