namespace CleanApiStarter.Application.Features.Projects;

public sealed class ProjectDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string OwnerUserId { get; init; }

    public required DateTime CreatedAt { get; init; }
}

public sealed class CreateProjectDto
{
    public required string Name { get; init; }

    public required string Description { get; init; }
}