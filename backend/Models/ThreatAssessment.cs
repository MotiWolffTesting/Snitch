// ThreatAssessment represents an analysis of a person's threat level over a period.
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class ThreatAssessment
{
    public int Id { get; set; } // Primary key

    [Required]
    public int TargetId { get; set; } // Foreign key to the target person

    [Required]
    public DateOnly AssessmentDate { get; set; } // Date of the assessment

    // Individual threat components
    public decimal FrequencyScore { get; set; }
    public decimal VelocityScore { get; set; }
    public decimal NetworkExposureScore { get; set; }
    public decimal SentimentThreatScore { get; set; }
    public decimal TemporalClusteringScore { get; set; }

    // Composite metrics
    public decimal CompositeThreatScore { get; set; }
    public decimal EscalationProbability { get; set; }
    public ThreatTrend ThreatTrend { get; set; } = ThreatTrend.STABLE;

    // Analysis period
    public int AnalysisPeriodDays { get; set; } = 30;
    public int TotalReportsInPeriod { get; set; }
    public int UniqueReportersInPeriod { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // When the assessment was created

    // Navigation properties
    public Person Target { get; set; } = null!;
}

// Enum for threat trend
public enum ThreatTrend
{
    RISING,
    FALLING,
    STABLE,
    VOLATILE
}