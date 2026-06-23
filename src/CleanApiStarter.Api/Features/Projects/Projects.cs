namespace CleanApiStarter.Api.Features.Projects;

public sealed class Projects : IEndpointGroup
{
    public static int MajorVersion => 1;

    public static string RoutePrefix => "/api/projects";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        CreateProject.Map(groupBuilder);
        GetProjects.Map(groupBuilder);
        GetProject.Map(groupBuilder);
        DeleteProject.Map(groupBuilder);

        CreateTask.Map(groupBuilder);
        GetTasks.Map(groupBuilder);
        GetTask.Map(groupBuilder);
        UpdateTask.Map(groupBuilder);
        CompleteTask.Map(groupBuilder);
        DeleteTask.Map(groupBuilder);
    }
}
