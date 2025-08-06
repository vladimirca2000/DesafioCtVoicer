using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

namespace ChatBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Endpoint básico de health check que retorna o status da aplicação.
    /// </summary>
    /// <returns>Status de saúde da aplicação.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(HealthResponse), (int)HttpStatusCode.ServiceUnavailable)]
    public async Task<IActionResult> Get()
    {
        var healthReport = await _healthCheckService.CheckHealthAsync();
        
        var response = new HealthResponse
        {
            Status = healthReport.Status.ToString(),
            TotalDuration = healthReport.TotalDuration,
            Checks = healthReport.Entries.Select(entry => new HealthCheckItem
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString(),
                Duration = entry.Value.Duration,
                Description = entry.Value.Description,
                Exception = entry.Value.Exception?.Message
            }).ToList()
        };

        return healthReport.Status == HealthStatus.Healthy
            ? Ok(response)
            : StatusCode((int)HttpStatusCode.ServiceUnavailable, response);
    }

    /// <summary>
    /// Endpoint simplificado de health check que retorna apenas o status.
    /// </summary>
    /// <returns>Status simples da aplicação.</returns>
    [HttpGet("simple")]
    [ProducesResponseType(typeof(SimpleHealthResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(SimpleHealthResponse), (int)HttpStatusCode.ServiceUnavailable)]
    public async Task<IActionResult> GetSimple()
    {
        var healthReport = await _healthCheckService.CheckHealthAsync();
        
        var response = new SimpleHealthResponse
        {
            Status = healthReport.Status.ToString(),
            Timestamp = DateTime.UtcNow
        };

        return healthReport.Status == HealthStatus.Healthy
            ? Ok(response)
            : StatusCode((int)HttpStatusCode.ServiceUnavailable, response);
    }
}

public class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public TimeSpan TotalDuration { get; set; }
    public List<HealthCheckItem> Checks { get; set; } = new();
}

public class HealthCheckItem
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string? Description { get; set; }
    public string? Exception { get; set; }
}

public class SimpleHealthResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}