// IAlertService defines the contract for alert management services.
using backend.Models;

namespace backend.Services;

public interface IAlertService
{
    // Create a new alert
    Task<Alert> CreateAlertAsync(Alert alert);

    // Get an alert by ID
    Task<Alert?> GetAlertAsync(int id);

    // Get all alerts with optional filtering
    Task<IEnumerable<Alert>> GetAlertsAsync(
        int? targetId = null,
        AlertType? alertType = null,
        AlertSeverity? severity = null,
        AlertStatus? status = null);

    // Acknowledge an alert
    Task<Alert> AcknowledgeAlertAsync(int id, string acknowledgedBy);

    // Resolve an alert
    Task<Alert> ResolveAlertAsync(int id, string resolvedBy);

    // Mark an alert as false positive
    Task<Alert> MarkAlertAsFalsePositiveAsync(int id, string resolvedBy);

    // Delete an alert
    Task DeleteAlertAsync(int id);

    Task<IEnumerable<Alert>> GetActiveAlertsAsync();
    Task<IEnumerable<Alert>> GetAlertsByTargetIdAsync(int targetId);
    Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(AlertSeverity severity);
    Task<Alert> UpdateAlertAsync(Alert alert);
    Task<IEnumerable<Alert>> GetAlertsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Alert>> GetAlertsByTypeAsync(AlertType type);
    Task<decimal> CalculateAlertThreatScoreAsync(int alertId);
    Task<bool> ShouldEscalateAlertAsync(int alertId);
    Task<IEnumerable<Alert>> GetUnresolvedAlertsAsync();
    Task<IEnumerable<Alert>> GetAlertsByStatusAsync(AlertStatus status);
}