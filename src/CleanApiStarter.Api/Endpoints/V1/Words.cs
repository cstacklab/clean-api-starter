namespace CleanApiStarter.Api.Endpoints.V1;

public sealed class Words : IEndpointGroup
{
    public static int MajorVersion => 1;

    public static string RoutePrefix => "/api/words";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapPost("/", CreateWord)
            .WithName("CreateWordV1");

        groupBuilder.MapGet("/", GetAllWords)
            .WithName("GetAllWordsV1");

        groupBuilder.MapGet("/{id:guid}", GetWord)
            .WithName("GetWordV1");

        groupBuilder.MapPut("/{id:guid}", UpdateWord)
            .WithName("UpdateWordV1");

        groupBuilder.MapDelete("/{id:guid}", DeleteWord)
            .WithName("DeleteWordV1");
    }

    private static async Task<IResult> CreateWord(
        CreateWordDto wordDto,
        IWordService wordService,
        CancellationToken cancellationToken)
    {
        Guid id = await wordService.AddWordAsync(wordDto, cancellationToken);

        return Results.Created($"/api/words/{id}", id);
    }

    private static async Task<IResult> GetWord(
        Guid id,
        IWordService wordService,
        CancellationToken cancellationToken)
    {
        WordDto? word = await wordService.GetWordByIdAsync(id, cancellationToken);

        return word == null ? Results.NotFound() : Results.Ok(word);
    }

    private static async Task<IResult> GetAllWords(
        IWordService wordService,
        CancellationToken cancellationToken)
    {
        IEnumerable<WordDto> words = await wordService.GetAllWordsAsync(cancellationToken);

        return Results.Ok(words);
    }

    private static async Task<IResult> UpdateWord(
        Guid id,
        CreateWordDto wordDto,
        IWordService wordService,
        CancellationToken cancellationToken)
    {
        bool success = await wordService.UpdateWordAsync(id, wordDto, cancellationToken);

        return success ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> DeleteWord(
        Guid id,
        IWordService wordService,
        CancellationToken cancellationToken)
    {
        bool success = await wordService.DeleteWordAsync(id, cancellationToken);

        return success ? Results.NoContent() : Results.NotFound();
    }
}