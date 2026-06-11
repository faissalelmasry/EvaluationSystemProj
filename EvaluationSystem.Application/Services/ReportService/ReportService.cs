using AutoMapper;
using EvaluationSystem.Application.DTOs.Reports;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Domain.Enums;
using EvaluationSystem.Domain.Models;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
namespace EvaluationSystem.Application.Services.ReportService
{
    public class ReportService : IEvaluationReportService
    {
        
            private readonly IUnitOfWork _unitOfWork;
            public ReportService(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
               
            }

        public async Task<byte[]> GenerateEvaluationPdfAsync(int assignmentId)
        {
            
            var assignment = await _unitOfWork.EvaluationAssignments.GetByIdAsync(
                assignmentId,
                query => query.Include(a => a.Evaluatee),
                query => query.Include(a => a.Evaluator),
                query => query.Include(a => a.Template),
                query => query.Include(a => a.Result)
            );

            
            if (assignment == null)
                throw new Exception("Evaluation assignment not found.");

            using (var stream = new MemoryStream())
            {
                // Initialize iText core rendering pipeline components
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);

                
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // 2. Define a clean, corporate visual branding color scheme
                iText.Kernel.Colors.Color primaryColor = new DeviceRgb(26, 54, 93);     
                iText.Kernel.Colors.Color secondaryColor = new DeviceRgb(74, 85, 104);  
                iText.Kernel.Colors.Color lightBgColor = new DeviceRgb(247, 250, 252);  
                iText.Kernel.Colors.Color borderColor = new DeviceRgb(226, 232, 240);  

               
                // SECTION 1: Top Header Banner
                
                Table headerBanner = new Table(UnitValue.CreatePercentArray(new float[] { 100f })).UseAllAvailableWidth();

                Cell bannerCell = new Cell()
                    .Add(new Paragraph("PERFORMANCE EVALUATION REPORT")
                        .SetFont(boldFont) 
                        .SetFontColor(ColorConstants.WHITE)
                        .SetFontSize(20)
                        .SetTextAlignment(TextAlignment.CENTER))
                    .SetBackgroundColor(primaryColor)
                    .SetPadding(15f)
                    .SetBorder(Border.NO_BORDER);

                headerBanner.AddCell(bannerCell);
                document.Add(headerBanner);

                // Add document generation timestamp aligned to the right side
                document.Add(new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(9)
                    .SetFontColor(secondaryColor)
                    .SetMarginTop(5f)
                    .SetMarginBottom(20f));

                
                // SECTION 2: General Information Card
               
                document.Add(new Paragraph("General Information")
                    .SetFont(boldFont) 
                    .SetFontSize(14)
                    .SetFontColor(primaryColor)
                    .SetMarginBottom(8f));

                // Define a 2-column key-value layout table (30% width for labels, 70% width for data values)
                Table infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 30f, 70f })).UseAllAvailableWidth();
                infoTable.SetBackgroundColor(lightBgColor);
                infoTable.SetBorder(new SolidBorder(borderColor, 1f));
                infoTable.SetPadding(10f);

                // Data Row 1: Template Title
                infoTable.AddCell(new Cell().Add(new Paragraph("Evaluation Title:").SetFont(boldFont).SetFontColor(secondaryColor)).SetBorder(Border.NO_BORDER).SetPadding(6f));
                infoTable.AddCell(new Cell().Add(new Paragraph(assignment.Template?.Title ?? "N/A")).SetBorder(Border.NO_BORDER).SetPadding(6f));

                // Data Row 2: Employee Name
                string evaluateeLabel = assignment.Evaluatee != null ? $"{assignment.Evaluatee.JobTitle} Name:" : "Evaluatee Name:";

                infoTable.AddCell(new Cell().Add(new Paragraph(evaluateeLabel).SetFont(boldFont).SetFontColor(secondaryColor)).SetBorder(Border.NO_BORDER).SetPadding(6f));
                infoTable.AddCell(new Cell().Add(new Paragraph(assignment.Evaluatee?.FullName ?? "N/A")).SetBorder(Border.NO_BORDER).SetPadding(6f));

                // Data Row 3: Evaluator Name
                infoTable.AddCell(new Cell().Add(new Paragraph("Evaluated By:").SetFont(boldFont).SetFontColor(secondaryColor)).SetBorder(Border.NO_BORDER).SetPadding(6f));
                infoTable.AddCell(new Cell().Add(new Paragraph(assignment.Evaluator?.FullName ?? "N/A")).SetBorder(Border.NO_BORDER).SetPadding(6f));

                // Data Row 4: Due Date
                infoTable.AddCell(new Cell().Add(new Paragraph("Due Date:").SetFont(boldFont).SetFontColor(secondaryColor)).SetBorder(Border.NO_BORDER).SetPadding(6f));
                infoTable.AddCell(new Cell().Add(new Paragraph(assignment.DueDate.ToString("yyyy-MM-dd"))).SetBorder(Border.NO_BORDER).SetPadding(6f));

                document.Add(infoTable);
                document.Add(new Paragraph("\n")); // Divider spacing

               
                // SECTION 3: Evaluation Score Summary Badges
               
                document.Add(new Paragraph("Evaluation Score Summary")
                    .SetFont(boldFont)
                    .SetFontSize(14)
                    .SetFontColor(primaryColor)
                    .SetMarginBottom(8f));

                Table scoreCardTable = new Table(UnitValue.CreatePercentArray(new float[] { 50f, 50f })).UseAllAvailableWidth();

                string scoreText = assignment.Result != null
                    ? $"{assignment.Result.TotalScore} / {assignment.Result.MaxPossibleScore} ({assignment.Result.Percentage}%)"
                    : "Pending Evaluation";
                string gradeText = assignment.Result?.Grade ?? "N/A";

                // Left Badge Container: Final Numerical Score
                Cell scoreCell = new Cell()
                    .SetBackgroundColor(lightBgColor)
                    .SetBorder(new SolidBorder(primaryColor, 1.5f))
                    .SetPadding(12f)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph("FINAL SCORE").SetFont(boldFont).SetFontSize(10).SetFontColor(secondaryColor))
                    .Add(new Paragraph(scoreText).SetFont(boldFont).SetFontSize(16).SetFontColor(primaryColor));

                // Right Badge Container: Alphabetical/Word Grade
                Cell gradeCell = new Cell()
                    .SetBackgroundColor(lightBgColor)
                    .SetBorder(new SolidBorder(primaryColor, 1.5f))
                    .SetPadding(12f)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph("FINAL GRADE").SetFont(boldFont).SetFontSize(10).SetFontColor(secondaryColor))
                    .Add(new Paragraph(gradeText).SetFont(boldFont).SetFontSize(16).SetFontColor(new DeviceRgb(47, 133, 90)));

                scoreCardTable.AddCell(scoreCell);
                scoreCardTable.AddCell(gradeCell);
                document.Add(scoreCardTable);
                document.Add(new Paragraph("\n\n\n\n"));

                // SECTION 4: Authorized Signatures Footer
              
                Table footerTable = new Table(UnitValue.CreatePercentArray(new float[] { 50f, 50f })).UseAllAvailableWidth();

                Cell hrSignature = new Cell()
                    .Add(new Paragraph("___________________________\nHuman Resources Department")
                        .SetFontSize(10)
                        .SetFontColor(secondaryColor))
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.LEFT);

                Cell managerSignature = new Cell()
                    .Add(new Paragraph("___________________________\nAuthorized Manager Signature")
                        .SetFontSize(10)
                        .SetFontColor(secondaryColor))
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT);

                footerTable.AddCell(hrSignature);
                footerTable.AddCell(managerSignature);
                document.Add(footerTable);

                document.Close();
                return stream.ToArray();
            }
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
