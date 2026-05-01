namespace CleanApiStarter.Application.Features.Projects;

public enum DeleteProjectResult
{
    Deleted,
    NotFound,
    Forbidden
}

public enum ProjectTaskMutationResult
{
    Success,
    NotFound,
    AlreadyCompleted
}