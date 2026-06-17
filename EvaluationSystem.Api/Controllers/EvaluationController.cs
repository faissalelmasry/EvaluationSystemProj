using EvaluationSystem.Application.DTOs.Evaluation_Response;
using EvaluationSystem.Application.DTOs.Evaluation_Reviewer;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using EvaluationSystem.Domain.Exceptions; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EvaluationSystem.API.Extensions;

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
        [Authorize(Roles = "Evaluator")]
        public async Task<IActionResult> SubmitEvaluation(int assignmentId, [FromBody] SubmitEvaluationDto dto)
        {
            var result = await _evaluationService.SubmitEvaluationAsync(assignmentId, dto);
            return Ok(result);
        }

        [HttpGet("{assignmentId}/responses")]
        [Authorize(Roles ="Reviewer")]
        public async Task<IActionResult> GetResponses(int assignmentId)
        {
            var responses = await _evaluationService.GetResponsesByAssignmentAsync(assignmentId);
            return Ok(responses);
        }

        [HttpGet("{assignmentId}/result")]
        [Authorize(Roles ="Evaluatee")]
        public async Task<IActionResult> GetResult(int assignmentId)
        {
            var result = await _evaluationService.GetResultByAssignmentAsync(assignmentId);
            return Ok(result);
        }

        [HttpPost("{assignmentId}/approve")]
        [Authorize(Roles = "Reviewer")]
        public async Task<IActionResult> ApproveEvaluation(int assignmentId, [FromBody] SubmitReviewDto dto)
        {
            var reviewerId = User.GetUserId();
            var reviewResult = await _evaluationService.ReviewEvaluationAsync(assignmentId, reviewerId, dto, ReviewStatus.Approved);
            return Ok(reviewResult);
        }
        [HttpPost("{assignmentId}/reject")]
        [Authorize(Roles = "Reviewer")]
        public async Task<IActionResult> RejectEvaluation(int assignmentId, [FromBody] SubmitReviewDto dto)
        {
            var reviewerId = User.GetUserId();
            var reviewResult = await _evaluationService.ReviewEvaluationAsync(assignmentId, reviewerId, dto, ReviewStatus.Rejected);
            return Ok(reviewResult);
        }
    }
}