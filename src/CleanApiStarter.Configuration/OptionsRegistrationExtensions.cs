namespace CleanApiStarter.Configuration;

public static class OptionsRegistrationExtensions
{
    public static IServiceCollection AddAppSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<AppSettings>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value);

        return services;
    }
}