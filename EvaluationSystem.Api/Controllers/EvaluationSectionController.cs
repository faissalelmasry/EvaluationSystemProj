using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Application.Services.TemplateServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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
            await evaluationSectionService.AddSectionAsync(templateid, dto);
            return Created();
        }
        [HttpPut("id")]
        public async Task<IActionResult> UpdateSection(int id, AddEvaluationSectionDto dto)
        {
            await evaluationSectionService.UpdateSectionAsync(id, dto);
            return NoContent();
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            await evaluationSectionService.DeleteSectionAsync(id);
            return NoContent();
        }
    }
}
