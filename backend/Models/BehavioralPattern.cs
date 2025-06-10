// BehavioralPattern represents behavioral analytics for a person.
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace backend.Models;

public class BehavioralPattern
{
    public int Id { get; set; } // Primary key

    [Required]
    public int PersonId { get; set; } // Foreign key to the person

    // Temporal behavior
    public string? PreferredHours { get; set; } // JSON array of preferred reporting hours
    public decimal AvgReportsPerDay { get; set; }
    public decimal AvgTimeBetweenReportsHours { get; set; }
    public ReportingPattern ReportingPattern { get; set; } = ReportingPattern.IRREGULAR;

    // Content analysis
    public decimal AvgSentiment { get; set; }
    public decimal VocabularyDiversity { get; set; }
    public decimal TruthfulnessIndicator { get; set; } = 0.5m;

    // Quality metrics
    public decimal AvgInformationDensity { get; set; }
    public decimal CorroborationRate { get; set; }

    // Network behavior
    public int UniqueTargetsReported { get; set; }
    public decimal NetworkInfluence { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow; // Last update time

    // Navigation properties
    public Person Person { get; set; } = null!;

    // Helper methods for JSON properties
    public List<int> GetPreferredHours()
    {
        if (string.IsNullOrEmpty(PreferredHours))
            return new List<int>();
        return JsonSerializer.Deserialize<List<int>>(PreferredHours) ?? new List<int>();
    }

    public void SetPreferredHours(List<int> hours)
    {
        PreferredHours = JsonSerializer.Serialize(hours);
    }
}

// Enum for reporting pattern
public enum ReportingPattern
{
    REGULAR,
    BURST,
    IRREGULAR,
    DECLINING
}