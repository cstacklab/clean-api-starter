# CleanApiStarter

A Clean Architecture API starter template for .NET, Aspire, PostgreSQL, and OpenTelemetry.

The API uses Minimal APIs with endpoint group classes. `CleanApiStarter.AspNetCore` discovers endpoint groups from the API assembly so `Program.cs` stays small while routes remain easy to find.

API versions are selected with the required `X-Api-Version` request header. Endpoint groups live under version folders such as `Endpoints/V1` and `Endpoints/V2`.

## Learn OpenTelemetry with .NET Aspire

This solution can run the existing API with a local PostgreSQL container through .NET Aspire. Aspire also provides the local dashboard where you can inspect logs, traces, metrics, and resource health.

### Run with Aspire

```bash
dotnet run --project src/CleanApiStarter.AppHost/CleanApiStarter.AppHost.csproj
```

Open the Aspire dashboard URL printed in the terminal. The AppHost starts:

- `api`: the ASP.NET Core API
- `postgres`: a local PostgreSQL container
- `postgres`: the API database
- `pgAdmin`: a browser UI for inspecting PostgreSQL

The API receives its `postgres` connection string from Aspire, which binds to `ConnectionStrings:Postgres`. Running the API directly uses `ConnectionStrings:Postgres` from `src/CleanApiStarter.Api/appsettings.Development.json`, which matches the existing `docker-compose.yml`.

Database scripts live in `database`. Aspire and Docker Compose copy `database/migrations` into the Postgres container init folder. Docker runs them only when the Postgres data directory is created for the first time. Because the AppHost uses the persistent `clean-api-starter-postgres-data` volume, delete that Docker volume if you need to replay the scripts from scratch.

- `database/migrations`: versioned schema scripts that can also be reused by CI/CD migration tools such as Flyway.

### What to Observe

After the dashboard is running, send requests to the API through Scalar or another HTTP client. In the Aspire dashboard:

- Logs show structured application and framework log entries. The word use cases emit fields such as `WordId`, `SynonymCount`, `WordCount`, and `UpdateSucceeded`.
- Request logs show method, path, response status code, and duration without logging request or response bodies. `CleanApiStarter.AspNetCore` keeps the `Microsoft.AspNetCore.HttpLogging` category at `Information` even when broader ASP.NET Core logs are filtered to `Warning`.
- Traces show incoming HTTP requests and PostgreSQL commands from Npgsql.
- Metrics show ASP.NET Core, HTTP client, and .NET runtime measurements.
- `/version` exposes the running application version.
- Health checks expose `/health` and `/alive` in Development.

### Coding Conventions

Cancellation tokens are explicit at application and infrastructure boundaries. Do not use `CancellationToken cancellationToken = default` in service or repository contracts. API actions should accept a `CancellationToken` parameter and pass it through the application and repository calls.

Use structured logging message templates instead of interpolated log strings. Prefer stable property names such as `{WordId}` or `{WordCount}` so Aspire and OpenTelemetry can index and filter them.

Configuration classes live in `CleanApiStarter.Configuration`. Register root settings once with `AddAppSettings(builder.Configuration)`, then inject `AppSettings` directly where configuration values are needed.

### Run with Docker Compose Only

If you want to run PostgreSQL without Aspire:

```bash
docker compose up -d
dotnet run --project src/CleanApiStarter.Api/CleanApiStarter.Api.csproj
```

### Troubleshooting Aspire Certificates

If the Aspire dashboard logs an `UntrustedRoot` error while calling gRPC services, or the AppHost prints `No trusted Aspire development certificate was found`, trust the local ASP.NET Core development certificate:

```bash
dotnet dev-certs https --check --trust
```

If the certificate is missing or untrusted, reset it and trust it again:

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
dotnet dev-certs https --check --trust
```

On macOS, accept the Keychain prompt and enter your password if requested. Then close and reopen the browser, and start Aspire again:

```bash
dotnet run --project src/CleanApiStarter.AppHost/CleanApiStarter.AppHost.csproj
```
