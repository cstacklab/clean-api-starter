# Architecture Decision Records

This folder records the significant architectural decisions for this solution and
how they evolved. Each record follows a standard format (Title, Date, Status,
Context, Decision, Consequences) and is immutable once accepted — superseding
decisions are captured in new records rather than by editing old ones.

| ADR | Title | Status |
| --- | --- | --- |
| [ADR-001](ADR-001.md) | Fine-grained seven-project Clean Architecture structure | Superseded by ADR-002 |
| [ADR-002](ADR-002.md) | Maintain two parallel variants — layered and modular | Superseded by ADR-003 |
| [ADR-003](ADR-003.md) | Consolidate to a single modular solution at the root (three projects) | Accepted |

The progression in one line: a fine-grained seven-project split → two parallel
variants (layered + modular) → a single modular solution of three projects, where
boundaries are enforced by a namespace-dependency analyzer rather than by project
count.

Background reading on the principles behind these decisions lives in
[`../docs/architecture`](../docs/architecture/clean-architecture-and-vertical-slices.md).
