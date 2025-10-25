using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Models;

namespace UserManagementAPI.Middleware
{
  public class ValidationMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationMiddleware> _logger;

    public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var path = context.Request.Path.Value ?? string.Empty;
      var method = context.Request.Method;

      // Only validate POST/PUT to /api/users
      if ((method == HttpMethods.Post || method == HttpMethods.Put) && path.StartsWith("/api/users"))
      {
        // Ensure we have a body
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        if (string.IsNullOrWhiteSpace(body))
        {
          context.Response.StatusCode = StatusCodes.Status400BadRequest;
          await context.Response.WriteAsync("Request body is required");
          return;
        }

        User? user = null;
        try
        {
          user = JsonSerializer.Deserialize<User>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
          _logger.LogWarning(ex, "Malformed JSON in request to {path}", path);
          context.Response.StatusCode = StatusCodes.Status400BadRequest;
          await context.Response.WriteAsync("Malformed JSON");
          return;
        }

        if (user == null)
        {
          context.Response.StatusCode = StatusCodes.Status400BadRequest;
          await context.Response.WriteAsync("Invalid user payload");
          return;
        }

        var validationResults = new System.Collections.Generic.List<ValidationResult>();
        var validationContext = new ValidationContext(user, serviceProvider: null, items: null);
        // Validate required fields
        Validator.TryValidateObject(user, validationContext, validationResults, validateAllProperties: true);

        // Additional manual checks if needed
        if (string.IsNullOrWhiteSpace(user.FirstName)) validationResults.Add(new ValidationResult("FirstName is required", new[] { "FirstName" }));
        if (string.IsNullOrWhiteSpace(user.LastName)) validationResults.Add(new ValidationResult("LastName is required", new[] { "LastName" }));
        if (string.IsNullOrWhiteSpace(user.Email)) validationResults.Add(new ValidationResult("Email is required", new[] { "Email" }));
        else
        {
          var emailAttr = new EmailAddressAttribute();
          if (!emailAttr.IsValid(user.Email)) validationResults.Add(new ValidationResult("Email is not a valid email address", new[] { "Email" }));
        }

        if (validationResults.Count > 0)
        {
          context.Response.StatusCode = StatusCodes.Status400BadRequest;
          context.Response.ContentType = "application/json";
          var errors = new System.Collections.Generic.Dictionary<string, string[]>();
          foreach (var vr in validationResults)
          {
            foreach (var m in vr.MemberNames)
            {
              if (!errors.ContainsKey(m)) errors[m] = new string[] { vr.ErrorMessage ?? "" };
              else
              {
                var list = new System.Collections.Generic.List<string>(errors[m]) { vr.ErrorMessage ?? "" };
                errors[m] = list.ToArray();
              }
            }
          }

          var result = JsonSerializer.Serialize(new { errors });
          await context.Response.WriteAsync(result);
          return;
        }
      }

      await _next(context);
    }
  }
}
