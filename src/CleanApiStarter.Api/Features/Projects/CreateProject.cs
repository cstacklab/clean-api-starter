namespace CleanApiStarter.Api.Features.Projects;

public static class CreateProject
{
    public sealed record Request(string Name, string Description);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(request => request.Description)
                .MaximumLength(2_000);
        }
    }

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .WithName("CreateProjectV1");
    }

    private static async Task<IResult> Handle(
        Request request,
        IProjectRepository projectRepository,
        IUser currentUser,
        CancellationToken cancellationToken)
    {
        string userId = currentUser.RequireId();
        DateTime now = DateTime.UtcNow;
        Guid projectId = Guid.NewGuid();
        Project project = new()
        {
            Id = projectId,
            Name = request.Name,
            Description = request.Description,
            OwnerUserId = userId,
            CreatedAt = now,
            Members =
            [
                new ProjectMember
                {
                    ProjectId = projectId,
                    UserId = userId,
                    CreatedAt = now
                }
            ]
        };

        Guid createdProjectId = await projectRepository.AddProjectAsync(project, cancellationToken);

        return TypedResults.Created($"/api/projects/{createdProjectId}", createdProjectId);
    }
}
