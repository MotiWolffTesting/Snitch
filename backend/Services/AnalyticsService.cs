using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace backend.Services;

// AnalyticsService handles intelligence analytics for reports and people.
public class AnalyticsService : IAnalyticsService
{
    private readonly SnitchDbContext _context;
    private readonly OpenAIService _openAIService;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        SnitchDbContext context,
        OpenAIService openAIService,
        ILogger<AnalyticsService> logger)
    {
        _context = context;
        _openAIService = openAIService;
        _logger = logger;
    }

    // Process a report for analytics
    public async Task<ReportAnalytics> ProcessReportAsync(Report report)
    {
        try
        {
            var analysis = await _openAIService.AnalyzeTextAsync(report.ReportText);

            // Update target's risk level and recruit score
            var target = await _context.People.FindAsync(report.TargetId);
            if (target != null)
            {
                target.RiskLevel = Enum.TryParse<RiskLevel>(analysis.RiskLevel, true, out var parsedRiskLevel) ? parsedRiskLevel : RiskLevel.LOW;
                target.RecruitScore = (decimal)analysis.RecruitScore;
                target.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            // Calculate quality metrics
            var qualityScore = CalculateQualityScore(report);
            var informationDensity = CalculateInformationDensity(report);
            var specificityScore = CalculateSpecificityScore(report);
            var sentimentScore = CalculateSentimentScore(report);

            // Convert RiskLevel string to enum
            var riskLevelEnum = Enum.TryParse<RiskLevel>(analysis.RiskLevel, true, out var riskLevel) ? riskLevel : RiskLevel.LOW;
            // Convert RecruitScore to decimal
            var recruitScoreDecimal = (decimal)analysis.RecruitScore;
            // Optionally, parse ConfidenceLevel string to a decimal or use as string elsewhere

            return new ReportAnalytics
            {
                RiskLevel = riskLevelEnum,
                RecruitScore = recruitScoreDecimal,
                ConfidenceLevel = 0, // You can implement a mapping if needed
                QualityScore = qualityScore,
                InformationDensity = informationDensity,
                SpecificityScore = specificityScore,
                SentimentScore = sentimentScore,
                ThreatScore = CalculateThreatScore(analysis, qualityScore)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing report analytics");
            throw;
        }
    }

    // Calculate quality score for a report
    private decimal CalculateQualityScore(Report report)
    {
        // Implement quality scoring logic
        return 0.5m;
    }

    // Calculate information density for a report
    private decimal CalculateInformationDensity(Report report)
    {
        // Implement information density calculation
        return 0.5m;
    }

    // Calculate specificity score for a report
    private decimal CalculateSpecificityScore(Report report)
    {
        // Implement specificity scoring logic
        return 0.5m;
    }

    // Calculate sentiment score for a report
    private decimal CalculateSentimentScore(Report report)
    {
        // Implement sentiment analysis
        return 0.5m;
    }

    // Calculate threat score based on analysis and quality
    private decimal CalculateThreatScore(OpenAIAnalysis analysis, decimal qualityScore)
    {
        // Implement threat scoring logic
        return 0.5m;
    }

    // Get analytics for a person
    public async Task<PersonAnalytics> GetPersonAnalyticsAsync(int personId)
    {
        var person = await _context.People
            .Include(p => p.ReportsMade)
            .Include(p => p.ReportsReceived)
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (person == null)
            throw new ArgumentException("Person not found");

        return new PersonAnalytics
        {
            TotalReportsMade = person.TotalReportsMade,
            TotalReportsReceived = person.TotalReportsReceived,
            AvgReportLength = person.AvgReportLength,
            LastReportAt = person.LastReportAt,
            RiskLevel = person.RiskLevel,
            RecruitScore = person.RecruitScore,
            ThreatScore = person.ThreatScore,
            ReportingConsistency = person.ReportingConsistency,
            NetworkCentrality = person.NetworkCentrality,
            InfluenceScore = person.InfluenceScore
        };
    }

    // Get network analytics for a person
    public async Task<NetworkAnalytics> GetNetworkAnalyticsAsync(int personId)
    {
        var connections = await _context.IntelligenceNetworks
            .Include(n => n.PersonA)
            .Include(n => n.PersonB)
            .Where(n => n.PersonAId == personId || n.PersonBId == personId)
            .ToListAsync();

        return new NetworkAnalytics
        {
            TotalConnections = connections.Count,
            AvgConnectionStrength = connections.Average(c => c.ConnectionStrength),
            SharedTargetsCount = connections.Sum(c => c.SharedTargetsCount),
            TemporalClusteringScore = connections.Average(c => c.TemporalClusteringScore)
        };
    }

    public async Task<decimal> CalculatePersonRiskLevelAsync(int personId)
    {
        var person = await _context.People
            .Include(p => p.ReportsMade)
            .Include(p => p.ReportsReceived)
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (person == null)
            throw new KeyNotFoundException($"Person with ID {personId} not found");

        // Calculate risk based on:
        // 1. Number of reports received
        // 2. Average threat score of reports
        // 3. Network influence
        var reportsReceived = person.ReportsReceived.Count;
        var avgThreatScore = person.ReportsReceived.Any()
            ? person.ReportsReceived.Average(r => r.QualityScore)
            : 0;
        var networkInfluence = await CalculatePersonNetworkInfluenceAsync(personId);

        // Weighted combination
        return (reportsReceived * 0.4m + avgThreatScore * 0.4m + networkInfluence * 0.2m) / 3;
    }

    public async Task<BehavioralPattern> AnalyzePersonBehaviorAsync(int personId)
    {
        var person = await _context.People
            .Include(p => p.ReportsMade)
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (person == null)
            throw new KeyNotFoundException($"Person with ID {personId} not found");

        var pattern = new BehavioralPattern
        {
            PersonId = personId,
            AvgReportsPerDay = CalculateAverageReportsPerDay(person.ReportsMade),
            AvgTimeBetweenReportsHours = CalculateAverageTimeBetweenReports(person.ReportsMade),
            ReportingPattern = DetermineReportingPattern(person.ReportsMade),
            AvgSentiment = person.ReportsMade.Any()
                ? person.ReportsMade.Average(r => r.SentimentScore)
                : 0,
            VocabularyDiversity = person.ReportsMade.Any()
                ? person.ReportsMade.Average(r => r.SpecificityScore)
                : 0,
            AvgInformationDensity = person.ReportsMade.Any()
                ? person.ReportsMade.Average(r => r.InformationDensity)
                : 0,
            UniqueTargetsReported = person.ReportsMade.Select(r => r.TargetId).Distinct().Count(),
            NetworkInfluence = await CalculatePersonNetworkInfluenceAsync(personId)
        };

        // Calculate preferred hours
        var preferredHours = person.ReportsMade
            .GroupBy(r => r.SubmittedAt.Hour)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.Key)
            .ToList();
        pattern.SetPreferredHours(preferredHours);

        return pattern;
    }

    public async Task<decimal> CalculatePersonNetworkInfluenceAsync(int personId)
    {
        var connections = await _context.IntelligenceNetworks
            .Where(n => n.PersonAId == personId || n.PersonBId == personId)
            .ToListAsync();

        if (!connections.Any())
            return 0;

        // Calculate influence based on:
        // 1. Number of connections
        // 2. Average connection strength
        // 3. Shared targets
        var connectionCount = connections.Count;
        var avgConnectionStrength = connections.Average(c => c.ConnectionStrength);
        var avgSharedTargets = connections.Average(c => c.SharedTargetsCount);

        return (connectionCount * 0.3m + (decimal)avgConnectionStrength * 0.4m + (decimal)avgSharedTargets * 0.3m) / 3m;
    }

    public async Task<IEnumerable<Person>> GetHighRiskPersonsAsync()
    {
        var people = await _context.People
            .Include(p => p.ReportsReceived)
            .ToListAsync();

        var highRiskPeople = new List<Person>();
        foreach (var person in people)
        {
            var riskLevel = await CalculatePersonRiskLevelAsync(person.Id);
            if (riskLevel > 0.7m) // Threshold for high risk
            {
                highRiskPeople.Add(person);
            }
        }

        return highRiskPeople.OrderByDescending(p => p.RiskLevel);
    }

    public async Task<IEnumerable<Person>> GetPersonsByRiskLevelAsync(RiskLevel riskLevel)
    {
        return await _context.People
            .Where(p => p.RiskLevel == riskLevel)
            .OrderByDescending(p => p.ReportsReceived.Count)
            .ToListAsync();
    }

    public async Task<IntelligenceNetwork> AnalyzeConnectionAsync(int personAId, int personBId)
    {
        var connection = await _context.IntelligenceNetworks
            .FirstOrDefaultAsync(n =>
                (n.PersonAId == personAId && n.PersonBId == personBId) ||
                (n.PersonAId == personBId && n.PersonBId == personAId));

        if (connection == null)
        {
            // Create new connection
            connection = new IntelligenceNetwork
            {
                PersonAId = personAId,
                PersonBId = personBId,
                ConnectionStrength = 0,
                ConnectionType = "UNKNOWN",
                SharedTargetsCount = 0,
                TemporalClusteringScore = 0,
                LastInteraction = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Calculate connection metrics
        var reportsA = await _context.Reports
            .Where(r => r.ReporterId == personAId)
            .ToListAsync();
        var reportsB = await _context.Reports
            .Where(r => r.ReporterId == personBId)
            .ToListAsync();

        // Calculate shared targets
        var targetsA = reportsA.Select(r => r.TargetId).Distinct();
        var targetsB = reportsB.Select(r => r.TargetId).Distinct();
        connection.SharedTargetsCount = targetsA.Intersect(targetsB).Count();

        // Calculate temporal clustering
        connection.TemporalClusteringScore = CalculateTemporalClustering(reportsA, reportsB);

        // Update connection strength
        connection.ConnectionStrength = CalculateConnectionStrength(connection);

        return connection;
    }

    public async Task<IEnumerable<IntelligenceNetwork>> GetPersonConnectionsAsync(int personId)
    {
        return await _context.IntelligenceNetworks
            .Where(n => n.PersonAId == personId || n.PersonBId == personId)
            .OrderByDescending(n => n.ConnectionStrength)
            .ToListAsync();
    }

    public async Task<decimal> CalculateNetworkDensityAsync()
    {
        var totalPeople = await _context.People.CountAsync();
        var totalConnections = await _context.IntelligenceNetworks.CountAsync();

        if (totalPeople <= 1)
            return 0;

        // Network density = actual connections / possible connections
        var possibleConnections = (totalPeople * (totalPeople - 1)) / 2;
        return (decimal)totalConnections / possibleConnections;
    }

    public async Task<IEnumerable<Person>> GetKeyInfluencersAsync()
    {
        var people = await _context.People
            .Include(p => p.ReportsMade)
            .ToListAsync();

        var influencers = new List<(Person Person, decimal Influence)>();
        foreach (var person in people)
        {
            var influence = await CalculatePersonNetworkInfluenceAsync(person.Id);
            influencers.Add((person, influence));
        }

        return influencers
            .OrderByDescending(i => i.Influence)
            .Select(i => i.Person)
            .Take(10);
    }

    public async Task<ThreatAssessment> GenerateThreatAssessmentAsync(int targetId)
    {
        var target = await _context.People
            .Include(p => p.ReportsReceived)
            .FirstOrDefaultAsync(p => p.Id == targetId);

        if (target == null)
            throw new KeyNotFoundException($"Target with ID {targetId} not found");

        var assessment = new ThreatAssessment
        {
            TargetId = targetId,
            AssessmentDate = DateOnly.FromDateTime(DateTime.UtcNow),
            FrequencyScore = CalculateFrequencyScore(target.ReportsReceived),
            VelocityScore = CalculateVelocityScore(target.ReportsReceived),
            NetworkExposureScore = await CalculateNetworkExposureScoreAsync(targetId),
            SentimentThreatScore = CalculateSentimentThreatScore(target.ReportsReceived),
            TemporalClusteringScore = await CalculateTemporalClusteringScoreAsync(targetId),
            TotalReportsInPeriod = target.ReportsReceived.Count,
            UniqueReportersInPeriod = target.ReportsReceived.Select(r => r.ReporterId).Distinct().Count()
        };

        // Calculate composite scores
        assessment.CompositeThreatScore = CalculateCompositeThreatScore(assessment);
        assessment.EscalationProbability = CalculateEscalationProbability(assessment);
        assessment.ThreatTrend = DetermineThreatTrend(target.ReportsReceived);

        return assessment;
    }

    public async Task<IEnumerable<ThreatAssessment>> GetRecentThreatAssessmentsAsync(int count = 10)
    {
        return await _context.ThreatAssessments
            .OrderByDescending(t => t.AssessmentDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<decimal> CalculateThreatScoreAsync(int targetId)
    {
        var assessment = await GenerateThreatAssessmentAsync(targetId);
        return assessment.CompositeThreatScore;
    }

    public async Task<IEnumerable<Person>> GetHighThreatTargetsAsync()
    {
        var assessments = await _context.ThreatAssessments
            .Where(t => t.CompositeThreatScore > 0.7m) // Threshold for high threat
            .OrderByDescending(t => t.CompositeThreatScore)
            .ToListAsync();

        var targetIds = assessments.Select(a => a.TargetId).Distinct();
        return await _context.People
            .Where(p => targetIds.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<Dictionary<int, int>> GetReportingHourDistributionAsync()
    {
        var reports = await _context.Reports.ToListAsync();
        return reports
            .GroupBy(r => r.SubmittedAt.Hour)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<DayOfWeek, int>> GetReportingDayDistributionAsync()
    {
        var reports = await _context.Reports.ToListAsync();
        return reports
            .GroupBy(r => r.SubmittedAt.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<decimal> CalculateTemporalClusteringScoreAsync(int personId)
    {
        var reports = await _context.Reports
            .Where(r => r.ReporterId == personId)
            .OrderBy(r => r.SubmittedAt)
            .ToListAsync();

        if (reports.Count < 2)
            return 0;

        var timeDiffs = new List<TimeSpan>();
        for (int i = 1; i < reports.Count; i++)
        {
            timeDiffs.Add(reports[i].SubmittedAt - reports[i - 1].SubmittedAt);
        }

        var avgTimeDiff = timeDiffs.Average(t => t.TotalHours);
        var stdDev = CalculateStandardDeviation(timeDiffs.Select(t => (decimal)t.TotalHours));

        // Lower standard deviation indicates more clustering
        return 1 - (stdDev / (decimal)avgTimeDiff);
    }

    public async Task<decimal> CalculateReportQualityScoreAsync(int reportId)
    {
        var report = await _context.Reports.FindAsync(reportId);
        if (report == null)
            throw new KeyNotFoundException($"Report with ID {reportId} not found");

        // Calculate quality based on:
        // 1. Information density
        // 2. Vocabulary diversity
        // 3. Sentiment score
        // 4. Corroboration status
        var infoDensity = report.InformationDensity;
        var vocabDiversity = report.SpecificityScore;
        var sentiment = Math.Abs(report.SentimentScore); // Absolute value to consider both positive and negative sentiment
        var corroboration = report.IsCorroborated ? 1.0m : 0.5m;

        return (infoDensity * 0.4m + vocabDiversity * 0.3m + sentiment * 0.2m + corroboration * 0.1m);
    }

    public async Task<decimal> CalculateInformationDensityAsync(string text)
    {
        // Simple implementation - can be enhanced with NLP
        var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var uniqueWords = words.Distinct().Count();
        return words.Length > 0 ? (decimal)uniqueWords / words.Length : 0;
    }

    public async Task<decimal> CalculateVocabularyDiversityAsync(string text)
    {
        // Simple implementation - can be enhanced with more sophisticated analysis
        var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var uniqueWords = words.Distinct().Count();
        return words.Length > 0 ? (decimal)uniqueWords / words.Length : 0;
    }

    public async Task<Dictionary<string, int>> GetSystemMetricsAsync()
    {
        return new Dictionary<string, int>
        {
            ["TotalPeople"] = await _context.People.CountAsync(),
            ["TotalReports"] = await _context.Reports.CountAsync(),
            ["TotalAlerts"] = await _context.Alerts.CountAsync(),
            ["ActiveAlerts"] = await _context.Alerts.CountAsync(a => a.Status != AlertStatus.RESOLVED),
            ["TotalConnections"] = await _context.IntelligenceNetworks.CountAsync(),
            ["TotalThreatAssessments"] = await _context.ThreatAssessments.CountAsync()
        };
    }

    public async Task<IEnumerable<SystemLog>> GetRecentSystemLogsAsync(int count = 100)
    {
        return await _context.SystemLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetAlertDistributionAsync()
    {
        var alerts = await _context.Alerts.ToListAsync();
        return alerts
            .GroupBy(a => a.AlertType.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<decimal> CalculateSentimentScoreAsync(string text)
    {
        // Implement sentiment analysis
        return await Task.FromResult(0.5m);
    }

    // Helper methods
    private decimal CalculateAverageReportsPerDay(IEnumerable<Report> reports)
    {
        if (!reports.Any())
            return 0;

        var days = (DateTime.UtcNow - reports.Min(r => r.SubmittedAt)).TotalDays;
        if (days == 0)
            return reports.Count();

        return (decimal)reports.Count() / (decimal)days;
    }

    private decimal CalculateAverageTimeBetweenReports(IEnumerable<Report> reports)
    {
        if (reports.Count() < 2)
            return 0;

        var orderedReports = reports.OrderBy(r => r.SubmittedAt).ToList();
        var timeDiffs = new List<decimal>();
        for (int i = 1; i < orderedReports.Count; i++)
        {
            timeDiffs.Add((decimal)(orderedReports[i].SubmittedAt - orderedReports[i - 1].SubmittedAt).TotalHours);
        }

        return timeDiffs.Average();
    }

    private ReportingPattern DetermineReportingPattern(IEnumerable<Report> reports)
    {
        if (!reports.Any())
            return ReportingPattern.IRREGULAR;

        var avgTimeBetweenReports = CalculateAverageTimeBetweenReports(reports);
        var stdDev = CalculateStandardDeviation(
            reports.OrderBy(r => r.SubmittedAt)
                .Select((r, i) => i > 0 ? (decimal)(r.SubmittedAt - reports.ElementAt(i - 1).SubmittedAt).TotalHours : 0m)
                .Skip(1)
        );

        if (stdDev < avgTimeBetweenReports * 0.2m)
            return ReportingPattern.REGULAR;
        else if (stdDev > avgTimeBetweenReports * 0.8m)
            return ReportingPattern.BURST;
        else
            return ReportingPattern.IRREGULAR;
    }

    private decimal CalculateTemporalClustering(IEnumerable<Report> reportsA, IEnumerable<Report> reportsB)
    {
        if (!reportsA.Any() || !reportsB.Any())
            return 0;

        var timesA = reportsA.Select(r => r.SubmittedAt).OrderBy(t => t).ToList();
        var timesB = reportsB.Select(r => r.SubmittedAt).OrderBy(t => t).ToList();

        var timeDiffs = new List<decimal>();
        foreach (var timeA in timesA)
        {
            foreach (var timeB in timesB)
            {
                timeDiffs.Add(Math.Abs((decimal)(timeA - timeB).TotalHours));
            }
        }

        var avgTimeDiff = timeDiffs.Average();
        var stdDev = CalculateStandardDeviation(timeDiffs);

        return stdDev > 0 ? 1m / (1m + avgTimeDiff / stdDev) : 0;
    }

    private decimal CalculateConnectionStrength(IntelligenceNetwork connection)
    {
        return (connection.SharedTargetsCount * 0.4m +
                connection.TemporalClusteringScore * 0.3m +
                (decimal)connection.ConnectionType.Length / 10m * 0.3m);
    }

    private decimal CalculateFrequencyScore(IEnumerable<Report> reports)
    {
        if (!reports.Any())
            return 0;

        var reportCount = reports.Count();
        var timeSpan = reports.Max(r => r.SubmittedAt) - reports.Min(r => r.SubmittedAt);
        var days = Math.Max(1, (decimal)timeSpan.TotalDays);

        return reportCount / days;
    }

    private decimal CalculateVelocityScore(IEnumerable<Report> reports)
    {
        if (!reports.Any())
            return 0;

        var orderedReports = reports.OrderBy(r => r.SubmittedAt).ToList();
        var timeDiffs = new List<decimal>();

        for (int i = 1; i < orderedReports.Count; i++)
        {
            timeDiffs.Add((decimal)(orderedReports[i].SubmittedAt - orderedReports[i - 1].SubmittedAt).TotalHours);
        }

        if (!timeDiffs.Any())
            return 0;

        var avgTimeDiff = timeDiffs.Average();
        var stdDev = CalculateStandardDeviation(timeDiffs);

        return stdDev > 0 ? 1m / (1m + avgTimeDiff / stdDev) : 0;
    }

    private async Task<decimal> CalculateNetworkExposureScoreAsync(int targetId)
    {
        var reports = await _context.Reports
            .Where(r => r.TargetId == targetId)
            .ToListAsync();

        if (!reports.Any())
            return 0;

        var uniqueReporters = reports.Select(r => r.ReporterId).Distinct().Count();
        var totalPeople = await _context.People.CountAsync();

        return (decimal)uniqueReporters / totalPeople;
    }

    private decimal CalculateSentimentThreatScore(IEnumerable<Report> reports)
    {
        if (!reports.Any())
            return 0;

        return Math.Abs(reports.Average(r => r.SentimentScore));
    }

    private decimal CalculateCompositeThreatScore(ThreatAssessment assessment)
    {
        return (assessment.FrequencyScore * 0.3m +
                assessment.VelocityScore * 0.2m +
                assessment.NetworkExposureScore * 0.2m +
                assessment.SentimentThreatScore * 0.15m +
                assessment.TemporalClusteringScore * 0.15m);
    }

    private decimal CalculateEscalationProbability(ThreatAssessment assessment)
    {
        return (assessment.CompositeThreatScore * 0.6m +
                (decimal)assessment.TotalReportsInPeriod / 100 * 0.2m +
                (decimal)assessment.UniqueReportersInPeriod / 50 * 0.2m);
    }

    private ThreatTrend DetermineThreatTrend(IEnumerable<Report> reports)
    {
        if (!reports.Any())
            return ThreatTrend.STABLE;

        var recentReports = reports
            .Where(r => r.SubmittedAt >= DateTime.UtcNow.AddDays(-30))
            .OrderBy(r => r.SubmittedAt)
            .ToList();

        if (recentReports.Count < 2)
            return ThreatTrend.STABLE;

        var weeklyCounts = recentReports
            .GroupBy(r => r.SubmittedAt.DayOfWeek)
            .Select(g => (decimal)g.Count())
            .ToList();

        var stdDev = CalculateStandardDeviation(weeklyCounts);
        var avg = weeklyCounts.Average();

        if (stdDev > avg * 0.5m)
            return ThreatTrend.VOLATILE;
        else if (weeklyCounts.Last() > weeklyCounts.First() * 1.2m)
            return ThreatTrend.RISING;
        else if (weeklyCounts.Last() < weeklyCounts.First() * 0.8m)
            return ThreatTrend.FALLING;
        else
            return ThreatTrend.STABLE;
    }

    private decimal CalculateStandardDeviation(IEnumerable<decimal> values)
    {
        var valuesList = values.ToList();
        if (valuesList.Count == 0)
            return 0;
        var avg = valuesList.Average();
        var sumOfSquares = valuesList.Sum(x => (x - avg) * (x - avg));
        // Use decimal division and Math.Sqrt on double, then cast back to decimal
        return (decimal)Math.Sqrt((double)(sumOfSquares / valuesList.Count));
    }
}

// Models for analytics results
public class ReportAnalytics
{
    public RiskLevel RiskLevel { get; set; }
    public decimal RecruitScore { get; set; }
    public decimal ConfidenceLevel { get; set; }
    public decimal QualityScore { get; set; }
    public decimal InformationDensity { get; set; }
    public decimal SpecificityScore { get; set; }
    public decimal SentimentScore { get; set; }
    public decimal ThreatScore { get; set; }
}

public class PersonAnalytics
{
    public int TotalReportsMade { get; set; }
    public int TotalReportsReceived { get; set; }
    public decimal AvgReportLength { get; set; }
    public DateTime? LastReportAt { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public decimal RecruitScore { get; set; }
    public decimal ThreatScore { get; set; }
    public decimal ReportingConsistency { get; set; }
    public decimal NetworkCentrality { get; set; }
    public decimal InfluenceScore { get; set; }
}

public class NetworkAnalytics
{
    public int TotalConnections { get; set; }
    public decimal AvgConnectionStrength { get; set; }
    public int SharedTargetsCount { get; set; }
    public decimal TemporalClusteringScore { get; set; }
}

public class OpenAIAnalysis
{
    public string RiskLevel { get; set; } = "";
    public double RecruitScore { get; set; }
    public string ConfidenceLevel { get; set; } = "";
}