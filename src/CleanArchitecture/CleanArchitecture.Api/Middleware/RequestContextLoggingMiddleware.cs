using Serilog.Context;

public class RequestContextLoggingMiddleware
{
    private readonly string _correlationIdHeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public RequestContextLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext httpContext)
    {
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(httpContext)))
        {
            return _next(httpContext);
        }
    }

    private object? GetCorrelationId(HttpContext httpContext)
    {
        httpContext.Request.Headers.TryGetValue(
            _correlationIdHeaderName,
            out var correlationId);

        return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;
    }
}
