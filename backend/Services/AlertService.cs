using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services;

// AlertService handles alert creation, retrieval, and management.
public class AlertService : IAlertService
{
    private readonly SnitchDbContext _context;
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AlertService> _logger;

    public AlertService(SnitchDbContext context, IAnalyticsService analyticsService, ILogger<AlertService> logger)
    {
        _context = context;
        _analyticsService = analyticsService;
        _logger = logger;
    }

    // Create a new alert
    public async Task<Alert> CreateAlertAsync(Alert alert)
    {
        try
        {
            // Validate target exists
            var target = await _context.People.FindAsync(alert.TargetId);
            if (target == null)
                throw new ArgumentException("Target not found");

            // Set alert metadata
            alert.CreatedAt = DateTime.UtcNow;
            alert.Status = AlertStatus.ACTIVE;

            // Calculate threat score and escalation probability
            alert.ThreatScore = await _analyticsService.CalculateThreatScoreAsync(alert.TargetId);
            alert.EscalationProbability = await CalculateEscalationProbabilityAsync(alert);

            // Save alert
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            return alert;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating alert");
            throw;
        }
    }

    // Get an alert by ID
    public async Task<Alert?> GetAlertByIdAsync(int id)
    {
        return await _context.Alerts
            .Include(a => a.Target)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    // Get all alerts with optional filtering
    public async Task<IEnumerable<Alert>> GetAlertsAsync(
        int? targetId = null,
        AlertType? alertType = null,
        AlertSeverity? severity = null,
        AlertStatus? status = null)
    {
        var query = _context.Alerts
            .Include(a => a.Target)
            .AsQueryable();

        if (targetId.HasValue)
            query = query.Where(a => a.TargetId == targetId.Value);

        if (alertType.HasValue)
            query = query.Where(a => a.AlertType == alertType.Value);

        if (severity.HasValue)
            query = query.Where(a => a.Severity == severity.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        return await query.ToListAsync();
    }

    // Acknowledge an alert
    public async Task<Alert> AcknowledgeAlertAsync(int id, string acknowledgedBy)
    {
        var alert = await _context.Alerts.FindAsync(id);
        if (alert == null)
            throw new ArgumentException("Alert not found");

        alert.Status = AlertStatus.ACKNOWLEDGED;
        alert.AcknowledgedAt = DateTime.UtcNow;
        alert.AcknowledgedBy = acknowledgedBy;

        await _context.SaveChangesAsync();
        return alert;
    }

    // Resolve an alert
    public async Task<Alert> ResolveAlertAsync(int id, string resolvedBy)
    {
        var alert = await _context.Alerts.FindAsync(id);
        if (alert == null)
            throw new ArgumentException("Alert not found");

        alert.Status = AlertStatus.RESOLVED;
        alert.ResolvedAt = DateTime.UtcNow;
        alert.ResolvedBy = resolvedBy;

        await _context.SaveChangesAsync();
        return alert;
    }

    // Mark an alert as false positive
    public async Task<Alert> MarkAlertAsFalsePositiveAsync(int id, string resolvedBy)
    {
        var alert = await _context.Alerts.FindAsync(id);
        if (alert == null)
            throw new ArgumentException("Alert not found");

        alert.Status = AlertStatus.FALSE_POSITIVE;
        alert.ResolvedAt = DateTime.UtcNow;
        alert.ResolvedBy = resolvedBy;

        await _context.SaveChangesAsync();
        return alert;
    }

    // Delete an alert
    public async Task DeleteAlertAsync(int id)
    {
        var alert = await _context.Alerts.FindAsync(id);
        if (alert == null)
            throw new ArgumentException("Alert not found");

        _context.Alerts.Remove(alert);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Alert>> GetActiveAlertsAsync()
    {
        return await _context.Alerts
            .Include(a => a.Target)
            .Where(a => a.Status == AlertStatus.ACTIVE || a.Status == AlertStatus.ACKNOWLEDGED)
            .OrderByDescending(a => a.ThreatScore)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAlertsByTargetIdAsync(int targetId)
    {
        return await _context.Alerts
            .Include(a => a.Target)
            .Where(a => a.TargetId == targetId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(AlertSeverity severity)
    {
        return await _context.Alerts
            .Include(a => a.Target)
            .Where(a => a.Severity == severity)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAlertsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Alerts
            .Include(a => a.Target)
            .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAlertsByTypeAsync(AlertType type)
    {
        return await _context.Alerts
            .Include(a => a.Target)
            .Where(a => a.AlertType == type)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> CalculateAlertThreatScoreAsync(int alertId)
    {
        var alert = await _context.Alerts.FindAsync(alertId);
        if (alert == null)
            throw new KeyNotFoundException($"Alert with ID {alertId} not found");

        return await _analyticsService.CalculateThreatScoreAsync(alert.TargetId);
    }

    public async Task<bool> ShouldEscalateAlertAsync(int alertId)
    {
        var alert = await _context.Alerts.FindAsync(alertId);
        if (alert == null)
            throw new KeyNotFoundException($"Alert with ID {alertId} not found");

        return alert.EscalationProbability > 0.7m; // Threshold for escalation
    }

    public async Task<IEnumerable<Alert>> GetUnresolvedAlertsAsync()
    {
        return await _context.Alerts
            .Include(a => a.Target)
            .Where(a => a.Status != AlertStatus.RESOLVED)
            .OrderByDescending(a => a.ThreatScore)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAlertsByStatusAsync(AlertStatus status)
    {
        return await _context.Alerts
            .Include(a => a.Target)
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    private async Task<decimal> CalculateEscalationProbabilityAsync(Alert alert)
    {
        // Get recent reports about the target
        var recentReports = await _context.Reports
            .Where(r => r.TargetId == alert.TargetId &&
                       r.SubmittedAt >= DateTime.UtcNow.AddDays(-7))
            .ToListAsync();

        if (!recentReports.Any())
            return 0.0m;

        // Calculate escalation probability based on:
        // 1. Number of recent reports
        // 2. Average threat score
        // 3. Temporal clustering
        var reportCount = recentReports.Count;
        var avgThreatScore = recentReports.Average(r => r.QualityScore);
        var temporalClustering = await _analyticsService.CalculateTemporalClusteringScoreAsync(alert.TargetId);

        // Weighted combination of factors
        return (reportCount * 0.3m + avgThreatScore * 0.4m + temporalClustering * 0.3m) / 3;
    }

    public async Task<IEnumerable<Alert>> GetAlertsAsync()
    {
        return await _context.Alerts
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Alert?> GetAlertAsync(int id)
    {
        return await _context.Alerts.FindAsync(id);
    }

    public async Task<Alert> UpdateAlertAsync(Alert alert)
    {
        var existingAlert = await _context.Alerts.FindAsync(alert.Id);
        if (existingAlert == null)
            throw new KeyNotFoundException($"Alert with ID {alert.Id} not found");

        _context.Entry(existingAlert).CurrentValues.SetValues(alert);
        await _context.SaveChangesAsync();
        return existingAlert;
    }
}