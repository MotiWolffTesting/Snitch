using backend.Models;

namespace backend.Services;

public interface IOpenAIService
{
    Task<OpenAIAnalysis> AnalyzeTextAsync(string text, Person? target = null);
}