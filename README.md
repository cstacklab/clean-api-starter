# CleanApiStarter

`CleanApiStarter` is a Clean Architecture API starter for .NET 10, ASP.NET Core
Minimal APIs, Aspire, PostgreSQL, OpenTelemetry, JWT authentication, API
versioning, FluentValidation, Scalar, EF Core, and automated tests.

The whole application lives in a single `CleanApiStarter.Api` project, organized by
feature and layer folders, with the Clean Architecture dependency rule enforced at
compile time by [NsDepCop](https://github.com/realvizu/NsDepCop) rather than by
separate projects. The sample domain is project management: authenticated users
create projects, manage tasks, filter tasks by status, and complete them.

## Architecture in brief

CleanApiStarter treats architecture as three separate decisions rather than one:

- **Feature organization** — *where do I find the code?* Related code for a
  capability lives together (vertical slices), not scattered across technical
  folders.
- **Dependency management** — *what may depend on what?* Clean Architecture is, at
  its core, about dependency direction: business logic stays free of
  infrastructure, and dependencies point inward. Here that rule is enforced at
  build time by an analyzer (NsDepCop) rather than by splitting the app into many
  projects.
- **Code reuse** — *is logic consistent across features?* Share what is genuinely
  shared (the domain model, infrastructure); duplicate only slice-level glue
  (request/response, validators, handlers) instead of coupling features through the
  wrong abstraction.

Project count is an implementation detail, not an architectural principle. For the
full reasoning see
[docs/architecture](docs/architecture/clean-architecture-and-vertical-slices.md);
for how the structure evolved, see the [decision records](adr/).

## Features

- Single application project with feature-centric organization, plus an
  application-agnostic `AspNetCoreDefaults` project and an Aspire `AppHost`.
- Compile-time boundary enforcement via NsDepCop (`config.nsdepcop`,
  `WarningsAsErrors=NSDEPCOP01`): a forbidden dependency fails the build.
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
- xUnit v3 unit and Testcontainers-backed integration tests.

## Solution Layout

```text
CleanApiStarter
├── database
│   └── migrations
├── scripts
├── src
│   ├── CleanApiStarter.Api              ← the whole application
│   │   ├── config.nsdepcop              ← enforced dependency rules
│   │   ├── Common                       ← shared kernel (paged results, IUser)
│   │   ├── Domain                       ← entities (no outward dependencies)
│   │   ├── Features                     ← Auth, Projects (endpoints + handlers + validators)
│   │   ├── Infrastructure               ← DbContext, EF config, repositories, identity
│   │   ├── Configuration                ← settings classes + options registration
│   │   ├── Endpoints, Services          ← composition / web wiring
│   │   └── Program.cs
│   └── Common
│       ├── CleanApiStarter.AppHost            ← Aspire orchestration
│       └── CleanApiStarter.AspNetCoreDefaults ← reusable, app-agnostic web/runtime defaults
└── tests
    ├── CleanApiStarter.Api.IntegrationTests
    ├── CleanApiStarter.Application.UnitTests
    └── CleanApiStarter.Tests
```

Dependency direction (enforced by `config.nsdepcop`):

- `Domain` depends on nothing else in the app — not `Features`, not `Infrastructure`.
- `Features` (application logic) may not depend on `Infrastructure`; it talks to
  abstractions that `Infrastructure` implements and DI wires up.
- `Infrastructure`, `Endpoints`, `Configuration`, and `Program.cs` form the
  composition root.
- `AspNetCoreDefaults` is an application-agnostic platform project — it holds no
  knowledge of the application's settings; the Api binds configuration-dependent
  options (such as JWT validation) itself.

A violation — for example `using` an `Infrastructure` type from `Domain` — fails
the build with `error NSDEPCOP01`.

## Requirements

- .NET SDK `10.0.203` or a compatible latest-feature SDK (pinned in `global.json`).
- Docker Desktop (for integration tests and local PostgreSQL).

## Getting started

```bash
dotnet run --project src/CleanApiStarter.AppHost   # API + PostgreSQL via Aspire
# or
docker compose up -d && dotnet run --project src/CleanApiStarter.Api
```

Run the tests with `dotnet test CleanApiStarter.slnx`.

To generate a new project from the published template:

```bash
dotnet new clean-api-starter -n MyApi
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).
