using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    // OpenAIService handles interactions with OpenAI's API for text analysis.
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey");
            _logger = logger;
        }

        // Analyze text using OpenAI's API
        public async Task<OpenAIAnalysis> AnalyzeTextAsync(string text)
        {
            try
            {
                // Prepare request
                var request = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are an intelligence analyst. Analyze the following text and respond ONLY with a JSON object with the following fields: riskLevel (string: 'Low', 'Medium', or 'High'), recruitScore (number between 0-10), and confidenceLevel (string: 'Low', 'Medium', or 'High'). Do not include any explanation or text outside the JSON object." },
                        new { role = "user", content = text }
                    }
                };

                // Send request to OpenAI
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
                var response = await _httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json")
                );

                // Parse response
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("OpenAI API response: {Response}", responseContent);

                // Use case-insensitive deserialization options
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<OpenAIResponse>(responseContent, options);

                if (result?.Choices == null || result.Choices.Count == 0)
                {
                    _logger.LogError("OpenAI API returned no choices. Response content: {Content}", responseContent);
                    throw new Exception("Failed to parse OpenAI response or no choices returned");
                }

                var content = result.Choices[0].Message.Content.Trim();
                _logger.LogInformation("Parsing OpenAI analysis content: {Content}", content);

                try
                {
                    // If content is a stringified JSON, deserialize it first
                    if (content.StartsWith("\"") && content.EndsWith("\""))
                    {
                        content = JsonSerializer.Deserialize<string>(content);
                    }

                    // Parse the content as OpenAIAnalysis
                    var analysis = JsonSerializer.Deserialize<OpenAIAnalysis>(
                        content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (analysis == null)
                    {
                        throw new Exception("Failed to deserialize analysis content");
                    }

                    return analysis;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception while parsing OpenAI analysis content: {Content}\nException: {Exception}", content, ex.ToString());
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing text with OpenAI");
                throw;
            }
        }
    }

    // Response model for OpenAI API
    public class OpenAIResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; } = string.Empty;

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; } = new List<Choice>();
    }

    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; } = new Message();

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = string.Empty;
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}