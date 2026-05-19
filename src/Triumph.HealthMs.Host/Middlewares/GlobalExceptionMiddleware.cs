namespace Triumph.HealthMs.Host.Middlewares;

public sealed class GlobalExceptionMiddleware : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;
        
        BaseResponse<object> response;
        int statusCode;

        switch (exception)
        {
            case UnauthorizedAccessException ex:
                statusCode = StatusCodes.Status401Unauthorized;
                response = new BaseResponse<object>
                {
                    Message = "Unauthorized",
                    Errors = [ex.Message, $"Trace Id: {traceId}"],
                    Status = 401
                };
                break;
            default:
                _logger.LogCritical(exception, "An internal error occured at {Timespan}, with TraceId of {TraceId}", DateTimeOffset.UtcNow, traceId);
                statusCode = StatusCodes.Status500InternalServerError;
                response = new BaseResponse<object>
                {
                    Message = "Internal Server Error",
                    Errors = ["An internal error occured", $"Trace Id: {traceId}"],
                    Status = 500
                };
                break;
        }
        
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}