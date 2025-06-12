using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class PersonService : IPersonService
{
    private readonly SnitchDbContext _context;
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<PersonService> _logger;

    public PersonService(
        SnitchDbContext context,
        IAnalyticsService analyticsService,
        ILogger<PersonService> logger)
    {
        _context = context;
        _analyticsService = analyticsService;
        _logger = logger;
    }

    public async Task<Person> CreatePersonAsync(Person person)
    {
        person.CreatedAt = DateTime.UtcNow;
        person.UpdatedAt = DateTime.UtcNow;
        person.RiskLevel = RiskLevel.LOW;

        _context.People.Add(person);
        await _context.SaveChangesAsync();

        return person;
    }

    public async Task<Person?> GetPersonAsync(int id)
    {
        return await _context.People
            .Include(p => p.ReportsMade)
            .Include(p => p.ReportsReceived)
            .Include(p => p.Alerts)
            .Include(p => p.ThreatAssessments)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Person>> GetPeopleAsync(string? searchTerm = null, RiskLevel? riskLevel = null)
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

        return await query
            .OrderByDescending(p => p.ReportsReceived.Count)
            .ToListAsync();
    }

    public async Task<Person> UpdatePersonAsync(int id, Person person)
    {
        if (id != person.Id)
            throw new ArgumentException("ID mismatch");

        var existingPerson = await _context.People.FindAsync(id);
        if (existingPerson == null)
            throw new KeyNotFoundException($"Person with ID {id} not found");

        existingPerson.Name = person.Name;
        existingPerson.SecretCode = person.SecretCode;
        existingPerson.UpdatedAt = DateTime.UtcNow;

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

    public async Task DeletePersonAsync(int id)
    {
        var person = await _context.People.FindAsync(id);
        if (person == null)
            throw new KeyNotFoundException($"Person with ID {id} not found");

        _context.People.Remove(person);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Report>> GetReportsMadeAsync(int id)
    {
        var person = await _context.People
            .Include(p => p.ReportsMade)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            throw new KeyNotFoundException($"Person with ID {id} not found");

        return person.ReportsMade.OrderByDescending(r => r.SubmittedAt);
    }

    public async Task<IEnumerable<Report>> GetReportsReceivedAsync(int id)
    {
        var person = await _context.People
            .Include(p => p.ReportsReceived)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            throw new KeyNotFoundException($"Person with ID {id} not found");

        return person.ReportsReceived.OrderByDescending(r => r.SubmittedAt);
    }

    public async Task<IEnumerable<Alert>> GetAlertsAsync(int id)
    {
        var person = await _context.People
            .Include(p => p.Alerts)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            throw new KeyNotFoundException($"Person with ID {id} not found");

        return person.Alerts.OrderByDescending(a => a.CreatedAt);
    }

    public async Task<IEnumerable<ThreatAssessment>> GetThreatAssessmentsAsync(int id)
    {
        var person = await _context.People
            .Include(p => p.ThreatAssessments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            throw new KeyNotFoundException($"Person with ID {id} not found");

        return person.ThreatAssessments.OrderByDescending(t => t.AssessmentDate);
    }

    public async Task<BehavioralPattern> GetBehaviorAsync(int id)
    {
        return await _analyticsService.AnalyzePersonBehaviorAsync(id);
    }

    public async Task<decimal> GetNetworkInfluenceAsync(int id)
    {
        return await _analyticsService.CalculatePersonNetworkInfluenceAsync(id);
    }

    public async Task<decimal> GetRiskLevelAsync(int id)
    {
        return await _analyticsService.CalculatePersonRiskLevelAsync(id);
    }
}