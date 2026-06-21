namespace CleanApiStarter.Api.Features.Projects;

public static class DeleteProject
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", Handle)
            .WithName("DeleteProjectV1");
    }

    private static async Task<IResult> Handle(
        Guid id,
        IProjectRepository projectRepository,
        IUser currentUser,
        CancellationToken cancellationToken)
    {
        string userId = currentUser.RequireId();

        if (!await projectRepository.IsProjectMemberAsync(id, userId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        if (!await projectRepository.IsProjectOwnerAsync(id, userId, cancellationToken))
        {
            return TypedResults.Forbid();
        }

        bool deleted = await projectRepository.DeleteProjectAsync(id, cancellationToken);

        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
