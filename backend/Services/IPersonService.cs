using backend.Models;

namespace backend.Services;

public interface IPersonService
{
    Task<Person> CreatePersonAsync(Person person);
    Task<Person?> GetPersonAsync(int id);
    Task<IEnumerable<Person>> GetPeopleAsync(string? searchTerm = null, RiskLevel? riskLevel = null);
    Task<Person> UpdatePersonAsync(int id, Person person);
    Task DeletePersonAsync(int id);
    Task<IEnumerable<Report>> GetReportsMadeAsync(int id);
    Task<IEnumerable<Report>> GetReportsReceivedAsync(int id);
    Task<IEnumerable<Alert>> GetAlertsAsync(int id);
    Task<IEnumerable<ThreatAssessment>> GetThreatAssessmentsAsync(int id);
    Task<BehavioralPattern> GetBehaviorAsync(int id);
    Task<decimal> GetNetworkInfluenceAsync(int id);
    Task<decimal> GetRiskLevelAsync(int id);
}