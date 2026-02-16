using System.Text.Json;
using AbySalto.Mid.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AbySalto.Mid.WebApi.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;

            var (status, title, detail, errorCode) = ex switch
            {
                AppException app => (app.StatusCode, "Request failed", app.Message, app.ErrorCode),
                InvalidOperationException ioe => (400, "Invalid operation", ioe.Message, null),
                ArgumentException ae => (400, "Invalid argument", ae.Message, null),
                UnauthorizedAccessException => (401, "Unauthorized", "Authentication is required.", null),
                _ => (500, "Unexpected error", "An unexpected error occurred.", null)
            };

            if (status >= 500)
                logger.LogError(ex, "Unhandled exception. TraceId={traceId}", traceId);
            else
                logger.LogWarning(ex, "Handled exception. TraceId={traceId}", traceId);

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;
            if (!string.IsNullOrWhiteSpace(errorCode))
                problem.Extensions["errorCode"] = errorCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
        }
    }
}