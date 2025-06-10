using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using CsvHelper;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace backend.Services;

public class ReportService : IReportService
{
    private readonly SnitchDbContext _context;
    private readonly IAnalyticsService _analyticsService;
    private readonly IAlertService _alertService;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        SnitchDbContext context,
        IAnalyticsService analyticsService,
        IAlertService alertService,
        ILogger<ReportService> logger)
    {
        _context = context;
        _analyticsService = analyticsService;
        _alertService = alertService;
        _logger = logger;
    }

    // Create a new report and process it for analytics
    public async Task<Report> CreateReportAsync(Report report)
    {
        // Validate reporter and target exist
        var reporter = await _context.People.FindAsync(report.ReporterId);
        var target = await _context.People.FindAsync(report.TargetId);

        if (reporter == null || target == null)
            throw new ArgumentException("Reporter or target not found");

        // Calculate basic metrics
        report.CharCount = report.ReportText.Length;
        report.WordCount = report.ReportText.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        report.SubmittedAt = DateTime.UtcNow;

        // Process report for analytics
        var analytics = await _analyticsService.ProcessReportAsync(report);
        report.QualityScore = analytics.QualityScore;
        report.InformationDensity = analytics.InformationDensity;
        report.SpecificityScore = analytics.SpecificityScore;
        report.SentimentScore = analytics.SentimentScore;

        // Update cached analytics for reporter and target
        await UpdateCachedAnalyticsAsync(reporter.Id);
        await UpdateCachedAnalyticsAsync(target.Id);

        // Check for high-risk reports and create alerts
        if (analytics.RiskLevel == RiskLevel.HIGH || analytics.RiskLevel == RiskLevel.CRITICAL)
        {
            await _alertService.CreateAlertAsync(new Alert
            {
                TargetId = target.Id,
                AlertType = AlertType.POTENTIAL_THREAT,
                Severity = analytics.RiskLevel == RiskLevel.CRITICAL ? AlertSeverity.CRITICAL : AlertSeverity.HIGH,
                Title = $"High Risk Report: {target.Name}",
                Description = $"Report from {reporter.Name} indicates high risk level: {analytics.RiskLevel}",
                TriggerCondition = $"Risk Level: {analytics.RiskLevel}",
                ThreatScore = analytics.ThreatScore,
                ConfidenceLevel = analytics.ConfidenceLevel
            });
        }

        // Save report
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        return report;
    }

    // Get a report by ID
    public async Task<Report?> GetReportAsync(int id)
    {
        return await _context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Target)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    // Get all reports with optional filtering
    public async Task<IEnumerable<Report>> GetReportsAsync(
        int? reporterId = null,
        int? targetId = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Target)
            .AsQueryable();

        if (reporterId.HasValue)
            query = query.Where(r => r.ReporterId == reporterId.Value);

        if (targetId.HasValue)
            query = query.Where(r => r.TargetId == targetId.Value);

        if (startDate.HasValue)
            query = query.Where(r => r.SubmittedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(r => r.SubmittedAt <= endDate.Value);

        return await query.ToListAsync();
    }

    // Update cached analytics for a person
    private async Task UpdateCachedAnalyticsAsync(int personId)
    {
        var person = await _context.People
            .Include(p => p.ReportsMade)
            .Include(p => p.ReportsReceived)
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (person == null) return;

        // Update reports made analytics
        person.TotalReportsMade = person.ReportsMade.Count;
        person.AvgReportLength = person.ReportsMade.Any()
            ? (decimal)person.ReportsMade.Average(r => r.WordCount)
            : 0;
        person.LastReportAt = person.ReportsMade
            .OrderByDescending(r => r.SubmittedAt)
            .FirstOrDefault()?.SubmittedAt;

        // Update reports received analytics
        person.TotalReportsReceived = person.ReportsReceived.Count;

        await _context.SaveChangesAsync();
    }

    // Import reports from a CSV file
    public async Task<IEnumerable<Report>> ImportReportsFromCsvAsync(Stream csvStream, string batchId)
    {
        var reports = new List<Report>();
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Read CSV records
        var records = csv.GetRecords<dynamic>();
        foreach (var record in records)
        {
            try
            {
                // Parse CSV fields
                var reporterId = int.Parse(record.ReporterId.ToString());
                var targetId = int.Parse(record.TargetId.ToString());
                var reportText = record.ReportText.ToString();

                // Create report
                var report = new Report
                {
                    ReporterId = reporterId,
                    TargetId = targetId,
                    ReportText = reportText,
                    SourceType = SourceType.CSV_IMPORT,
                    ImportBatchId = batchId
                };

                // Process report
                var processedReport = await CreateReportAsync(report);
                reports.Add(processedReport);
            }
            catch (Exception ex)
            {
                var recordJson = JsonSerializer.Serialize(record);
                LoggerExtensions.LogError(_logger, ex, "Error importing report from CSV: {Record}", recordJson);
            }
        }

        return reports;
    }

    // Get reports by batch ID
    public async Task<IEnumerable<Report>> GetReportsByBatchAsync(string batchId)
    {
        return await _context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Target)
            .Where(r => r.ImportBatchId == batchId)
            .ToListAsync();
    }

    // Delete a report
    public async Task DeleteReportAsync(int id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null)
            throw new ArgumentException("Report not found");

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();

        // Update cached analytics for reporter and target
        await UpdateCachedAnalyticsAsync(report.ReporterId);
        await UpdateCachedAnalyticsAsync(report.TargetId);
    }

    public async Task<IEnumerable<Report>> GetReportsByReporterIdAsync(int reporterId)
    {
        return await _context.Reports
            .Include(r => r.Target)
            .Where(r => r.ReporterId == reporterId)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetReportsByTargetIdAsync(int targetId)
    {
        return await _context.Reports
            .Include(r => r.Reporter)
            .Where(r => r.TargetId == targetId)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetRecentReportsAsync(int count = 10)
    {
        return await _context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Target)
            .OrderByDescending(r => r.SubmittedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Report> UpdateReportAsync(Report report)
    {
        var existingReport = await _context.Reports.FindAsync(report.Id);
        if (existingReport == null)
            throw new KeyNotFoundException($"Report with ID {report.Id} not found");

        // Update properties
        existingReport.ReportText = report.ReportText;
        existingReport.CharCount = report.ReportText.Length;
        existingReport.WordCount = report.ReportText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        // Recalculate quality metrics
        existingReport.QualityScore = await _analyticsService.CalculateReportQualityScoreAsync(report.Id);
        existingReport.InformationDensity = await _analyticsService.CalculateInformationDensityAsync(report.ReportText);
        existingReport.SentimentScore = await _analyticsService.CalculateSentimentScoreAsync(report.ReportText);
        existingReport.SpecificityScore = await _analyticsService.CalculateVocabularyDiversityAsync(report.ReportText);

        await _context.SaveChangesAsync();
        return existingReport;
    }

    public async Task<IEnumerable<Report>> SearchReportsAsync(string searchTerm)
    {
        return await _context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Target)
            .Where(r => r.ReportText.Contains(searchTerm))
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetReportsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Target)
            .Where(r => r.SubmittedAt >= startDate && r.SubmittedAt <= endDate)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }

    public async Task<decimal> CalculateReportQualityScoreAsync(int reportId)
    {
        return await _analyticsService.CalculateReportQualityScoreAsync(reportId);
    }

    public async Task<bool> CorroborateReportAsync(int reportId)
    {
        var report = await _context.Reports.FindAsync(reportId);
        if (report == null)
            throw new KeyNotFoundException($"Report with ID {reportId} not found");

        // Check for similar reports about the same target
        var similarReports = await _context.Reports
            .Where(r => r.TargetId == report.TargetId && r.Id != reportId)
            .ToListAsync();

        // Implement corroboration logic here
        // For now, we'll just mark it as corroborated if there are any similar reports
        report.IsCorroborated = similarReports.Any();
        await _context.SaveChangesAsync();
        return report.IsCorroborated;
    }

    public async Task<IEnumerable<Report>> GetUnprocessedReportsAsync()
    {
        return await _context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Target)
            .Where(r => !r.IsProcessed)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }

    public async Task ProcessReportAsync(int reportId)
    {
        var report = await _context.Reports.FindAsync(reportId);
        if (report == null)
            throw new KeyNotFoundException($"Report with ID {reportId} not found");

        report.IsProcessed = true;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Report>> GetReportsBySourceTypeAsync(SourceType sourceType)
    {
        return await _context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Target)
            .Where(r => r.SourceType == sourceType)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }
}