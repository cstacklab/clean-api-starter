namespace CleanApiStarter.Api.Features.Projects.Tasks;

public static class CreateTask
{
    public sealed record Request(string Title, string Description, DateTime? DueDate);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.Title)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(request => request.Description)
                .MaximumLength(4_000);

            RuleFor(request => request.DueDate)
                .Must(dueDate => !dueDate.HasValue || dueDate.Value > DateTime.UtcNow)
                .When(request => request.DueDate.HasValue)
                .WithMessage("Due date must be in the future.");
        }
    }

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{projectId:guid}/tasks", Handle)
            .WithName("CreateProjectTaskV1");
    }

    private static async Task<IResult> Handle(
        Guid projectId,
        Request request,
        IProjectRepository projectRepository,
        IUser currentUser,
        CancellationToken cancellationToken)
    {
        string userId = currentUser.RequireId();

        if (!await projectRepository.IsProjectMemberAsync(projectId, userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        ProjectTask task = new()
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            Status = ProjectTaskStatus.Todo,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        Guid taskId = await projectRepository.AddTaskAsync(task, cancellationToken);

        return TypedResults.Created($"/api/projects/{projectId}/tasks/{taskId}", taskId);
    }
}
