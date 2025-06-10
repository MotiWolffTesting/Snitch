// IAnalyticsService defines the contract for intelligence analytics services.
using backend.Models;

namespace backend.Services;

public interface IAnalyticsService
{
    // Process a report for analytics
    Task<ReportAnalytics> ProcessReportAsync(Report report);

    // Get analytics for a person
    Task<PersonAnalytics> GetPersonAnalyticsAsync(int personId);

    // Get network analytics for a person
    Task<NetworkAnalytics> GetNetworkAnalyticsAsync(int personId);

    // Person analytics
    Task<decimal> CalculatePersonRiskLevelAsync(int personId);
    Task<BehavioralPattern> AnalyzePersonBehaviorAsync(int personId);
    Task<decimal> CalculatePersonNetworkInfluenceAsync(int personId);
    Task<IEnumerable<Person>> GetHighRiskPersonsAsync();
    Task<IEnumerable<Person>> GetPersonsByRiskLevelAsync(RiskLevel riskLevel);

    // Network analytics
    Task<IntelligenceNetwork> AnalyzeConnectionAsync(int personAId, int personBId);
    Task<IEnumerable<IntelligenceNetwork>> GetPersonConnectionsAsync(int personId);
    Task<decimal> CalculateNetworkDensityAsync();
    Task<IEnumerable<Person>> GetKeyInfluencersAsync();

    // Threat assessment
    Task<ThreatAssessment> GenerateThreatAssessmentAsync(int targetId);
    Task<IEnumerable<ThreatAssessment>> GetRecentThreatAssessmentsAsync(int count = 10);
    Task<decimal> CalculateThreatScoreAsync(int targetId);
    Task<IEnumerable<Person>> GetHighThreatTargetsAsync();

    // Temporal analysis
    Task<Dictionary<int, int>> GetReportingHourDistributionAsync();
    Task<Dictionary<DayOfWeek, int>> GetReportingDayDistributionAsync();
    Task<decimal> CalculateTemporalClusteringScoreAsync(int personId);

    // Quality metrics
    Task<decimal> CalculateReportQualityScoreAsync(int reportId);
    Task<decimal> CalculateInformationDensityAsync(string text);
    Task<decimal> CalculateSentimentScoreAsync(string text);
    Task<decimal> CalculateVocabularyDiversityAsync(string text);

    // System metrics
    Task<Dictionary<string, int>> GetSystemMetricsAsync();
    Task<IEnumerable<SystemLog>> GetRecentSystemLogsAsync(int count = 100);
    Task<Dictionary<string, int>> GetAlertDistributionAsync();
}