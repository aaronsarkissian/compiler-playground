using Microsoft.AspNetCore.Mvc;
using PlaygroundCompiler.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Web.Dto;

namespace PlaygroundService.Controllers
{
    [Route("new-playground")]
    [ApiController]
    public class PlaygroundController : ControllerBase
    {
        private readonly IPlaygroundLogic _playgroundLogic;
        public PlaygroundController(IPlaygroundLogic playgroundLogic)
        {
            _playgroundLogic = playgroundLogic;
        }

        /// <summary>
        /// Multi File Support Playground
        /// </summary>
        /// <param name="codeDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> BuildAndRun([FromBody] CodeDto codeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            CompilerOutput result = await _playgroundLogic.BuildAndRun(codeDto.ToCode());

            if (result is null)
            {
                return StatusCode(500);
            }

            return Ok(result.Output.ToString());
        }
    }
}
