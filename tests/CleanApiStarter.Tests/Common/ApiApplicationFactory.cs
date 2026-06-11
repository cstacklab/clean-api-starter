namespace CleanApiStarter.Tests.Common;

public sealed class ApiApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private const string TestIssuer = "CleanApiStarter.Tests";
    private const string TestAudience = "CleanApiStarter.Api.Tests";
    private const string TestSigningKey = "integration-tests-signing-key-change-me";

    private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder("postgres:18")
        .WithDatabase("postgres")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async ValueTask InitializeAsync()
    {
        await postgres.StartAsync();
        await RunDatabaseScriptsAsync();
    }

    public string CreateAccessToken(string userId)
    {
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(TestSigningKey));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId),
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, $"{userId}@example.test"),
            new(ClaimTypes.Name, userId),
            new(ClaimTypes.Role, "User")
        ];

        JwtSecurityToken token = new(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            string settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Testing.json");
            Dictionary<string, string?> connectionStringOverride = new()
            {
                ["ConnectionStrings:Postgres"] = postgres.GetConnectionString()
            };

            configurationBuilder.AddJsonFile(settingsPath, optional: false);
            configurationBuilder.AddInMemoryCollection(connectionStringOverride);
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await postgres.DisposeAsync();
    }

    private async Task RunDatabaseScriptsAsync()
    {
        await using NpgsqlConnection connection = new(postgres.GetConnectionString());
        await connection.OpenAsync();

        foreach (string scriptPath in GetDatabaseScriptPaths())
        {
            string sql = await File.ReadAllTextAsync(scriptPath);

            await using NpgsqlCommand command = new(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    private static IEnumerable<string> GetDatabaseScriptPaths()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory != null)
        {
            string migrationsPath = Path.Combine(directory.FullName, "database", "migrations");

            if (Directory.Exists(migrationsPath))
            {
                return Directory.GetFiles(migrationsPath, "*.sql").Order(StringComparer.Ordinal);
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find database/migrations from the test output directory.");
    }
}
