namespace CleanApiStarter.Application.Common.Models;

public sealed class PaginatedQuery
{
    public int Limit { get; set; } = 20;

    public int Offset { get; set; }
}
