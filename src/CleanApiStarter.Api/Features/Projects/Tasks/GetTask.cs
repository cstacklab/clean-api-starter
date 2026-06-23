namespace CleanApiStarter.Api.Features.Projects.Tasks;

public static class GetTask
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{projectId:guid}/tasks/{taskId:guid}", Handle)
            .WithName("GetProjectTaskV1");
    }

    private static async Task<IResult> Handle(
        Guid projectId,
        Guid taskId,
        IProjectRepository projectRepository,
        IUser currentUser,
        CancellationToken cancellationToken)
    {
        ProjectTask? task = await projectRepository.GetTaskAsync(
            projectId,
            taskId,
            currentUser.RequireId(),
            cancellationToken);

        return task is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(ProjectTaskDto.From(task));
    }
}
