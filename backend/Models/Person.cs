// Person represents an individual in the intelligence system (agent, target, etc.).
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Person
{
    public int Id { get; set; } // Primary key

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty; // Person's name

    [Required]
    [MaxLength(20)]
    public string SecretCode { get; set; } = string.Empty; // Unique code for the person

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // When the person was created
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Last update time

    // Cached analytics
    public int TotalReportsMade { get; set; }
    public int TotalReportsReceived { get; set; }
    public decimal AvgReportLength { get; set; }
    public DateTime? LastReportAt { get; set; }

    // Intelligence status
    public bool IsPotentialRecruit { get; set; }
    public bool IsHighRiskTarget { get; set; }
    public RiskLevel RiskLevel { get; set; } = RiskLevel.LOW;
    public decimal RecruitScore { get; set; }
    public decimal ThreatScore { get; set; }

    // Behavioral metrics
    public decimal ReportingConsistency { get; set; }
    public decimal NetworkCentrality { get; set; }
    public decimal InfluenceScore { get; set; }

    // Navigation properties
    public ICollection<Report> ReportsMade { get; set; } = new List<Report>();
    public ICollection<Report> ReportsReceived { get; set; } = new List<Report>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    public ICollection<IntelligenceNetwork> NetworkConnectionsA { get; set; } = new List<IntelligenceNetwork>();
    public ICollection<IntelligenceNetwork> NetworkConnectionsB { get; set; } = new List<IntelligenceNetwork>();
    public BehavioralPattern? BehavioralPattern { get; set; }
    public ICollection<ThreatAssessment> ThreatAssessments { get; set; } = new List<ThreatAssessment>();

    public PersonRole Role { get; set; } = PersonRole.Civilian; // The role of the person
}

// Enum for risk level
public enum RiskLevel
{
    LOW,
    MEDIUM,
    HIGH,
    CRITICAL
}

// Enum for the role of a person
public enum PersonRole
{
    Agent,
    Terrorist,
    Intel,
    Civilian
}