namespace CareerGuidance.Exceptions;

public class BusinessException : Exception
{
    public string? ErrorCode { get; set; }
    public int? StatusCode { get; set; }

    public BusinessException(string message, string? errorCode = null, int statusCode = 400)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

public class NotFoundException : BusinessException
{
    public NotFoundException(string message, string? errorCode = null)
        : base(message, errorCode, 404)
    {
    }
}

public class ValidationException : BusinessException
{
    public Dictionary<string, string[]>? Errors { get; set; }

    public ValidationException(string message, Dictionary<string, string[]>? errors = null)
        : base(message, "VALIDATION_ERROR", 400)
    {
        Errors = errors;
    }
}

public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(string message = "Unauthorized")
        : base(message, "UNAUTHORIZED", 401)
    {
    }
}

public class ForbiddenException : BusinessException
{
    public ForbiddenException(string message = "Forbidden")
        : base(message, "FORBIDDEN", 403)
    {
    }
}
