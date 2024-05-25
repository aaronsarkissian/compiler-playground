using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlaygroundCompiler.Infrastrucutre.Entities;
using PlaygroundCompiler.Web.Dto;
using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Web.Dto;

namespace PlaygroundService.Controllers
{
    [Route("")]
    [ApiController]
    public class SingleFilePlaygroundController : ControllerBase
    {
        private readonly IPlaygroundLogic _playgroundLogic;
        private readonly IMapper _mapper;

        public SingleFilePlaygroundController(IPlaygroundLogic playgroundLogic, IMapper mapper)
        {
            _playgroundLogic = playgroundLogic;
            _mapper = mapper;
        }

        /// <summary>
        /// Legacy Single File Playground
        /// </summary>
        /// <param name="codeDto"></param>
        /// <returns></returns>
        [HttpPost("playground")]
        public async Task<ActionResult> BuildAndRun([FromBody] CodeDtoSingle codeDto)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine("The playground endpoint failed: {0}", e.ToString());
                return StatusCode(500);
            }

        }

        /// <summary>
        /// Single File Playground With Image output
        /// </summary>
        /// <param name="codeDto"></param>
        /// <returns></returns>
        [HttpPost("playgroundv2")]
        public async Task<ActionResult> BuildAndRunWithImage([FromBody] CodeDtoSingle codeDto)
        {
            try
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

                var compilerOutputDTO = _mapper.Map<CompilerOutputDTO>(result);

                return Ok(compilerOutputDTO);
            }
            catch (Exception e)
            {
                Console.WriteLine("The playgroundv2 endpoint failed: {0}", e.ToString());
                return StatusCode(500);
            }
        }
    }
}
