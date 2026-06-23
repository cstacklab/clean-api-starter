---
name: add-endpoint
description: Add or change an API endpoint in this codebase as a vertical slice — one file per operation co-locating its request, validator, endpoint mapping, and handler. Use when adding an endpoint, operation, or whole feature to the CleanApiStarter API.
paths: src/CleanApiStarter.Api/Features/**
---

# Add an API endpoint (vertical slice)

This API is a single `CleanApiStarter.Api` project organised as **vertical slices**:
one file per operation. Boundaries are enforced at build time by NsDepCop
(`src/CleanApiStarter.Api/config.nsdepcop`) — a handler must not reference an
`Infrastructure` type or the build fails.

## Add an operation to an existing feature

1. Create one file per operation in the feature folder, e.g.
   `src/CleanApiStarter.Api/Features/Projects/ArchiveProject.cs`:

   ```csharp
   namespace CleanApiStarter.Api.Features.Projects;

   public static class ArchiveProject
   {
       public sealed record Request(string Reason);              // omit if no body

       public sealed class Validator : AbstractValidator<Request>
       {
           public Validator() => RuleFor(request => request.Reason).NotEmpty();
       }

       public static void Map(RouteGroupBuilder group) =>
           group.MapPost("/{id:guid}/archive", Handle).WithName("ArchiveProjectV1");

       private static async Task<IResult> Handle(
           Guid id,
           Request request,
           IProjectRepository projectRepository,   // an interface — never an Infrastructure type
           IUser currentUser,
           CancellationToken cancellationToken)
       {
           string userId = currentUser.RequireId();
           // orchestrate via the repository, then return a typed result
           return TypedResults.NoContent();
       }
   }
   ```

2. Register the slice in the feature's endpoint group `Map`
   (`Features/Projects/Projects.cs`): add `ArchiveProject.Map(groupBuilder);`.

3. If the handler needs new data access, add a method to `IProjectRepository`
   (`Features/Projects/IProjectRepository.cs`) and implement it in
   `Infrastructure/Repositories/ProjectRepository.cs`.

## Add a new feature (capability)

1. Create `Features/<Capability>/`.
2. Add a thin endpoint group implementing `IEndpointGroup` — it sets `RoutePrefix`,
   `MajorVersion`, and a `Map` that calls each slice's `Map`. It is discovered by
   reflection; no manual registration needed.
3. Add slices as above. Put a nested resource in a sub-folder (e.g.
   `Projects/Tasks/`) and a new API version in a `V2/` sub-folder.

## Conventions

- **Handlers depend only on interfaces** (`IProjectRepository`, `IAuthService`),
  never on `Infrastructure` types. NsDepCop (`Features ✗→ Infrastructure`) breaks
  the build otherwise.
- Validators are auto-registered (`AddValidatorsFromAssembly`) and run by the global
  `ValidationFilter`, returning `422` on failure. Just add a nested
  `Validator : AbstractValidator<Request>`.
- Return `TypedResults.*` (`Ok` / `Created` / `NoContent` / `NotFound` /
  `Conflict` / `Forbid`) directly — there is no result-enum indirection.
- Map entities to DTOs with a static `From(...)` factory on the DTO
  (e.g. `ProjectDto.From(project)`); for pages use `result.Map(ProjectDto.From)`.
- Group-level auth: the `Projects` group calls `RequireAuthorization()`; set
  `.AllowAnonymous()` / `.RequireAuthorization()` per route when it differs.

## Verify

```bash
dotnet build CleanApiStarter.slnx
dotnet format CleanApiStarter.slnx --verify-no-changes
dotnet test CleanApiStarter.slnx        # Docker required for the integration tests
```
