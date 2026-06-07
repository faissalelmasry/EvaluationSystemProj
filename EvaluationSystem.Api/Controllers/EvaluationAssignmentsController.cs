using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvaluationSystem.Application.interfaces;         
using EvaluationSystem.Application.DTOs.Assignments;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssignmentById(int id)
        {
            try
            {
                var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
                return Ok(assignment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] CreateAssignmentDto dto)
        {
            try
            {
                var result = await _assignmentService.UpdateAssignmentAsync(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
