# Contributing to CleanApiStarter

Thanks for your interest in contributing. This repository is both a working
reference application and a packaged `dotnet new` template, so changes are
verified in both forms. The whole application is a single
`CleanApiStarter.Api` project with boundaries enforced by an analyzer; see
[`adr/`](adr/) and [docs/architecture](docs/architecture/clean-architecture-and-vertical-slices.md)
for the reasoning.

## Prerequisites

- **.NET SDK** matching [global.json](global.json) (10.0.2xx; `rollForward:
  latestFeature` applies).
- **Docker** — required for the integration tests (Testcontainers) and local
  PostgreSQL.

## Getting started

```bash
git clone https://github.com/cbjpdev/clean-api-starter.git
cd clean-api-starter
dotnet build CleanApiStarter.slnx
dotnet test CleanApiStarter.slnx
```

To run the API locally via Aspire (starts PostgreSQL with the schema applied, plus
pgAdmin):

```bash
dotnet run --project src/CleanApiStarter.AppHost
```

Alternatively, start PostgreSQL with `docker compose up -d` and run
`src/CleanApiStarter.Api` directly.

## Making changes

1. Branch from `main` (convention: `feature/<issue-number>-short-description`).
2. Keep changes focused; unrelated refactoring belongs in its own PR.
3. Open a pull request against `main`. CI must pass before review.

Working in Claude Code? This repo ships project skills in
[`.claude/skills/`](.claude/skills/) — run `/add-endpoint` or `/add-migration`
(or let Claude apply them automatically) for step-by-step, convention-correct
recipes.

### What CI enforces

The same checks run on every PR ([build.yml](.github/workflows/build.yml),
[template.yml](.github/workflows/template.yml),
[codeql.yml](.github/workflows/codeql.yml)):

- **Formatting** — `dotnet format CleanApiStarter.slnx --verify-no-changes`. Run
  `dotnet format CleanApiStarter.slnx` before pushing; style rules live in
  [.editorconfig](.editorconfig).
- **Build and tests** — the full suite, including the Testcontainers-based
  integration tests.
- **Template verification** — the template is packed, installed, and instantiated
  to confirm generated projects build.
- **CodeQL** security scanning.

## Tests

- Unit tests: `tests/CleanApiStarter.Application.UnitTests` (xUnit v3, AutoFixture,
  NSubstitute, Shouldly).
- Integration tests: `tests/CleanApiStarter.Api.IntegrationTests` (xUnit v3
  against a real PostgreSQL container; Docker must be running).
- Shared test infrastructure (the `ApiApplicationFactory` class fixture) lives in
  `tests/CleanApiStarter.Tests`.

New behavior needs tests. The integration test factory is an `IClassFixture`, so
all tests in a class share one database — isolate through unique user ids or
resource names rather than assuming a fresh schema.

For a local coverage report: `./scripts/test-coverage.sh`.

## Boundary enforcement

Dependency direction is enforced by [NsDepCop](https://github.com/realvizu/NsDepCop):
`src/CleanApiStarter.Api/config.nsdepcop` lists the illegal namespace directions
(for example `Domain ✗→ Infrastructure`, `Features ✗→ Infrastructure`), and
`WarningsAsErrors=NSDEPCOP01` makes a violation a build error. Update
`config.nsdepcop` when you add a layer or rule. `AspNetCoreDefaults` is kept
application-agnostic — don't introduce a dependency from it onto the Api's
settings or features.

## Database changes

`database/migrations` is the schema's source of truth. The EF Core entity
configurations in `src/CleanApiStarter.Api/Infrastructure/Persistence/Configuration`
must be kept in sync with the SQL by hand. When you change one, change the other.

Conventions:

- Timestamp columns are `TIMESTAMP WITH TIME ZONE`; all values are stored in UTC
  (`DateTime.UtcNow` in code).
- Migration files are named `V<NNN>__description.sql` and applied in file-name
  order by both the AppHost and the integration test factory.
- Migrations must be idempotent (`CREATE TABLE IF NOT EXISTS`, etc.).

## Template changes

If a change affects project structure, file names, or
[.template.config/template.json](.template.config/template.json), verify the
template output locally:

```bash
dotnet pack CleanApiStarter.Template.csproj --configuration Release --output artifacts
dotnet new install artifacts/CleanApiStarter.Template.0.0.0.nupkg --force
dotnet new clean-api-starter -n DemoProduct -o /tmp/DemoProduct
dotnet build /tmp/DemoProduct
```

Repository-only files (this guide, `adr/`, `docs/`, release/template CI workflows,
install scripts) must be listed in the `exclude` section of `template.json` so they
don't ship inside generated projects.

## Reporting issues

Open a GitHub issue with reproduction steps, expected vs. actual behavior, and
your environment (OS, .NET SDK version). For security vulnerabilities, please do
not open a public issue — contact the maintainer directly instead.
