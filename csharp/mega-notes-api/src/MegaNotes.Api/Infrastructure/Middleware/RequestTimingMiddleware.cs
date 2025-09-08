namespace MegaNotes.Api.Infrastructure.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;
    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next; _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await _next(ctx);
        sw.Stop();
        _logger.LogInformation("{Method} {Path} -> {StatusCode} in {Elapsed}ms",
            ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}
