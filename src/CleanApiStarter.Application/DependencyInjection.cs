namespace CleanApiStarter.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWordService, WordService>();
        return services;
    }
}