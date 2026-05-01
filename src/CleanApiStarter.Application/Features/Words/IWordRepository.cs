namespace CleanApiStarter.Application.Features.Words;

public interface IWordRepository
{
    Task<Guid> AddWordAsync(Word word, CancellationToken cancellationToken);
    Task<Word?> GetWordByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PaginatedResult<Word>> GetWordsAsync(PaginatedQuery query, CancellationToken cancellationToken);
    Task<bool> UpdateWordAsync(Word word, CancellationToken cancellationToken);
    Task<bool> DeleteWordAsync(Guid id, CancellationToken cancellationToken);
}