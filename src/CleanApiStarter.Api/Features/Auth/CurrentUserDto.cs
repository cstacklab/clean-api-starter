namespace CleanApiStarter.Api.Features.Auth;

public sealed class CurrentUserDto
{
    public required string UserId { get; init; }

    public required string Email { get; init; }

    public required string Name { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = [];
}
