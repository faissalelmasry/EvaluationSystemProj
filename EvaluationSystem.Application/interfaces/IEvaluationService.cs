using EvaluationSystem.Application.DTOs.Evaluation_Response;
using EvaluationSystem.Application.DTOs.Evaluation_Result;
using EvaluationSystem.Application.DTOs.Evaluation_Reviewer;
using EvaluationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.interfaces
{
    public interface IEvaluationService
    {
        Task<EvaluationResultDto> SubmitEvaluationAsync(int assignmentId, SubmitEvaluationDto dto);
        Task<EvaluationReviewDto> ReviewEvaluationAsync(int assignmentId, int reviewerId, SubmitReviewDto dto, ReviewStatus newStatus);
        Task<List<EvaluationResponseDto>> GetResponsesByAssignmentAsync(int assignmentId);
        Task<EvaluationResultDto> GetResultByAssignmentAsync(int assignmentId);
    }
}
