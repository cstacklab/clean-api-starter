namespace CleanApiStarter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDatabaseConnectionFactory, PostgresConnectionFactory>();
        services.AddScoped<IWordRepository, WordRepository>();

        return services;
    }
}
