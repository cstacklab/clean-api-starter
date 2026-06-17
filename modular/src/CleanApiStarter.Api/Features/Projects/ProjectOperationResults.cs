namespace CleanApiStarter.Api.Features.Projects;

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
