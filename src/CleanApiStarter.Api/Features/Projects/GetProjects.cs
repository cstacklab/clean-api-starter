namespace CleanApiStarter.Api.Features.Projects;

public static class GetProjects
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithName("GetProjectsV1");
    }

    private static async Task<IResult> Handle(
        [AsParameters] PaginatedQuery query,
        IProjectRepository projectRepository,
        IUser currentUser,
        CancellationToken cancellationToken)
    {
        PaginatedResult<Project> projects = await projectRepository.GetProjectsAsync(
            currentUser.RequireId(),
            query,
            cancellationToken);

        return TypedResults.Ok(projects.Map(ProjectDto.From));
    }
}
