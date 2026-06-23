namespace CleanApiStarter.Api.Features.Projects.Tasks;

public static class UpdateTask
{
    public sealed record Request(string Title, string Description, ProjectTaskStatus Status, DateTime? DueDate);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.Title)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(request => request.Description)
                .MaximumLength(4_000);

            RuleFor(request => request.Status)
                .IsInEnum();

            RuleFor(request => request.DueDate)
                .Must((request, dueDate) => !dueDate.HasValue
                    || request.Status == ProjectTaskStatus.Done
                    || dueDate.Value > DateTime.UtcNow)
                .When(request => request.DueDate.HasValue && request.Status != ProjectTaskStatus.Done)
                .WithMessage("Due date must be in the future.");
        }
    }

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{projectId:guid}/tasks/{taskId:guid}", Handle)
            .WithName("UpdateProjectTaskV1");
    }

    private static async Task<IResult> Handle(
        Guid projectId,
        Guid taskId,
        Request request,
        IProjectRepository projectRepository,
        IUser currentUser,
        CancellationToken cancellationToken)
    {
        ProjectTask? task = await projectRepository.GetTaskForUpdateAsync(
            projectId,
            taskId,
            currentUser.RequireId(),
            cancellationToken);

        if (task is null)
        {
            return TypedResults.NotFound();
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.DueDate = request.DueDate;
        task.Status = request.Status;
        task.CompletedAt = request.Status == ProjectTaskStatus.Done
            ? task.CompletedAt ?? DateTime.UtcNow
            : null;

        await projectRepository.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
