# Skills — How-To Playbook

Task recipes for working in this codebase. Unlike an `AGENTS.md` / `CLAUDE.md`,
this file is **not auto-loaded** into agent context — open the recipe you need when
you need it. Keep entries short and example-first.

The app is a single `CleanApiStarter.Api` project organised as **vertical slices**:
one file per operation, co-locating its request, validator, endpoint, and handler.
Dependency boundaries are enforced at build time by NsDepCop
(`src/CleanApiStarter.Api/config.nsdepcop`).

## Add an endpoint to an existing feature

1. Add one file per operation in the feature folder, e.g.
   `Features/Projects/ArchiveProject.cs`:

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

2. Register it in the feature's endpoint group `Map`
   (`Features/Projects/Projects.cs`): `ArchiveProject.Map(groupBuilder);`

3. If the handler needs new data access, add a method to `IProjectRepository`
   (in `Features/Projects/`) and implement it in
   `Infrastructure/Repositories/ProjectRepository.cs`.

Conventions:

- **Handlers depend only on interfaces** (`IProjectRepository`, `IAuthService`),
  never on `Infrastructure` types — NsDepCop breaks the build otherwise.
- Validators are auto-registered (`AddValidatorsFromAssembly`) and run by the
  global `ValidationFilter`, returning `422` on failure.
- Return `TypedResults.*` (`Ok` / `Created` / `NoContent` / `NotFound` /
  `Conflict` / `Forbid`) directly — no result-enum indirection.
- Map entities to DTOs with a static `From(...)` factory on the DTO.

## Add a new feature (capability)

1. Create `Features/<Capability>/`.
2. Add a thin endpoint group implementing `IEndpointGroup` (`RoutePrefix`,
   `MajorVersion`, and `Map` delegating to each slice's `Map`). It is discovered by
   reflection — no manual registration needed.
3. Add slices as above. Put a nested resource in a sub-folder (e.g.
   `Projects/Tasks/`); a new API version in a `V2/` sub-folder.

## Add a database change

1. Add `database/migrations/V<NNN>__description.sql` (idempotent;
   `TIMESTAMP WITH TIME ZONE`, values in UTC).
2. Keep the matching EF config in
   `Infrastructure/Persistence/Configuration/*Configuration.cs` in sync by hand.

## Verify

```bash
dotnet build CleanApiStarter.slnx
dotnet format CleanApiStarter.slnx --verify-no-changes
dotnet test CleanApiStarter.slnx        # Docker required for the integration tests
```
