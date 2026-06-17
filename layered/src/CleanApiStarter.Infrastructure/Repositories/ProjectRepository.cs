namespace CleanApiStarter.Infrastructure.Repositories;

public sealed class ProjectRepository(ApplicationDbContext dbContext) : IProjectRepository
{
    public async Task<Guid> AddProjectAsync(Project project, CancellationToken cancellationToken)
    {
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);

        return project.Id;
    }

    public async Task<Project?> GetProjectAsync(Guid id, string userId, CancellationToken cancellationToken)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(project => project.Id == id)
            .Where(project => project.Members.Any(member => member.UserId == userId))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Project>> GetProjectsAsync(
        string userId,
        PaginatedQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<Project> projectsQuery = dbContext.Projects
            .AsNoTracking()
            .Where(project => project.Members.Any(member => member.UserId == userId))
            .OrderBy(project => project.Name);

        int totalCount = await projectsQuery.CountAsync(cancellationToken);
        List<Project> projects = await projectsQuery
            .Skip(query.Offset)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Project>
        {
            Items = projects,
            Limit = query.Limit,
            Offset = query.Offset,
            TotalCount = totalCount
        };
    }

    public async Task<bool> IsProjectMemberAsync(Guid projectId, string userId, CancellationToken cancellationToken)
    {
        return await dbContext.ProjectMembers
            .AnyAsync(member => member.ProjectId == projectId && member.UserId == userId, cancellationToken);
    }

    public async Task<bool> IsProjectOwnerAsync(Guid projectId, string userId, CancellationToken cancellationToken)
    {
        return await dbContext.Projects
            .AnyAsync(project => project.Id == projectId && project.OwnerUserId == userId, cancellationToken);
    }

    public async Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await dbContext.Projects
            .AnyAsync(project => project.Id == projectId, cancellationToken);
    }

    public async Task<bool> DeleteProjectAsync(Guid id, CancellationToken cancellationToken)
    {
        int rowsAffected = await dbContext.Projects
            .Where(project => project.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<Guid> AddTaskAsync(ProjectTask task, CancellationToken cancellationToken)
    {
        dbContext.ProjectTasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return task.Id;
    }

    public async Task<ProjectTask?> GetTaskAsync(
        Guid projectId,
        Guid taskId,
        string userId,
        CancellationToken cancellationToken)
    {
        return await ProjectTasksForMember(projectId, userId)
            .AsNoTracking()
            .SingleOrDefaultAsync(task => task.Id == taskId, cancellationToken);
    }

    public async Task<ProjectTask?> GetTaskForUpdateAsync(
        Guid projectId,
        Guid taskId,
        string userId,
        CancellationToken cancellationToken)
    {
        return await ProjectTasksForMember(projectId, userId)
            .SingleOrDefaultAsync(task => task.Id == taskId, cancellationToken);
    }

    public async Task<PaginatedResult<ProjectTask>> GetTasksAsync(
        Guid projectId,
        string userId,
        ProjectTaskStatus? status,
        PaginatedQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<ProjectTask> tasksQuery = ProjectTasksForMember(projectId, userId);

        if (status.HasValue)
        {
            tasksQuery = tasksQuery.Where(task => task.Status == status.Value);
        }

        tasksQuery = tasksQuery
            .AsNoTracking()
            .OrderBy(task => task.Status)
            .ThenBy(task => task.DueDate)
            .ThenBy(task => task.Title);

        int totalCount = await tasksQuery.CountAsync(cancellationToken);
        List<ProjectTask> tasks = await tasksQuery
            .Skip(query.Offset)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ProjectTask>
        {
            Items = tasks,
            Limit = query.Limit,
            Offset = query.Offset,
            TotalCount = totalCount
        };
    }

    public async Task<bool> DeleteTaskAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken)
    {
        int rowsAffected = await dbContext.ProjectTasks
            .Where(task => task.ProjectId == projectId && task.Id == taskId)
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<ProjectTask> ProjectTasksForMember(Guid projectId, string userId)
    {
        return dbContext.ProjectTasks
            .Where(task => task.ProjectId == projectId)
            .Where(task => task.Project.Members.Any(member => member.UserId == userId));
    }
}
