namespace CleanApiStarter.AspNetCoreDefaults;

public sealed class RequestIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Request-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = GetRequestId(context);

            return Task.CompletedTask;
        });

        await next(context);
    }

    private static string GetRequestId(HttpContext context)
    {
        return Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
    }
}
