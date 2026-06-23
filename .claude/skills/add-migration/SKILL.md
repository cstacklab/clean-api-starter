---
name: add-migration
description: Add or change the PostgreSQL schema in this codebase. Use when adding or modifying a database table, column, index, or migration in CleanApiStarter, including keeping the EF Core entity configuration in sync.
paths: database/migrations/**, src/CleanApiStarter.Api/Infrastructure/Persistence/**
---

# Add a database change

`database/migrations` is the schema's single source of truth. There are no EF
migrations; the EF Core entity configurations must be kept in sync with the SQL
by hand.

## Steps

1. Add `database/migrations/V<NNN>__short_description.sql`, numbered after the
   latest file. Migrations are applied in file-name order by both the Aspire
   AppHost and the integration test factory, so they must be **idempotent**
   (`CREATE TABLE IF NOT EXISTS`, `CREATE INDEX IF NOT EXISTS`, …).

2. Update the matching EF config in
   `src/CleanApiStarter.Api/Infrastructure/Persistence/Configuration/*Configuration.cs`
   (an `IEntityTypeConfiguration<T>`), and the entity in
   `src/CleanApiStarter.Api/Domain/Entities/` if columns changed.

## Conventions

- Timestamp columns are `TIMESTAMP WITH TIME ZONE`; all values are stored in UTC
  (`DateTime.UtcNow` in code). Do not use plain `TIMESTAMP`.
- Identity tables live in `V002__create_identity_tables.sql`; application tables in
  `V001`. Keep new application tables in their own `V<NNN>` file.
- Entities are persistence-agnostic POCOs; mapping details (column names, lengths,
  relationships, delete behaviour) belong in the EF configuration, not the entity.

## Verify

```bash
dotnet build CleanApiStarter.slnx
dotnet test CleanApiStarter.slnx        # integration tests apply the scripts to a real container
```
