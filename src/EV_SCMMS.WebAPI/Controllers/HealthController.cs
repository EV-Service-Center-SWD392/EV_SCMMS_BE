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
            // Test database connection with timeout
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            
            var startTime = DateTime.UtcNow;
            var canConnect = await _context.Database.CanConnectAsync(cancellationTokenSource.Token);
            var responseTime = DateTime.UtcNow - startTime;
            
            if (!canConnect)
            {
                var failedResponse = new
                {
                    Status = "Unhealthy",
                    Database = "Disconnected",
                    Timestamp = DateTime.UtcNow,
                    Message = "Cannot connect to database",
                    ResponseTimeMs = responseTime.TotalMilliseconds
                };
                return StatusCode(503, failedResponse);
            }

            var healthStatus = new
            {
                Status = "Healthy",
                Database = "Connected",
                Timestamp = DateTime.UtcNow,
                ResponseTimeMs = responseTime.TotalMilliseconds,
                Provider = "PostgreSQL/Supabase"
            };

            return Ok(healthStatus);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Database health check timed out");
            
            var timeoutResponse = new
            {
                Status = "Unhealthy",
                Database = "Timeout",
                Timestamp = DateTime.UtcNow,
                Error = "Database connection timed out after 10 seconds",
                Type = "TimeoutException"
            };

            return StatusCode(503, timeoutResponse);
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
                using var dbCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var dbStartTime = DateTime.UtcNow;
                databaseHealthy = await _context.Database.CanConnectAsync(dbCancellationTokenSource.Token);
                databaseResponseTime = (DateTime.UtcNow - dbStartTime).TotalMilliseconds;
            }
            catch (OperationCanceledException)
            {
                databaseError = "Database connection timed out";
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