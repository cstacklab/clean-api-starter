namespace CleanApiStarter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register database connection factory
        string? connectionString =
            configuration.GetConnectionString("postgres") ??
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'postgres' or 'DefaultConnection' is missing.");
        }

        services.AddSingleton<IDatabaseConnectionFactory>(_ => new PostgresConnectionFactory(connectionString!));

        // Register repositories
        services.AddScoped<IWordRepository, WordRepository>();

        return services;
    }
}
