using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Application.Services.TemplateServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationSectionController : ControllerBase
    {
        private readonly IEvaluationSectionService evaluationSectionService;
        public EvaluationSectionController(IEvaluationSectionService _evaluationSectionService)
        {
            evaluationSectionService = _evaluationSectionService;
        }
        [HttpPost("templateid")]
        public async Task<IActionResult> AddSection(int templateid, AddEvaluationSectionDto dto)
        {
            var res = await evaluationSectionService.AddSectionAsync(templateid, dto);
            if (!res)
            {
                return BadRequest("can't add this section");
            }
            return Created();
        }
        [HttpPut("id")]
        public async Task<IActionResult> UpdateSection(int id, AddEvaluationSectionDto dto)
        {
            var res = await evaluationSectionService.UpdateSectionAsync(id, dto);
            if (!res)
            {
                return BadRequest("can't update this section");
            }
            return NoContent();
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var res = await evaluationSectionService.DeleteSectionAsync(id);
            if (!res)
            {
                return BadRequest("can't Delete this section");
            }
            return NoContent();
        }
    }
}
