namespace CleanApiStarter.Api.Features.Projects.V2;

public static class GetProjects
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithName("GetProjectsV2");
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

        PaginatedResult<ProjectDto> page = projects.Map(ProjectDto.From);

        return TypedResults.Ok(new
        {
            ApiVersion = "2.0",
            page.Items,
            page.Limit,
            page.Offset,
            page.TotalCount,
            page.HasPreviousPage,
            page.HasNextPage
        });
    }
}
