using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace UserManagementAPI.Middleware
{
  public class AuthenticationMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _env;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger, IConfiguration configuration, IHostEnvironment env)
    {
      _next = next;
      _logger = logger;
      _configuration = configuration;
      _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      // Allow swagger and static files and development when API key not configured
      var path = context.Request.Path.Value ?? string.Empty;
      if (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico"))
      {
        await _next(context);
        return;
      }

      var configuredKey = _configuration["ApiKey"];
      if (string.IsNullOrEmpty(configuredKey))
      {
        // No API key configured; skip authentication (useful for local/dev)
        await _next(context);
        return;
      }

      if (!context.Request.Headers.TryGetValue("X-API-KEY", out var providedKey) || string.IsNullOrEmpty(providedKey))
      {
        _logger.LogWarning("No API key provided for request {path}", path);
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Missing API key");
        return;
      }

      if (!string.Equals(providedKey, configuredKey))
      {
        _logger.LogWarning("Invalid API key for request {path}", path);
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Invalid API key");
        return;
      }

      // authenticated
      await _next(context);
    }
  }
}
