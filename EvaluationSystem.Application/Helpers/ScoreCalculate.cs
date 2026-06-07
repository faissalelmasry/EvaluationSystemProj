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
        public EvaluationResult CalculateFinalScore(int assignmentId, List<EvaluationResponse> responses)
        {
            decimal totalScore = 0;
            decimal maxPossibleScore = 0;

            foreach (var response in responses)
            {
                if (response.Criterion == null)
                    throw new ArgumentNullException(nameof(response.Criterion), "Criteria must be loaded to calculate scores.");

                if (response.Criterion.QuestionType == QuestionType.RatingScale ||
                    response.Criterion.QuestionType == QuestionType.SingleChoice)
                {
                    totalScore += (response.Score * response.Criterion.Weight);
                    maxPossibleScore += (response.Criterion.MaxScore * response.Criterion.Weight);
                }

                else if (response.Criterion.QuestionType == QuestionType.Boolean)
                {
                    bool isYes = string.Equals(response.SelectedOption, "Yes", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(response.SelectedOption, "True", StringComparison.OrdinalIgnoreCase);

                    decimal earnedScore = isYes ? response.Criterion.MaxScore : 0;

                    totalScore += (earnedScore * response.Criterion.Weight);
                    maxPossibleScore += (response.Criterion.MaxScore * response.Criterion.Weight);
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