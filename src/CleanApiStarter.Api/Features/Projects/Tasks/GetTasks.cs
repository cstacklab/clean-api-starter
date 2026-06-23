namespace CleanApiStarter.Api.Features.Projects.Tasks;

public static class GetTasks
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{projectId:guid}/tasks", Handle)
            .WithName("GetProjectTasksV1");
    }

    private static async Task<IResult> Handle(
        Guid projectId,
        [FromQuery] ProjectTaskStatus? status,
        [AsParameters] PaginatedQuery query,
        IProjectRepository projectRepository,
        IUser currentUser,
        CancellationToken cancellationToken)
    {
        string userId = currentUser.RequireId();

        if (!await projectRepository.IsProjectMemberAsync(projectId, userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        PaginatedResult<ProjectTask> tasks = await projectRepository.GetTasksAsync(
            projectId,
            userId,
            status,
            query,
            cancellationToken);

        return TypedResults.Ok(tasks.Map(ProjectTaskDto.From));
    }
}
