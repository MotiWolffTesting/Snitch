using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class AnalysisPreference
{
    [Key]
    public int Id { get; set; }

    [Required]
    public bool UseOpenAI { get; set; }

    [Required]
    public DateTime LastUpdated { get; set; }

    [Required]
    public string UpdatedBy { get; set; } = string.Empty;
}