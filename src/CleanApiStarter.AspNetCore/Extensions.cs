namespace CleanApiStarter.AspNetCore;

public static partial class Extensions
{
    public static WebApplicationBuilder AddAspNetCoreDefaults(this WebApplicationBuilder builder)
    {
        builder.Host.UseDefaultServiceProvider((_, options) =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        builder.ConfigureOpenTelemetry();
        builder.AddSecurityDefaults();
        builder.AddProblemDetailsDefaults();
        builder.AddApiVersioningDefaults();
        builder.AddAuthenticationDefaults();
        builder.AddResponseCompressionDefaults();
        builder.AddHttpLoggingDefaults();
        builder.AddServiceDiscoveryDefaults();
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication UseAspNetCoreDefaults(this WebApplication app)
    {
        app.UseMiddleware<RequestIdMiddleware>();
        app.UseExceptionHandler();
        app.UseResponseCompression();
        app.UseAuthentication();
        app.UseHttpLogging();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapGet("/version", () =>
            {
                Assembly assembly = Assembly.GetEntryAssembly() ?? typeof(Extensions).Assembly;
                AssemblyInformationalVersionAttribute? informationalVersion = assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                return Results.Ok(new
                {
                    Application = app.Environment.ApplicationName,
                    Version = assembly.GetName().Version?.ToString(),
                    informationalVersion?.InformationalVersion
                });
            })
            .WithName("GetVersion")
            .WithTags("Version");

        if (app.Environment.IsDevelopment())
        {
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains("live")
            });
        }

        return app;
    }
}