namespace CleanApiStarter.Application.Common.Models;

public sealed class ArrayResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }

    public int Count => Items.Count;
}