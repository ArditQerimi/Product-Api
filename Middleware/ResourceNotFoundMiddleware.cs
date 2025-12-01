

using System.Text.Json;

public class ResourceNotFoundMiddleware
{
    private readonly RequestDelegate _next;

    public ResourceNotFoundMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
        {
            var errorDetails = new
            {
                error = "Resource not found",
                status = 404,
                method = context.Request.Method,
                path = context.Request.Path.Value,
                queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                timestamp = DateTime.UtcNow,
                details = "The requested resource does not exist on the server."
            };

            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(errorDetails, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}