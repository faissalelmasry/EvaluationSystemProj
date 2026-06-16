using AutoMapper;
using EvaluationSystem.Application.DTOs.Evaluation_Response;
using EvaluationSystem.Application.DTOs.Evaluation_Result;
using EvaluationSystem.Application.DTOs.Evaluation_Reviewer;
using EvaluationSystem.Application.Exceptions;
using EvaluationSystem.Application.Helpers;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Domain.Enums;
using EvaluationSystem.Domain.Exceptions;
using EvaluationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Services.Evaluation_Service
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenericRepo<EvaluationCriteria> _evaluationCriteriaRepo;
        private readonly ScoreCalculator _calculator;
        public EvaluationService(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepo<EvaluationCriteria> evaluationCriteriaRepo, ScoreCalculator calculator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _evaluationCriteriaRepo = evaluationCriteriaRepo;
            _calculator = calculator;
        }

        public async Task<EvaluationResultDto> SubmitEvaluationAsync(int assignmentId, SubmitEvaluationDto dto)
{
    var assignment = await _unitOfWork.EvaluationAssignments.GetByIdAsync(assignmentId);
    if (assignment == null) throw new NotFoundException($"Assignment {assignmentId} not found.");

    // Allow re-submission if it was InProgress (rejected)
    if (assignment.Status == EvaluationStatus.Submitted || assignment.Status == EvaluationStatus.Completed)
    {
        throw new BadRequestException($"Assignment {assignmentId} cannot be modified.");
    }

    // 1. Delete existing responses so we can save the new ones
    var oldResponses = await _unitOfWork.EvaluationResponses.FindByCondition(r => r.AssignmentId == assignmentId, trackChanges: true).ToListAsync();
    foreach(var r in oldResponses) _unitOfWork.EvaluationResponses.Delete(r);

    // 2. Map and save new responses
    var responses = _mapper.Map<List<EvaluationResponse>>(dto.Responses);
    var criteriaIds = responses.Select(r => r.CriterionId).ToList();
    var criteriaList = await _evaluationCriteriaRepo.FindByCondition(c => criteriaIds.Contains(c.Id), trackChanges: false).ToListAsync();
    var criteriaDictionary = criteriaList.ToDictionary(c => c.Id);

    foreach (var response in responses)
    {
        response.AssignmentId = assignmentId;
        await _unitOfWork.EvaluationResponses.AddAsync(response);
    }

    // 3. Upsert logic for Results
    var finalResult = _calculator.CalculateFinalScore(assignmentId, responses, criteriaDictionary);
    
    var existingResult = await _unitOfWork.EvaluationResults
        .FindByCondition(r => r.AssignmentId == assignmentId, trackChanges: true)
        .FirstOrDefaultAsync();

    if (existingResult != null)
    {
        // Update existing record
        existingResult.TotalScore = finalResult.TotalScore; 
        // ... update other properties as needed
        _unitOfWork.EvaluationResults.Update(existingResult);
    }
    else
    {
        // Add new record
        await _unitOfWork.EvaluationResults.AddAsync(finalResult);
    }

    assignment.Status = EvaluationStatus.Submitted;
    _unitOfWork.EvaluationAssignments.Update(assignment);

    await _unitOfWork.SaveChangesAsync();
    return _mapper.Map<EvaluationResultDto>(existingResult ?? finalResult);
}
        public async Task<EvaluationReviewDto> ReviewEvaluationAsync(int assignmentId, int reviewerId, SubmitReviewDto dto, ReviewStatus newStatus)
        {
            var assignment = await _unitOfWork.EvaluationAssignments.GetByIdAsync(assignmentId);
            if (assignment == null)
            {
                throw new NotFoundException($"Assignment with ID {assignmentId} was not found.");
            }
            //if (assignment.EvaluatorId != reviewerId)
            //{
            //    throw new UnauthorizedException("You are not authorized to review this evaluation. Only the assigned evaluator can perform this action.");
            //}
            if (assignment.Status != EvaluationStatus.Submitted)
            {
                throw new BadRequestException($"Cannot review Assignment {assignmentId} because its current status is '{assignment.Status}'. It must be 'Submitted'.");
            }

            var existingResult = await _unitOfWork.EvaluationResults
            .FindByCondition(r => r.AssignmentId == assignmentId, trackChanges: false)
            .FirstOrDefaultAsync();

            if (existingResult == null)
            {
                throw new BadRequestException($"Cannot review Assignment {assignmentId} because no calculation results were found in the database.");
            }

            var review = new EvaluationReview
            {
                AssignmentId = assignmentId,
                ReviewerId = reviewerId,
                ReviewComment = dto.ReviewComment,
                Status = newStatus,
                ReviewedAt = DateTime.UtcNow
            };

            await _unitOfWork.EvaluationReviews.AddAsync(review);

            if (newStatus == ReviewStatus.Approved)
            {
                assignment.Status = EvaluationStatus.Completed;
            }
            else if (newStatus == ReviewStatus.Rejected || newStatus == ReviewStatus.Amended)
            {
                assignment.Status = EvaluationStatus.InProgress;
            }

            _unitOfWork.EvaluationAssignments.Update(assignment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EvaluationReviewDto>(review);
        }

        public async Task<List<EvaluationResponseDto>> GetResponsesByAssignmentAsync(int assignmentId)
        {
            var assignmentExists = await _unitOfWork.EvaluationAssignments
                .FindByCondition(a => a.Id == assignmentId, trackChanges: false)
                .AnyAsync();

            if (!assignmentExists)
            {
                throw new NotFoundException($"Assignment with ID {assignmentId} was not found.");
            }

            var responses = await _unitOfWork.EvaluationResponses
                .FindByCondition(r => r.AssignmentId == assignmentId, trackChanges: false)
                .Include(r => r.Criterion)
                .ToListAsync();

            if (!responses.Any())
            {
                throw new NotFoundException($"No responses found for Assignment ID {assignmentId}. The evaluation has not been submitted yet.");
            }

            return _mapper.Map<List<EvaluationResponseDto>>(responses);
        }

        public async Task<EvaluationResultDto> GetResultByAssignmentAsync(int assignmentId)
        {
            var assignmentExists = await _unitOfWork.EvaluationAssignments
                    .FindByCondition(a => a.Id == assignmentId, trackChanges: false)
                    .AnyAsync();

            if (!assignmentExists)
            {
                throw new NotFoundException($"Assignment with ID {assignmentId} was not found.");
            }

            var result = await _unitOfWork.EvaluationResults
                .FindByCondition(r => r.AssignmentId == assignmentId, trackChanges: false)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                throw new NotFoundException($"No result found for Assignment ID {assignmentId}. The evaluation might not be submitted yet.");
            }

            return _mapper.Map<EvaluationResultDto>(result);
        }
    }
}
