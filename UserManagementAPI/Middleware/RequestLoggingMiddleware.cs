using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Middleware
{
  public class RequestLoggingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var sw = Stopwatch.StartNew();
      _logger.LogInformation("Incoming request {method} {path}", context.Request.Method, context.Request.Path);

      await _next(context);

      sw.Stop();
      _logger.LogInformation("Handled {method} {path} responded {status} in {ms}ms",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode,
        sw.ElapsedMilliseconds);
    }
  }
}
