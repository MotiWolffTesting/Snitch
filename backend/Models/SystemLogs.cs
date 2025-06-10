// SystemLog represents a log entry for system events and errors.
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class SystemLog
{
    public int Id { get; set; } // Primary key

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow; // When the log was created

    [Required]
    public LogLevel Level { get; set; } // Severity level of the log

    [Required]
    [MaxLength(255)]
    public string Component { get; set; } = null!; // Component that generated the log

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = null!; // Log message

    public string? Details { get; set; } // Optional details
    public string? UserId { get; set; } // Optional user ID
    public string? IpAddress { get; set; } // Optional IP address
    public string? RequestPath { get; set; } // Optional request path
    public string? Exception { get; set; } // Optional exception details
}

// Enum for log severity levels
public enum LogLevel
{
    DEBUG,
    INFO,
    WARNING,
    ERROR,
    CRITICAL
}