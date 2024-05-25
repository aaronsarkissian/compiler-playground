using Microsoft.AspNetCore.Mvc;
using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Infrastrucutre.Entities;
using PlaygroundService.Web.Dto;

namespace PlaygroundService.Web.Controllers
{
    [Route("judge")]
    [ApiController]
    public class JudgeController : ControllerBase
    {
        private readonly IJudgeLogic _judgeLogic;
        public JudgeController(IJudgeLogic judgeLogic)
        {
            _judgeLogic = judgeLogic;
        }

        /// <summary>
        /// Unit Test
        /// </summary>
        /// <param name="judgeDto"></param>
        /// <returns></returns>
        [HttpPost("unittest")]
        public async Task<ActionResult> BuildAndUnitTest([FromBody] JudgeUnitDto judgeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            JudgeOutput result = await _judgeLogic.BuildAndTest(judgeDto.ToCode());

            if (result is null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Input/Output Test
        /// </summary>
        /// <param name="judgeIoDto"></param>
        /// <returns></returns>
        [HttpPost("inputoutput")]
        public async Task<ActionResult> BuildAndIoTest([FromBody] JudgeIoDto judgeIoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            JudgeOutput result = await _judgeLogic.BuildAndTest(judgeIoDto.ToCode());

            if (result is null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }
    }
}