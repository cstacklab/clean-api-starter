namespace CleanApiStarter.AspNetCore;

public static partial class Extensions
{
    private static void AddSecurityDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(
            options => options.AddServerHeader = false);
    }

    private static void AddProblemDetailsDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
        builder.Services.AddProblemDetails();
    }

    private static void AddApiVersioningDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = false;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
        });
    }

    private static void AddHttpLoggingDefaults(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields =
                HttpLoggingFields.RequestMethod |
                HttpLoggingFields.RequestPath |
                HttpLoggingFields.ResponseStatusCode |
                HttpLoggingFields.Duration;
        });
    }

    private static void AddServiceDiscoveryDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });
    }
}
