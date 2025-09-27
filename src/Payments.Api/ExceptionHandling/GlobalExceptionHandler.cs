using System.Reflection.Metadata;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.Exceptions;

namespace Payments.Api.ExceptionHandling;

public class GlobalExceptionHandler : IExceptionHandler
{
    private static readonly Dictionary<Type, Func<HttpContext, Exception, Task>> Handlers = new()
    {
        { typeof(NotFoundException), HandleNotFoundAsync },
        { typeof(DataFormatException), HandleDataFormatAsync },
        { typeof(ValidationException), HandleValidationAsync },
        { typeof(Exception), HandleExceptionAsync }
    };
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var type = exception.GetType();
        if (!Handlers.TryGetValue(type, out var handleAsync)) 
            return false;
        
        await handleAsync(httpContext, exception);
        return true;
    }

    private static async Task HandleAsync(
        HttpContext httpContext, string errorType, int statusCode, string? message = null)
    {
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Detail = message,
            Status = statusCode,
            Instance = httpContext.Request.Path,
            Type = errorType
        });
    }
    
    private static async Task HandleNotFoundAsync(HttpContext httpContext, Exception exception) 
        => await HandleAsync(
            httpContext, 
            "Not found", 
            StatusCodes.Status404NotFound,
            message: exception.Message);

    private static async Task HandleDataFormatAsync(HttpContext httpContext, Exception exception)
        => await HandleAsync(
            httpContext,
            "Invalid data format", 
            StatusCodes.Status409Conflict,
            message: exception.Message);

    private static async Task HandleValidationAsync(HttpContext httpContext, Exception exception)
        => await HandleAsync(
            httpContext,
            "Entity state conflict", 
            StatusCodes.Status409Conflict,
            message: exception.Message);
    
    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        => await HandleAsync(
            httpContext,
            "Internal server error", 
            StatusCodes.Status500InternalServerError);
}