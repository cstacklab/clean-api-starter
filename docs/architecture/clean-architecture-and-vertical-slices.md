# Clean Architecture, Vertical Slices, and Duplication

This document records the architectural reasoning behind CleanApiStarter and, in
particular, why the template ships in two flavors (`layered` and `modular`). It
exists so that contributors and template users understand *why* the structure is
the way it is, not just *what* it is.

The thinking here was heavily shaped by a talk from Steve Smith (Ardalis)
responding to the "is Clean Architecture dead?" debate. A cleaned-up transcript
of that talk is kept alongside this file in
[`ardalis-clean-architecture-vsa-transcript.md`](ardalis-clean-architecture-vsa-transcript.md).

## The core idea: three independent decisions

The single most useful reframing is that "architecture" is not one choice. It is
at least three *separate, orthogonal* decisions, and most online arguments
conflate them:

1. **Feature organization** — *"Where do I find the code?"*
   Do related files for one capability live together, or are they scattered
   across `Controllers/`, `Services/`, `Repositories/` folders?

2. **Code reuse** — *"Is logic consistent across features?"*
   When two features need the same rule or the same infrastructure, do they
   share one implementation or each carry their own?

3. **Dependency management** — *"What is allowed to depend on what?"*
   Which directions of dependency are legal, and how is that enforced?

Vertical Slice Architecture (VSA) is primarily an answer to **#1**.
Clean Architecture is primarily an answer to **#3**. They are not opposites and
they are not mutually exclusive — they answer different questions. You can, and
generally should, do both.

## What Clean Architecture actually is

Clean Architecture is **about dependency direction**, not about a project count
or a specific 2018-era folder structure. Its essential rules:

- Protect business logic from external concerns (UI, DB, frameworks).
- Isolate infrastructure behind abstractions.
- Make dependencies point *inward* — outer layers depend on inner layers, never
  the reverse.

That is the whole of it. "Six projects" is one *implementation* of that rule, not
the rule itself. A single project can be a Clean Architecture as long as the
dependency direction is preserved and enforced.

## What Vertical Slices actually are (and the duplication myth)

A "vertical slice" groups everything needed for one operation together — for a
"create project" endpoint, that's the route, request/response DTOs, validation,
and the handler logic in one place — so adding or changing a feature means
working in one location instead of scrolling through parallel `Controllers/`,
`Services/`, `Models/` folders.

The common misconception is that VSA means **"duplicate everything into every
feature folder."** It does not, and almost no real-world VSA codebase does this.
Even popular VSA templates centralize the domain model, persistence, and
infrastructure in a shared/common area; only the *endpoint-layer* types live in
the slice. In practice, "vertical slices" usually means *the UI/endpoint layer is
organized by feature* — not that business rules and persistence are copied per
feature.

> Not everything belongs duplicated into every feature folder forever. Some
> abstractions are useful. Sharing domain concepts instead of duplicating them is
> helpful. Reusing consistent infrastructure (e.g. one DbContext) makes sense.

## Duplication: the practical rule

"Is duplication okay?" is a **code-reuse** decision (#2) and the answer is
*"yes for some things, no for others."* The line that has served us well:

| Keep per-slice (duplication is fine, often preferred) | Share (duplication is a bug) |
| --- | --- |
| Request / response DTOs | Domain entities and their invariants |
| Validators | The DbContext and EF configuration |
| Handler / orchestration logic | Cross-cutting infrastructure (auth, logging) |
| Query shapes and projections | Genuinely shared business rules |

Why per-slice duplication is *good*: forcing two features through one shared
service couples them, so a change for feature A can break feature B (the "ripple
effect"). Two near-identical handlers that evolve independently are cheaper than
one shared abstraction fighting both callers. This is the classic *"prefer
duplication over the wrong abstraction"* rule — but scoped to slice-level glue.

Why domain/infrastructure duplication is *bad*: if a business rule (e.g. "a
task's `CompletedAt` is set when its status becomes `Done`") is copied into
multiple handlers, the copies drift out of sync. That rule belongs on the entity,
defined once.

## How the boundaries are enforced

Dependency rules are only real if something *fails the build* when they are
broken. The two variants enforce the same rule with different mechanisms:

- **`layered` (multi-project):** enforced by **project references**. `Domain`
  has no reference to `Infrastructure`, so a violating `using` simply does not
  compile. This is the strongest possible enforcement and comes "for free" from
  the project graph — at the cost of more projects and more ceremony.

- **`modular` (single app project):** enforced by **[NsDepCop](https://github.com/realvizu/NsDepCop)**,
  a Roslyn analyzer that polices *namespace* dependencies at compile time. A
  declarative `config.nsdepcop` lists illegal directions, and
  `<WarningsAsErrors>NSDEPCOP01</WarningsAsErrors>` in the csproj turns a
  violation into a build-breaking error. This recreates the layered guarantee
  inside one project. The ruleset is default-allow, then blacklists the few
  inward-violating directions:

  ```xml
  <Disallowed From="CleanApiStarter.Api.Domain.*"   To="CleanApiStarter.Api.Infrastructure.*" />
  <Disallowed From="CleanApiStarter.Api.Features.*" To="CleanApiStarter.Api.Infrastructure.*" />
  ```

## The two variants in this repo

| | `layered` | `modular` |
| --- | --- | --- |
| Shape | Multi-project (Api, Application, Domain, Infrastructure, …) | One business project (`Api`) + reused platform projects (`AspNetCore`, `Configuration`) + `AppHost` |
| Feature organization | Feature folders inside layered projects | Vertical slices in `Features/` |
| Enforcement | Project references (compiler) | NsDepCop (analyzer, build-breaking) |
| Best for | Teams who want hard, physical boundaries | MVPs, smaller apps, less ceremony, feature-centric work |

Both are legitimate Clean Architectures — they differ in feature organization and
enforcement mechanism, not in whether they respect dependency direction.

## Where this is heading: the modular monolith

The natural destination of "fewer projects + feature-centric + enforced
boundaries" is the **modular monolith**: top-level folders organized by *business
capability* (not by technical layer), each module owning its own slices, domain,
and persistence, exposing an explicit public contract, with a shared kernel only
for genuinely shared concepts. NsDepCop then enforces both the inward dependency
rule *and* inter-module isolation (e.g. `Modules.Projects.* ✗→
Modules.Billing.Internal.*`).

This gives much of the modularity and independence people reach microservices for
— autonomy, encapsulation, clear contracts — without the operational tax of a
distributed system. The `modular` variant is intentionally a stepping stone in
this direction: its `Features/` layout can grow into `Modules/` over time.

## Summary

- Clean Architecture = dependency direction. Vertical Slices = feature
  organization. Code reuse = a third, separate decision. Don't conflate them.
- Project count is an implementation detail, not an architectural principle.
- Duplicate slice-level glue freely; never duplicate domain rules or
  infrastructure.
- Enforce boundaries with something that breaks the build — project references
  (`layered`) or NsDepCop (`modular`).
- The modular monolith is the long-term target for serious business apps.

## Source

Steve Smith (Ardalis), talk on Clean Architecture vs. Vertical Slice
Architecture (responding to Nick Chapsas). See the transcript in
[`ardalis-clean-architecture-vsa-transcript.md`](ardalis-clean-architecture-vsa-transcript.md).
Prefer linking to the original video over redistributing the transcript; see the
attribution note in that file.
