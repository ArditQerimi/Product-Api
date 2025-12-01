
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var traceId = Guid.NewGuid().ToString();
        var timestamp = DateTime.UtcNow.ToString("o");

        httpContext.Response.StatusCode = exception switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new
        {
            Title = "Server error",
            Type = exception.GetType().Name,
            Detail = exception.Message,
            Status = httpContext.Response.StatusCode,
            TraceId = traceId,
            Method = httpContext.Request.Method,
            Timestamp = timestamp,
            Path = httpContext.Request.Path.Value,
            RequestId = httpContext.TraceIdentifier
        };

        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}