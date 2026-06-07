using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationTemplateController : ControllerBase
    {
        IEvaluationTemplateService TemplateService { get; set; }
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
            var res = await TemplateService.AddTemplate(dto);
            if (!res)
            {
                return BadRequest("can't add this template");
            }
            return Created();
        }
        [HttpPut("id")]
        public async Task<IActionResult> UpdateTemplate(int id, EvaluationTemplateDto dto)
        {
            var res = await TemplateService.UpdateTemplate(id, dto);
            if (!res)
            {
                return BadRequest("can't update this template");
            }
            return NoContent();
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetTemplate(int id)
        {
            var temp = await TemplateService.GetTemplateAsync(id);
            if (temp==null)
                return NotFound();
            return Ok(temp);
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var res = await TemplateService.DeleteTemplateAsync(id);
            if (!res)
                return NotFound("Can't delete template");
            return NoContent();
        }

    }
}
