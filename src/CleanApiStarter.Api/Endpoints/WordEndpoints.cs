namespace CleanApiStarter.Api.Endpoints;

public static class WordEndpoints
{
    public static IEndpointRouteBuilder MapWordEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/words")
            .WithTags("Words");

        group.MapPost("/", CreateWord)
            .WithName(nameof(CreateWord));

        group.MapGet("/{id:guid}", GetWord)
            .WithName(nameof(GetWord));

        group.MapGet("/", GetAllWords)
            .WithName(nameof(GetAllWords));

        group.MapPut("/{id:guid}", UpdateWord)
            .WithName(nameof(UpdateWord));

        group.MapDelete("/{id:guid}", DeleteWord)
            .WithName(nameof(DeleteWord));

        return endpoints;
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
