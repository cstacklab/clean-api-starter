namespace CleanApiStarter.Domain.Entities;

public sealed class ProjectTask
{
    public required Guid Id { get; init; }

    public required Guid ProjectId { get; init; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public ProjectTaskStatus Status { get; set; }

    public DateTime? DueDate { get; set; }

    public required DateTime CreatedAt { get; init; }

    public DateTime? CompletedAt { get; set; }

    public Project Project { get; init; } = null!;
}