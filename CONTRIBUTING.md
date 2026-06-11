# Contributing to CleanApiStarter

Thanks for your interest in contributing. This repository is both a working reference application and a packaged `dotnet new` template, so changes are verified in both forms.

## Prerequisites

- **.NET SDK** matching [global.json](global.json) (10.0.2xx; `rollForward: latestFeature` applies).
- **Docker** — required to run the API integration tests (Testcontainers) and local PostgreSQL.
- No IDE requirement; the solution file is [CleanApiStarter.slnx](CleanApiStarter.slnx).

## Getting started

```bash
git clone https://github.com/cbjpdev/clean-api-starter.git
cd clean-api-starter
dotnet build CleanApiStarter.slnx
dotnet test CleanApiStarter.slnx
```

To run the API locally, use the Aspire AppHost (starts PostgreSQL with the schema scripts applied, plus pgAdmin):

```bash
dotnet run --project src/CleanApiStarter.AppHost
```

Alternatively, start PostgreSQL with `docker compose up -d` and run `src/CleanApiStarter.Api` directly.

See the [README](README.md) for the solution layout and layer dependency rules. The short version: dependencies point inward — `Domain` references nothing, `Application` references only `Domain`, and web/persistence concerns stay in `Api`, `AspNetCore`, and `Infrastructure`.

## Making changes

1. Branch from `main` (the convention here is `feature/<issue-number>-short-description`).
2. Keep changes focused; unrelated refactoring belongs in its own PR.
3. Open a pull request against `main`. CI must pass before review.

### What CI enforces

The same checks run on every PR ([build.yml](.github/workflows/build.yml), [template.yml](.github/workflows/template.yml), [codeql.yml](.github/workflows/codeql.yml)):

- **Formatting** — `dotnet format CleanApiStarter.slnx --verify-no-changes`. Run `dotnet format CleanApiStarter.slnx` before pushing; style rules live in [.editorconfig](.editorconfig).
- **Build and tests** — the full suite, including the Testcontainers-based integration tests.
- **Template verification** — the template is packed, installed, and instantiated to confirm generated projects still build.
- **CodeQL** security scanning.

## Tests

- Unit tests: `tests/CleanApiStarter.Application.UnitTests` (xUnit v3, AutoFixture, NSubstitute, Shouldly).
- Integration tests: `tests/CleanApiStarter.Api.IntegrationTests` (xUnit v3 against a real PostgreSQL container; Docker must be running).
- Shared test infrastructure (the `ApiApplicationFactory` class fixture, AutoFixture attributes) lives in `tests/CleanApiStarter.Tests`.

New behavior needs tests. Integration tests share one database per test class, so isolate through unique user ids or resource names rather than assuming a fresh schema.

For a local coverage report:

```bash
./scripts/test-coverage.sh
```

## Database changes

The SQL scripts in `database/migrations` are the schema's source of truth; the EF Core entity configurations in `src/CleanApiStarter.Infrastructure/Persistence/Configuration` must be kept in sync with them manually. When changing either, update both.

Conventions:

- Timestamp columns are `TIMESTAMP WITH TIME ZONE`; all values are stored in UTC (`DateTime.UtcNow` in code).
- Migration files are named `V<NNN>__description.sql` and are applied in file-name order by both the AppHost and the integration test factory.
- Migrations must be idempotent (`CREATE TABLE IF NOT EXISTS`, etc.).

## Template changes

If your change affects the project structure, file names, or anything under [.template.config/template.json](.template.config/template.json), verify the template output locally:

```bash
dotnet pack CleanApiStarter.Template.csproj --configuration Release --output artifacts
dotnet new install artifacts/CleanApiStarter.Template.0.0.0.nupkg --force
dotnet new clean-api-starter -n DemoProduct -o /tmp/DemoProduct
dotnet build /tmp/DemoProduct
```

Repository-only files (CI release workflows, this guide, install scripts) must be listed in the `exclude` section of `template.json` so they don't ship inside generated projects.

## Reporting issues

Open a GitHub issue with reproduction steps, expected vs. actual behavior, and your environment (OS, .NET SDK version). For security vulnerabilities, please do not open a public issue — contact the maintainer directly instead.
