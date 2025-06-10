// IReportService defines the contract for report management services.
using backend.Models;

namespace backend.Services;

public interface IReportService
{
    // Create a new report
    Task<Report> CreateReportAsync(Report report);

    // Get a report by ID
    Task<Report?> GetReportAsync(int id);

    // Get all reports with optional filtering
    Task<IEnumerable<Report>> GetReportsAsync(
        int? reporterId = null,
        int? targetId = null,
        DateTime? startDate = null,
        DateTime? endDate = null);

    // Import reports from a CSV file
    Task<IEnumerable<Report>> ImportReportsFromCsvAsync(Stream csvStream, string batchId);

    // Get reports by batch ID
    Task<IEnumerable<Report>> GetReportsByBatchAsync(string batchId);

    // Delete a report
    Task DeleteReportAsync(int id);

    Task<IEnumerable<Report>> GetReportsByReporterIdAsync(int reporterId);
    Task<IEnumerable<Report>> GetReportsByTargetIdAsync(int targetId);
    Task<IEnumerable<Report>> GetRecentReportsAsync(int count = 10);
    Task<Report> UpdateReportAsync(Report report);
    Task<IEnumerable<Report>> SearchReportsAsync(string searchTerm);
    Task<IEnumerable<Report>> GetReportsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> CalculateReportQualityScoreAsync(int reportId);
    Task<bool> CorroborateReportAsync(int reportId);
    Task<IEnumerable<Report>> GetUnprocessedReportsAsync();
    Task ProcessReportAsync(int reportId);
    Task<IEnumerable<Report>> GetReportsBySourceTypeAsync(SourceType sourceType);
}