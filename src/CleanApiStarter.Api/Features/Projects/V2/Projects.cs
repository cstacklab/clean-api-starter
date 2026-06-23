namespace CleanApiStarter.Api.Features.Projects.V2;

public sealed class Projects : IEndpointGroup
{
    public static int MajorVersion => 2;

    public static string RoutePrefix => "/api/projects";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        GetProjects.Map(groupBuilder);
    }
}
