namespace CleanApiStarter.Api.Features.Projects;

public static class GetProject
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", Handle)
            .WithName("GetProjectV1");
    }

    private static async Task<IResult> Handle(
        Guid id,
        IProjectRepository projectRepository,
        IUser currentUser,
        CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetProjectAsync(id, currentUser.RequireId(), cancellationToken);

        return project is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(ProjectDto.From(project));
    }
}
