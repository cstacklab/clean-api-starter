namespace CleanApiStarter.Api.Features.Projects.Tasks;

public static class CompleteTask
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{projectId:guid}/tasks/{taskId:guid}/complete", Handle)
            .WithName("CompleteProjectTaskV1");
    }

    public static async Task<IResult> Handle(
        Guid projectId,
        Guid taskId,
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

        if (task.Status == ProjectTaskStatus.Done)
        {
            return TypedResults.Conflict("Task is already completed.");
        }

        task.Status = ProjectTaskStatus.Done;
        task.CompletedAt = DateTime.UtcNow;

        await projectRepository.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
