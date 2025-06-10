// Alert represents a system alert for a person, triggered by analytics or rules.
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Alert
{
    public int Id { get; set; } // Primary key

    [Required]
    public int TargetId { get; set; } // Foreign key to the target person

    [Required]
    public AlertType AlertType { get; set; } // Type of alert

    public AlertSeverity Severity { get; set; } = AlertSeverity.MEDIUM; // Severity of the alert
    public AlertStatus Status { get; set; } = AlertStatus.ACTIVE; // Status of the alert

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty; // Alert title

    [Required]
    public string Description { get; set; } = string.Empty; // Alert description

    [Required]
    public string TriggerCondition { get; set; } = string.Empty; // Condition that triggered the alert

    // Metrics that triggered the alert
    public decimal ThreatScore { get; set; }
    public decimal EscalationProbability { get; set; }
    public decimal ConfidenceLevel { get; set; } = 0.5m;

    // Time window analysis
    public DateTime? TimeWindowStart { get; set; }
    public DateTime? TimeWindowEnd { get; set; }
    public int ReportsInWindow { get; set; }
    public int UniqueReportersInWindow { get; set; }

    // Alert lifecycle
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // When the alert was created
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? AcknowledgedBy { get; set; }
    public string? ResolvedBy { get; set; }

    // Additional context
    public string? Metadata { get; set; }
    public string? RelatedReportIds { get; set; }

    // Navigation properties
    public Person Target { get; set; } = null!;
}

// Enum for alert type
public enum AlertType
{
    HIGH_FREQUENCY_TARGET,
    BURST_ACTIVITY,
    POTENTIAL_THREAT,
    RECRUIT_IDENTIFIED,
    NETWORK_ANOMALY
}

// Enum for alert severity
public enum AlertSeverity
{
    LOW,
    MEDIUM,
    HIGH,
    CRITICAL
}

// Enum for alert status
public enum AlertStatus
{
    ACTIVE,
    ACKNOWLEDGED,
    RESOLVED,
    FALSE_POSITIVE
}