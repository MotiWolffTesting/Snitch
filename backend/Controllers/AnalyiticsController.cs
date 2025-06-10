// AnalyticsController provides endpoints for analytics, metrics, and intelligence analysis.
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    // Constructor injects the analytics service
    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    // --- Person analytics ---

    // Get the risk level for a person
    [HttpGet("person/{personId}/risk-level")]
    public async Task<ActionResult<decimal>> GetPersonRiskLevel(int personId)
    {
        try
        {
            var riskLevel = await _analyticsService.CalculatePersonRiskLevelAsync(personId);
            return Ok(riskLevel);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get behavioral pattern for a person
    [HttpGet("person/{personId}/behavior")]
    public async Task<ActionResult<BehavioralPattern>> GetPersonBehavior(int personId)
    {
        try
        {
            var behavior = await _analyticsService.AnalyzePersonBehaviorAsync(personId);
            return Ok(behavior);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get network influence for a person
    [HttpGet("person/{personId}/network-influence")]
    public async Task<ActionResult<decimal>> GetPersonNetworkInfluence(int personId)
    {
        try
        {
            var influence = await _analyticsService.CalculatePersonNetworkInfluenceAsync(personId);
            return Ok(influence);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get all high risk persons
    [HttpGet("high-risk-persons")]
    public async Task<ActionResult<IEnumerable<Person>>> GetHighRiskPersons()
    {
        var persons = await _analyticsService.GetHighRiskPersonsAsync();
        return Ok(persons);
    }

    // Get persons by risk level
    [HttpGet("persons/risk-level/{riskLevel}")]
    public async Task<ActionResult<IEnumerable<Person>>> GetPersonsByRiskLevel(RiskLevel riskLevel)
    {
        var persons = await _analyticsService.GetPersonsByRiskLevelAsync(riskLevel);
        return Ok(persons);
    }

    // --- Network analytics ---

    // Analyze the connection between two people
    [HttpGet("network/connection")]
    public async Task<ActionResult<IntelligenceNetwork>> AnalyzeConnection(
        [FromQuery] int personAId,
        [FromQuery] int personBId)
    {
        try
        {
            var connection = await _analyticsService.AnalyzeConnectionAsync(personAId, personBId);
            return Ok(connection);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get all network connections for a person
    [HttpGet("network/person/{personId}/connections")]
    public async Task<ActionResult<IEnumerable<IntelligenceNetwork>>> GetPersonConnections(int personId)
    {
        try
        {
            var connections = await _analyticsService.GetPersonConnectionsAsync(personId);
            return Ok(connections);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get the overall network density
    [HttpGet("network/density")]
    public async Task<ActionResult<decimal>> GetNetworkDensity()
    {
        var density = await _analyticsService.CalculateNetworkDensityAsync();
        return Ok(density);
    }

    // Get key influencers in the network
    [HttpGet("network/key-influencers")]
    public async Task<ActionResult<IEnumerable<Person>>> GetKeyInfluencers()
    {
        var influencers = await _analyticsService.GetKeyInfluencersAsync();
        return Ok(influencers);
    }

    // --- Threat assessment ---

    // Generate a threat assessment for a target
    [HttpGet("threat-assessment/{targetId}")]
    public async Task<ActionResult<ThreatAssessment>> GenerateThreatAssessment(int targetId)
    {
        try
        {
            var assessment = await _analyticsService.GenerateThreatAssessmentAsync(targetId);
            return Ok(assessment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get recent threat assessments
    [HttpGet("threat-assessments/recent")]
    public async Task<ActionResult<IEnumerable<ThreatAssessment>>> GetRecentThreatAssessments(
        [FromQuery] int count = 10)
    {
        var assessments = await _analyticsService.GetRecentThreatAssessmentsAsync(count);
        return Ok(assessments);
    }

    // Get the threat score for a target
    [HttpGet("threat-score/{targetId}")]
    public async Task<ActionResult<decimal>> GetThreatScore(int targetId)
    {
        try
        {
            var score = await _analyticsService.CalculateThreatScoreAsync(targetId);
            return Ok(score);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get all high threat targets
    [HttpGet("high-threat-targets")]
    public async Task<ActionResult<IEnumerable<Person>>> GetHighThreatTargets()
    {
        var targets = await _analyticsService.GetHighThreatTargetsAsync();
        return Ok(targets);
    }

    // --- Temporal analysis ---

    // Get the distribution of reports by hour
    [HttpGet("temporal/hour-distribution")]
    public async Task<ActionResult<Dictionary<int, int>>> GetReportingHourDistribution()
    {
        var distribution = await _analyticsService.GetReportingHourDistributionAsync();
        return Ok(distribution);
    }

    // Get the distribution of reports by day of week
    [HttpGet("temporal/day-distribution")]
    public async Task<ActionResult<Dictionary<DayOfWeek, int>>> GetReportingDayDistribution()
    {
        var distribution = await _analyticsService.GetReportingDayDistributionAsync();
        return Ok(distribution);
    }

    // Get the temporal clustering score for a person
    [HttpGet("temporal/clustering/{personId}")]
    public async Task<ActionResult<decimal>> GetTemporalClusteringScore(int personId)
    {
        try
        {
            var score = await _analyticsService.CalculateTemporalClusteringScoreAsync(personId);
            return Ok(score);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // --- Quality metrics ---

    // Get the quality score for a report
    [HttpGet("quality/report/{reportId}")]
    public async Task<ActionResult<decimal>> GetReportQualityScore(int reportId)
    {
        try
        {
            var score = await _analyticsService.CalculateReportQualityScoreAsync(reportId);
            return Ok(score);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get the information density of a text
    [HttpGet("quality/information-density")]
    public async Task<ActionResult<decimal>> GetInformationDensity([FromQuery] string text)
    {
        var density = await _analyticsService.CalculateInformationDensityAsync(text);
        return Ok(density);
    }

    // Get the sentiment score of a text
    [HttpGet("quality/sentiment")]
    public async Task<ActionResult<decimal>> GetSentimentScore([FromQuery] string text)
    {
        var score = await _analyticsService.CalculateSentimentScoreAsync(text);
        return Ok(score);
    }

    // Get the vocabulary diversity of a text
    [HttpGet("quality/vocabulary-diversity")]
    public async Task<ActionResult<decimal>> GetVocabularyDiversity([FromQuery] string text)
    {
        var diversity = await _analyticsService.CalculateVocabularyDiversityAsync(text);
        return Ok(diversity);
    }

    // --- System metrics ---

    // Get system-wide metrics
    [HttpGet("system/metrics")]
    public async Task<ActionResult<Dictionary<string, int>>> GetSystemMetrics()
    {
        var metrics = await _analyticsService.GetSystemMetricsAsync();
        return Ok(metrics);
    }

    // Get recent system logs
    [HttpGet("system/logs")]
    public async Task<ActionResult<IEnumerable<SystemLog>>> GetRecentSystemLogs(
        [FromQuery] int count = 100)
    {
        var logs = await _analyticsService.GetRecentSystemLogsAsync(count);
        return Ok(logs);
    }

    // Get the distribution of alerts by type or status
    [HttpGet("system/alert-distribution")]
    public async Task<ActionResult<Dictionary<string, int>>> GetAlertDistribution()
    {
        var distribution = await _analyticsService.GetAlertDistributionAsync();
        return Ok(distribution);
    }
}