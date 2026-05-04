namespace CleanApiStarter.Application.Features.Projects;

public sealed class ProjectTaskDto
{
    public required Guid Id { get; init; }

    public required Guid ProjectId { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required ProjectTaskStatus Status { get; init; }

    public DateTime? DueDate { get; init; }

    public required DateTime CreatedAt { get; init; }

    public DateTime? CompletedAt { get; init; }
}

public sealed class CreateProjectTaskDto
{
    public required string Title { get; init; }

    public required string Description { get; init; }

    public DateTime? DueDate { get; init; }
}

public sealed class UpdateProjectTaskDto
{
    public required string Title { get; init; }

    public required string Description { get; init; }

    public required ProjectTaskStatus Status { get; init; }

    public DateTime? DueDate { get; init; }
}
