// PeopleController manages CRUD operations and analytics endpoints for Person entities.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly SnitchDbContext _context;
    private readonly IAnalyticsService _analyticsService;

    // Constructor injects the database context and analytics service
    public PeopleController(SnitchDbContext context, IAnalyticsService analyticsService)
    {
        _context = context;
        _analyticsService = analyticsService;
    }

    // DTO for person details with analytics fields
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SecretCode { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalReportsMade { get; set; }
        public int TotalReportsReceived { get; set; }
        public decimal ReportingConsistency { get; set; }
        public decimal NetworkCentrality { get; set; }
        public decimal InfluenceScore { get; set; }
        public decimal RecruitScore { get; set; }
        public decimal ThreatScore { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
    }

    // Create a new person
    [HttpPost]
    public async Task<ActionResult<Person>> CreatePerson(Person person)
    {
        person.CreatedAt = DateTime.UtcNow;
        person.UpdatedAt = DateTime.UtcNow;
        person.RiskLevel = RiskLevel.LOW;

        _context.People.Add(person);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
    }

    // Get a person by ID (with related data)
    [HttpGet("{id}")]
    public async Task<ActionResult<PersonDto>> GetPerson(int id)
    {
        var person = await _context.People
            .Include(p => p.ReportsMade)
            .Include(p => p.ReportsReceived)
            .Include(p => p.Alerts)
            .Include(p => p.ThreatAssessments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            return NotFound();

        var dto = new PersonDto
        {
            Id = person.Id,
            Name = person.Name,
            SecretCode = person.SecretCode,
            Role = person.Role.ToString(),
            CreatedAt = person.CreatedAt,
            UpdatedAt = person.UpdatedAt,
            TotalReportsMade = person.TotalReportsMade,
            TotalReportsReceived = person.TotalReportsReceived,
            ReportingConsistency = person.ReportingConsistency,
            NetworkCentrality = person.NetworkCentrality,
            InfluenceScore = person.InfluenceScore,
            RecruitScore = person.RecruitScore,
            ThreatScore = person.ThreatScore,
            RiskLevel = person.RiskLevel.ToString(),
        };

        return dto;
    }

    // Get all people, with optional search and risk level filter
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Person>>> GetPeople(
        [FromQuery] string? searchTerm,
        [FromQuery] RiskLevel? riskLevel)
    {
        var query = _context.People.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm));
        }

        if (riskLevel.HasValue)
        {
            query = query.Where(p => p.RiskLevel == riskLevel.Value);
        }

        var people = await query
            .OrderByDescending(p => p.ReportsReceived.Count)
            .ToListAsync();

        return people;
    }

    // Update a person
    [HttpPut("{id}")]
    public async Task<ActionResult<Person>> UpdatePerson(int id, Person person)
    {
        if (id != person.Id)
            return BadRequest("ID mismatch");

        var existingPerson = await _context.People.FindAsync(id);
        if (existingPerson == null)
            return NotFound();

        // Update properties
        existingPerson.Name = person.Name;
        existingPerson.SecretCode = person.SecretCode;
        existingPerson.UpdatedAt = DateTime.UtcNow;

        // Recalculate risk level
        var riskLevel = await _analyticsService.CalculatePersonRiskLevelAsync(id);
        existingPerson.RiskLevel = riskLevel switch
        {
            var r when r < 0.3m => RiskLevel.LOW,
            var r when r < 0.6m => RiskLevel.MEDIUM,
            var r when r < 0.8m => RiskLevel.HIGH,
            _ => RiskLevel.CRITICAL
        };

        await _context.SaveChangesAsync();
        return existingPerson;
    }

    // Delete a person
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePerson(int id)
    {
        var person = await _context.People.FindAsync(id);
        if (person == null)
            return NotFound();

        _context.People.Remove(person);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Get all reports made by a person
    [HttpGet("{id}/reports-made")]
    public async Task<ActionResult<IEnumerable<Report>>> GetReportsMade(int id)
    {
        var person = await _context.People
            .Include(p => p.ReportsMade)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            return NotFound();

        return person.ReportsMade.OrderByDescending(r => r.SubmittedAt).ToList();
    }

    // Get all reports received by a person
    [HttpGet("{id}/reports-received")]
    public async Task<ActionResult<IEnumerable<Report>>> GetReportsReceived(int id)
    {
        var person = await _context.People
            .Include(p => p.ReportsReceived)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            return NotFound();

        return person.ReportsReceived.OrderByDescending(r => r.SubmittedAt).ToList();
    }

    // Get all alerts for a person
    [HttpGet("{id}/alerts")]
    public async Task<ActionResult<IEnumerable<Alert>>> GetAlerts(int id)
    {
        var person = await _context.People
            .Include(p => p.Alerts)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            return NotFound();

        return person.Alerts.OrderByDescending(a => a.CreatedAt).ToList();
    }

    // Get all threat assessments for a person
    [HttpGet("{id}/threat-assessments")]
    public async Task<ActionResult<IEnumerable<ThreatAssessment>>> GetThreatAssessments(int id)
    {
        var person = await _context.People
            .Include(p => p.ThreatAssessments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            return NotFound();

        return person.ThreatAssessments.OrderByDescending(t => t.AssessmentDate).ToList();
    }

    // Get behavioral pattern for a person
    [HttpGet("{id}/behavior")]
    public async Task<ActionResult<BehavioralPattern>> GetBehavior(int id)
    {
        try
        {
            var behavior = await _analyticsService.AnalyzePersonBehaviorAsync(id);
            return behavior;
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get network influence for a person
    [HttpGet("{id}/network-influence")]
    public async Task<ActionResult<decimal>> GetNetworkInfluence(int id)
    {
        try
        {
            var influence = await _analyticsService.CalculatePersonNetworkInfluenceAsync(id);
            return influence;
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get risk level for a person
    [HttpGet("{id}/risk-level")]
    public async Task<ActionResult<decimal>> GetRiskLevel(int id)
    {
        try
        {
            var riskLevel = await _analyticsService.CalculatePersonRiskLevelAsync(id);
            return riskLevel;
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}