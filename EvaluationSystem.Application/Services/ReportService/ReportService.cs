using AutoMapper;
using EvaluationSystem.Application.DTOs.Reports;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Domain.Enums;
using EvaluationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Services.ReportService
{
    public class ReportService : IEvaluationReportService
    {
        
            private readonly IUnitOfWork _unitOfWork;
            public ReportService(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
               
            }
            public async Task<CompletionRateDto> GetCompletionRateAsync()
        {
            var assignments = await _unitOfWork.EvaluationAssignments
            .GetAll(trackChanges: false)
            .ToListAsync();
            var total = assignments.Count;
            var completed = assignments.Count(a => a.Status == EvaluationStatus.Completed);
            var percentage = total > 0 ? ((double)completed / total) * 100 : 0;
            var monthlyTrends = assignments
         .GroupBy(a => new { a.CreatedAt.Year, a.CreatedAt.Month })
         .Select(g => new MonthlyTrendDto
         {
             Month = $"{g.Key.Year}-{g.Key.Month:D2}",
             TotalCreated = g.Count(),
             TotalCompleted = g.Count(a => a.Status == EvaluationStatus.Completed)
         })
         .OrderBy(m => m.Month) 
         .ToList();
            return new CompletionRateDto
            {
                TotalAssignments = total,
                CompletedAssignments = completed,
                RatePercentage = Math.Round(percentage, 2), 
                MonthlyTrends = monthlyTrends
            };


        }

            public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var users =  _unitOfWork.Users.GetAll();
            var totalUsers = await users.CountAsync();
            var templates =_unitOfWork.EvaluationTemplates.GetAll();
            var totalTemplates= await templates.CountAsync();
            var assignments = await _unitOfWork.EvaluationAssignments
         .GetAll(trackChanges: false)
         .Include(a => a.Result) 
         .ToListAsync();
            var pendingCount = assignments.Count(a => a.Status == EvaluationStatus.Pending);
            var completedCount = assignments.Count(a => a.Status == EvaluationStatus.Completed);
            var completedWithResult = assignments.Where(a => a.Status == EvaluationStatus.Completed && a.Result != null);
            var averageScore = completedWithResult.Any() ? completedWithResult.Average(a => (double)a.Result.TotalScore) : 0;
            var recentActivity = assignments
                 .OrderByDescending(a => a.Id)
                 .Take(5)
                 .Select(a => $"Assignment #{a.Id} status changed to {a.Status}")
                 .ToList();

            return new DashboardSummaryDto
            {
                TotalUsers = totalUsers,
                TotalTemplates = totalTemplates,
                PendingEvaluations = pendingCount,
                CompletedEvaluations = completedCount,
                AverageScore = Math.Round(averageScore, 2),
                RecentActivity = recentActivity
            };
        }

            public async Task<IEnumerable<DepartmentReportDto>> GetReportByDepartmentAsync()
        {
            return await _unitOfWork.EvaluationAssignments
                .GetAll(trackChanges: false)
                .Where(a => a.Status == EvaluationStatus.Completed && a.Result != null && a.Evaluatee.Department != null)
                .GroupBy(a => new { a.Evaluatee.DepartmentId, a.Evaluatee.Department.Name })
                .Select(g => new DepartmentReportDto
                {
                    DepartmentId = g.Key.DepartmentId, 
                    DepartmentName = g.Key.Name,
                    AverageScore = (double)Math.Round(g.Average(a => a.Result.TotalScore), 2)
                })
                .ToListAsync();
        }
            public async Task<UserReportDto> GetReportByUserAsync(int userId)
        {
            var report = await _unitOfWork.EvaluationAssignments
                .GetAll(trackChanges: false)
                .Where(a => a.Status == EvaluationStatus.Completed && a.Result != null && a.EvaluateeId == userId)
                .GroupBy(a => new { a.EvaluateeId, a.Evaluatee.UserName }) 
                .Select(g => new UserReportDto
                {
                    UserId = g.Key.EvaluateeId,
                    UserName = g.Key.UserName,
                    AverageScore = (double)Math.Round(g.Average(a => a.Result.TotalScore), 2),
                    CompletedEvaluations = g.Count() 
                })
                .FirstOrDefaultAsync();
            return report ?? new UserReportDto
            {
                UserId = userId,
                UserName = "No Data",
                AverageScore = 0,
                CompletedEvaluations = 0
            };
        }

            public async Task<TopScoresDto> GetTopScoresAsync()
        {
            var completedAssignments = await _unitOfWork.EvaluationAssignments
                .GetAll(trackChanges: false)
                .Where(a => a.Status == EvaluationStatus.Completed && a.Result != null)
                .Include(a => a.Evaluatee)
                .Include(a => a.Template)
                .Include(a => a.Result)
                .ToListAsync();

            var topEvaluatees = completedAssignments
                .OrderByDescending(a => a.Result.TotalScore)
                .Take(5)
                .Select(a => new EvaluatedScoreDto
                {
                    EvaluateeName = a.Evaluatee.FullName,
                    Score = (double)a.Result.TotalScore,
                    TemplateTitle = a.Template.Title
                }).ToList();

            var lowScores = completedAssignments
                .OrderBy(a => a.Result.TotalScore)
                .Take(5)
                .Select(a => new EvaluatedScoreDto
                {
                    EvaluateeName = a.Evaluatee.FullName,
                    Score = (double)a.Result.TotalScore,
                    TemplateTitle = a.Template.Title
                }).ToList();

            return new TopScoresDto
            {
                TopEvaluatees = topEvaluatees,
                LowScoreEvaluations = lowScores
            };
        }
    }
}
