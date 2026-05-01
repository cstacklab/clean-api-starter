namespace CleanApiStarter.Api.Endpoints.V2;

public sealed class Words : IEndpointGroup
{
    public static int MajorVersion => 2;

    public static string RoutePrefix => "/api/words";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet("/", GetWords)
            .WithName("GetWordsV2");
    }

    private static async Task<IResult> GetWords(
        [AsParameters] PaginatedQuery query,
        IWordService wordService,
        CancellationToken cancellationToken)
    {
        PaginatedResult<WordDto> words = await wordService.GetWordsAsync(query, cancellationToken);

        return Results.Ok(new
        {
            ApiVersion = "2.0",
            words.Items,
            words.Limit,
            words.Offset,
            words.TotalCount,
            words.HasPreviousPage,
            words.HasNextPage
        });
    }
}