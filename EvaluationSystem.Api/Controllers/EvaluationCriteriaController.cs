using EvaluationSystem.Application.DTOs.EvaluationCriteria;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class EvaluationCriteriaController : ControllerBase
    {
        private IEvaluationCriteriaService CriteriaService { get; set; }
        public EvaluationCriteriaController(IEvaluationCriteriaService _CriteriaService)
        {
            CriteriaService = _CriteriaService;
        }
        [HttpPost("sectionid")]
        public async Task<IActionResult> AddCriteria(int sectionid, AddEvaluationCriteriaDto dto)
        {
            await CriteriaService.AddCriteriaAsync(sectionid, dto);
            return CreatedAtAction(nameof(AddCriteria), dto);
        }
        [HttpPut("criteriaid")]
        public async Task<IActionResult> UpdateCriteria(int criteriaid, AddEvaluationCriteriaDto dto)
        {
            await CriteriaService.UpdateCriteriaAsync(criteriaid, dto);
            return NoContent();
        }
        [HttpDelete("criteriaid")]
        public async Task<IActionResult> DeleteCriteria(int criteriaid)
        {
            await CriteriaService.DeleteCriteriaAsync(criteriaid);
            return NoContent();

        }
    }
}
