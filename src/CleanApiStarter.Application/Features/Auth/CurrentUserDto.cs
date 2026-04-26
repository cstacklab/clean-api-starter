namespace CleanApiStarter.Application.Features.Auth;

public sealed class CurrentUserDto
{
    public string UserId { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public IReadOnlyCollection<string> Roles { get; init; } = [];
}