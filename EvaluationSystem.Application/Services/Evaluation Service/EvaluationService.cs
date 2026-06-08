using AutoMapper;
using EvaluationSystem.Application.DTOs.Evaluation_Response;
using EvaluationSystem.Application.DTOs.Evaluation_Result;
using EvaluationSystem.Application.DTOs.Evaluation_Reviewer;
using EvaluationSystem.Application.Helpers;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Domain.Enums;
using EvaluationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Services.Evaluation_Service
{
    public class EvaluationService: IEvaluationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenericRepo<EvaluationCriteria> _evaluationCriteriaRepo;
        private readonly ScoreCalculator _calculator;
        public EvaluationService(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepo<EvaluationCriteria> evaluationCriteriaRepo, ScoreCalculator calculator    )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _evaluationCriteriaRepo = evaluationCriteriaRepo;
            _calculator = calculator;
        }

        public async Task<EvaluationResultDto> SubmitEvaluationAsync( int assignmentId, SubmitEvaluationDto dto)
        {
            var responses = _mapper.Map<List<EvaluationResponse>>(dto.Responses);

            var criteriaIds = responses
                .Select(r => r.CriterionId)
                .ToList();

            foreach (var response in responses)
            {
                response.AssignmentId = assignmentId;

                await _unitOfWork.EvaluationResponses.AddAsync(response);
            }

            var criteriaList = await _evaluationCriteriaRepo
                .FindByCondition(
                    c => criteriaIds.Contains(c.Id),
                    trackChanges: false)
                .ToListAsync();

            var criteriaDictionary = criteriaList.ToDictionary(c => c.Id);

            foreach (var response in responses)
            {
                if (criteriaDictionary.TryGetValue(
                        response.CriterionId,
                        out var criterion))
                {
                    response.Criterion = criterion;
                }
                else
                {
                    throw new Exception(
                        $"Criterion with ID {response.CriterionId} not found.");
                }
            }


            var finalResult = _calculator.CalculateFinalScore(
                assignmentId,
                responses);

            await _unitOfWork.EvaluationResults.AddAsync(finalResult);

            var assignment = await _unitOfWork
                .EvaluationAssignments
                .GetByIdAsync(assignmentId);

            if (assignment != null)
            {
                assignment.Status = EvaluationStatus.Submitted;

                _unitOfWork.EvaluationAssignments.Update(assignment);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EvaluationResultDto>(finalResult);
        }

        public async Task<EvaluationReviewDto> ReviewEvaluationAsync(int assignmentId, int reviewerId, SubmitReviewDto dto, ReviewStatus newStatus)
        {
            var existingResult = await _unitOfWork.EvaluationResults
                    .FindByCondition(r => r.AssignmentId == assignmentId, trackChanges: false)
                    .FirstOrDefaultAsync();
            if (existingResult == null)
            {
                throw new Exception($"Cannot review Assignment {assignmentId} because it has not been submitted yet.");
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
            var assignment = await _unitOfWork.EvaluationAssignments.GetByIdAsync(assignmentId);
            if (assignment != null)
            {
                if (newStatus == ReviewStatus.Approved)
                {
                    assignment.Status = EvaluationStatus.Completed;
                }
                else if (newStatus == ReviewStatus.Rejected || newStatus == ReviewStatus.Amended)
                {
                    assignment.Status = EvaluationStatus.InProgress;
                }

                _unitOfWork.EvaluationAssignments.Update(assignment);
            }


            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EvaluationReviewDto>(review);
        }

        public async Task<List<EvaluationResponseDto>> GetResponsesByAssignmentAsync(int assignmentId)
        {
            var responses = await _unitOfWork.EvaluationResponses
                .FindByCondition(r => r.AssignmentId == assignmentId, trackChanges: false)
                .Include(r => r.Criterion) 
                .ToListAsync();

            if (!responses.Any())
            {
                throw new Exception($"No responses found for Assignment ID {assignmentId}.");
            }

            return _mapper.Map<List<EvaluationResponseDto>>(responses);
        }

        public async Task<EvaluationResultDto> GetResultByAssignmentAsync(int assignmentId)
        {
            var result = await _unitOfWork.EvaluationResults
                .FindByCondition(r => r.AssignmentId == assignmentId, trackChanges: false)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception($"No result found for Assignment ID {assignmentId}. The evaluation might not be submitted yet.");
            }

            return _mapper.Map<EvaluationResultDto>(result);
        }
    }

    }
