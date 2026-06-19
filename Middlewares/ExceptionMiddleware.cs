using System.Net;
using System.Text.Json;
using CareerGuidance.Exceptions;

namespace CareerGuidance.Middlewares;

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
            _logger.LogError(ex, "An exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case NotFoundException notFoundEx:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = notFoundEx.Message,
                    ErrorCode = notFoundEx.ErrorCode,
                    StatusCode = 404
                };
                break;

            case ValidationException validationEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = validationEx.Message,
                    ErrorCode = validationEx.ErrorCode,
                    StatusCode = 400,
                    Errors = validationEx.Errors
                };
                break;

            case UnauthorizedException unauthorizedEx:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = unauthorizedEx.Message,
                    ErrorCode = unauthorizedEx.ErrorCode,
                    StatusCode = 401
                };
                break;

            case ForbiddenException forbiddenEx:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = forbiddenEx.Message,
                    ErrorCode = forbiddenEx.ErrorCode,
                    StatusCode = 403
                };
                break;

            case BusinessException businessEx:
                context.Response.StatusCode = businessEx.StatusCode ?? StatusCodes.Status400BadRequest;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = businessEx.Message,
                    ErrorCode = businessEx.ErrorCode,
                    StatusCode = businessEx.StatusCode ?? 400
                };
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ErrorResponse
                {
                    Success = false,
                    Message = "An internal server error occurred",
                    ErrorCode = "INTERNAL_SERVER_ERROR",
                    StatusCode = 500
                };
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public int StatusCode { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}
