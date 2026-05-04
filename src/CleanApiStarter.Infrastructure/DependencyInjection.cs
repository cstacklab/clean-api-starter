namespace CleanApiStarter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            AppSettings appSettings = serviceProvider.GetRequiredService<AppSettings>();
            options.UseNpgsql(appSettings.ConnectionStrings.Postgres);
        });

        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<IAuthService, GoogleAuthService>();
        services.AddScoped<IProjectRepository, ProjectRepository>();

        return services;
    }
}
