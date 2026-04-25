namespace CleanApiStarter.Application.Interfaces;

public interface IWordService
{
    Task<Guid> AddWordAsync(CreateWordDto wordDto, CancellationToken cancellationToken);
    Task<WordDto?> GetWordByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<WordDto>> GetAllWordsAsync(CancellationToken cancellationToken);
    Task<bool> UpdateWordAsync(Guid id, CreateWordDto wordDto, CancellationToken cancellationToken);
    Task<bool> DeleteWordAsync(Guid id, CancellationToken cancellationToken);
}
