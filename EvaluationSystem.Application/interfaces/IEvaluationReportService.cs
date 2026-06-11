using EvaluationSystem.Application.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.interfaces
{
    public interface IEvaluationReportService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<IEnumerable<DepartmentReportDto>> GetReportByDepartmentAsync();
        Task<UserReportDto> GetReportByUserAsync(int userId);
        Task<CompletionRateDto> GetCompletionRateAsync();
        Task<TopScoresDto> GetTopScoresAsync();
        Task<byte[]> GenerateEvaluationPdfAsync(int assignmentId);
    }
}
