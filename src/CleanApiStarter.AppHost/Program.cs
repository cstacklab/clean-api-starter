IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

string databasePath = Path.GetFullPath(Path.Combine(builder.AppHostDirectory, "..", "..", "database"));
string databaseMigrationsPath = Path.Combine(databasePath, "migrations");

IResourceBuilder<PostgresServerResource> postgresServer = builder.AddPostgres("postgres-server")
    .WithImageTag("latest")
    .WithVolume("clean-api-starter-postgres-data", "/var/lib/postgresql")
    .WithInitFiles(databaseMigrationsPath)
    .WithPgAdmin();

IResourceBuilder<PostgresDatabaseResource> database = postgresServer.AddDatabase("postgres")
    .WithCreationScript("SELECT 1;");

builder.AddProject<Projects.CleanApiStarter_Api>("api")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();