// ReportsController manages CRUD operations and analytics endpoints for Report entities.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using backend.Models;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    // Constructor injects the report service
    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // Create a new report
    [HttpPost]
    public async Task<ActionResult<Report>> CreateReport(Report report)
    {
        try
        {
            var createdReport = await _reportService.CreateReportAsync(report);
            return CreatedAtAction(nameof(GetReport), new { id = createdReport.Id }, createdReport);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Get a report by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Report>> GetReport(int id)
    {
        var report = await _reportService.GetReportAsync(id);
        if (report == null)
            return NotFound();

        return report;
    }

    // Get all reports made by a specific reporter
    [HttpGet("reporter/{reporterId}")]
    public async Task<ActionResult<IEnumerable<Report>>> GetReportsByReporter(int reporterId)
    {
        var reports = await _reportService.GetReportsByReporterIdAsync(reporterId);
        return Ok(reports);
    }

    // Get all reports targeting a specific person
    [HttpGet("target/{targetId}")]
    public async Task<ActionResult<IEnumerable<Report>>> GetReportsByTarget(int targetId)
    {
        var reports = await _reportService.GetReportsByTargetIdAsync(targetId);
        return Ok(reports);
    }

    // Get the most recent reports
    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<Report>>> GetRecentReports([FromQuery] int count = 10)
    {
        var reports = await _reportService.GetRecentReportsAsync(count);
        return Ok(reports);
    }

    // Update a report
    [HttpPut("{id}")]
    public async Task<ActionResult<Report>> UpdateReport(int id, Report report)
    {
        if (id != report.Id)
            return BadRequest("ID mismatch");

        try
        {
            var updatedReport = await _reportService.UpdateReportAsync(report);
            return Ok(updatedReport);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Delete a report
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteReport(int id)
    {
        try
        {
            await _reportService.DeleteReportAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Search reports by a search term
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Report>>> SearchReports([FromQuery] string term)
    {
        var reports = await _reportService.SearchReportsAsync(term);
        return Ok(reports);
    }

    // Get reports in a date range
    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<Report>>> GetReportsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var reports = await _reportService.GetReportsByDateRangeAsync(startDate, endDate);
        return Ok(reports);
    }

    // Get the quality score for a report
    [HttpGet("{id}/quality")]
    public async Task<ActionResult<decimal>> GetReportQualityScore(int id)
    {
        try
        {
            var score = await _reportService.CalculateReportQualityScoreAsync(id);
            return Ok(score);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Corroborate a report
    [HttpPost("{id}/corroborate")]
    public async Task<ActionResult<bool>> CorroborateReport(int id)
    {
        try
        {
            var isCorroborated = await _reportService.CorroborateReportAsync(id);
            return Ok(isCorroborated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get all unprocessed reports
    [HttpGet("unprocessed")]
    public async Task<ActionResult<IEnumerable<Report>>> GetUnprocessedReports()
    {
        var reports = await _reportService.GetUnprocessedReportsAsync();
        return Ok(reports);
    }

    // Mark a report as processed
    [HttpPost("{id}/process")]
    public async Task<ActionResult> ProcessReport(int id)
    {
        try
        {
            await _reportService.ProcessReportAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get all reports by source type
    [HttpGet("source-type/{sourceType}")]
    public async Task<ActionResult<IEnumerable<Report>>> GetReportsBySourceType(SourceType sourceType)
    {
        var reports = await _reportService.GetReportsBySourceTypeAsync(sourceType);
        return Ok(reports);
    }

    // Import reports from CSV
    [HttpPost("import-csv")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<IEnumerable<Report>>> ImportReportsFromCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest("File must be a CSV");

        try
        {
            using var stream = file.OpenReadStream();
            var batchId = Guid.NewGuid().ToString();
            var reports = await _reportService.ImportReportsFromCsvAsync(stream, batchId);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error importing CSV: {ex.Message}");
        }
    }
}