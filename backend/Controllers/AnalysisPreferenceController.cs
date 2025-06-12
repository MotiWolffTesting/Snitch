using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisPreferenceController : ControllerBase
{
    private readonly IAnalysisPreferenceService _preferenceService;
    private readonly ILogger<AnalysisPreferenceController> _logger;

    public AnalysisPreferenceController(
        IAnalysisPreferenceService preferenceService,
        ILogger<AnalysisPreferenceController> logger)
    {
        _preferenceService = preferenceService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<bool>> GetUseOpenAIPreference()
    {
        try
        {
            var useOpenAI = await _preferenceService.GetUseOpenAIPreferenceAsync();
            return Ok(useOpenAI);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting analysis preference");
            return StatusCode(500, "Error getting analysis preference");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SetUseOpenAIPreference([FromBody] bool useOpenAI)
    {
        try
        {
            var username = User.Identity?.Name ?? "Anonymous";
            await _preferenceService.SetUseOpenAIPreferenceAsync(useOpenAI, username);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting analysis preference");
            return StatusCode(500, "Error setting analysis preference");
        }
    }
}