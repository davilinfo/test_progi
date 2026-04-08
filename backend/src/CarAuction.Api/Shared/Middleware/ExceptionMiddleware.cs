using System.Net;
using System.Text.Json;
using CarAuction.Api.Shared.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Api.Shared.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Resource Not Found", exception.Message),
            DomainException => (HttpStatusCode.BadRequest, "Domain Error", exception.Message),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden", exception.Message),
            ValidationException ve => (HttpStatusCode.BadRequest, "Validation Error",
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Server Error", "An unexpected error occurred.")
        };

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
    }
}
