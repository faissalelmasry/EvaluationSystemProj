using EvaluationSystem.Application.DTOs.EvaluationCriteria;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var res =await CriteriaService.AddCriteriaAsync(sectionid, dto);
            if (!res)
            {
                return BadRequest("can't add this criteria");
            }
            return CreatedAtAction(nameof(AddCriteria), dto);
        }
        [HttpPut("criteriaid")]
        public async Task<IActionResult> UpdateCriteria(int criteriaid, AddEvaluationCriteriaDto dto)
        {
            var res = await CriteriaService.UpdateCriteriaAsync(criteriaid, dto);
            if (!res)
            {
                return BadRequest("can't update this criteria");
            }
            return NoContent();
        }
        [HttpDelete("criteriaid")]
        public async Task<IActionResult> DeleteCriteria(int criteriaid)
        {
            var res = await CriteriaService.DeleteCriteriaAsync(criteriaid);
            if (!res)
            {
                return BadRequest("can't delete this criteria");
            }
            return NoContent();

        }
    }
}
