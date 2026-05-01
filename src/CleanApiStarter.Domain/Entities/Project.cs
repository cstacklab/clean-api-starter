namespace CleanApiStarter.Domain.Entities;

public sealed class Project
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string OwnerUserId { get; init; }

    public required DateTime CreatedAt { get; init; }

    public List<ProjectMember> Members { get; init; } = [];

    public List<ProjectTask> Tasks { get; init; } = [];
}