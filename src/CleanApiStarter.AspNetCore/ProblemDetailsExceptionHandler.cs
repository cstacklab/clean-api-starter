namespace CleanApiStarter.AspNetCore;

public sealed class ProblemDetailsExceptionHandler(ILogger<ProblemDetailsExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        (int statusCode, ProblemDetails problemDetails) = exception switch
        {
            BadHttpRequestException badRequestException => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Detail = badRequestException.Message
                }),
            ArgumentException argumentException => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Detail = argumentException.Message
                }),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
                }),
            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                    Detail = "An unexpected error occurred."
                })
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Unhandled exception while processing {RequestMethod} {RequestPath}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(
                exception,
                "Handled exception with status {StatusCode} while processing {RequestMethod} {RequestPath}",
                statusCode,
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}