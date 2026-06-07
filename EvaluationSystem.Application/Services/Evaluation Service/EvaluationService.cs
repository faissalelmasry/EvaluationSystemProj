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
        public EvaluationService(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepo<EvaluationCriteria> evaluationCriteriaRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _evaluationCriteriaRepo = evaluationCriteriaRepo;

        }

        public async Task<EvaluationResultDto> SubmitEvaluationAsync(int assignmentId, SubmitEvaluationDto dto)
        {
            var responses = _mapper.Map<List<EvaluationResponse>>(dto.Responses);

            // ========================================================
            // 🛑 SIMULATION MODE: BYPASSING THE DATABASE
            // ========================================================

            // 1. Fake the database criteria in memory so we don't need SQL
            var fakeCriteriaList = new List<EvaluationCriteria>
    {
        // Question 10: Rating Scale, Weight 2
        new EvaluationCriteria { Id = 10, Weight = 2, MaxScore = 5, QuestionType = QuestionType.RatingScale },
        // Question 11: Yes/No, Weight 1
        new EvaluationCriteria { Id = 11, Weight = 1, MaxScore = 5, QuestionType = QuestionType.Boolean }
    };

            // 2. Attach our fake criteria to the incoming answers
            foreach (var response in responses)
            {
                response.AssignmentId = assignmentId;
                response.Criterion = fakeCriteriaList.FirstOrDefault(c => c.Id == response.CriterionId);

                if (response.Criterion == null) throw new Exception($"Criterion {response.CriterionId} not found.");
            }

            // 3. Run your Math Engine!
            var calculator = new ScoreCalculator();
            var finalResult = calculator.CalculateFinalScore(assignmentId, responses);

            // 4. We COMMENT OUT the database saves so SQL Server doesn't crash!
            // await _unitOfWork.EvaluationResults.AddAsync(finalResult);
            // await _unitOfWork.SaveChangesAsync();

            // 5. Return the result straight to Swagger
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
    }

    }
