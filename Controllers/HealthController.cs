using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppBoleteriaApi.Data;

namespace AppBoleteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check realizado");
            return Ok(new
            {
                status = "healthy",
                service = "Boleteria API",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                version = "1.0.0"
            });
        }

        [HttpGet("db")]
        public async Task<IActionResult> CheckDatabase([FromServices] AppDbContext context)
        {
            try
            {
                var canConnect = await context.Database.CanConnectAsync();
                var migrations = await context.Database.GetAppliedMigrationsAsync();

                return Ok(new
                {
                    database = canConnect ? "connected" : "disconnected",
                    applied_migrations = migrations.Count(),
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database");
                return StatusCode(500, new
                {
                    database = "error",
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("detailed")]
        public IActionResult GetDetailed()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "1.0.0";

            return Ok(new
            {
                status = "healthy",
                service = "AppBoleteriaApi",
                version = version,
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                machine = Environment.MachineName,
                os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                dotnet_version = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                memory = GC.GetTotalMemory(false) / 1024 / 1024 + " MB"
            });
        }
    }
}