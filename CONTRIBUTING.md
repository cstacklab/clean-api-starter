# Contributing to CleanApiStarter

Thanks for your interest in contributing. This repository ships **two `dotnet new`
template variants** that share documentation and a single source of truth for the
database schema:

- [`layered/`](layered/) — multi-project Clean Architecture (`clean-api-layered`).
- [`modular/`](modular/) — single business project with NsDepCop-enforced
  boundaries (`clean-api-modular`).

Each variant is a self-contained solution. A change usually touches one variant;
some (docs, the database schema, CI) span both. See
[docs/architecture](docs/architecture/clean-architecture-and-vertical-slices.md)
for the reasoning behind the split.

## Prerequisites

- **.NET SDK** matching each variant's `global.json` (10.0.2xx; `rollForward:
  latestFeature` applies).
- **Docker** — required for the integration tests (Testcontainers) and local
  PostgreSQL.

## Getting started

```bash
git clone https://github.com/cbjpdev/clean-api-starter.git
cd clean-api-starter

# Populate each variant's database/ from the shared root copy (see below).
./scripts/sync-database.sh

# Work inside whichever variant you're changing:
cd layered            # or: cd modular
dotnet build CleanApiStarter.slnx
dotnet test CleanApiStarter.slnx
```

To run a variant's API locally via Aspire (starts PostgreSQL with the schema
applied, plus pgAdmin):

```bash
cd layered            # or: cd modular
dotnet run --project src/CleanApiStarter.AppHost
```

## Making changes

1. Branch from `main` (convention: `feature/<issue-number>-short-description`).
2. Keep changes focused; unrelated refactoring belongs in its own PR.
3. If a change is conceptually shared (the domain model, the database schema, a
   platform default), apply it to **both** variants so they don't drift.
4. Open a pull request against `main`. CI must pass before review.

### What CI enforces

Every workflow runs as a matrix over both variants
([build.yml](.github/workflows/build.yml),
[template.yml](.github/workflows/template.yml),
[codeql.yml](.github/workflows/codeql.yml)):

- **Formatting** — `dotnet format CleanApiStarter.slnx --verify-no-changes`. Run
  `dotnet format CleanApiStarter.slnx` in the variant before pushing; style rules
  live in each variant's `.editorconfig`.
- **Build and tests** — the full suite per variant, including the
  Testcontainers-based integration tests.
- **Template verification** — each variant is packed, installed, and instantiated
  to confirm generated projects build.
- **CodeQL** security scanning per variant.

## Tests

- Unit tests: `<variant>/tests/CleanApiStarter.Application.UnitTests` (xUnit v3,
  AutoFixture, NSubstitute, Shouldly).
- Integration tests: `<variant>/tests/CleanApiStarter.Api.IntegrationTests`
  (xUnit v3 against a real PostgreSQL container; Docker must be running).
- Shared test infrastructure (the `ApiApplicationFactory` class fixture)
  lives in `<variant>/tests/CleanApiStarter.Tests`.

New behavior needs tests. The integration test factory is an `IClassFixture`, so
all tests in a class share one database — isolate through unique user ids or
resource names rather than assuming a fresh schema.

For a local coverage report: `cd <variant> && ./scripts/test-coverage.sh`.

## Database changes

`database/migrations` **at the repository root is the single source of truth.**
Each variant gets its own git-ignored copy via
[`scripts/sync-database.sh`](scripts/sync-database.sh); never edit the variant
copies directly. After changing the root scripts, re-run the sync (CI does this
automatically before building or packing).

The EF Core entity configurations in each variant
(`Infrastructure/Persistence/Configuration`) must be kept in sync with the SQL by
hand. When you change one, change the other — and apply it to both variants.

Conventions:

- Timestamp columns are `TIMESTAMP WITH TIME ZONE`; all values are stored in UTC
  (`DateTime.UtcNow` in code).
- Migration files are named `V<NNN>__description.sql` and applied in file-name
  order by both the AppHost and the integration test factory.
- Migrations must be idempotent (`CREATE TABLE IF NOT EXISTS`, etc.).

### Boundary enforcement

- **Layered** enforces dependency direction through project references — a
  violation simply doesn't compile.
- **Modular** enforces it with NsDepCop: `config.nsdepcop` in the `Api` project
  lists illegal namespace directions and `WarningsAsErrors=NSDEPCOP01` makes a
  violation a build error. Update `config.nsdepcop` when you add a layer or rule.

## Template changes

If a change affects project structure, file names, or a variant's
`.template.config/template.json`, verify that variant's output locally:

```bash
./scripts/sync-database.sh
cd layered                                    # or: cd modular
dotnet pack CleanApiStarter.Template.csproj --configuration Release --output ../artifacts
# layered -> CleanApiStarter.Template.Layered / clean-api-layered
# modular -> CleanApiStarter.Template.Modular / clean-api-modular
dotnet new install ../artifacts/CleanApiStarter.Template.Layered.0.0.0.nupkg --force
dotnet new clean-api-layered -n DemoProduct -o /tmp/DemoProduct
dotnet build /tmp/DemoProduct
```

Repository-only files (this guide, `docs/`, root CI/release workflows, install
scripts) must stay outside the variant folders or be listed in the variant's
`template.json` `exclude` section so they don't ship inside generated projects.

## Reporting issues

Open a GitHub issue with reproduction steps, expected vs. actual behavior, and
your environment (OS, .NET SDK version). For security vulnerabilities, please do
not open a public issue — contact the maintainer directly instead.
