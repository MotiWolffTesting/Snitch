// AlertsController handles all API endpoints related to alerts in the system.
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    // Constructor injects the alert service
    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    // Create a new alert
    [HttpPost]
    public async Task<ActionResult<Alert>> CreateAlert(Alert alert)
    {
        try
        {
            var createdAlert = await _alertService.CreateAlertAsync(alert);
            return CreatedAtAction(nameof(GetAlert), new { id = createdAlert.Id }, createdAlert);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Get a specific alert by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Alert>> GetAlert(int id)
    {
        var alert = await _alertService.GetAlertAsync(id);
        if (alert == null)
            return NotFound();

        return alert;
    }

    // Get all active alerts
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Alert>>> GetActiveAlerts()
    {
        var alerts = await _alertService.GetActiveAlertsAsync();
        return Ok(alerts);
    }

    // Get all alerts for a specific target
    [HttpGet("target/{targetId}")]
    public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsByTarget(int targetId)
    {
        var alerts = await _alertService.GetAlertsByTargetIdAsync(targetId);
        return Ok(alerts);
    }

    // Get alerts by severity
    [HttpGet("severity/{severity}")]
    public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsBySeverity(AlertSeverity severity)
    {
        var alerts = await _alertService.GetAlertsBySeverityAsync(severity);
        return Ok(alerts);
    }

    // Update an alert
    [HttpPut("{id}")]
    public async Task<ActionResult<Alert>> UpdateAlert(int id, Alert alert)
    {
        if (id != alert.Id)
            return BadRequest("ID mismatch");

        try
        {
            var updatedAlert = await _alertService.UpdateAlertAsync(alert);
            return Ok(updatedAlert);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Delete an alert
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAlert(int id)
    {
        try
        {
            await _alertService.DeleteAlertAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Acknowledge an alert
    [HttpPost("{id}/acknowledge")]
    public async Task<ActionResult<Alert>> AcknowledgeAlert(int id, [FromBody] string acknowledgedBy)
    {
        try
        {
            var alert = await _alertService.AcknowledgeAlertAsync(id, acknowledgedBy);
            return Ok(alert);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Resolve an alert
    [HttpPost("{id}/resolve")]
    public async Task<ActionResult<Alert>> ResolveAlert(int id, [FromBody] string resolvedBy)
    {
        try
        {
            var alert = await _alertService.ResolveAlertAsync(id, resolvedBy);
            return Ok(alert);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get alerts in a date range
    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var alerts = await _alertService.GetAlertsByDateRangeAsync(startDate, endDate);
        return Ok(alerts);
    }

    // Get alerts by type
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsByType(AlertType type)
    {
        var alerts = await _alertService.GetAlertsByTypeAsync(type);
        return Ok(alerts);
    }

    // Get the threat score for a specific alert
    [HttpGet("{id}/threat-score")]
    public async Task<ActionResult<decimal>> GetAlertThreatScore(int id)
    {
        try
        {
            var score = await _alertService.CalculateAlertThreatScoreAsync(id);
            return Ok(score);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Check if an alert should be escalated
    [HttpGet("{id}/should-escalate")]
    public async Task<ActionResult<bool>> ShouldEscalateAlert(int id)
    {
        try
        {
            var shouldEscalate = await _alertService.ShouldEscalateAlertAsync(id);
            return Ok(shouldEscalate);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Get all unresolved alerts
    [HttpGet("unresolved")]
    public async Task<ActionResult<IEnumerable<Alert>>> GetUnresolvedAlerts()
    {
        var alerts = await _alertService.GetUnresolvedAlertsAsync();
        return Ok(alerts);
    }

    // Get alerts by status
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsByStatus(AlertStatus status)
    {
        var alerts = await _alertService.GetAlertsByStatusAsync(status);
        return Ok(alerts);
    }
}