using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvaluationSystem.Application.interfaces;         
using EvaluationSystem.Application.DTOs.Assignments;
using Microsoft.AspNetCore.Authorization;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvaluationAssignmentsController : ControllerBase
    {
        private readonly IEvaluationAssignmentService _assignmentService;

        public EvaluationAssignmentsController(IEvaluationAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDto dto, [FromQuery] int adminId)
        {
            try
            {

                var result = await _assignmentService.CreateAssignmentAsync(dto, adminId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAssignments()
        {
            var assignments = await _assignmentService.GetAllAssignmentsAsync();
            return Ok(assignments);
        }
        [HttpGet("my-pending")]
        public async Task<IActionResult> GetMyPendingEvaluations([FromQuery] int evaluatorId)
        {
            try
            {
                var pendingEvaluations = await _assignmentService.GetMyPendingEvaluationsAsync(evaluatorId);


                if (pendingEvaluations == null || !pendingEvaluations.Any())
                {

                    return Ok(new { message = "No pending evaluation assignments found for this evaluator." });
                }


                return Ok(pendingEvaluations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
