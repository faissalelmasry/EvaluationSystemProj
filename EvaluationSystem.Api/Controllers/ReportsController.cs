using EvaluationSystem.Application.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IEvaluationReportService _reportService;

        public ReportsController(IEvaluationReportService reportService)
        {
            _reportService = reportService;
        }

        // 1. GET: api/reports/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                var result = await _reportService.GetDashboardSummaryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. GET: api/reports/by-department
        [HttpGet("by-department")]
        public async Task<IActionResult> GetReportByDepartment()
        {
            try
            {
                var result = await _reportService.GetReportByDepartmentAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. GET: api/reports/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReportByUser(int userId)
        {
            try
            {
                var result = await _reportService.GetReportByUserAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4. GET: api/reports/completion-rate
        [HttpGet("completion-rate")]
        public async Task<IActionResult> GetCompletionRate()
        {
            try
            {
                var result = await _reportService.GetCompletionRateAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 5. GET: api/reports/top-scores
        [HttpGet("top-scores")]
        public async Task<IActionResult> GetTopScores()
        {
            try
            {
                var result = await _reportService.GetTopScoresAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        //6
        [HttpGet("assignment/{id}/pdf")]
        public async Task<IActionResult> DownloadEvaluationPdf(int id)
        {
            try
            {
                
                byte[] pdfBytes = await _reportService.GenerateEvaluationPdfAsync(id);

               
                string fileName = $"Evaluation_Report_{id}.pdf";

               
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
