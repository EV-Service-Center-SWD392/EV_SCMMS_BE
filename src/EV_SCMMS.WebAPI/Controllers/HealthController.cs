using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Health check controller for monitoring application and database status
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(AppDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Application health status</returns>
    [HttpGet]
    public IActionResult GetHealth()
    {
        try
        {
            var healthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Version = "1.0.0"
            };

            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new { Status = "Unhealthy", Error = ex.Message });
        }
    }

    /// <summary>
    /// Database connectivity health check
    /// </summary>
    /// <returns>Database connection status</returns>
    [HttpGet("database")]
    public async Task<IActionResult> GetDatabaseHealth()
    {
        try
        {
            // Test database connection
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                var failedResponse = new
                {
                    Status = "Unhealthy",
                    Database = "Disconnected",
                    Timestamp = DateTime.UtcNow,
                    Message = "Cannot connect to database"
                };
                return StatusCode(503, failedResponse);
            }

            // Test database responsiveness with a simple query
            var startTime = DateTime.UtcNow;
            await _context.Database.ExecuteSqlRawAsync("SELECT 1");
            var responseTime = DateTime.UtcNow - startTime;

            var healthStatus = new
            {
                Status = "Healthy",
                Database = "Connected",
                Timestamp = DateTime.UtcNow,
                ResponseTimeMs = responseTime.TotalMilliseconds,
                ConnectionString = _context.Database.GetConnectionString()?.Replace(_context.Database.GetConnectionString()?.Split("Password=")[1]?.Split(";")[0] ?? "", "***") // Hide password
            };

            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            
            var errorResponse = new
            {
                Status = "Unhealthy",
                Database = "Error",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message,
                Type = ex.GetType().Name
            };

            return StatusCode(503, errorResponse);
        }
    }

    /// <summary>
    /// Comprehensive health check including database and system info
    /// </summary>
    /// <returns>Complete system health status</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailedHealth()
    {
        try
        {
            var startTime = DateTime.UtcNow;
            
            // Check database
            var databaseHealthy = false;
            var databaseResponseTime = 0.0;
            string? databaseError = null;

            try
            {
                var dbStartTime = DateTime.UtcNow;
                databaseHealthy = await _context.Database.CanConnectAsync();
                if (databaseHealthy)
                {
                    await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                }
                databaseResponseTime = (DateTime.UtcNow - dbStartTime).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                databaseError = ex.Message;
            }

            var totalResponseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

            var healthStatus = new
            {
                Status = databaseHealthy ? "Healthy" : "Unhealthy",
                Timestamp = DateTime.UtcNow,
                ResponseTimeMs = totalResponseTime,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Version = "1.0.0",
                Database = new
                {
                    Status = databaseHealthy ? "Healthy" : "Unhealthy",
                    ResponseTimeMs = databaseResponseTime,
                    Error = databaseError
                },
                System = new
                {
                    MachineName = Environment.MachineName,
                    ProcessorCount = Environment.ProcessorCount,
                    WorkingSet = Environment.WorkingSet,
                    OSVersion = Environment.OSVersion.ToString(),
                    CLRVersion = Environment.Version.ToString()
                }
            };

            return databaseHealthy ? Ok(healthStatus) : StatusCode(503, healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detailed health check failed");
            
            var errorResponse = new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message,
                Type = ex.GetType().Name
            };

            return StatusCode(500, errorResponse);
        }
    }
}