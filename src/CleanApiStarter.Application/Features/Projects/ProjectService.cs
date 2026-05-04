namespace CleanApiStarter.Application.Features.Projects;

public sealed class ProjectService(
    IProjectRepository projectRepository,
    IUser currentUser,
    ILogger<ProjectService> logger) : IProjectService
{
    public async Task<Guid> CreateProjectAsync(CreateProjectDto projectDto, CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        DateTime now = DateTime.UtcNow;
        Guid projectId = Guid.NewGuid();
        Project project = new()
        {
            Id = projectId,
            Name = projectDto.Name,
            Description = projectDto.Description,
            OwnerUserId = userId,
            CreatedAt = now,
            Members =
            [
                new ProjectMember
                {
                    ProjectId = projectId,
                    UserId = userId,
                    CreatedAt = now
                }
            ]
        };

        Guid createdProjectId = await projectRepository.AddProjectAsync(project, cancellationToken);

        logger.LogInformation("Created project {ProjectId} for user {UserId}", createdProjectId, userId);

        return createdProjectId;
    }

    public async Task<ProjectDto?> GetProjectAsync(Guid id, CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        Project? project = await projectRepository.GetProjectAsync(id, userId, cancellationToken);

        return project == null ? null : MapProject(project);
    }

    public async Task<PaginatedResult<ProjectDto>> GetProjectsAsync(
        PaginatedQuery query,
        CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        PaginatedResult<Project> projects = await projectRepository.GetProjectsAsync(userId, query, cancellationToken);

        logger.LogInformation(
            "Retrieved {ProjectCount} projects for user {UserId} with limit {Limit} and offset {Offset}",
            projects.Items.Count,
            userId,
            projects.Limit,
            projects.Offset);

        return projects.Map(MapProject);
    }

    public async Task<DeleteProjectResult> DeleteProjectAsync(Guid id, CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        bool isMember = await projectRepository.IsProjectMemberAsync(id, userId, cancellationToken);

        if (!isMember)
        {
            return DeleteProjectResult.NotFound;
        }

        bool isOwner = await projectRepository.IsProjectOwnerAsync(id, userId, cancellationToken);

        if (!isOwner)
        {
            return DeleteProjectResult.Forbidden;
        }

        bool deleted = await projectRepository.DeleteProjectAsync(id, cancellationToken);

        return deleted ? DeleteProjectResult.Deleted : DeleteProjectResult.NotFound;
    }

    public async Task<Guid?> CreateTaskAsync(
        Guid projectId,
        CreateProjectTaskDto taskDto,
        CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        bool isMember = await projectRepository.IsProjectMemberAsync(projectId, userId, cancellationToken);

        if (!isMember)
        {
            return null;
        }

        ProjectTask task = new()
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = taskDto.Title,
            Description = taskDto.Description,
            Status = ProjectTaskStatus.Todo,
            DueDate = taskDto.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        Guid taskId = await projectRepository.AddTaskAsync(task, cancellationToken);

        logger.LogInformation("Created task {TaskId} in project {ProjectId}", taskId, projectId);

        return taskId;
    }

    public async Task<ProjectTaskDto?> GetTaskAsync(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        ProjectTask? task = await projectRepository.GetTaskAsync(projectId, taskId, userId, cancellationToken);

        return task == null ? null : MapTask(task);
    }

    public async Task<PaginatedResult<ProjectTaskDto>?> GetTasksAsync(
        Guid projectId,
        ProjectTaskStatus? status,
        PaginatedQuery query,
        CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        bool isMember = await projectRepository.IsProjectMemberAsync(projectId, userId, cancellationToken);

        if (!isMember)
        {
            return null;
        }

        PaginatedResult<ProjectTask> tasks = await projectRepository.GetTasksAsync(
            projectId,
            userId,
            status,
            query,
            cancellationToken);

        return tasks.Map(MapTask);
    }

    public async Task<ProjectTaskMutationResult> UpdateTaskAsync(
        Guid projectId,
        Guid taskId,
        UpdateProjectTaskDto taskDto,
        CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        ProjectTask? task = await projectRepository.GetTaskForUpdateAsync(projectId, taskId, userId, cancellationToken);

        if (task == null)
        {
            return ProjectTaskMutationResult.NotFound;
        }

        task.Title = taskDto.Title;
        task.Description = taskDto.Description;
        task.DueDate = taskDto.DueDate;
        task.Status = taskDto.Status;
        task.CompletedAt = taskDto.Status == ProjectTaskStatus.Done
            ? task.CompletedAt ?? DateTime.UtcNow
            : null;

        await projectRepository.SaveChangesAsync(cancellationToken);

        return ProjectTaskMutationResult.Success;
    }

    public async Task<ProjectTaskMutationResult> CompleteTaskAsync(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        ProjectTask? task = await projectRepository.GetTaskForUpdateAsync(projectId, taskId, userId, cancellationToken);

        if (task == null)
        {
            return ProjectTaskMutationResult.NotFound;
        }

        if (task.Status == ProjectTaskStatus.Done)
        {
            return ProjectTaskMutationResult.AlreadyCompleted;
        }

        task.Status = ProjectTaskStatus.Done;
        task.CompletedAt = DateTime.UtcNow;

        await projectRepository.SaveChangesAsync(cancellationToken);

        return ProjectTaskMutationResult.Success;
    }

    public async Task<bool> DeleteTaskAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken)
    {
        string userId = GetCurrentUserId();
        bool isMember = await projectRepository.IsProjectMemberAsync(projectId, userId, cancellationToken);

        if (!isMember)
        {
            return false;
        }

        return await projectRepository.DeleteTaskAsync(projectId, taskId, cancellationToken);
    }

    private string GetCurrentUserId()
    {
        return currentUser.Id ?? throw new UnauthorizedAccessException();
    }

    private static ProjectDto MapProject(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            OwnerUserId = project.OwnerUserId,
            CreatedAt = project.CreatedAt
        };
    }

    private static ProjectTaskDto MapTask(ProjectTask task)
    {
        return new ProjectTaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt
        };
    }
}
