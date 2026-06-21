namespace CleanApiStarter.Api.Features.Projects;

public sealed class Projects : IEndpointGroup
{
    public static int MajorVersion => 1;

    public static string RoutePrefix => "/api/projects";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapPost("/", CreateProject)
            .WithName("CreateProjectV1");

        groupBuilder.MapGet("/", GetProjects)
            .WithName("GetProjectsV1");

        groupBuilder.MapGet("/{id:guid}", GetProject)
            .WithName("GetProjectV1");

        groupBuilder.MapDelete("/{id:guid}", DeleteProject)
            .WithName("DeleteProjectV1");

        groupBuilder.MapPost("/{projectId:guid}/tasks", CreateTask)
            .WithName("CreateProjectTaskV1");

        groupBuilder.MapGet("/{projectId:guid}/tasks", GetTasks)
            .WithName("GetProjectTasksV1");

        groupBuilder.MapGet("/{projectId:guid}/tasks/{taskId:guid}", GetTask)
            .WithName("GetProjectTaskV1");

        groupBuilder.MapPut("/{projectId:guid}/tasks/{taskId:guid}", UpdateTask)
            .WithName("UpdateProjectTaskV1");

        groupBuilder.MapPost("/{projectId:guid}/tasks/{taskId:guid}/complete", CompleteTask)
            .WithName("CompleteProjectTaskV1");

        groupBuilder.MapDelete("/{projectId:guid}/tasks/{taskId:guid}", DeleteTask)
            .WithName("DeleteProjectTaskV1");
    }

    private static async Task<IResult> CreateProject(
        CreateProjectDto projectDto,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        Guid id = await projectService.CreateProjectAsync(projectDto, cancellationToken);

        return Results.Created($"/api/projects/{id}", id);
    }

    private static async Task<IResult> GetProjects(
        [AsParameters] PaginatedQuery query,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        PaginatedResult<ProjectDto> projects = await projectService.GetProjectsAsync(query, cancellationToken);

        return Results.Ok(projects);
    }

    private static async Task<IResult> GetProject(
        Guid id,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        ProjectDto? project = await projectService.GetProjectAsync(id, cancellationToken);

        return project == null ? Results.NotFound() : Results.Ok(project);
    }

    private static async Task<IResult> DeleteProject(
        Guid id,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        DeleteProjectResult result = await projectService.DeleteProjectAsync(id, cancellationToken);

        return result switch
        {
            DeleteProjectResult.Deleted => Results.NoContent(),
            DeleteProjectResult.Forbidden => Results.Forbid(),
            _ => Results.NotFound()
        };
    }

    private static async Task<IResult> CreateTask(
        Guid projectId,
        CreateProjectTaskDto taskDto,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        Guid? taskId = await projectService.CreateTaskAsync(projectId, taskDto, cancellationToken);

        return taskId == null
            ? Results.NotFound()
            : Results.Created($"/api/projects/{projectId}/tasks/{taskId}", taskId);
    }

    private static async Task<IResult> GetTasks(
        Guid projectId,
        [FromQuery] ProjectTaskStatus? status,
        [AsParameters] PaginatedQuery query,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        PaginatedResult<ProjectTaskDto>? tasks = await projectService.GetTasksAsync(
            projectId,
            status,
            query,
            cancellationToken);

        return tasks == null ? Results.NotFound() : Results.Ok(tasks);
    }

    private static async Task<IResult> GetTask(
        Guid projectId,
        Guid taskId,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        ProjectTaskDto? task = await projectService.GetTaskAsync(projectId, taskId, cancellationToken);

        return task == null ? Results.NotFound() : Results.Ok(task);
    }

    private static async Task<IResult> UpdateTask(
        Guid projectId,
        Guid taskId,
        UpdateProjectTaskDto taskDto,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        ProjectTaskMutationResult result = await projectService.UpdateTaskAsync(
            projectId,
            taskId,
            taskDto,
            cancellationToken);

        return result == ProjectTaskMutationResult.Success
            ? Results.NoContent()
            : Results.NotFound();
    }

    private static async Task<IResult> CompleteTask(
        Guid projectId,
        Guid taskId,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        ProjectTaskMutationResult result = await projectService.CompleteTaskAsync(projectId, taskId, cancellationToken);

        return result switch
        {
            ProjectTaskMutationResult.Success => Results.NoContent(),
            ProjectTaskMutationResult.AlreadyCompleted => Results.Conflict(new
            {
                detail = "Task is already completed."
            }),
            _ => Results.NotFound()
        };
    }

    private static async Task<IResult> DeleteTask(
        Guid projectId,
        Guid taskId,
        IProjectService projectService,
        CancellationToken cancellationToken)
    {
        bool deleted = await projectService.DeleteTaskAsync(projectId, taskId, cancellationToken);

        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
