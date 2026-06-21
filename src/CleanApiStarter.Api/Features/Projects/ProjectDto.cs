namespace CleanApiStarter.Api.Features.Projects;

public sealed class ProjectDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string OwnerUserId { get; init; }

    public required DateTime CreatedAt { get; init; }

    public static ProjectDto From(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            OwnerUserId = project.OwnerUserId,
            CreatedAt = project.CreatedAt
        };
    }
}
