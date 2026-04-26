# Agent Instructions

This repository is a Clean Architecture API starter template named `CleanApiStarter`.

## Naming

- Use `CleanApiStarter` for solution, project, assembly, and namespace naming.
- Do not introduce `CleanArchitecture` namespaces or assembly names.
- Keep project names fully qualified:
  - `CleanApiStarter.Api`
  - `CleanApiStarter.Application`
  - `CleanApiStarter.Configuration`
  - `CleanApiStarter.Domain`
  - `CleanApiStarter.Infrastructure`
  - `CleanApiStarter.AppHost`
  - `CleanApiStarter.AspNetCore`
  - `CleanApiStarter.UnitTests`

## Solution Structure

- Keep clean architecture application layers under the `/src/` solution folder:
  - API
  - Application
  - Configuration
  - Domain
  - Infrastructure
- Keep Aspire/runtime support projects under the `/src/Common/` solution folder in `CleanApiStarter.slnx`:
  - AppHost
  - AspNetCore
- Keep database scripts under top-level `database/migrations`.
- Keep important root files in solution items, including `README.md`, `docker-compose.yml`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, `.gitignore`, and `global.json`.

## Clean Architecture Rules

- Dependencies point inward:
  - `Domain` references nothing.
  - `Application` references `Domain`.
  - `Configuration` references no application layers and contains only plain options classes plus options registration helpers.
  - `Infrastructure` references `Application` and `Configuration`.
  - `Api` composes `Application`, `Infrastructure`, `Configuration`, and `AspNetCore`.
- Keep repository interfaces in `Application`, not `Domain`.
- Keep database implementation details in `Infrastructure`.
- Keep domain models persistence-agnostic. For example, `Word.Synonyms` stays `List<string>`, even though Postgres stores it as `jsonb`.
- Use EF Core through `ApplicationDbContext` for application persistence. Do not reintroduce Dapper for the default template data access path.
- Keep `ApplicationDbContext` in `CleanApiStarter.Infrastructure/Persistence`, not in `Identity`. The context owns both application entities and Identity storage, so it is a persistence concern.
- Keep EF Core fluent API entity maps in `CleanApiStarter.Infrastructure/Persistence/Configuration` using `IEntityTypeConfiguration<T>`. Do not put entity mapping logic directly inside `ApplicationDbContext` unless there is a very small one-off reason.
- Keep Identity-specific classes, such as `ApplicationUser` and auth services, under `CleanApiStarter.Infrastructure/Identity`.

## API and Application Conventions

- Organize the `Application` project by feature. Put feature-specific contracts, DTOs, and application services under folders such as `Application/Features/Words` or `Application/Features/Auth`.
- Do not create generic `Application/Services` or `Application/Models` folders for feature-specific code.
- Keep cross-cutting application abstractions under `Application/Common`, for example `Application/Common/Interfaces`.
- Use Scalar, not Swagger/Swashbuckle.
- Use `Microsoft.AspNetCore.OpenApi` with:
  - `builder.Services.AddOpenApi();`
  - `app.MapOpenApi();`
  - `app.MapScalarApiReference();`
- Do not add `Swashbuckle.AspNetCore`.
- Cancellation tokens are explicit:
  - Do not use `CancellationToken cancellationToken = default` in service or repository contracts.
  - API actions should accept `CancellationToken cancellationToken` and pass it through.
- Use explicit local types. The `.editorconfig` prefers explicit types over `var`.
- Use project-level `GlobalUsings.cs`; avoid adding file-level `using` directives unless there is a very specific reason.

## Database and Aspire

- Use a single Postgres database: `postgres`.
- Do not create or reference a separate `wordlibrary` database.
- AppHost should expose the API connection from the `postgres` resource, and application settings should read it through `ConnectionStrings:Postgres`.
- `docker-compose.yml` should use `POSTGRES_DB=postgres`.
- Database schema scripts live in `database/migrations`, for example:
  - `database/migrations/V001__create_words_table.sql`
- Do not add API startup database initialization such as `DbInitializer` or DbUp calls. Aspire/Docker init scripts own local schema creation.
- EF Core is used for application data access and Identity storage, but schema creation still belongs to the SQL scripts in `database/migrations`.
- Docker Postgres init scripts run only on first volume creation. If scripts need to replay, delete the old volume.
- This repo uses `postgres:latest`. Because Postgres 18+ expects the data volume mounted at `/var/lib/postgresql`, do not mount the volume at `/var/lib/postgresql/data`.
- In Aspire, use a server resource name that does not conflict with the database resource name, for example:
  - server resource: `postgres-server`
  - database resource: `postgres`
- Aspire Postgres should use a volume mounted at `/var/lib/postgresql`.

## ASP.NET Core Defaults

- Keep `CleanApiStarter.AspNetCore`.
- It centralizes Aspire-friendly runtime defaults:
  - OpenTelemetry traces, metrics, logs, and OTLP export
  - problem-details exception handling
  - `/version` endpoint
  - health endpoints
  - service discovery
  - default HTTP client resilience
  - security defaults such as removing the Kestrel `Server` response header
- API `Program.cs` should stay small and call:
  - `builder.AddAspNetCoreDefaults();`
  - `app.UseAspNetCoreDefaults();`
  - `app.MapDefaultEndpoints();`
- Shared middleware such as HTTP request logging belongs in `CleanApiStarter.AspNetCore`, not duplicated inside each API project.
- Keep OpenTelemetry logs configured to include scopes, formatted messages, and parsed state values so structured message-template properties show up in Aspire.
- Use structured logging message templates instead of interpolated log strings. Prefer stable property names like `{WordId}` and `{WordCount}`.
- Register root settings once with `AddAppSettings(builder.Configuration)`, then inject `AppSettings` directly when services need configuration values.
- Do not add generic options registration helpers until the template has multiple real options sections that need them.
- Do not create a broad `Shared` project. Keep cross-project settings in `CleanApiStarter.Configuration`.
- Keep dependency injection validation enabled with `ValidateOnBuild` and `ValidateScopes`.

## API Style

- Prefer Minimal APIs for this template.
- Keep `Program.cs` small by placing route groups in endpoint group classes under version folders such as `Api/Endpoints/V1/Words.cs`.
- Endpoint group classes should implement `IEndpointGroup` from `CleanApiStarter.AspNetCore` and be mapped through `app.MapEndpoints(Assembly.GetExecutingAssembly());`.
- Use built-in Minimal API mapping methods with explicit `.WithName(...)`; do not add custom `MapGet`/`MapPost` overloads that shadow framework methods.
- API versions are selected with the optional `X-Api-Version` request header. Missing versions default to v1.
- Endpoint groups should declare `MajorVersion` to match their folder, for example `V1` uses `1` and `V2` uses `2`.
- Endpoint names must be globally unique across versions. Prefer names suffixed with the version, such as `GetAllWordsV1` and `GetAllWordsV2`.
- Do not reintroduce MVC controllers unless the template intentionally changes direction.

## Authentication

- Authentication is API-first:
  - clients obtain a Google ID token
  - `POST /api/auth/google` validates it
  - the API issues its own JWT
- Keep JWT bearer authentication setup in `CleanApiStarter.AspNetCore`.
- Keep local user/role storage in ASP.NET Core Identity under `CleanApiStarter.Infrastructure`.
- Do not use cookies as the default API auth mechanism.
- Keep the development Google login helper page unversioned so it can be opened directly in a browser.
- Protected API calls should send `Authorization: Bearer <api-jwt>`. Send `X-Api-Version` only when selecting a non-default API version.

## Packages

- Manage versions centrally in `Directory.Packages.props`.
- Keep `PackageVersion` items sorted alphabetically by `Include`.
- Do not add package versions directly in individual `.csproj` files.

## Verification

- After structural or package changes, run:

```bash
dotnet restore CleanApiStarter.slnx --disable-parallel
dotnet build CleanApiStarter.slnx --no-restore /nr:false -v:minimal
```

- Aspire AppHost builds may need to run outside a sandbox because the Aspire SDK touches local runtime/process resources.
