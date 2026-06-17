namespace CleanApiStarter.Api.Domain.Entities;

public sealed class ProjectMember
{
    public required Guid ProjectId { get; init; }

    public required string UserId { get; init; }

    public required DateTime CreatedAt { get; init; }

    public Project Project { get; init; } = null!;
}
