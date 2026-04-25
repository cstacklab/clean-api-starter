# Agent Instructions

This repository is a Clean Architecture API starter template named `CleanApiStarter`.

## Naming

- Use `CleanApiStarter` for solution, project, assembly, and namespace naming.
- Do not introduce `CleanArchitecture` namespaces or assembly names.
- Keep project names fully qualified:
  - `CleanApiStarter.Api`
  - `CleanApiStarter.Application`
  - `CleanApiStarter.Domain`
  - `CleanApiStarter.Infrastructure`
  - `CleanApiStarter.AppHost`
  - `CleanApiStarter.AspNetCore`
  - `CleanApiStarter.UnitTests`

## Solution Structure

- Keep clean architecture application layers under the `/src/` solution folder:
  - API
  - Application
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
  - `Infrastructure` references `Application`.
  - `Api` composes `Application`, `Infrastructure`, and `AspNetCore`.
- Keep repository interfaces in `Application`, not `Domain`.
- Keep database implementation details in `Infrastructure`.
- Do not move `IDatabaseConnectionFactory` into `Application` or `Domain`.
- Keep domain models persistence-agnostic. For example, `Word.Synonyms` stays `List<string>`, even though Postgres stores it as `jsonb`.
- Private database projection types such as `WordRow` may live inside their repository when they are only used there.

## API and Application Conventions

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
- AppHost should expose the API connection as `ConnectionStrings__postgres`.
- `docker-compose.yml` should use `POSTGRES_DB=postgres`.
- Database schema scripts live in `database/migrations`, for example:
  - `database/migrations/V001__create_words_table.sql`
- Do not add API startup database initialization such as `DbInitializer` or DbUp calls. Aspire/Docker init scripts own local schema creation.
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
  - health endpoints
  - service discovery
  - default HTTP client resilience
- API `Program.cs` should stay small and call:
  - `builder.AddAspNetCoreDefaults();`
  - `app.UseAspNetCoreDefaults();`
  - `app.MapDefaultEndpoints();`
- Shared middleware such as HTTP request logging belongs in `CleanApiStarter.AspNetCore`, not duplicated inside each API project.
- Keep OpenTelemetry logs configured to include scopes, formatted messages, and parsed state values so structured message-template properties show up in Aspire.
- Use structured logging message templates instead of interpolated log strings. Prefer stable property names like `{WordId}` and `{WordCount}`.

## API Style

- Prefer Minimal APIs for this template.
- Keep `Program.cs` small by placing route groups in endpoint mapping classes such as `Api/Endpoints/WordEndpoints.cs`.
- Do not reintroduce MVC controllers unless the template intentionally changes direction.

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
