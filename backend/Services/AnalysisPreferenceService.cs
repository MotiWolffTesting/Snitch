using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public interface IAnalysisPreferenceService
{
    Task<bool> GetUseOpenAIPreferenceAsync();
    Task SetUseOpenAIPreferenceAsync(bool useOpenAI, string updatedBy);
}

public class AnalysisPreferenceService : IAnalysisPreferenceService
{
    private readonly SnitchDbContext _context;
    private readonly ILogger<AnalysisPreferenceService> _logger;

    public AnalysisPreferenceService(SnitchDbContext context, ILogger<AnalysisPreferenceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> GetUseOpenAIPreferenceAsync()
    {
        var preference = await _context.AnalysisPreferences
            .OrderByDescending(p => p.LastUpdated)
            .FirstOrDefaultAsync();

        if (preference == null)
        {
            // Create default preference if none exists
            preference = new AnalysisPreference
            {
                UseOpenAI = true,
                LastUpdated = DateTime.UtcNow,
                UpdatedBy = "System"
            };
            _context.AnalysisPreferences.Add(preference);
            await _context.SaveChangesAsync();
        }

        return preference.UseOpenAI;
    }

    public async Task SetUseOpenAIPreferenceAsync(bool useOpenAI, string updatedBy)
    {
        var preference = new AnalysisPreference
        {
            UseOpenAI = useOpenAI,
            LastUpdated = DateTime.UtcNow,
            UpdatedBy = updatedBy
        };

        _context.AnalysisPreferences.Add(preference);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Analysis preference updated to use OpenAI: {UseOpenAI} by {UpdatedBy}", useOpenAI, updatedBy);
    }
}