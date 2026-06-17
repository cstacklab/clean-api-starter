# CleanApiStarter

A Clean Architecture API starter for .NET 10, shipped as two `dotnet new`
template variants so you can pick the structure that fits the project — without
giving up enforced dependency boundaries in either.

| Variant | Folder | `dotnet new` | Shape | Boundaries enforced by |
| --- | --- | --- | --- | --- |
| **Layered** | [`layered/`](layered/) | `clean-api-layered` | Multi-project (Api, Application, Domain, Infrastructure, …) | Project references (compiler) |
| **Modular** | `modular/` *(in progress)* | `clean-api-modular` | One business project + reused platform projects | NsDepCop analyzer (build-breaking) |

Both are genuine Clean Architectures — they differ in feature organization and in
*how* boundaries are enforced, not in whether dependencies point inward. See
[docs/architecture](docs/architecture/clean-architecture-and-vertical-slices.md)
for the reasoning behind the two variants, including how this relates to Vertical
Slice Architecture and the modular monolith.

## Which one should I use?

- **Layered** — when you want hard, physical boundaries and don't mind the
  ceremony of multiple projects. Violations literally don't compile.
- **Modular** — for MVPs, smaller services, or teams that want feature-centric
  organization and less ceremony, with boundaries still enforced at build time.

## Getting started

Each variant is a self-contained solution. Pick one and read its README:

```bash
# Layered (available now)
cd layered
dotnet build CleanApiStarter.slnx
```

To generate a new project from a variant once the templates are published:

```bash
dotnet new clean-api-layered -n MyApi
# dotnet new clean-api-modular -n MyApi   (coming soon)
```

## Repository layout

```
clean-api-starter/
├── docs/architecture/   ← reasoning: Clean Architecture, vertical slices, duplication
├── layered/             ← variant 1: multi-project template (self-contained)
├── modular/             ← variant 2: single-project + NsDepCop template (in progress)
├── CONTRIBUTING.md
└── LICENSE
```

Repo-level files (this README, `LICENSE`, `CONTRIBUTING.md`, `docs/`, CI) live at
the root and are shared across variants. Everything a generated project needs is
inside the variant folder.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).
