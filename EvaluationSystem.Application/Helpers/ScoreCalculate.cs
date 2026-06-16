using EvaluationSystem.Domain.Enums;
using EvaluationSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Helpers
{
    public class ScoreCalculator
    {
        public EvaluationResult CalculateFinalScore(int assignmentId, List<EvaluationResponse> responses, Dictionary<int, EvaluationCriteria> criteriaDictionary)
        {
            decimal totalScore = 0;
            decimal maxPossibleScore = 0;

            foreach (var response in responses)
            {
                if (!criteriaDictionary.TryGetValue(response.CriterionId, out var criterion))
                    throw new ArgumentException($"Criterion with ID {response.CriterionId} was not found in the provided criteria dictionary.");

                if (criterion.QuestionType == QuestionType.RatingScale ||
                    criterion.QuestionType == QuestionType.SingleChoice)
                {
                    totalScore += (response.Score * criterion.Weight);
                    maxPossibleScore += (criterion.MaxScore * criterion.Weight);
                }
                else if (criterion.QuestionType == QuestionType.Boolean)
                {
                    bool isYes = string.Equals(response.SelectedOption, "Yes", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(response.SelectedOption, "True", StringComparison.OrdinalIgnoreCase);

                    decimal earnedScore = isYes ? criterion.MaxScore : 0;

                    totalScore += (earnedScore * criterion.Weight);
                    maxPossibleScore += (criterion.MaxScore * criterion.Weight);
                }
            }

            decimal percentage = maxPossibleScore == 0 ? 0 : (totalScore / maxPossibleScore) * 100;

            return new EvaluationResult
            {
                AssignmentId = assignmentId,
                TotalScore = totalScore,
                MaxPossibleScore = maxPossibleScore,
                Percentage = Math.Round(percentage, 2),
                Grade = CalculateGrade(percentage)
            };
        }

        private string CalculateGrade(decimal percentage)
        {
            return percentage switch
            {
                >= 90 => "Excellent",
                >= 80 => "Very Good",
                >= 70 => "Good",
                >= 60 => "Pass",
                _ => "Needs Improvement"
            };
        }
    }
}