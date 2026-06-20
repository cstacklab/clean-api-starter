namespace CleanApiStarter.Api.Features.Projects;

public interface IProjectRepository
{
    Task<Guid> AddProjectAsync(Project project, CancellationToken cancellationToken);

    Task<Project?> GetProjectAsync(Guid id, string userId, CancellationToken cancellationToken);

    Task<PaginatedResult<Project>> GetProjectsAsync(string userId, PaginatedQuery query, CancellationToken cancellationToken);

    Task<bool> IsProjectMemberAsync(Guid projectId, string userId, CancellationToken cancellationToken);

    Task<bool> IsProjectOwnerAsync(Guid projectId, string userId, CancellationToken cancellationToken);

    Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken cancellationToken);

    Task<bool> DeleteProjectAsync(Guid id, CancellationToken cancellationToken);

    Task<Guid> AddTaskAsync(ProjectTask task, CancellationToken cancellationToken);

    Task<ProjectTask?> GetTaskAsync(Guid projectId, Guid taskId, string userId, CancellationToken cancellationToken);

    Task<ProjectTask?> GetTaskForUpdateAsync(Guid projectId, Guid taskId, string userId, CancellationToken cancellationToken);

    Task<PaginatedResult<ProjectTask>> GetTasksAsync(
        Guid projectId,
        string userId,
        ProjectTaskStatus? status,
        PaginatedQuery query,
        CancellationToken cancellationToken);

    Task<bool> DeleteTaskAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
