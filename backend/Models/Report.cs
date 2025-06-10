// Report represents an intelligence report submitted by a person about a target.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

public class Report
{
    public int Id { get; set; } // Primary key

    [Required]
    public int ReporterId { get; set; } // Foreign key to the reporter (Person)

    [Required]
    public int TargetId { get; set; } // Foreign key to the target (Person)

    [Required]
    public string ReportText { get; set; } = string.Empty; // The content of the report

    public int CharCount { get; set; } // Character count of the report
    public int WordCount { get; set; } // Word count of the report
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow; // When the report was submitted

    // Quality and classification metrics
    public decimal QualityScore { get; set; }
    public decimal InformationDensity { get; set; }
    public decimal SpecificityScore { get; set; }
    public decimal SentimentScore { get; set; }

    // Source and processing info
    public SourceType SourceType { get; set; } = SourceType.MANUAL; // How the report was submitted
    public string? ImportBatchId { get; set; } // Optional batch import ID
    public bool IsProcessed { get; set; } // Whether the report has been processed
    public bool IsCorroborated { get; set; } // Whether the report has been corroborated

    // Network analysis helpers
    [NotMapped]
    public int HourOfDay => SubmittedAt.Hour;

    [NotMapped]
    public int DayOfWeek => (int)SubmittedAt.DayOfWeek;

    // Navigation properties
    public Person? Reporter { get; set; }
    public Person? Target { get; set; }
}

// Enum for the source of the report
public enum SourceType
{
    MANUAL,
    CSV_IMPORT,
    API
}