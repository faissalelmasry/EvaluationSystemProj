using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvaluationTemplateController : ControllerBase
    {
        private IEvaluationTemplateService TemplateService { get; set; }
        public EvaluationTemplateController(IEvaluationTemplateService _templateService)
        {
            TemplateService = _templateService;
        }
        [HttpGet]
        public async Task<IActionResult> GetTemplates(int PageNumber = 1, int PageSize = 10, string? Search = "")
        {
            var Templates = await TemplateService.GetTemplatesAsync(PageNumber, PageSize, Search);
            return Ok(Templates);
        }
        [HttpPost]
        public async Task<IActionResult> AddTemplate(EvaluationTemplateDto dto)
        {
            await TemplateService.AddTemplateAsync(dto);
            return CreatedAtAction(nameof(GetTemplate),dto);
        }
        [HttpPut("id")]
        public async Task<IActionResult> UpdateTemplate(int id, EvaluationTemplateDto dto)
        {
            await TemplateService.UpdateTemplateAsync(id, dto);

            return NoContent();
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetTemplate(int id)
        {
            var temp = await TemplateService.GetTemplateAsync(id);
            return Ok(temp);
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            await TemplateService.DeleteTemplateAsync(id);
            return NoContent();
        }

    }
}
