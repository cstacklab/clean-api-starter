using Microsoft.AspNetCore.Authorization;

namespace CleanApiStarter.AspNetCoreDefaults;

public static partial class AspNetCoreDefaultsExtensions
{
    private static void AddSecurityDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(
            options => options.AddServerHeader = false);

        builder.Services.AddHttpContextAccessor();
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
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
        })
        .AddOpenApi();
    }

    private static void AddAuthenticationDefaults(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        // The JWT bearer token validation parameters are bound from application
        // settings by the composition root (the Api project), so these defaults
        // stay free of any app-specific configuration types.
    }

    private static void AddHttpLoggingDefaults(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

        builder.Services.AddHttpLogging(options =>
        {
            options.CombineLogs = true;
            options.LoggingFields =
                HttpLoggingFields.RequestMethod |
                HttpLoggingFields.RequestPath |
                HttpLoggingFields.ResponseStatusCode |
                HttpLoggingFields.Duration;
        });

        builder.Services.AddHttpLoggingInterceptor<UserIdHttpLoggingInterceptor>();
    }

    private static void AddResponseCompressionDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            [
                "application/json",
                "application/problem+json"
            ]);
        });

        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
            options.Level = CompressionLevel.Fastest);

        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            options.Level = CompressionLevel.Fastest);
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
