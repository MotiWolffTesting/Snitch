using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

// This class represents our database context - it's the main class that coordinates Entity Framework functionality
// for our database. It inherits from DbContext which is the primary class that coordinates Entity Framework functionality
public class SnitchDbContext : DbContext
{
    // Constructor that takes database options and passes them to the base DbContext class
    // This is required for dependency injection to work
    public SnitchDbContext(DbContextOptions<SnitchDbContext> options) : base(options)
    {
    }

    // These properties represent our database tables
    // Each DbSet<T> represents a table in the database where T is the model class
    public DbSet<Person> People { get; set; } = null!; // Table for storing people (reporters and targets)
    public DbSet<Report> Reports { get; set; } = null!; // Table for storing intelligence reports
    public DbSet<Alert> Alerts { get; set; } = null!; // Table for storing alerts
    public DbSet<IntelligenceNetwork> IntelligenceNetworks { get; set; } = null!; // Table for storing connections between people
    public DbSet<BehavioralPattern> BehavioralPatterns { get; set; } = null!; // Table for storing behavioral patterns
    public DbSet<ThreatAssessment> ThreatAssessments { get; set; } = null!; // Table for storing threat assessments
    public DbSet<SystemLog> SystemLogs { get; set; } = null!; // Table for storing system logs
    public DbSet<User> Users { get; set; }
    public DbSet<AnalysisPreference> AnalysisPreferences { get; set; } = null!;

    // This method is called when the model is being created
    // We use it to configure our database schema and relationships
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Person table
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SecretCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RiskLevel).IsRequired().HasDefaultValue(RiskLevel.LOW);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // Configure the Report table
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReportText).IsRequired().HasMaxLength(4000);
            entity.Property(e => e.ReporterId).IsRequired();
            entity.Property(e => e.TargetId).IsRequired();
            entity.HasOne(e => e.Reporter)
                .WithMany(p => p.ReportsMade)
                .HasForeignKey(e => e.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Target)
                .WithMany(p => p.ReportsReceived)
                .HasForeignKey(e => e.TargetId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable(t => t.HasCheckConstraint("CK_Reports_DifferentPeople", "ReporterId != TargetId"));
            entity.Property(e => e.SubmittedAt).IsRequired();
        });

        // Configure the Alert table
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AlertType).IsRequired();
            entity.Property(e => e.Severity).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasDefaultValue(AlertStatus.ACTIVE);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.TargetId).IsRequired();
            entity.HasOne(e => e.Target)
                .WithMany(p => p.Alerts)
                .HasForeignKey(e => e.TargetId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure the IntelligenceNetwork table with explicit relationships
        modelBuilder.Entity<IntelligenceNetwork>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.PersonA)
                .WithMany()
                .HasForeignKey(e => e.PersonAId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.PersonB)
                .WithMany()
                .HasForeignKey(e => e.PersonBId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable(t => t.HasCheckConstraint("CK_IntelligenceNetworks_DifferentPeople", "PersonAId != PersonBId"));
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure the BehavioralPattern table
        modelBuilder.Entity<BehavioralPattern>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Person)
                .WithMany()
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.LastUpdated).IsRequired();
        });

        // Configure the ThreatAssessment table
        modelBuilder.Entity<ThreatAssessment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TargetId).IsRequired();
            entity.Property(e => e.AssessmentDate).IsRequired();
            entity.HasOne(e => e.Target)
                .WithMany(p => p.ThreatAssessments)
                .HasForeignKey(e => e.TargetId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure the SystemLog table
        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.Component).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Timestamp).IsRequired();
        });
    }
}