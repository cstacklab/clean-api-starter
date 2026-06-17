namespace CleanApiStarter.Application.Features.Projects;

public interface IProjectService
{
    Task<Guid> CreateProjectAsync(CreateProjectDto projectDto, CancellationToken cancellationToken);

    Task<ProjectDto?> GetProjectAsync(Guid id, CancellationToken cancellationToken);

    Task<PaginatedResult<ProjectDto>> GetProjectsAsync(PaginatedQuery query, CancellationToken cancellationToken);

    Task<DeleteProjectResult> DeleteProjectAsync(Guid id, CancellationToken cancellationToken);

    Task<Guid?> CreateTaskAsync(Guid projectId, CreateProjectTaskDto taskDto, CancellationToken cancellationToken);

    Task<ProjectTaskDto?> GetTaskAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken);

    Task<PaginatedResult<ProjectTaskDto>?> GetTasksAsync(
        Guid projectId,
        ProjectTaskStatus? status,
        PaginatedQuery query,
        CancellationToken cancellationToken);

    Task<ProjectTaskMutationResult> UpdateTaskAsync(
        Guid projectId,
        Guid taskId,
        UpdateProjectTaskDto taskDto,
        CancellationToken cancellationToken);

    Task<ProjectTaskMutationResult> CompleteTaskAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken);

    Task<bool> DeleteTaskAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken);
}
