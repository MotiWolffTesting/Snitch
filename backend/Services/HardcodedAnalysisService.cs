using System.Text.RegularExpressions;
using backend.Models;

namespace backend.Services;

public class HardcodedAnalysisService : IOpenAIService
{
    private readonly ILogger<HardcodedAnalysisService> _logger;
    private readonly Dictionary<string, int> _riskKeywords;
    private readonly Dictionary<string, int> _sentimentWords;
    private readonly Dictionary<string, int> _threatPatterns;

    public HardcodedAnalysisService(ILogger<HardcodedAnalysisService> logger)
    {
        _logger = logger;

        // Initialize risk keywords with weights
        _riskKeywords = new Dictionary<string, int>
        {
            // High risk keywords
            {"attack", 10}, {"bomb", 10}, {"weapon", 10}, {"terror", 10}, {"threat", 8},
            {"kill", 9}, {"murder", 9}, {"violence", 8}, {"danger", 7}, {"suspicious", 6},
            {"explode", 10}, {"massacre", 10}, {"hostage", 9}, {"execute", 9},
            {"detonate", 10}, {"radical", 8}, {"extremist", 9}, {"assassinate", 10},
            {"ambush", 9}, {"gunfire", 9},
            
            
            // Medium risk keywords
            {"unusual", 5}, {"strange", 5}, {"concerning", 5}, {"worried", 4}, {"nervous", 4},
            {"secret", 5}, {"hidden", 5}, {"private", 4}, {"confidential", 4}, {"anomaly", 5}, {"alert", 5}, {"monitor", 4}, {"infiltrate", 5},
{"evade", 5}, {"intel", 4}, {"observe", 4}, {"track", 5},
            
            // Low risk keywords
            {"maybe", 2}, {"possibly", 2}, {"might", 2}, {"could", 2}, {"perhaps", 2}
        };

        // Initialize sentiment words with weights (-5 to 5)
        _sentimentWords = new Dictionary<string, int>
        {
            // Negative sentiment
            {"terrible", -5}, {"horrible", -5}, {"awful", -4}, {"bad", -3}, {"poor", -3},
            {"worried", -3}, {"concerned", -3}, {"scared", -4}, {"frightened", -4},
            {"angry", -4}, {"upset", -3}, {"disappointed", -3}, {"maybe suspicious", 2}, {"potentially", 2}, {"unsure", 2},
            {"unknown", 2}, {"in question", 2},

            
            // Positive sentiment
            {"good", 3}, {"great", 4}, {"excellent", 5}, {"wonderful", 4}, {"amazing", 4},
            {"happy", 3}, {"pleased", 3}, {"satisfied", 3}, {"confident", 3}
        };

        // Initialize threat patterns with weights
        _threatPatterns = new Dictionary<string, int>
        {
            // Direct threats
            {"will kill", 10}, {"going to attack", 10}, {"plan to harm", 9},
            {"threaten to", 8}, {"promise to hurt", 8},
            
            // Indirect threats
            {"better watch out", 7}, {"you'll regret", 7}, {"pay for this", 7},
            {"get what's coming", 6}, {"deserve what's coming", 6},
            
            // Suspicious patterns
            {"meet in secret", 5}, {"hidden location", 5}, {"underground", 5},
            {"private meeting", 4}, {"confidential information", 4}
        };
    }

    public async Task<OpenAIAnalysis> AnalyzeTextAsync(string text, Person? target = null)
    {
        try
        {
            // Calculate base risk score from keywords
            var riskScore = CalculateRiskScore(text);

            // Calculate sentiment score
            var sentimentScore = CalculateSentimentScore(text);

            // Calculate threat pattern score
            var threatScore = CalculateThreatScore(text);

            // Calculate text quality metrics
            var (specificity, informationDensity) = CalculateTextQuality(text);

            // Consider historical context if target is provided
            if (target != null)
            {
                riskScore = AdjustRiskScoreWithContext(riskScore, target);
            }

            // Combine scores to determine final risk level
            var finalRiskScore = CombineScores(riskScore, sentimentScore, threatScore, specificity, informationDensity);

            // Determine risk level
            var riskLevel = DetermineRiskLevel(finalRiskScore);

            // Calculate recruit score based on all factors
            var recruitScore = CalculateRecruitScore(finalRiskScore, sentimentScore, specificity);

            // Determine confidence level
            var confidenceLevel = DetermineConfidenceLevel(specificity, informationDensity);

            return new OpenAIAnalysis
            {
                RiskLevel = riskLevel,
                RecruitScore = recruitScore,
                ConfidenceLevel = confidenceLevel
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in hardcoded text analysis");
            throw;
        }
    }

    private decimal CalculateRiskScore(string text)
    {
        var words = text.ToLower().Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        var score = 0.0m;
        var keywordCount = 0;

        foreach (var word in words)
        {
            if (_riskKeywords.TryGetValue(word, out var weight))
            {
                score += weight;
                keywordCount++;
            }
        }

        // Normalize score based on text length and keyword frequency
        return keywordCount > 0 ? score / (words.Length * 0.1m) : 0;
    }

    private decimal CalculateSentimentScore(string text)
    {
        var words = text.ToLower().Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        var score = 0.0m;
        var sentimentCount = 0;

        foreach (var word in words)
        {
            if (_sentimentWords.TryGetValue(word, out var weight))
            {
                score += weight;
                sentimentCount++;
            }
        }

        // Normalize to 0-10 scale
        return sentimentCount > 0 ? (score + 5) * 2 : 5;
    }

    private decimal CalculateThreatScore(string text)
    {
        var score = 0.0m;
        var patternCount = 0;

        foreach (var pattern in _threatPatterns)
        {
            var matches = Regex.Matches(text.ToLower(), pattern.Key);
            if (matches.Count > 0)
            {
                score += pattern.Value * matches.Count;
                patternCount++;
            }
        }

        // Normalize to 0-10 scale
        return patternCount > 0 ? score / (patternCount * 2) : 0;
    }

    private (decimal specificity, decimal informationDensity) CalculateTextQuality(string text)
    {
        var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var uniqueWords = words.Distinct().Count();

        // Calculate specificity (unique words ratio)
        var specificity = words.Length > 0 ? (decimal)uniqueWords / words.Length : 0;

        // Calculate information density (non-stop words ratio)
        var stopWords = new HashSet<string> { "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for", "with", "by", "about", "as" };
        var nonStopWords = words.Count(w => !stopWords.Contains(w.ToLower()));
        var informationDensity = words.Length > 0 ? (decimal)nonStopWords / words.Length : 0;

        return (specificity, informationDensity);
    }

    private decimal AdjustRiskScoreWithContext(decimal baseScore, Person target)
    {
        var adjustment = 0.0m;

        // Adjust based on target's historical risk level
        adjustment += (int)target.RiskLevel * 0.2m;

        // Adjust based on number of reports received
        adjustment += Math.Min(target.TotalReportsReceived * 0.1m, 1.0m);

        // Adjust based on network centrality
        adjustment += target.NetworkCentrality * 0.2m;

        // Adjust based on influence score
        adjustment += target.InfluenceScore * 0.2m;

        return Math.Min(baseScore + adjustment, 10.0m);
    }

    private decimal CombineScores(decimal riskScore, decimal sentimentScore, decimal threatScore, decimal specificity, decimal informationDensity)
    {
        return (riskScore * 0.4m +
                sentimentScore * 0.2m +
                threatScore * 0.3m +
                specificity * 0.05m +
                informationDensity * 0.05m);
    }

    private string DetermineRiskLevel(decimal score)
    {
        return score switch
        {
            >= 7.5m => "HIGH",
            >= 4.0m => "MEDIUM",
            _ => "LOW"
        };
    }

    private double CalculateRecruitScore(decimal riskScore, decimal sentimentScore, decimal specificity)
    {
        // Recruit score is based on risk, sentiment, and specificity
        var score = (riskScore * 0.5m + sentimentScore * 0.3m + specificity * 0.2m);
        return (double)Math.Min(score, 10.0m);
    }

    private string DetermineConfidenceLevel(decimal specificity, decimal informationDensity)
    {
        var confidenceScore = (specificity + informationDensity) / 2;
        return confidenceScore switch
        {
            >= 0.7m => "HIGH",
            >= 0.4m => "MEDIUM",
            _ => "LOW"
        };
    }
}