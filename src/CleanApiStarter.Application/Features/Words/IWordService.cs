namespace CleanApiStarter.Application.Features.Words;

public interface IWordService
{
    Task<Guid> AddWordAsync(CreateWordDto wordDto, CancellationToken cancellationToken);
    Task<WordDto?> GetWordByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PaginatedResult<WordDto>> GetWordsAsync(PaginatedQuery query, CancellationToken cancellationToken);
    Task<bool> UpdateWordAsync(Guid id, CreateWordDto wordDto, CancellationToken cancellationToken);
    Task<bool> DeleteWordAsync(Guid id, CancellationToken cancellationToken);
}