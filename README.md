# CleanApiStarter

`CleanApiStarter` is a Clean Architecture API starter template for .NET 10, ASP.NET Core Minimal APIs, Aspire, PostgreSQL, OpenTelemetry, JWT authentication, API versioning, FluentValidation, Scalar, EF Core, and automated tests.

The project is intentionally both a template and a working reference application. The sample domain is project management: authenticated users can create projects, manage project tasks, filter tasks by status, and complete tasks.

## Features

- Clean Architecture solution structure with separate `Api`, `Application`, `Domain`, `Infrastructure`, `Configuration`, and shared ASP.NET Core defaults projects.
- Minimal APIs organized by endpoint group and API version folders.
- Header-based API versioning with optional `X-Api-Version`; missing versions default to v1.
- Versioned OpenAPI documents shown with Scalar.
- Google ID token sign-in that issues the API's own JWT.
- ASP.NET Core Identity for local users, roles, and external login storage.
- Real JWT bearer authentication for protected APIs.
- EF Core with PostgreSQL for application data and Identity storage.
- PostgreSQL local development through Aspire or Docker Compose.
- Database schema scripts under `database/migrations`.
- OpenTelemetry traces, metrics, and structured logs through Aspire-friendly defaults.
- HTTP request logging with authenticated user id.
- `X-Request-ID` response header containing the current trace id.
- Problem Details exception responses.
- Response compression with Brotli and gzip.
- FluentValidation endpoint validation returning `422 Unprocessable Entity`.
- Result wrappers for collection endpoints: `PaginatedResult<T>` and `ArrayResult<T>`.
- xUnit v3 unit tests with AutoFixture, AutoFixture.AutoNSubstitute, NSubstitute, and Shouldly.
- MSTest API integration tests with Testcontainers for PostgreSQL.
- Local coverage script that generates an HTML report with ReportGenerator.
- GitHub Actions CI with build, test, template verification, and CodeQL security scanning.

## Solution Layout

```text
CleanApiStarter
├── database
│   └── migrations
├── scripts
├── src
│   ├── CleanApiStarter.Api
│   ├── CleanApiStarter.Application
│   ├── CleanApiStarter.Domain
│   ├── CleanApiStarter.Infrastructure
│   └── Common
│       ├── CleanApiStarter.AppHost
│       ├── CleanApiStarter.AspNetCore
│       └── CleanApiStarter.Configuration
└── tests
    ├── CleanApiStarter.Api.IntegrationTests
    ├── CleanApiStarter.Application.UnitTests
    └── CleanApiStarter.Tests
```

Layer dependencies point inward:

- `Domain` references nothing.
- `Application` references `Domain`.
- `Infrastructure` references `Application` and `Configuration`.
- `Api` composes `Application`, `Infrastructure`, `Configuration`, and `AspNetCore`.
- `Configuration` contains plain settings classes and options registration.
- `AspNetCore` contains reusable web/runtime defaults.
- `AppHost` contains Aspire orchestration.

## Requirements

- .NET SDK `10.0.203` or compatible latest feature SDK
- Docker Desktop
- Google Chrome, only for the local coverage script opening the HTML report

The SDK is pinned in `global.json`:

```json
{
  "sdk": {
    "version": "10.0.203",
    "rollForward": "latestFeature"
  }
}
```

## Use As A Template

Install the template package from NuGet:

```bash
dotnet new install CleanApiStarter.Template
```

Create a new solution:

```bash
dotnet new clean-api-starter -n MyProduct
```

The template replaces `CleanApiStarter` in solution, project, file, and namespace names. For example, `CleanApiStarter.Api` becomes `MyProduct.Api`.

While developing the template locally, run:

```bash
scripts/install-template.sh
```

That script:

- uninstalls the previous local template package
- packs the current repo with `CleanApiStarter.nuspec`
- installs the generated local `.nupkg`
- deletes the temporary package from `artifacts`

Then create a local test solution:

```bash
dotnet new clean-api-starter -n DemoProduct
```

## Run With Aspire

```bash
dotnet run --project src/CleanApiStarter.AppHost/CleanApiStarter.AppHost.csproj
```

Aspire starts:

- the API
- a PostgreSQL container using `postgres:latest`
- pgAdmin
- the Aspire dashboard

Open the Aspire dashboard URL printed in the terminal. Use it to inspect resources, logs, traces, metrics, health, and PostgreSQL activity.

The AppHost configures PostgreSQL like this:

- server resource: `postgres-server`
- database resource: `postgres`
- data volume: `clean-api-starter-postgres-data`
- volume mount: `/var/lib/postgresql`
- init scripts: `database/migrations`

PostgreSQL init scripts run only when the database volume is first created. If you change scripts and want them replayed locally, delete the old Docker volume.

## Run With Docker Compose

If you want only PostgreSQL without Aspire:

```bash
docker compose up -d
dotnet run --project src/CleanApiStarter.Api/CleanApiStarter.Api.csproj
```

`docker-compose.yml` starts one database named `postgres` and mounts `database/migrations` into the Postgres init directory.

## Database

Database scripts live here:

```text
database/migrations
├── V001__create_projects_and_tasks_tables.sql
└── V002__create_identity_tables.sql
```

The application uses EF Core through `ApplicationDbContext`, but local schema creation is owned by the SQL scripts. There are no DbUp calls or API startup database initializers.

`ApplicationDbContext` lives in:

```text
src/CleanApiStarter.Infrastructure/Persistence
```

EF Core mappings live in:

```text
src/CleanApiStarter.Infrastructure/Persistence/Configuration
```

## API Style

The API uses Minimal APIs. `Program.cs` stays small and delegates endpoint registration to endpoint group classes.

Endpoint groups live under version folders:

```text
src/CleanApiStarter.Api/Endpoints/V1
src/CleanApiStarter.Api/Endpoints/V2
```

Each endpoint group implements `IEndpointGroup` from `CleanApiStarter.AspNetCore` and is discovered through:

```csharp
app.MapEndpoints(Assembly.GetExecutingAssembly());
```

Endpoint names are globally unique across versions, for example:

```text
GetProjectsV1
GetProjectsV2
```

## API Versioning

API versioning uses header-based version selection:

```http
X-Api-Version: 1.0
```

The header is optional. Requests without `X-Api-Version` use v1.

The project uses the .NET 10 API versioning/OpenAPI package setup:

- `Asp.Versioning.Http`
- `Asp.Versioning.Mvc.ApiExplorer`
- `Asp.Versioning.OpenApi`
- `Microsoft.AspNetCore.OpenApi`

OpenAPI documents are generated per version and displayed in Scalar.

## Scalar

This project uses Scalar instead of Swagger/Swashbuckle.

When running in Development, OpenAPI and Scalar are mapped by the API:

- versioned OpenAPI documents
- Scalar API reference with version selection

## Authentication

Authentication is API-first.

The intended production flow is:

1. A client obtains a Google ID token.
2. The client sends it to `POST /api/auth/google`.
3. The API validates the Google ID token.
4. The API creates or updates the local ASP.NET Core Identity user.
5. The API issues its own JWT.
6. Protected API calls use:

```http
Authorization: Bearer <api-jwt>
```

The API does not use cookies as the default authentication mechanism.

### Google Client ID

Create an OAuth client in Google Cloud Console and set the client id with user secrets:

```bash
dotnet user-secrets set "Authentication:Google:ClientId" "<google-client-id>" --project src/CleanApiStarter.Api/CleanApiStarter.Api.csproj
```

For local browser testing, open the development helper page:

```text
https://localhost:7285/auth/google-login
```

The helper page signs in with Google, calls the API token endpoint, displays the API JWT, and includes a copy button.

## Authorization

Protected endpoint groups call `RequireAuthorization()` once at the group level, so individual project/task endpoints do not need repeated authorization declarations.

Project authorization rules are implemented in the application service and repository queries:

- users can only see projects they belong to
- users can only see and manage tasks inside projects they belong to
- project deletion is owner-only

## Application Features

The reference feature is project and task management.

Projects:

- create project
- list current user's projects
- get project by id
- delete project as owner

Tasks:

- create task under a project
- list tasks with `limit` and `offset`
- filter tasks by status
- get task by id
- update task
- complete task
- delete task

The application project is organized by feature:

```text
src/CleanApiStarter.Application/Features/Auth
src/CleanApiStarter.Application/Features/Projects
```

Cross-cutting abstractions live under:

```text
src/CleanApiStarter.Application/Common
```

## Response Shapes

Single-resource endpoints return the resource DTO directly.

Paginated endpoints return `PaginatedResult<T>`:

```json
{
  "items": [],
  "limit": 20,
  "offset": 0,
  "totalCount": 0,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

Non-paginated collection endpoints should return `ArrayResult<T>`:

```json
{
  "items": [],
  "count": 0
}
```

## Validation

Request validation uses FluentValidation, not DataAnnotations.

Validators live beside request models in the `Application` project. The shared Minimal API validation filter lives in `CleanApiStarter.AspNetCore`.

Validation failures return:

```http
422 Unprocessable Entity
```

## Configuration

Configuration classes live in `CleanApiStarter.Configuration`.

The root configuration object is:

```csharp
AppSettings
```

It includes:

- `ConnectionStrings.Postgres`
- `Authentication.Google.ClientId`
- `Authentication.Jwt.Issuer`
- `Authentication.Jwt.Audience`
- `Authentication.Jwt.SigningKey`
- `Authentication.Jwt.ExpirationMinutes`

The API registers settings once:

```csharp
builder.Services.AddAppSettings(builder.Configuration);
```

Services can inject `AppSettings` directly.

## Observability

`CleanApiStarter.AspNetCore` centralizes runtime defaults:

- OpenTelemetry traces
- OpenTelemetry metrics
- OpenTelemetry logs
- OTLP export for Aspire
- structured log formatting
- HTTP request logging
- health checks
- service discovery
- HTTP client resilience
- problem details
- response compression
- request id header
- Kestrel `Server` header removal

When the API runs under Aspire, inspect the dashboard for:

- API request traces
- Npgsql database traces
- structured logs
- request logs with `UserId`
- runtime metrics
- resource health

Every response includes:

```http
X-Request-ID: <trace-id>
```

Use this value to correlate client responses with logs and traces.

## Health And Version Endpoints

The shared ASP.NET Core defaults map:

```text
/health
/alive
/version
```

`/version` exposes the running application version.

## Testing

### Unit Tests

Application unit tests use:

- xUnit v3
- AutoFixture.xUnit3
- AutoFixture.AutoNSubstitute
- NSubstitute
- Shouldly

Common test helpers live in:

```text
tests/CleanApiStarter.Tests
```

Current reusable helpers:

- `AutoNSubstituteDataAttribute`
- `ApiApplicationFactory<TProgram>`

Test names follow:

```text
UnitOfWork_StateUnderTest_ExpectedBehavior
```

Tests use AAA sections:

```csharp
// Arrange
// Act
// Assert
```

Run unit tests:

```bash
dotnet test tests/CleanApiStarter.Application.UnitTests/CleanApiStarter.Application.UnitTests.csproj
```

### API Integration Tests

API integration tests use:

- MSTest
- Microsoft.AspNetCore.Mvc.Testing
- Testcontainers for PostgreSQL
- Shouldly

The integration test factory starts a real Postgres container, applies scripts from `database/migrations`, boots the API through `WebApplicationFactory<Program>`, and creates real JWTs from `appsettings.Testing.json`.

The API integration tests keep JWT bearer authentication active. They do not replace authentication with a fake scheme.

Run integration tests:

```bash
dotnet test tests/CleanApiStarter.Api.IntegrationTests/CleanApiStarter.Api.IntegrationTests.csproj
```

Run all tests:

```bash
dotnet test CleanApiStarter.slnx
```

## Coverage

Generate coverage and open the HTML report in Chrome:

```bash
scripts/test-coverage.sh
```

The script:

- deletes the previous `artifacts/coverage` folder
- restores local .NET tools
- runs tests with `XPlat Code Coverage`
- generates an HTML report with ReportGenerator
- opens the report in Google Chrome

Coverage output:

```text
artifacts/coverage/report/index.html
```

## Build

Restore and build:

```bash
dotnet restore CleanApiStarter.slnx
dotnet build CleanApiStarter.slnx --no-restore /nr:false -v:minimal
```

## CI

GitHub Actions workflows live in `.github/workflows`:

- `build.yml` restores, builds, and tests the repository on pull requests and pushes to `main`.
- `codeql.yml` runs CodeQL static security analysis on pull requests, pushes to `main`, and weekly on Monday.
- `template.yml` packs the template, installs it locally, creates a sample solution, then restores, builds, and tests the generated output.
- `release.yml` publishes the template to NuGet when a GitHub Release is published with a tag such as `v1.0.0`.

## Package Management

Package versions are managed centrally in:

```text
Directory.Packages.props
```

Keep package versions sorted alphabetically by `Include`. Do not add package versions directly to individual project files.

## Coding Conventions

- Use explicit local types.
- Use project-level `GlobalUsings.cs`.
- Keep cancellation tokens explicit. Do not use `CancellationToken cancellationToken = default` in service or repository contracts.
- Use `required` and `init` for required non-null DTO and domain properties.
- Use nullable types only for genuinely optional values.
- Use structured logging message templates instead of interpolated log strings.
- Keep repository interfaces in `Application`.
- Keep repository implementations and EF Core details in `Infrastructure`.
- Keep domain models persistence-agnostic.
- Do not reintroduce Dapper for the default data access path.
- Do not reintroduce MVC controllers unless the template intentionally changes direction.
- Do not add Swashbuckle; use Scalar.

## Troubleshooting

### Aspire certificate errors

If Aspire logs an `UntrustedRoot` error or reports that no trusted development certificate exists, trust the local ASP.NET Core development certificate:

```bash
dotnet dev-certs https --check --trust
```

If needed, reset and trust again:

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
dotnet dev-certs https --check --trust
```

On macOS, accept the Keychain prompt and enter your password if requested.

### PostgreSQL init scripts did not run

Postgres init scripts only run when the data directory is created. Delete the existing volume and start again.

For Docker Compose:

```bash
docker compose down -v
docker compose up -d
```

For Aspire, delete the `clean-api-starter-postgres-data` Docker volume.

### Postgres latest and data volumes

This template uses `postgres:latest`. PostgreSQL 18+ expects data mounted at:

```text
/var/lib/postgresql
```

Do not mount the volume at:

```text
/var/lib/postgresql/data
```

## License

This project is licensed under the GNU General Public License v3.0. See `LICENSE` for details.
