namespace CleanApiStarter.Application.Common.Models;

public sealed class PaginatedResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }

    public required int Limit { get; init; }

    public required int Offset { get; init; }

    public required int TotalCount { get; init; }

    public bool HasPreviousPage => Offset > 0;

    public bool HasNextPage => Offset + Limit < TotalCount;

    public PaginatedResult<TDestination> Map<TDestination>(Func<T, TDestination> map)
    {
        return new PaginatedResult<TDestination>
        {
            Items = Items.Select(map).ToArray(),
            Limit = Limit,
            Offset = Offset,
            TotalCount = TotalCount
        };
    }
}