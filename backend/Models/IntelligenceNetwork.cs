// IntelligenceNetwork represents a connection between two people in the network.
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class IntelligenceNetwork
{
    public int Id { get; set; } // Primary key

    [Required]
    public int PersonAId { get; set; } // Foreign key to person A

    [Required]
    public int PersonBId { get; set; } // Foreign key to person B

    public decimal ConnectionStrength { get; set; } // Strength of the connection
    public string ConnectionType { get; set; } = "REPORTED_TOGETHER"; // Type of connection
    public int SharedTargetsCount { get; set; } // Number of shared targets
    public decimal TemporalClusteringScore { get; set; } // Temporal clustering score
    public DateTime LastInteraction { get; set; } = DateTime.UtcNow; // Last interaction time
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // When the connection was created

    // Navigation properties
    public Person PersonA { get; set; } = null!;
    public Person PersonB { get; set; } = null!;
}