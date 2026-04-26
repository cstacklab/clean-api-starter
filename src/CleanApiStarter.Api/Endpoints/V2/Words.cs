namespace CleanApiStarter.Api.Endpoints.V2;

public sealed class Words : IEndpointGroup
{
    public static int MajorVersion => 2;

    public static string RoutePrefix => "/api/words";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet("/", GetAllWords)
            .WithName("GetAllWordsV2");
    }

    private static async Task<IResult> GetAllWords(
        IWordService wordService,
        CancellationToken cancellationToken)
    {
        IEnumerable<WordDto> words = await wordService.GetAllWordsAsync(cancellationToken);

        return Results.Ok(new
        {
            ApiVersion = "2.0",
            Items = words
        });
    }
}