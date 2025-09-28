using System.Reflection.Metadata;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.Exceptions;

namespace Payments.Api.ExceptionHandling;

public class GlobalExceptionHandler: IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _handlers;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
        _handlers = new()
        {
            { typeof(NotFoundException), HandleNotFoundAsync },
            { typeof(ValidationException), HandleValidationAsync },
            { typeof(Exception), HandleExceptionAsync }
        };
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var type = exception.GetType();
        if (!_handlers.TryGetValue(type, out var handleAsync)) 
            return false;
        
        await handleAsync(httpContext, exception);
        return true;
    }

    private async Task HandleAsync(
        HttpContext httpContext, 
        string errorType, 
        int statusCode, 
        string messageForClient,
        string? additionalInfoForServer = null)
    {
        var additionalInfo = additionalInfoForServer is null ? string.Empty : $" {additionalInfoForServer}";
        _logger.LogError("{MessageForClient}.{AdditionalInfo}", messageForClient, additionalInfo);
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Detail = messageForClient,
            Status = statusCode,
            Instance = httpContext.Request.Path,
            Type = errorType
        });
    }
    
    private async Task HandleNotFoundAsync(HttpContext httpContext, Exception exception) 
        => await HandleAsync(
            httpContext, 
            "Not found", 
            StatusCodes.Status404NotFound,
            exception.Message);

    private async Task HandleValidationAsync(HttpContext httpContext, Exception exception)
    {
        var ex = (ValidationException)exception;
        await HandleAsync(
            httpContext,
            "Validation error",
            StatusCodes.Status400BadRequest,
            exception.Message,
            additionalInfoForServer: ex.EntityStateInfo?.ToString());
    }
    
    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        => await HandleAsync(
            httpContext,
            "Internal server error", 
            StatusCodes.Status500InternalServerError,
            exception.Message);
}