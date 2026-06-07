using EvaluationSystem.Application.DTOs.Evaluation_Response;
using EvaluationSystem.Application.DTOs.Evaluation_Reviewer;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Application.Services.Evaluation_Service;
using EvaluationSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvaluationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationsController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;

        public EvaluationsController(IEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        [HttpPost("{assignmentId}/submit")]
        public async Task<IActionResult> SubmitEvaluation(int assignmentId, [FromBody] SubmitEvaluationDto dto)
        {
            try
            {
                var result = await _evaluationService.SubmitEvaluationAsync(assignmentId, dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("{assignmentId}/review")]
        public async Task<IActionResult> ReviewEvaluation(int assignmentId, [FromQuery] ReviewStatus status, [FromBody] SubmitReviewDto dto)
        {
            try
            {
                var reviewerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(reviewerIdClaim, out int reviewerId))
                {
                    return Unauthorized(new { Message = "You must be logged in to review evaluations." });
                }

                var reviewResult = await _evaluationService.ReviewEvaluationAsync(assignmentId, reviewerId, dto, status);

                return Ok(reviewResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("{assignmentId}/responses")]
        public async Task<IActionResult> GetResponses(int assignmentId)
        {
            try
            {
                var responses = await _evaluationService.GetResponsesByAssignmentAsync(assignmentId);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("{assignmentId}/result")]
        public async Task<IActionResult> GetResult(int assignmentId)
        {
            try
            {
                var result = await _evaluationService.GetResultByAssignmentAsync(assignmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
    }