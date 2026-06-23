namespace CleanApiStarter.Api.Features.Projects.Tasks;

public static class DeleteTask
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{projectId:guid}/tasks/{taskId:guid}", Handle)
            .WithName("DeleteProjectTaskV1");
    }

    private static async Task<IResult> Handle(
        Guid projectId,
        Guid taskId,
        IProjectRepository projectRepository,
        IUser currentUser,
        CancellationToken cancellationToken)
    {
        string userId = currentUser.RequireId();

        if (!await projectRepository.IsProjectMemberAsync(projectId, userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        bool deleted = await projectRepository.DeleteTaskAsync(projectId, taskId, cancellationToken);

        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
