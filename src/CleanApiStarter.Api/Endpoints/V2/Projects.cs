namespace CleanApiStarter.Api.Endpoints.V2;

public sealed class Projects : IEndpointGroup
{
    public static int MajorVersion => 2;

    public static string RoutePrefix => "/api/projects";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet("/", GetProjects)
            .WithName("GetProjectsV2");
    }

    private static async Task<IResult> GetProjects(
        [AsParameters] PaginatedQuery query,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        PaginatedResult<ProjectDto> projects = await projectService.GetProjectsAsync(query, cancellationToken);

        return Results.Ok(new
        {
            ApiVersion = "2.0",
            projects.Items,
            projects.Limit,
            projects.Offset,
            projects.TotalCount,
            projects.HasPreviousPage,
            projects.HasNextPage
        });
    }
}
