namespace CleanApiStarter.Api.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    List<string>? Roles { get; }
}
